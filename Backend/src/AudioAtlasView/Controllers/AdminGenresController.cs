using System.Security.Claims;
using AudioAtlasApplication.DTOs.AdminGenres;
using AudioAtlasDomain.Enums;
using AudioAtlasDomain.Genres;
using AudioAtlasDomain.Geography;
using AudioAtlasDomain.MusicMetadata;
using AudioAtlasInfrastructure.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AudioAtlasView.Controllers;

[ApiController]
[Route("api/admin/genres")]
[Authorize(Roles = "Admin,Curator")]
public class AdminGenresController : ControllerBase
{
    private readonly AppDbContext _db;

    public AdminGenresController(AppDbContext db) => _db = db;

    // ── GET /api/admin/genres ─────────────────────────────────────────────────
    // Paginated list with optional search and quality/status filters.
    // filter values: all | active | archived | below-gate | missing-note | orphans
    [HttpGet]
    public async Task<ActionResult<AdminGenrePageResponse>> GetPage(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        [FromQuery] string? search = null,
        [FromQuery] string? filter = "all",
        CancellationToken ct = default)
    {
        if (page < 1) page = 1;
        if (pageSize is < 1 or > 200) pageSize = 50;

        // Start from unfiltered set so admins can see archived genres.
        var query = _db.Genres
            .IgnoreQueryFilters()
            .Include(g => g.Countries)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(g => g.Name.Contains(search));

        query = filter switch
        {
            "active"     => query.Where(g => !g.IsArchived),
            "archived"   => query.Where(g => g.IsArchived),
            // "missing-note" is a legacy alias for "below-gate" — both mean "no
            // description". Kept so existing dashboard links and bookmarks work.
            "below-gate" or "missing-note"
                         => query.Where(g => !g.IsArchived && string.IsNullOrEmpty(g.Description)),
            "orphans"    => query.Where(g => !g.IsArchived && !g.Countries.Any()),
            "missing-sources"
                         => query.Where(g => !g.IsArchived && !g.Sources.Any()),
            "sensitive-incomplete"
                         => query.Where(g => !g.IsArchived && g.IsSensitive
                                             && string.IsNullOrEmpty(g.SensitiveDescription)),
            _            => query
        };

        var total = await query.CountAsync(ct);

        var rows = await query
            .OrderBy(g => g.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(g => new
            {
                g.Id,
                g.Name,
                HasDescription = !string.IsNullOrEmpty(g.Description),
                CountryNames = g.Countries.Select(c => c.Name).ToList(),
                g.IsArchived,
                g.ArchivedAt,
                g.LastEditedAt,
                g.RowVersion
            })
            .ToListAsync(ct);

        var items = rows.Select(r => new AdminGenreRow
        {
            Id = r.Id,
            Name = r.Name,
            HasDescription = r.HasDescription,
            CountryNames = r.CountryNames,
            IsArchived = r.IsArchived,
            ArchivedAt = r.ArchivedAt,
            LastEditedAt = r.LastEditedAt,
            RowVersion = Convert.ToBase64String(r.RowVersion)
        }).ToList();

        return Ok(new AdminGenrePageResponse
        {
            Items = items,
            TotalCount = total,
            Page = page,
            PageSize = pageSize
        });
    }

    // ── GET /api/admin/genres/lookup ──────────────────────────────────────────
    // Thin name+id list for relation dropdowns. Must be declared before {id:guid}.
    [HttpGet("lookup")]
    public async Task<ActionResult<IReadOnlyList<LookupItem>>> GetLookup(CancellationToken ct)
    {
        var items = await _db.Genres
            .OrderBy(g => g.Name)
            .Select(g => new LookupItem { Id = g.Id, Name = g.Name })
            .ToListAsync(ct);

        return Ok(items);
    }

    // ── GET /api/admin/genres/{id} ────────────────────────────────────────────
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<AdminGenreDetail>> GetDetail(Guid id, CancellationToken ct)
    {
        var genre = await _db.Genres
            .IgnoreQueryFilters()
            .Include(g => g.Countries)
            .Include(g => g.Instruments)
            .Include(g => g.SimilarGenres)
            .Include(g => g.SubGenres)
            .Include(g => g.ParentGenres)
            .Include(g => g.Aliases)
            .Include(g => g.Sources)
            .Include(g => g.ArchivedBy)
            .Include(g => g.LastEditedBy)
            .FirstOrDefaultAsync(g => g.Id == id, ct);

        if (genre is null) return NotFound();

        return Ok(MapDetail(genre));
    }

    // ── PUT /api/admin/genres/{id} ────────────────────────────────────────────
    // Optimistic concurrency via If-Match header (base64 RowVersion).
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] AdminGenrePutRequest request,
        [FromHeader(Name = "If-Match")] string? ifMatch,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            return BadRequest(new { message = "Name is required." });

