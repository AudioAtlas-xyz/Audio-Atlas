using System.Text.Json.Serialization;
using AudioAtlasDomain.Genres;
using AudioAtlasDomain.Submissions;

namespace AudioAtlasDomain.Geography
{
    /// <summary>
    /// Represents a country as a cultural and geographical context
    /// for musical genres and user contributions.
    /// 
    /// A country is used to associate genres with regions where they
    /// originated, evolved, or gained cultural significance.
    /// It does not imply exclusivity or ownership of a genre.
    /// </summary>
    public class Country
    {
        /// <summary>
        /// Unique identifier for the country.
        /// </summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// The commonly recognized name of the country.
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// Optional description providing cultural, historical,
        /// or musical context related to the country.
        /// 
        /// This may include information about regional scenes,
        /// influential movements, or notable characteristics
        /// of the country's music landscape.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Genres associated with this country.
        /// 
        /// This relationship reflects cultural relevance rather than strict origin.
        /// A genre may be linked to multiple countries due to migration,
        /// globalization, or parallel development.
        /// </summary>
        public ICollection<Genre> Genres { get; set; } = new List<Genre>();

        /// <summary>
        /// User submissions related to this country.
        /// 
        /// Represents contributed content (e.g., genre suggestions,
        /// edits, or metadata) tied to this geographical context.
        /// </summary>
        public ICollection<Submission> Submissions { get; set; } = new List<Submission>(); 
    }
}