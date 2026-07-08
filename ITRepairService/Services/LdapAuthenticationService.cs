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
    public string Username { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

public class LdapAuthenticationService : ILdapAuthenticationService
{
    private readonly LdapSettings _settings;

    public LdapAuthenticationService(IOptions<LdapSettings> settings)
    {
        _settings = settings.Value;
    }

    public LdapAuthenticationResult? Authenticate(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            return null;

        try
        {
            using var connection = new LdapConnection();
            connection.Connect(_settings.Server, _settings.Port);

            if (_settings.UseSSL)
            {
                connection.StartTls();
            }

            // Bind with the user's credentials to verify them
            string bindDn = $@"{_settings.Domain}\{username}";
            connection.Bind(bindDn, password);

            // Search for user details
            string searchFilter = _settings.SearchFilter.Replace("{0}", username);
            var searchResults = connection.Search(
                _settings.SearchBase,
                LdapConnection.ScopeSub,
                searchFilter,
                ["sAMAccountName", "displayName", "mail", "cn"],
                false
            );

            if (!searchResults.HasMore())
                return null;

            var entry = searchResults.Next();
            var attributes = entry.GetAttributeSet();

            return new LdapAuthenticationResult
            {
                Username = GetAttributeValue(attributes, "sAMAccountName") ?? username,
                DisplayName = GetAttributeValue(attributes, "displayName")
                              ?? GetAttributeValue(attributes, "cn")
                              ?? username,
                Email = GetAttributeValue(attributes, "mail")
                        ?? $"{username}@{_settings.Domain.ToLowerInvariant()}.com"
            };
        }
        catch
        {
            return null;
        }
    }

    private static string? GetAttributeValue(LdapAttributeSet attributes, string attributeName)
    {
        var attr = attributes.GetAttribute(attributeName);
        return attr?.StringValue;
    }
}