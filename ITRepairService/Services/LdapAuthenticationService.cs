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
            return LdapAuthenticationResult.Failure("Username is required.");
        if (string.IsNullOrWhiteSpace(password))
            return LdapAuthenticationResult.Failure("Password is required.");

        try
        {
            // 1. Search for the user in AD using anonymous bind first
            string? distinguishedName = null;
            string sAMAccountName = username;
            string displayName = username;
            string email = $"{username}@{_settings.Domain.ToLowerInvariant()}.local";

            using (var searchConnection = new LdapConnection(new LdapDirectoryIdentifier(_settings.Server, _settings.Port)))
            {
                searchConnection.AuthType = AuthType.Basic;

                // Try to search with the user's credentials directly
                string userPrincipal = $@"{_settings.Domain}\{username}";
                searchConnection.Bind(new System.Net.NetworkCredential(userPrincipal, password));

                _logger.LogInformation("Bound to AD as {User}", userPrincipal);

                // Search for user details
                string searchFilter = _settings.SearchFilter.Replace("{0}", EscapeLdapFilter(username));
                var searchRequest = new SearchRequest(
                    _settings.SearchBase,
                    searchFilter,
                    SearchScope.Subtree,
                    "sAMAccountName", "displayName", "mail", "cn", "distinguishedName"
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

                _logger.LogInformation("Found AD user: {DN}", distinguishedName ?? "(none)");
            }

            // 2. Now verify with the user's DN to be absolutely certain
            if (!string.IsNullOrEmpty(distinguishedName))
            {
                using var verifyConnection = new LdapConnection(new LdapDirectoryIdentifier(_settings.Server, _settings.Port));
                verifyConnection.AuthType = AuthType.Basic;
                verifyConnection.Bind(new System.Net.NetworkCredential(distinguishedName, password));

                _logger.LogInformation("Password fully verified for {Username}", username);
            }

            return LdapAuthenticationResult.Success(sAMAccountName, displayName, email);
        }
        catch (DirectoryOperationException ex)
        {
            _logger.LogWarning("LDAP directory operation error for {Username}: {Message}", username, ex.Message);
            string errorMsg = ParseLdapError(ex.Message);
            return LdapAuthenticationResult.Failure(errorMsg);
        }
        catch (LdapException ex)
        {
            _logger.LogWarning("LDAP error for {Username}: {Message}", username, ex.Message);
            string errorMsg = ParseLdapError(ex.Message);
            return LdapAuthenticationResult.Failure(errorMsg);
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

        if (message.Contains("52e", StringComparison.OrdinalIgnoreCase))
            return "Invalid username or password.";
        if (message.Contains("525", StringComparison.OrdinalIgnoreCase))
            return "User not found.";
        if (message.Contains("52d", StringComparison.OrdinalIgnoreCase))
            return "Invalid username or password.";
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