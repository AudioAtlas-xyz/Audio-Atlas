using AudioAtlasDomain.Genres;
using AudioAtlasDomain.Geography;
using AudioAtlasDomain.MusicMetadata;
using AudioAtlasDomain.Submissions;
using AudioAtlasDomain.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AudioAtlasInfrastructure.Database;

/// <summary>
/// Represents the primary database context for the application.
/// 
/// The context integrates identity management with the domain model,
/// including genres, metadata, geographical context, and user submissions.
/// 
/// It serves as the central coordination point between the domain layer
/// and the underlying database, managing entity persistence and relationships.
/// </summary>
public class AppDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    /// <summary>
    /// Initializes a new instance of the database context.
    /// </summary>
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }
    /// <summary>
    /// Canonical genre entities representing accepted knowledge.
    /// </summary>
    public DbSet<Genre> Genres => Set<Genre>();

    /// <summary>
    /// Alternative names for genres.
    /// </summary>
    public DbSet<GenreAlias> GenreAliases => Set<GenreAlias>();

    /// <summary>
    /// External sources supporting genre data.
    /// </summary>
    public DbSet<GenreSource> GenreSources => Set<GenreSource>();

    /// <summary>
    /// Countries providing geographical and cultural context.
    /// </summary>
    public DbSet<Country> Countries => Set<Country>();

    /// <summary>
    /// Instruments contributing to genre sound characteristics.
    /// </summary>
    public DbSet<Instrument> Instruments => Set<Instrument>();

    /// <summary>
    /// User submissions representing proposed or unvalidated data.
    /// </summary>
    public DbSet<Submission> Submissions => Set<Submission>();

    /// <summary>
    /// Proposed aliases within submissions.
    /// </summary>
    public DbSet<SubmissionAlias> SubmissionAliases => Set<SubmissionAlias>();

    /// <summary>
    /// Sources provided as part of submissions.
    /// </summary>
    public DbSet<SubmissionSource> SubmissionSources => Set<SubmissionSource>();

    /// <summary>
    /// Records of rejected submissions with associated reasoning.
    /// </summary>
    public DbSet<RejectedSubmission> RejectedSubmissions => Set<RejectedSubmission>();

    /// <summary>
    /// Configures the entity model and applies all configurations
    /// defined within the current assembly.
    /// 
    /// This ensures that entity relationships, constraints, and behaviors
    /// are consistently applied across the domain.
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
