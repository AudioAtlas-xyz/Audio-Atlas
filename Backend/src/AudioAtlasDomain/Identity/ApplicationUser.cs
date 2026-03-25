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

    }
}
