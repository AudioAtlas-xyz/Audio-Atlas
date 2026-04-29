using AudioAtlasDomain.Genres;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace AudioAtlasDomain.Users
{
    /// <summary>
    /// Represents an authenticated user within the system.
    /// 
    /// Extends the default IdentityUser with domain-specific relationships,
    /// primarily focused on user preferences and interactions with genres.
    /// </summary>
    public class ApplicationUser : IdentityUser<Guid>
    {
        /// <summary>
        /// Genres that the user has marked as favorites.
        /// 
        /// This relationship represents user preference and personal taste,
        /// not domain knowledge or classification.
        /// 
        /// Used for personalization, recommendations, and user-specific views.
        /// </summary>
        public ICollection<Genre> FavoriteGenres { get; set; } = new List<Genre>();

        /// <summary>
        /// The UTC timestamp when the user accepted the privacy policy.
        /// </summary>
        public DateTime? AcceptedPrivacyPolicyAtUtc { get; set; }

        /// <summary>
        /// The accepted privacy policy version identifier.
        /// </summary>
        public string? AcceptedPrivacyPolicyVersion { get; set; }

        /// <summary>
        /// The UTC timestamp when the user accepted the contribution guidelines.
        /// </summary>
        public DateTime? AcceptedContributionGuidelinesAtUtc { get; set; }

        /// <summary>
        /// The accepted contribution guidelines version identifier.
        /// </summary>
        public string? AcceptedContributionGuidelinesVersion { get; set; }

        public bool IsSystemUser { get; set; } = false;

        /// <summary>
        /// Indicates that this is a placeholder account used to represent deleted users.
        /// 
        /// When a user deletes their account, their contributions are reassigned to this
        /// placeholder so authorship records remain intact without exposing personal data.
        /// 
        /// This account cannot log in and should never be treated as a real user.
        /// </summary>
        public bool IsDeletedPlaceholder { get; set; } = false;

    }
}