        if (string.IsNullOrWhiteSpace(ifMatch))
            return BadRequest(new { message = "If-Match header with RowVersion is required." });

        byte[] clientRowVersion;
        try { clientRowVersion = Convert.FromBase64String(ifMatch); }
        catch { return BadRequest(new { message = "If-Match value is not valid base64." }); }

        var genre = await _db.Genres
            .IgnoreQueryFilters()
            .Include(g => g.Countries)
            .Include(g => g.Instruments)
            .Include(g => g.SimilarGenres)
            .Include(g => g.SubGenres)
            .Include(g => g.ParentGenres)
            .Include(g => g.Aliases)
            .Include(g => g.Sources)
            .FirstOrDefaultAsync(g => g.Id == id, ct);

        if (genre is null) return NotFound();

        var editorId = GetCallerId();
        if (editorId is null) return Unauthorized();

        // Scalar fields — AuthorId is intentionally never touched here.
        genre.Name = request.Name.Trim();
        genre.Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim();
        genre.StartYear = request.StartYear;
        genre.IsSensitive = request.IsSensitive;
        genre.SensitiveDescription = string.IsNullOrWhiteSpace(request.SensitiveDescription) ? null : request.SensitiveDescription.Trim();
        genre.PlaylistLink = string.IsNullOrWhiteSpace(request.PlaylistLink) ? null : request.PlaylistLink.Trim();

        // Audit stamp.
        genre.LastEditedAt = DateTime.UtcNow;
        genre.LastEditedById = editorId;

        // Many-to-many: Countries
        await SyncManyToManyAsync(
            genre.Countries,
            request.CountryIds,
            ids => _db.Countries.Where(c => ids.Contains(c.Id)).ToListAsync(ct));

        // Many-to-many: Instruments
        await SyncManyToManyAsync(
            genre.Instruments,
            request.InstrumentIds,
            ids => _db.Instruments.Where(i => ids.Contains(i.Id)).ToListAsync(ct));

        // Many-to-many: SubGenres (hierarchy)
        await SyncManyToManyAsync(
            genre.SubGenres,
            request.SubGenreIds,
            ids => _db.Genres.IgnoreQueryFilters().Where(g => ids.Contains(g.Id)).ToListAsync(ct));

        // Many-to-many: ParentGenres (hierarchy)
        await SyncManyToManyAsync(
            genre.ParentGenres,
            request.ParentGenreIds,
            ids => _db.Genres.IgnoreQueryFilters().Where(g => ids.Contains(g.Id)).ToListAsync(ct));

        // Similar genres — bidirectional sync
        await SyncSimilarGenresBidirectionalAsync(genre, request.SimilarGenreIds, ct);

        // One-to-many: Aliases (replace)
        genre.Aliases.Clear();
        foreach (var alias in request.Aliases.Where(a => !string.IsNullOrWhiteSpace(a)))
            genre.Aliases.Add(new GenreAlias { GenreId = genre.Id, Alias = alias.Trim() });

