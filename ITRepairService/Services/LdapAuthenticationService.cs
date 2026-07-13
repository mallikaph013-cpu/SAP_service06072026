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
            _logger.LogInformation("Starting LDAP authentication for user: {Username}", username);
            _logger.LogInformation("LDAP Settings - Server: {Server}, Port: {Port}, Domain: {Domain}, SearchBase: {SearchBase}", 
                _settings.Server, _settings.Port, _settings.Domain, _settings.SearchBase);

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

            // Connect and authenticate using Negotiate for AD compatibility
            using (var connection = new LdapConnection(new LdapDirectoryIdentifier(_settings.Server, _settings.Port)))
            {
                _logger.LogInformation("Connecting to LDAP server: {Server}:{Port}", _settings.Server, _settings.Port);

                // Use Negotiate for Kerberos/NTLM (required for AD with DOMAIN\user format)
                connection.AuthType = AuthType.Negotiate;

                // Try multiple credential formats
                List<string> credentialFormats = new List<string>
                {
                    $"{_settings.Domain}\\{username}",           // DOMAIN\username
                    $"{_settings.Domain}/{username}",           // DOMAIN/username
                    $"{username}@{_settings.Domain.ToLowerInvariant()}.local",  // username@domain.local
                    username                                     // username only
                };

                bool bindSuccess = false;
                Exception? lastException = null;

                foreach (var credentialFormat in credentialFormats)
                {
                    try
                    {
                        _logger.LogInformation("Attempting to bind with format: {CredentialFormat}", credentialFormat);
                        connection.Bind(new System.Net.NetworkCredential(credentialFormat, password));
                        _logger.LogInformation("Successfully bound to AD as {User} using format: {Format}", credentialFormat, credentialFormat);
                        bindSuccess = true;
                        break;
                    }
                    catch (Exception bindEx)
                    {
                        _logger.LogWarning("Failed to bind with format {Format}: {Message}", credentialFormat, bindEx.Message);
                        lastException = bindEx;
                        continue;
                    }
                }

                if (!bindSuccess)
                {
                    string errorMsg = lastException?.Message ?? "Unknown error";
                    _logger.LogError(lastException, "All credential formats failed. Last error: {Message}", errorMsg);
                    return LdapAuthenticationResult.Failure($"Authentication failed: {errorMsg}");
                }

                // Search for user details
                string searchFilter = _settings.SearchFilter.Replace("{0}", EscapeLdapFilter(username));
                _logger.LogInformation("Searching with filter: {Filter} in base: {SearchBase}", searchFilter, _settings.SearchBase);

                var searchRequest = new SearchRequest(
                    _settings.SearchBase,
                    searchFilter,
                    SearchScope.Subtree,
                    "sAMAccountName", "displayName", "mail", "cn", "distinguishedName",
                    "department", "title", "company", "manager", "telephoneNumber",
                    "employeeID", "memberOf"
                );

                SearchResponse searchResponse;
                try
                {
                    searchResponse = (SearchResponse)connection.SendRequest(searchRequest);
                    _logger.LogInformation("Search completed. Found {Count} entries", searchResponse.Entries.Count);
                }
                catch (Exception searchEx)
                {
                    _logger.LogError(searchEx, "LDAP search failed: {Message}", searchEx.Message);
                    return LdapAuthenticationResult.Failure($"Search failed: {searchEx.Message}");
                }

                if (searchResponse.Entries.Count == 0)
                {
                    _logger.LogWarning("User {Username} not found in AD with filter {Filter}", username, searchFilter);
                    return LdapAuthenticationResult.Failure("Invalid username or password.");
                }

                var entry = searchResponse.Entries[0];
                _logger.LogInformation("Found user entry with DN: {DN}", entry.DistinguishedName);

                // Log all available attributes for debugging
                _logger.LogInformation("Available attributes: {Attributes}",
                    string.Join(", ", entry.Attributes.AttributeNames.Cast<string>()));

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

                // Detailed logging for debugging Title attribute
                _logger.LogInformation("=== AD ATTRIBUTES DEBUG ===");
                _logger.LogInformation("Username: {Username}", sAMAccountName);
                _logger.LogInformation("DisplayName: {DisplayName}", displayName);
                _logger.LogInformation("Department: {Department}", department);
                _logger.LogInformation("Title (raw): {Title}", title);
                _logger.LogInformation("Title (isNullOrEmpty): {IsNullOrEmpty}", string.IsNullOrEmpty(title));
                _logger.LogInformation("Title (ToUpper): {TitleUpper}", title.ToUpperInvariant());
                _logger.LogInformation("Contains 'DM': {ContainsDM}", title.ToUpperInvariant().Contains("DM"));
                _logger.LogInformation("Contains 'SM': {ContainsSM}", title.ToUpperInvariant().Contains("SM"));
                _logger.LogInformation("Contains 'IT': {ContainsIT}", department.ToUpperInvariant().Contains("IT"));
                _logger.LogInformation("=== END DEBUG ===");
                
                _logger.LogInformation("Found AD user: {Username}, displayName={DisplayName}, email={Email}, dept={Dept}, title={Title}",
                    sAMAccountName, displayName, email, department, title);
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
            _logger.LogWarning("LDAP directory error for {Username}: {Message}", username, ex.Message);
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