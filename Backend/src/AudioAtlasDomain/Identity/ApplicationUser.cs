using AudioAtlasDomain.Genres;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace AudioAtlasDomain.Users
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public ICollection<Genre> FavoriteGenres { get; set; } = new List<Genre>();

    }
}
