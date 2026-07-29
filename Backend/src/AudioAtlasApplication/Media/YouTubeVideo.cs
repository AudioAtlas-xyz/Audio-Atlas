using System.Text.RegularExpressions;

namespace AudioAtlasApplication.Media;

/// <summary>
/// Parses a contributor-supplied YouTube reference down to its bare video ID.
///
/// The catalogue stores the ID, never the submitted URL. Storing a URL and
/// interpolating it into an iframe's src would let a submission choose the host
/// — or a javascript: scheme — and the genre page would happily render it. An
/// eleven-character ID drawn from a fixed alphabet cannot express any of that,
/// so the embed URL is always built by us from a value that has been proven
/// safe here.
///
/// Accepts the forms people actually paste (watch links, youtu.be short links,
/// shorts, embed and live URLs, with or without the www/m/music subdomain) plus
/// a bare ID, which is what the seed files carry.
/// </summary>
public static class YouTubeVideo
{
    private static readonly Regex IdPattern = new("^[A-Za-z0-9_-]{11}$", RegexOptions.Compiled);

    private static readonly HashSet<string> AllowedHosts = new(StringComparer.OrdinalIgnoreCase)
    {
        "youtube.com", "www.youtube.com", "m.youtube.com", "music.youtube.com",
        "youtu.be", "www.youtu.be",
        "youtube-nocookie.com", "www.youtube-nocookie.com"
    };

    /// <summary>Path prefixes that carry the ID as the next segment.</summary>
    private static readonly HashSet<string> PathPrefixes = new(StringComparer.OrdinalIgnoreCase)
    {
        "shorts", "embed", "live", "v"
    };

    /// <summary>
    /// True when <paramref name="input"/> resolves to a YouTube video ID.
    /// </summary>
    public static bool TryParseId(string? input, out string videoId)
    {
        videoId = string.Empty;

        string? value = input?.Trim();
        if (string.IsNullOrEmpty(value))
        {
            return false;
        }

        // Bare ID — how the seed files store it.
        if (IdPattern.IsMatch(value))
        {
            videoId = value;
            return true;
        }

        if (!Uri.TryCreate(value, UriKind.Absolute, out Uri? uri))
        {
            return false;
        }

        // Scheme is checked before host so that javascript:, data: and file:
        // are rejected outright rather than by failing the host list.
        if (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps)
        {
            return false;
        }

        if (!AllowedHosts.Contains(uri.Host))
        {
            return false;
        }

        string? candidate = ExtractCandidate(uri);

        if (candidate is null || !IdPattern.IsMatch(candidate))
        {
            return false;
        }

        videoId = candidate;
        return true;
    }

    private static string? ExtractCandidate(Uri uri)
    {
        string path = uri.AbsolutePath.Trim('/');

        // youtu.be/<id> — the whole path is the ID.
        if (uri.Host.EndsWith("youtu.be", StringComparison.OrdinalIgnoreCase))
        {
            return path;
        }

        string[] segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);

        if (segments.Length >= 2 && PathPrefixes.Contains(segments[0]))
        {
            return segments[1];
        }

        if (segments.Length >= 1 && segments[0].Equals("watch", StringComparison.OrdinalIgnoreCase))
        {
            return QueryValue(uri.Query, "v");
        }

        return null;
    }

    /// <summary>
    /// Minimal query reader. Avoids pulling an ASP.NET dependency into this
    /// project, which references only the domain.
    /// </summary>
    private static string? QueryValue(string query, string key)
    {
        foreach (string part in query.TrimStart('?').Split('&', StringSplitOptions.RemoveEmptyEntries))
        {
            string[] pair = part.Split('=', 2);
            if (pair.Length == 2 && pair[0].Equals(key, StringComparison.OrdinalIgnoreCase))
            {
                return Uri.UnescapeDataString(pair[1]);
            }
        }

        return null;
    }

    /// <summary>Canonical watch URL, for curators reviewing a submission.</summary>
    public static string WatchUrl(string videoId) => $"https://www.youtube.com/watch?v={videoId}";

    /// <summary>
    /// Embed URL. Uses youtube-nocookie.com so the player does not set tracking
    /// cookies; the genre page additionally defers loading it until the user
    /// clicks, so nothing is requested from Google before then.
    /// </summary>
    public static string EmbedUrl(string videoId) => $"https://www.youtube-nocookie.com/embed/{videoId}";
}