        // One-to-many: Sources (replace)
        genre.Sources.Clear();
        foreach (var src in request.Sources.Where(s => !string.IsNullOrWhiteSpace(s)))
            genre.Sources.Add(new GenreSource { GenreId = genre.Id, SourceLink = src.Trim() });

        // Set the client's rowversion so EF uses it in the WHERE clause.
        genre.RowVersion = clientRowVersion;

        try
        {
            await _db.SaveChangesAsync(ct);
            return NoContent();
        }
        catch (DbUpdateConcurrencyException)
        {
            return Conflict(new { message = "The genre was modified by someone else. Reload and try again." });
        }
    }

    // ── POST /api/admin/genres/{id}/archive ───────────────────────────────────
    [HttpPost("{id:guid}/archive")]
    public async Task<IActionResult> Archive(Guid id, CancellationToken ct)
    {
        var genre = await _db.Genres
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(g => g.Id == id, ct);

        if (genre is null) return NotFound();
        if (genre.IsArchived) return Conflict(new { message = "Genre is already archived." });

        var callerId = GetCallerId();
        if (callerId is null) return Unauthorized();

        genre.IsArchived = true;
        genre.ArchivedAt = DateTime.UtcNow;
        genre.ArchivedById = callerId;

        await _db.SaveChangesAsync(ct);
        return NoContent();
    }

    // ── POST /api/admin/genres/{id}/restore ───────────────────────────────────
    [HttpPost("{id:guid}/restore")]
    public async Task<IActionResult> Restore(Guid id, CancellationToken ct)
    {
        var genre = await _db.Genres
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(g => g.Id == id, ct);

        if (genre is null) return NotFound();
        if (!genre.IsArchived) return Conflict(new { message = "Genre is not archived." });

        genre.IsArchived = false;
        genre.ArchivedAt = null;
        genre.ArchivedById = null;

        await _db.SaveChangesAsync(ct);
        return NoContent();
    }

    // ── DELETE /api/admin/genres/{id} ─────────────────────────────────────────
    // Hard delete — Admin only.
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> HardDelete(Guid id, CancellationToken ct)
    {
        var genre = await _db.Genres
            .IgnoreQueryFilters()
            .Include(g => g.Aliases)
            .Include(g => g.Sources)
            .FirstOrDefaultAsync(g => g.Id == id, ct);

        if (genre is null) return NotFound();

        var hasPendingSuggestions = await _db.Submissions
            .AnyAsync(s => s.TargetGenreId == id && s.Status == SubmissionStatus.Pending, ct);

        if (hasPendingSuggestions)
            return Conflict(new { message = "This genre has pending edit suggestions. Approve or reject them before deleting." });

        _db.Genres.Remove(genre);
        await _db.SaveChangesAsync(ct);
        return NoContent();
    }

    // ── GET /api/admin/genres/countries-lookup ────────────────────────────────
    [HttpGet("countries-lookup")]
    public async Task<ActionResult<IReadOnlyList<LookupItem>>> GetCountriesLookup(CancellationToken ct)
    {
        var items = await _db.Countries
            .OrderBy(c => c.Name)
            .Select(c => new LookupItem { Id = c.Id, Name = c.Name })
            .ToListAsync(ct);

        return Ok(items);
    }

    // ── GET /api/admin/genres/instruments-lookup ──────────────────────────────
    [HttpGet("instruments-lookup")]
    public async Task<ActionResult<IReadOnlyList<LookupItem>>> GetInstrumentsLookup(CancellationToken ct)
    {
        var items = await _db.Instruments
            .OrderBy(i => i.Type)
            .Select(i => new LookupItem { Id = i.Id, Name = i.Type })
            .ToListAsync(ct);

        return Ok(items);
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private Guid? GetCallerId()
    {
        var value = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(value, out var id) ? id : null;
    }

    private static AdminGenreDetail MapDetail(Genre g) => new()
    {
        Id = g.Id,
        Name = g.Name,
        Description = g.Description,
        StartYear = g.StartYear,
        IsSensitive = g.IsSensitive,
        SensitiveDescription = g.SensitiveDescription,
        PlaylistLink = g.PlaylistLink,
        Countries = g.Countries.Select(c => new LookupItem { Id = c.Id, Name = c.Name }).ToList(),
        Instruments = g.Instruments.Select(i => new LookupItem { Id = i.Id, Name = i.Type }).ToList(),
        SimilarGenres = g.SimilarGenres.Select(s => new LookupItem { Id = s.Id, Name = s.Name }).ToList(),
        SubGenres = g.SubGenres.Select(s => new LookupItem { Id = s.Id, Name = s.Name }).ToList(),
        ParentGenres = g.ParentGenres.Select(p => new LookupItem { Id = p.Id, Name = p.Name }).ToList(),
        Aliases = g.Aliases.Select(a => a.Alias).ToList(),
        Sources = g.Sources.Select(s => s.SourceLink).ToList(),
        IsArchived = g.IsArchived,
        ArchivedAt = g.ArchivedAt,
        ArchivedByUsername = g.ArchivedBy?.UserName,
        LastEditedAt = g.LastEditedAt,
        LastEditedByUsername = g.LastEditedBy?.UserName,
        RowVersion = Convert.ToBase64String(g.RowVersion)
    };

    // Replaces a tracked navigation collection with the new set of IDs.
    private static async Task SyncManyToManyAsync<T>(
        ICollection<T> current,
        List<Guid> newIds,
        Func<List<Guid>, Task<List<T>>> load) where T : class
    {
        if (newIds.Count == 0)
        {
            current.Clear();
            return;
        }

        var incoming = await load(newIds);
        current.Clear();
        foreach (var item in incoming)
            current.Add(item);
    }

    // Syncs similar-genre links bidirectionally.
    // For each genre that G is being linked to (or unlinked from),
    // the reverse entry is also added or removed.
    private async Task SyncSimilarGenresBidirectionalAsync(
        Genre genre,
        List<Guid> newSimilarIds,
        CancellationToken ct)
    {
        var previousIds = genre.SimilarGenres.Select(s => s.Id).ToHashSet();
        var nextIds = newSimilarIds.ToHashSet();

        var toRemove = previousIds.Except(nextIds).ToList();
        var toAdd = nextIds.Except(previousIds).ToList();

        // Update this genre's collection.
        if (toRemove.Count > 0 || toAdd.Count > 0)
        {
            var incoming = newSimilarIds.Count > 0
                ? await _db.Genres.IgnoreQueryFilters()
                    .Where(g => newSimilarIds.Contains(g.Id))
                    .ToListAsync(ct)
                : [];

            genre.SimilarGenres.Clear();
            foreach (var s in incoming)
                genre.SimilarGenres.Add(s);
        }

        // Mirror the reverse direction for added genres.
        if (toAdd.Count > 0)
        {
            var addTargets = await _db.Genres
                .IgnoreQueryFilters()
                .Include(g => g.SimilarGenres)
                .Where(g => toAdd.Contains(g.Id))
                .ToListAsync(ct);

            foreach (var target in addTargets)
            {
                if (!target.SimilarGenres.Any(s => s.Id == genre.Id))
                    target.SimilarGenres.Add(genre);
            }
        }

        // Remove the reverse direction for removed genres.
        if (toRemove.Count > 0)
        {
            var removeTargets = await _db.Genres
                .IgnoreQueryFilters()
                .Include(g => g.SimilarGenres)
                .Where(g => toRemove.Contains(g.Id))
                .ToListAsync(ct);

            foreach (var target in removeTargets)
            {
                var link = target.SimilarGenres.FirstOrDefault(s => s.Id == genre.Id);
                if (link is not null)
                    target.SimilarGenres.Remove(link);
            }
        }
    }
}
