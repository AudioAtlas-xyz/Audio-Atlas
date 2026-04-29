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
	private readonly ILogger<UserDeletionService> _logger;
    private readonly AppDbContext _dbContext;


    public UserDeletionService(
		UserManager<ApplicationUser> userManager,
		IGenreRepository genreRepository,
		ILogger<UserDeletionService> logger,
		AppDbContext dbContext)
	{
		_userManager = userManager;
		_genreRepository = genreRepository;
		_logger = logger;
        _dbContext = dbContext;
    }

	public async Task<bool> DeleteUserAsync(Guid userId)
		{
		_logger.LogInformation("Starting account deletion for user {UserId}", userId);

		// Finds the user to delete
		ApplicationUser? user = await _userManager.FindByIdAsync(userId.ToString());

		if (user == null)
		{
			_logger.LogWarning("User {UserId} not found, cannot delete.", userId);
			return false;
		}

		// Finds the "deleted user"
		ApplicationUser? deletedPlaceholder = await _userManager.Users
			.FirstOrDefaultAsync(x => x.IsDeletedPlaceholder);

		if (deletedPlaceholder == null)
		{
			_logger.LogError("Deleted User placeholder not found. Cannot proceed with account deletion for user {UserId}.", userId);
			return false;
		}

		// Reassigns authored genres
		ICollection<Genre> authoredGenres = _genreRepository.getGenresByAuthorId(userId);

		foreach (Genre genre in authoredGenres)
		{
			genre.AuthorId = deletedPlaceholder.Id;
		}

        await _dbContext.SaveChangesAsync();

        // Deletes the user
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