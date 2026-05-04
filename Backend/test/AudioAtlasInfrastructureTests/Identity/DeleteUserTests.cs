using AudioAtlasDomain.Genres;
using AudioAtlasDomain.Users;
using AudioAtlasApplication.Repositories;
using AudioAtlasInfrastructure.Services;
using AudioAtlasView.Controllers;
using AudioAtlasApplication.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace AudioAtlasInfrastructureTests.Identity;

public class UserDeletionServiceTests
{
    private Mock<UserManager<ApplicationUser>> CreateUserManager()
    {
        var store = new Mock<IUserStore<ApplicationUser>>();
        return new Mock<UserManager<ApplicationUser>>(
            store.Object,
            null!, null!, null!, null!, null!, null!, null!, null!);
    }

    [Fact]
    public async Task DeleteUserAsync_WhenUserNotFound_ReturnsFalse()
    {
        var userManager = CreateUserManager();

        userManager
            .Setup(x => x.FindByIdAsync(It.IsAny<string>()))
            .ReturnsAsync((ApplicationUser?)null);

        var genreRepository = new Mock<IGenreRepository>();
        var logger = new Mock<ILogger<UserDeletionService>>();

        var service = new UserDeletionService(
            userManager.Object,
            genreRepository.Object,
            logger.Object);

        var result = await service.DeleteUserAsync(Guid.NewGuid());

        Assert.False(result);
    }

    [Fact]
    public async Task DeleteUserAsync_WhenDeletedPlaceholderNotFound_ReturnsFalse()
    {
        var userManager = CreateUserManager();
        var existingUser = new ApplicationUser { Id = Guid.NewGuid(), UserName = "testuser" };

        userManager
            .Setup(x => x.FindByIdAsync(existingUser.Id.ToString()))
            .ReturnsAsync(existingUser);

        userManager
            .Setup(x => x.Users)
            .Returns(new List<ApplicationUser>().AsQueryable());

        var genreRepository = new Mock<IGenreRepository>();
        var logger = new Mock<ILogger<UserDeletionService>>();

        var service = new UserDeletionService(
            userManager.Object,
            genreRepository.Object,
            logger.Object);

        var result = await service.DeleteUserAsync(existingUser.Id);

        Assert.False(result);
    }

    [Fact]
    public async Task DeleteUserAsync_WhenUserHasAuthoredGenres_ReassignsThemToPlaceholder()
    {
        var userManager = CreateUserManager();

        var userId = Guid.NewGuid();
        var placeholderId = Guid.NewGuid();

        var existingUser = new ApplicationUser { Id = userId, UserName = "testuser" };
        var placeholder = new ApplicationUser { Id = placeholderId, IsDeletedPlaceholder = true };

        var authoredGenres = new List<Genre>
        {
            new Genre { Id = Guid.NewGuid(), Name = "Jazz", AuthorId = userId },
            new Genre { Id = Guid.NewGuid(), Name = "Blues", AuthorId = userId }
        };

        userManager
            .Setup(x => x.FindByIdAsync(userId.ToString()))
            .ReturnsAsync(existingUser);

        userManager
            .Setup(x => x.Users)
            .Returns(new List<ApplicationUser> { placeholder }.AsQueryable());

        userManager
            .Setup(x => x.DeleteAsync(existingUser))
            .ReturnsAsync(IdentityResult.Success);

        var genreRepository = new Mock<IGenreRepository>();

        genreRepository
            .Setup(x => x.getGenresByAuthorId(userId))
            .Returns(authoredGenres);

        var logger = new Mock<ILogger<UserDeletionService>>();

        var service = new UserDeletionService(
            userManager.Object,
            genreRepository.Object,
            logger.Object);

        await service.DeleteUserAsync(userId);

        Assert.All(authoredGenres, g => Assert.Equal(placeholderId, g.AuthorId));
    }

