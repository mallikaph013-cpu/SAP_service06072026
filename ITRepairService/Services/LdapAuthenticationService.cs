using System.Text;
using ITRepairService.Models;
using Microsoft.Extensions.Options;
using Novell.Directory.Ldap;

namespace ITRepairService.Services;

public interface ILdapAuthenticationService
{
    /// <summary>
    /// Authenticates a user against Active Directory using the provided username and password.
    /// Returns the user's details if successful, null otherwise.
    /// </summary>
    LdapAuthenticationResult? Authenticate(string username, string password);
}

public class LdapAuthenticationResult
{
    public bool IsSuccess { get; set; }
    public string Username { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Stores any error message when authentication fails.
    /// </summary>
    public string? ErrorMessage { get; set; }

    public static LdapAuthenticationResult Success(string username, string displayName, string email) =>
        new() { IsSuccess = true, Username = username, DisplayName = displayName, Email = email };

    public static LdapAuthenticationResult Failure(string errorMessage) =>
        new() { IsSuccess = false, ErrorMessage = errorMessage };
}

public class LdapAuthenticationService : ILdapAuthenticationService
{
    private readonly LdapSettings _settings;
    private readonly ILogger<LdapAuthenticationService> _logger;

    public LdapAuthenticationService(IOptions<LdapSettings> settings, ILogger<LdapAuthenticationService> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }

    public LdapAuthenticationResult? Authenticate(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            _logger.LogWarning("LDAP auth failed: username is empty");
            return new LdapAuthenticationResult { ErrorMessage = "Username is required." };
        }
        if (string.IsNullOrWhiteSpace(password))
        {
            _logger.LogWarning("LDAP auth failed: password is empty for user {Username}", username);
            return new LdapAuthenticationResult { ErrorMessage = "Password is required." };
        }

        LdapConnection? connection = null;
        try
        {
            connection = new LdapConnection { SecureSocketLayer = _settings.UseSSL };

            // Connect to the LDAP server
            _logger.LogInformation("Connecting to LDAP server {Server}:{Port} (SSL={UseSSL})", 
                _settings.Server, _settings.Port, _settings.UseSSL);
            connection.Connect(_settings.Server, _settings.Port);

            // Try multiple bind formats for Active Directory compatibility
            bool bound = TryBind(connection, username, password);

            if (!bound)
            {
                _logger.LogWarning("LDAP bind failed for user {Username} with all formats", username);
                return new LdapAuthenticationResult { ErrorMessage = "Invalid username or password." };
            }

            _logger.LogInformation("LDAP bind succeeded for user {Username}", username);

            // Search for user details using the bound connection
            string searchFilter = _settings.SearchFilter.Replace("{0}", EscapeLdapSearchFilter(username));
            _logger.LogDebug("LDAP search filter: {Filter}, base: {Base}", searchFilter, _settings.SearchBase);

            var searchResults = connection.Search(
                _settings.SearchBase,
                LdapConnection.ScopeSub,
                searchFilter,
                new[] { "sAMAccountName", "displayName", "mail", "cn", "givenName", "sn" },
                false // typesOnly = false
            );

            if (!searchResults.HasMore())
            {
                _logger.LogWarning("LDAP search returned no entries for user {Username}", username);
                return new LdapAuthenticationResult { ErrorMessage = "User not found in directory." };
            }

            var entry = searchResults.Next();
            var attributes = entry.GetAttributeSet();

            var result = LdapAuthenticationResult.Success(
                GetAttributeValue(attributes, "sAMAccountName") ?? username,
                GetAttributeValue(attributes, "displayName")
                    ?? GetAttributeValue(attributes, "cn")
                    ?? GetAttributeValue(attributes, "givenName")
                    ?? username,
                GetAttributeValue(attributes, "mail")
                    ?? $"{username}@{_settings.Domain.ToLowerInvariant()}.local"
            );

            _logger.LogInformation("LDAP authentication successful for {Username} -> displayName: {DisplayName}, email: {Email}",
                result.Username, result.DisplayName, result.Email);

            return result;
        }
        catch (LdapException ex)
        {
            _logger.LogError(ex, "LDAP exception during authentication for user {Username}", username);
            
            string errorMsg = ex.LdapErrorMessage switch
            {
                string msg when msg.Contains("data 52e", StringComparison.OrdinalIgnoreCase) => "Invalid username or password.",
                string msg when msg.Contains("data 525", StringComparison.OrdinalIgnoreCase) => "User not found.",
                string msg when msg.Contains("data 531", StringComparison.OrdinalIgnoreCase) => "Account is disabled.",
                string msg when msg.Contains("data 532", StringComparison.OrdinalIgnoreCase) => "Account has expired.",
                string msg when msg.Contains("data 533", StringComparison.OrdinalIgnoreCase) => "Account is locked out.",
                string msg when msg.Contains("data 701", StringComparison.OrdinalIgnoreCase) => "Account has expired.",
                string msg when msg.Contains("data 773", StringComparison.OrdinalIgnoreCase) => "Password has expired.",
                string msg when msg.Contains("data 775", StringComparison.OrdinalIgnoreCase) => "Account is locked.",
                _ => $"LDAP error: {ex.Message}"
            };

            return new LdapAuthenticationResult { ErrorMessage = errorMsg };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during LDAP authentication for user {Username}", username);
            return new LdapAuthenticationResult { ErrorMessage = $"Connection error: {ex.Message}" };
        }
        finally
        {
            connection?.Disconnect();
            connection?.Dispose();
        }
    }

    /// <summary>
    /// Try to bind using different username formats for AD compatibility.
    /// Returns true if any format succeeds.
    /// </summary>
    private bool TryBind(LdapConnection connection, string username, string password)
    {
        // Format 1: DOMAIN\username (down-level logon name)
        string format1 = $@"{_settings.Domain}\{username}";
        if (BindWithFormat(connection, format1, password))
        {
            _logger.LogDebug("LDAP bind succeeded with format: DOMAIN\\username");
            return true;
        }

        // Format 2: username@domain (UPN format)
        string format2 = $"{username}@{_settings.Domain.ToLowerInvariant()}.local";
        if (BindWithFormat(connection, format2, password))
        {
            _logger.LogDebug("LDAP bind succeeded with format: user@domain");
            return true;
        }

        // Format 3: Just username (sometimes works with simple AD setups)
        if (BindWithFormat(connection, username, password))
        {
            _logger.LogDebug("LDAP bind succeeded with format: username only");
            return true;
        }

        return false;
    }

    private bool BindWithFormat(LdapConnection connection, string dn, string password)
    {
        try
        {
            connection.Bind(dn, password);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private static string? GetAttributeValue(LdapAttributeSet attributes, string attributeName)
    {
        try
        {
            var attr = attributes.GetAttribute(attributeName);
            return attr?.StringValue;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Escapes special characters in LDAP search filter values.
    /// </summary>
    private static string EscapeLdapSearchFilter(string value)
    {
        if (string.IsNullOrEmpty(value))
            return value;

        var sb = new StringBuilder(value.Length);
        foreach (char c in value)
        {
            sb.Append(c switch
            {
                '*' => "\\2a",
                '(' => "\\28",
                ')' => "\\29",
                '\\' => "\\5c",
                '/' => "\\2f",
                '\0' => "\\00",
                _ => c
            });
        }
        return sb.ToString();
    }
}
