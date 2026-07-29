using AudioAtlasApplication.Media;
using Xunit;

namespace AudioAtlasInfrastructureTests;

/// <summary>
/// This helper is the boundary between contributor input and what ends up in an
/// iframe src, so the rejection cases carry more weight than the happy path.
/// </summary>
public class YouTubeVideoTests
{
    private const string Id = "dQw4w9WgXcQ";

    [Theory]
    [InlineData("https://www.youtube.com/watch?v=dQw4w9WgXcQ")]
    [InlineData("https://youtube.com/watch?v=dQw4w9WgXcQ")]
    [InlineData("https://m.youtube.com/watch?v=dQw4w9WgXcQ")]
    [InlineData("https://music.youtube.com/watch?v=dQw4w9WgXcQ")]
    [InlineData("http://www.youtube.com/watch?v=dQw4w9WgXcQ")]
    [InlineData("https://youtu.be/dQw4w9WgXcQ")]
    [InlineData("https://www.youtube.com/shorts/dQw4w9WgXcQ")]
    [InlineData("https://www.youtube.com/embed/dQw4w9WgXcQ")]
    [InlineData("https://www.youtube.com/live/dQw4w9WgXcQ")]
    [InlineData("  https://www.youtube.com/watch?v=dQw4w9WgXcQ  ")]
    [InlineData("dQw4w9WgXcQ")]
    public void Accepts_the_forms_people_actually_paste(string input)
    {
        Assert.True(YouTubeVideo.TryParseId(input, out string id));
        Assert.Equal(Id, id);
    }

    [Theory]
    // Extra query parameters are normal on shared links.
    [InlineData("https://www.youtube.com/watch?v=dQw4w9WgXcQ&t=42s")]
    [InlineData("https://www.youtube.com/watch?app=desktop&v=dQw4w9WgXcQ")]
    [InlineData("https://youtu.be/dQw4w9WgXcQ?si=abcdef")]
    public void Ignores_surrounding_query_parameters(string input)
    {
        Assert.True(YouTubeVideo.TryParseId(input, out string id));
        Assert.Equal(Id, id);
    }

    [Theory]
    // Anything that could steer the iframe somewhere of the submitter's choosing.
    [InlineData("javascript:alert(1)")]
    [InlineData("data:text/html,<script>alert(1)</script>")]
    [InlineData("file:///etc/passwd")]
    [InlineData("https://evil.example.com/watch?v=dQw4w9WgXcQ")]
    [InlineData("https://youtube.com.evil.example/watch?v=dQw4w9WgXcQ")]
    [InlineData("https://notyoutube.com/watch?v=dQw4w9WgXcQ")]
    public void Rejects_anything_that_is_not_a_youtube_url(string input)
    {
        Assert.False(YouTubeVideo.TryParseId(input, out string id));
        Assert.Equal(string.Empty, id);
    }

    [Theory]
    [InlineData("https://www.youtube.com/watch?v=short")]
    [InlineData("https://www.youtube.com/watch?v=waaaaaaaaytoolong")]
    [InlineData("https://www.youtube.com/watch?v=has spaces!")]
    [InlineData("https://www.youtube.com/watch")]
    [InlineData("https://www.youtube.com/")]
    [InlineData("https://www.youtube.com/playlist?list=PLabcdefghij")]
    [InlineData("https://open.spotify.com/playlist/37i9dQZF1DX")]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Rejects_malformed_ids_and_non_video_urls(string? input)
    {
        Assert.False(YouTubeVideo.TryParseId(input, out string id));
        Assert.Equal(string.Empty, id);
    }

    [Fact]
    public void Playlist_urls_are_rejected_because_the_feature_is_one_song()
    {
        // The field this replaces held playlists. A playlist URL carries no video
        // id, so accepting one would produce an embed that cannot be built.
        Assert.False(YouTubeVideo.TryParseId("https://www.youtube.com/playlist?list=PLabc123", out _));
    }

    [Fact]
    public void Builds_urls_from_the_id_rather_than_the_submitted_string()
    {
        Assert.Equal($"https://www.youtube.com/watch?v={Id}", YouTubeVideo.WatchUrl(Id));
        Assert.Equal($"https://www.youtube-nocookie.com/embed/{Id}", YouTubeVideo.EmbedUrl(Id));
    }
}