    [Fact]
    public async Task DeleteUserAsync_WhenUserHasAuthoredGenres_CallsSaveChanges()
    {
        var userManager = CreateUserManager();

        var userId = Guid.NewGuid();
        var existingUser = new ApplicationUser { Id = userId, UserName = "testuser" };
        var placeholder = new ApplicationUser { Id = Guid.NewGuid(), IsDeletedPlaceholder = true };

        userManager
            .Setup(x => x.FindByIdAsync(userId.ToString()))
            .ReturnsAsync(existingUser);

        userManager
            .Setup(x => x.Users)
            .Returns(new List<ApplicationUser> { placeholder }.AsQueryable());

        userManager
            .Setup(x => x.DeleteAsync(existingUser))
            .ReturnsAsync(IdentityResult.Success);

        var genreRepository = new Mock<IGenreRepository>();

        genreRepository
            .Setup(x => x.getGenresByAuthorId(userId))
            .Returns(new List<Genre>
            {
                new Genre { Id = Guid.NewGuid(), Name = "Jazz", AuthorId = userId }
            });

        var logger = new Mock<ILogger<UserDeletionService>>();

        var service = new UserDeletionService(
            userManager.Object,
            genreRepository.Object,
            logger.Object);

        await service.DeleteUserAsync(userId);

        genreRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteUserAsync_WhenUserHasNoAuthoredGenres_StillSucceeds()
    {
        var userManager = CreateUserManager();

        var userId = Guid.NewGuid();
        var existingUser = new ApplicationUser { Id = userId, UserName = "testuser" };
        var placeholder = new ApplicationUser { Id = Guid.NewGuid(), IsDeletedPlaceholder = true };

        userManager
            .Setup(x => x.FindByIdAsync(userId.ToString()))
            .ReturnsAsync(existingUser);

        userManager
            .Setup(x => x.Users)
            .Returns(new List<ApplicationUser> { placeholder }.AsQueryable());

        userManager
            .Setup(x => x.DeleteAsync(existingUser))
            .ReturnsAsync(IdentityResult.Success);

        var genreRepository = new Mock<IGenreRepository>();

        genreRepository
            .Setup(x => x.getGenresByAuthorId(userId))
            .Returns(new List<Genre>());

        var logger = new Mock<ILogger<UserDeletionService>>();

        var service = new UserDeletionService(
            userManager.Object,
            genreRepository.Object,
            logger.Object);

        var result = await service.DeleteUserAsync(userId);

        Assert.True(result);
    }

    [Fact]
    public async Task DeleteUserAsync_WhenDeletionSucceeds_ReturnsTrue()
    {
        var userManager = CreateUserManager();

        var userId = Guid.NewGuid();
        var existingUser = new ApplicationUser { Id = userId, UserName = "testuser" };
        var placeholder = new ApplicationUser { Id = Guid.NewGuid(), IsDeletedPlaceholder = true };

        userManager
            .Setup(x => x.FindByIdAsync(userId.ToString()))
            .ReturnsAsync(existingUser);

        userManager
            .Setup(x => x.Users)
            .Returns(new List<ApplicationUser> { placeholder }.AsQueryable());

        userManager
            .Setup(x => x.DeleteAsync(existingUser))
            .ReturnsAsync(IdentityResult.Success);

        var genreRepository = new Mock<IGenreRepository>();

        genreRepository
            .Setup(x => x.getGenresByAuthorId(userId))
            .Returns(new List<Genre>());

        var logger = new Mock<ILogger<UserDeletionService>>();

        var service = new UserDeletionService(
            userManager.Object,
            genreRepository.Object,
            logger.Object);

        var result = await service.DeleteUserAsync(userId);

        Assert.True(result);
    }

    [Fact]
    public async Task DeleteUserAsync_WhenDeletionFails_ReturnsFalse()
    {
        var userManager = CreateUserManager();

        var userId = Guid.NewGuid();
        var existingUser = new ApplicationUser { Id = userId, UserName = "testuser" };
        var placeholder = new ApplicationUser { Id = Guid.NewGuid(), IsDeletedPlaceholder = true };

        userManager
            .Setup(x => x.FindByIdAsync(userId.ToString()))
            .ReturnsAsync(existingUser);

        userManager
            .Setup(x => x.Users)
            .Returns(new List<ApplicationUser> { placeholder }.AsQueryable());

        userManager
            .Setup(x => x.DeleteAsync(existingUser))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError
            {
                Description = "Deletion failed"
            }));

