using AudioAtlasDomain.Genres;

namespace AudioAtlasDomain.MusicMetadata
{
    /// <summary>
    /// Represents a musical instrument or sound-producing tool
    /// associated with one or more genres.
    /// 
    /// Instruments contribute to the sonic identity of a genre,
    /// but are not strictly required—genres may evolve beyond
    /// their traditional instrumentation.
    /// </summary>
    public class Instrument
    {
        /// <summary>
        /// Unique identifier for the instrument.
        /// </summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// The name or type of the instrument.
        /// 
        /// This should represent a recognizable category
        /// (e.g., "Electric Guitar", "Synthesizer", "Drum Machine").
        /// </summary>
        public string Type { get; set; } = null!;

        /// <summary>
        /// Optional description providing additional context
        /// about the instrument, its characteristics, or usage.
        /// 
        /// May include details about sound, playing technique,
        /// or its role within different musical styles.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Genres commonly associated with this instrument.
        /// 
        /// This reflects typical usage rather than strict rules.
        /// An instrument appearing in a genre does not imply exclusivity,
        /// nor does its absence prevent classification within that genre.
        /// </summary>
        public ICollection<Genre> Genres { get; set; } = new List<Genre>();
    }
}
