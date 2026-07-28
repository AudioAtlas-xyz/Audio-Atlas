namespace AudioAtlasDomain.Geography;

/// <summary>
/// The continent/region taxonomy used by the browse pages, with genre counts.
///
/// Browse pages are addressed by a single grouping name that may be either a
/// continent or a region (see GenreRepository.GetGenresByGrouping, which matches
/// Continent == grouping || Region == grouping). That makes a page unable to
/// tell, on its own, where it sits in the hierarchy — which is why there was no
/// way to navigate between sibling regions or back up to a continent.
///
/// Counts are distinct genres, matching how the browse listing itself counts:
/// a genre spanning several countries in the same grouping counts once.
/// </summary>
public class GroupingDTO
{
    public string Continent { get; set; } = string.Empty;

    /// <summary>Distinct genres with at least one country on this continent.</summary>
    public int GenreCount { get; set; }

    public List<RegionGroupingDTO> Regions { get; set; } = [];
}

public class RegionGroupingDTO
{
    public string Region { get; set; } = string.Empty;

    /// <summary>Distinct genres with at least one country in this region.</summary>
    public int GenreCount { get; set; }
}
