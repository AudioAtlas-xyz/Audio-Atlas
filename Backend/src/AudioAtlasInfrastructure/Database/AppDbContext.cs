using AudioAtlasDomain.Genres;
using AudioAtlasDomain.Geography;
using AudioAtlasDomain.MusicMetadata;
using AudioAtlasDomain.Submissions;
using AudioAtlasDomain.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AudioAtlasInfrastructure.Database;

public class AppDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Genre> Genres => Set<Genre>();
    public DbSet<GenreAlias> GenreAliases => Set<GenreAlias>();
    public DbSet<GenreSource> GenreSources => Set<GenreSource>();

    public DbSet<Country> Countries => Set<Country>();

    public DbSet<Instrument> Instruments => Set<Instrument>();

    public DbSet<Submission> Submissions => Set<Submission>();
    public DbSet<SubmissionAlias> SubmissionAliases => Set<SubmissionAlias>();
    public DbSet<SubmissionSource> SubmissionSources => Set<SubmissionSource>();
    public DbSet<RejectedSubmission> RejectedSubmissions => Set<RejectedSubmission>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