        var genreRepository = new Mock<IGenreRepository>();

        genreRepository
            .Setup(x => x.getGenresByAuthorId(userId))
            .Returns(new List<Genre>());

        var logger = new Mock<ILogger<UserDeletionService>>();

        var service = new UserDeletionService(
            userManager.Object,
            genreRepository.Object,
            logger.Object);

        var result = await service.DeleteUserAsync(userId);

        Assert.False(result);
    }

    [Fact]
    public async Task DeleteUserAsync_ShouldCallDeleteAsync_OnlyOnce()
    {
        var userManager = CreateUserManager();

        var userId = Guid.NewGuid();
        var existingUser = new ApplicationUser { Id = userId, UserName = "testuser" };
        var placeholder = new ApplicationUser { Id = Guid.NewGuid(), IsDeletedPlaceholder = true };

        userManager
            .Setup(x => x.FindByIdAsync(userId.ToString()))
            .ReturnsAsync(existingUser);

        userManager
            .Setup(x => x.Users)
            .Returns(new List<ApplicationUser> { placeholder }.AsQueryable());

        userManager
            .Setup(x => x.DeleteAsync(existingUser))
            .ReturnsAsync(IdentityResult.Success);

        var genreRepository = new Mock<IGenreRepository>();

        genreRepository
            .Setup(x => x.getGenresByAuthorId(userId))
            .Returns(new List<Genre>());

        var logger = new Mock<ILogger<UserDeletionService>>();

        var service = new UserDeletionService(
            userManager.Object,
            genreRepository.Object,
            logger.Object);

        await service.DeleteUserAsync(userId);

        userManager.Verify(x => x.DeleteAsync(existingUser), Times.Once);
    }

    [Fact]
    public async Task DeleteAccount_WhenUserIdIsMissing_ReturnsUnauthorized()
    {
        var service = new Mock<IUserDeletionService>();

        var controller = new DeleteUserController(service.Object);

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity())
            }
        };

        var result = await controller.DeleteAccount();

        Assert.IsType<UnauthorizedResult>(result);
    }

    [Fact]
    public async Task DeleteAccount_WhenUserIdIsInvalid_ReturnsBadRequest()
    {
        var service = new Mock<IUserDeletionService>();

        var controller = new DeleteUserController(service.Object);

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, "not-a-guid")
                }))
            }
        };

        var result = await controller.DeleteAccount();

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task DeleteAccount_WhenDeletionSucceeds_ReturnsOk()
    {
        var userId = Guid.NewGuid();
        var service = new Mock<IUserDeletionService>();

        service
            .Setup(x => x.DeleteUserAsync(userId))
            .ReturnsAsync(true);

        var controller = new DeleteUserController(service.Object);

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userId.ToString())
                }))
            }
        };

        var result = await controller.DeleteAccount();

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
        Assert.Contains("successfully deleted", okResult.Value.ToString());
    }

    [Fact]
    public async Task DeleteAccount_WhenDeletionFails_Returns500()
    {
        var userId = Guid.NewGuid();
        var service = new Mock<IUserDeletionService>();

        service
            .Setup(x => x.DeleteUserAsync(userId))
            .ReturnsAsync(false);

        var controller = new DeleteUserController(service.Object);

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userId.ToString())
                }))
            }
        };

        var result = await controller.DeleteAccount();

        var statusResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusResult.StatusCode);
    }
}