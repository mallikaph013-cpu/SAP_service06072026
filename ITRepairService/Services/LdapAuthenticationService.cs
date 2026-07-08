using System.DirectoryServices.Protocols;
using ITRepairService.Models;
using Microsoft.Extensions.Options;

namespace ITRepairService.Services;

public interface ILdapAuthenticationService
{
    LdapAuthenticationResult? Authenticate(string username, string password);
}

public class LdapAuthenticationResult
{
    public bool IsSuccess { get; set; }
    public string Username { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Company { get; set; } = string.Empty;
    public string Manager { get; set; } = string.Empty;
    public string TelephoneNumber { get; set; } = string.Empty;
    public string EmployeeID { get; set; } = string.Empty;
    public List<string> MemberOf { get; set; } = new();
    public string? ErrorMessage { get; set; }

    public static LdapAuthenticationResult Success(
        string username, string displayName, string email,
        string department, string title, string company,
        string manager, string telephoneNumber, string employeeID,
        List<string> memberOf) =>
        new()
        {
            IsSuccess = true,
            Username = username,
            DisplayName = displayName,
            Email = email,
            Department = department,
            Title = title,
            Company = company,
            Manager = manager,
            TelephoneNumber = telephoneNumber,
            EmployeeID = employeeID,
            MemberOf = memberOf ?? new List<string>()
        };

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
            return LdapAuthenticationResult.Failure("Username is required.");
        if (string.IsNullOrWhiteSpace(password))
            return LdapAuthenticationResult.Failure("Password is required.");

        try
        {
            string? distinguishedName = null;
            string sAMAccountName = username;
            string displayName = username;
            string email = $"{username}@{_settings.Domain.ToLowerInvariant()}.local";
            string department = string.Empty;
            string title = string.Empty;
            string company = string.Empty;
            string manager = string.Empty;
            string telephoneNumber = string.Empty;
            string employeeID = string.Empty;
            var memberOf = new List<string>();

            using (var searchConnection = new LdapConnection(new LdapDirectoryIdentifier(_settings.Server, _settings.Port)))
            {
                searchConnection.AuthType = AuthType.Basic;

                string userPrincipal = $@"{_settings.Domain}\{username}";
                searchConnection.Bind(new System.Net.NetworkCredential(userPrincipal, password));

                _logger.LogInformation("Bound to AD as {User}", userPrincipal);

                // Search for user details with all required attributes
                string searchFilter = _settings.SearchFilter.Replace("{0}", EscapeLdapFilter(username));
                var searchRequest = new SearchRequest(
                    _settings.SearchBase,
                    searchFilter,
                    SearchScope.Subtree,
                    "sAMAccountName", "displayName", "mail", "cn", "distinguishedName",
                    "department", "title", "company", "manager", "telephoneNumber",
                    "employeeID", "memberOf"
                );

                var searchResponse = (SearchResponse)searchConnection.SendRequest(searchRequest);

                if (searchResponse.Entries.Count == 0)
                {
                    _logger.LogWarning("User {Username} not found in AD", username);
                    return LdapAuthenticationResult.Failure("Invalid username or password.");
                }

                var entry = searchResponse.Entries[0];
                distinguishedName = GetAttributeValue(entry, "distinguishedName");
                sAMAccountName = GetAttributeValue(entry, "sAMAccountName") ?? username;
                displayName = GetAttributeValue(entry, "displayName")
                              ?? GetAttributeValue(entry, "cn")
                              ?? username;
                email = GetAttributeValue(entry, "mail")
                        ?? $"{username}@{_settings.Domain.ToLowerInvariant()}.local";
                department = GetAttributeValue(entry, "department");
                title = GetAttributeValue(entry, "title");
                company = GetAttributeValue(entry, "company");
                manager = GetAttributeValue(entry, "manager");
                telephoneNumber = GetAttributeValue(entry, "telephoneNumber");
                employeeID = GetAttributeValue(entry, "employeeID");
                memberOf = GetAttributeValues(entry, "memberOf");

                _logger.LogInformation("Found AD user: {DN}, department={Dept}, title={Title}, company={Company}",
                    distinguishedName ?? "(none)", department, title, company);
            }

            // 2. Verify with user's DN
            if (!string.IsNullOrEmpty(distinguishedName))
            {
                using var verifyConnection = new LdapConnection(new LdapDirectoryIdentifier(_settings.Server, _settings.Port));
                verifyConnection.AuthType = AuthType.Basic;
                verifyConnection.Bind(new System.Net.NetworkCredential(distinguishedName, password));
                _logger.LogInformation("Password fully verified for {Username}", username);
            }

            return LdapAuthenticationResult.Success(
                sAMAccountName, displayName, email,
                department, title, company,
                manager, telephoneNumber, employeeID,
                memberOf
            );
        }
        catch (DirectoryOperationException ex)
        {
            _logger.LogWarning("LDAP directory operation error for {Username}: {Message}", username, ex.Message);
            return LdapAuthenticationResult.Failure(ParseLdapError(ex.Message));
        }
        catch (LdapException ex)
        {
            _logger.LogWarning("LDAP error for {Username}: {Message}", username, ex.Message);
            return LdapAuthenticationResult.Failure(ParseLdapError(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected LDAP error for {Username}", username);
            return LdapAuthenticationResult.Failure($"Authentication error: {ex.Message}");
        }
    }

    private static string GetAttributeValue(SearchResultEntry entry, string name)
    {
        if (entry.Attributes.Contains(name))
        {
            var values = entry.Attributes[name];
            if (values is { Count: > 0 })
                return values[0]?.ToString() ?? string.Empty;
        }
        return string.Empty;
    }

    private static List<string> GetAttributeValues(SearchResultEntry entry, string name)
    {
        var result = new List<string>();
        if (entry.Attributes.Contains(name))
        {
            var values = entry.Attributes[name];
            if (values != null)
            {
                for (int i = 0; i < values.Count; i++)
                {
                    var val = values[i]?.ToString();
                    if (!string.IsNullOrEmpty(val))
                        result.Add(val);
                }
            }
        }
        return result;
    }

    private static string EscapeLdapFilter(string value)
    {
        return value
            .Replace("\\", "\\5c")
            .Replace("*", "\\2a")
            .Replace("(", "\\28")
            .Replace(")", "\\29")
            .Replace("/", "\\2f");
    }

    private static string ParseLdapError(string? message)
    {
        if (message is null) return "Invalid username or password.";

        if (message.Contains("52e", StringComparison.OrdinalIgnoreCase) ||
            message.Contains("52d", StringComparison.OrdinalIgnoreCase))
            return "Invalid username or password.";
        if (message.Contains("525", StringComparison.OrdinalIgnoreCase))
            return "User not found.";
        if (message.Contains("531", StringComparison.OrdinalIgnoreCase))
            return "Account is disabled.";
        if (message.Contains("532", StringComparison.OrdinalIgnoreCase))
            return "Account has expired.";
        if (message.Contains("533", StringComparison.OrdinalIgnoreCase))
            return "Account is locked.";
        if (message.Contains("701", StringComparison.OrdinalIgnoreCase))
            return "Account has expired.";
        if (message.Contains("773", StringComparison.OrdinalIgnoreCase))
            return "Password has expired.";
        if (message.Contains("775", StringComparison.OrdinalIgnoreCase))
            return "Account is locked.";
        return "Invalid username or password.";
    }
}