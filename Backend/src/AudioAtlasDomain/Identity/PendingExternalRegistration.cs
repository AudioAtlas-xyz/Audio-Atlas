namespace AudioAtlasDomain.Users;

/// <summary>
/// Represents a first-time external login that has not yet completed onboarding.
/// </summary>
public class PendingExternalRegistration
{
    /// <summary>
    /// Primary key for the pending registration.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// External provider name, for example Google or GitHub.
    /// </summary>
    public string LoginProvider { get; set; } = string.Empty;

    /// <summary>
    /// Display name of the provider for UI or logging purposes.
    /// </summary>
    public string ProviderDisplayName { get; set; } = string.Empty;

    /// <summary>
    /// Provider-specific stable key identifying the external account.
    /// </summary>
    public string ProviderKey { get; set; } = string.Empty;

    /// <summary>
    /// Email address supplied by the provider.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Username suggestion derived from provider claims.
    /// </summary>
    public string SuggestedUsername { get; set; } = string.Empty;

    /// <summary>
    /// UTC creation timestamp.
    /// </summary>
    public DateTime CreatedAtUtc { get; set; }

    /// <summary>
    /// UTC expiry timestamp for onboarding completion.
    /// </summary>
    public DateTime ExpiresAtUtc { get; set; }
}
