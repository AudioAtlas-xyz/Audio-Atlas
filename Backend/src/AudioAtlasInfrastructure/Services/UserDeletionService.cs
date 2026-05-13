using AudioAtlasApplication.Repositories;
using AudioAtlasApplication.Services;
using AudioAtlasDomain.Genres;
using AudioAtlasDomain.Users;
using AudioAtlasInfrastructure.Database;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AudioAtlasInfrastructure.Services;

public class UserDeletionService : IUserDeletionService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IGenreRepository _genreRepository;
    private readonly AppDbContext _dbContext;
    private readonly ILogger<UserDeletionService> _logger;

    public UserDeletionService(
        UserManager<ApplicationUser> userManager,
        IGenreRepository genreRepository,
        AppDbContext dbContext,
        ILogger<UserDeletionService> logger)
    {
        _userManager = userManager;
        _genreRepository = genreRepository;
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<bool> DeleteUserAsync(Guid userId)
    {
        _logger.LogInformation("Starting account deletion for user {UserId}", userId);

        ApplicationUser? user = await _userManager.FindByIdAsync(userId.ToString());

        if (user == null)
        {
            _logger.LogWarning("User {UserId} not found, cannot delete.", userId);
            return false;
        }

        ApplicationUser? deletedPlaceholder = await _userManager.Users
            .FirstOrDefaultAsync(x => x.IsDeletedPlaceholder);

        if (deletedPlaceholder == null)
        {
            _logger.LogError("Deleted User placeholder not found. Cannot proceed with account deletion for user {UserId}.", userId);
            return false;
        }

        // Reassign authored genres to the placeholder
        ICollection<Genre> authoredGenres = _genreRepository.getGenresByAuthorId(userId);

        foreach (Genre genre in authoredGenres)
        {
            genre.AuthorId = deletedPlaceholder.Id;
        }

        await _genreRepository.SaveChangesAsync();

        // Reassign submissions and audit-log rows to the placeholder so the
        // ApplicationUser delete isn't blocked by Restrict FKs.
        await _dbContext.Submissions
            .Where(s => s.AccountId == userId)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(s => s.AccountId, deletedPlaceholder.Id));

        await _dbContext.RoleChangeAuditLogs
            .Where(a => a.ChangedById == userId)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(a => a.ChangedById, deletedPlaceholder.Id));

        await _dbContext.RoleChangeAuditLogs
            .Where(a => a.TargetUserId == userId)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(a => a.TargetUserId, deletedPlaceholder.Id));

        IdentityResult result = await _userManager.DeleteAsync(user);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                _logger.LogError("Error deleting user {UserId}: {Code} - {Description}", userId, error.Code, error.Description);
            }
            return false;
        }

        _logger.LogInformation("Successfully deleted account for user {UserId}", userId);
        return true;
    }
}