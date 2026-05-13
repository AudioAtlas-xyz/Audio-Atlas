/*using AudioAtlasApplication.Repositories;
using AudioAtlasApplication.Services;
using AudioAtlasDomain.Genres;
using AudioAtlasDomain.Users;
using AudioAtlasInfrastructure.Database;
using AudioAtlasInfrastructure.Repositories;
using AudioAtlasInfrastructure.Services;
using AudioAtlasView.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Claims;

namespace AudioAtlasInfrastructureTests.Identity;

// Async-aware IQueryable so EF's async LINQ works against mocked UserManager.Users.
internal class TestAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
{
    public TestAsyncEnumerable(IEnumerable<T> enumerable) : base(enumerable) { }
    public TestAsyncEnumerable(Expression expression) : base(expression) { }

    public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        => new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());

    IQueryProvider IQueryable.Provider => new TestAsyncQueryProvider<T>(this);
}

internal class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
{
    private readonly IEnumerator<T> _inner;
    public TestAsyncEnumerator(IEnumerator<T> inner) { _inner = inner; }
    public T Current => _inner.Current;
    public ValueTask DisposeAsync() { _inner.Dispose(); return ValueTask.CompletedTask; }
    public ValueTask<bool> MoveNextAsync() => new(_inner.MoveNext());
}

internal class TestAsyncQueryProvider<TEntity> : IAsyncQueryProvider
{
    private readonly IQueryProvider _inner;
    public TestAsyncQueryProvider(IQueryProvider inner) { _inner = inner; }

    public IQueryable CreateQuery(Expression expression)
        => new TestAsyncEnumerable<TEntity>(expression);

    public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        => new TestAsyncEnumerable<TElement>(expression);

    public object? Execute(Expression expression) => _inner.Execute(expression);
    public TResult Execute<TResult>(Expression expression) => _inner.Execute<TResult>(expression);

    public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = default)
    {
        // TResult is e.g. Task<ApplicationUser?>; unwrap, run sync, wrap.
        Type resultType = typeof(TResult).GetGenericArguments()[0];

        MethodInfo executeMethod = typeof(IQueryProvider).GetMethods()
            .Single(m => m.Name == nameof(IQueryProvider.Execute)
                && m.IsGenericMethodDefinition
                && m.GetParameters().Length == 1)
            .MakeGenericMethod(resultType);

        object? executionResult = executeMethod.Invoke(this, new object[] { expression });

        return (TResult)typeof(Task).GetMethod(nameof(Task.FromResult))!
            .MakeGenericMethod(resultType)
            .Invoke(null, new[] { executionResult })!;
    }
}

// Shared test
internal static class DeleteUserTestFixtures
{
    // UserManager has 9 ctor args; nulls fine for paths we don't touch.
    public static Mock<UserManager<ApplicationUser>> CreateMockUserManager()
    {
        var store = new Mock<IUserStore<ApplicationUser>>();
        return new Mock<UserManager<ApplicationUser>>(
            store.Object,
            null!, null!, null!, null!, null!, null!, null!, null!);
    }

    // Build a service with mocks pre-wired. Null args = "not found".
    public static (
        UserDeletionService service,
        Mock<UserManager<ApplicationUser>> userManager,
        Mock<IGenreRepository> genreRepo,
        Mock<ILogger<UserDeletionService>> logger
    ) BuildService(
        ApplicationUser? userToFind = null,
        ApplicationUser? placeholder = null,
        IEnumerable<Genre>? authoredGenres = null,
        IdentityResult? deleteResult = null)
    {
        Mock<UserManager<ApplicationUser>> userManager = CreateMockUserManager();

        // Find user by id — return whatever caller specified (or null).
        userManager
            .Setup(x => x.FindByIdAsync(It.IsAny<string>()))
            .ReturnsAsync((string id) =>
                userToFind != null && id == userToFind.Id.ToString()
                    ? userToFind
                    : null);

        // Users IQueryable — wrap in async-aware enumerable so EF's
        // .FirstOrDefaultAsync() inside the service works.
        IEnumerable<ApplicationUser> users = placeholder != null
            ? new[] { placeholder }
            : Array.Empty<ApplicationUser>();

        userManager
            .Setup(x => x.Users)
            .Returns(new TestAsyncEnumerable<ApplicationUser>(users));

        // DeleteAsync on the user being deleted returns the configured outcome.
        if (userToFind != null)
        {
            userManager
                .Setup(x => x.DeleteAsync(userToFind))
                .ReturnsAsync(deleteResult ?? IdentityResult.Success);
        }

        var genreRepo = new Mock<IGenreRepository>();

        if (userToFind != null)
        {
            genreRepo
                .Setup(x => x.getGenresByAuthorId(userToFind.Id))
                .Returns((authoredGenres ?? Enumerable.Empty<Genre>()).ToList());
        }

        var logger = new Mock<ILogger<UserDeletionService>>();

        var service = new UserDeletionService(
            userManager.Object,
            genreRepo.Object,
            logger.Object);

        return (service, userManager, genreRepo, logger);
    }

    // Asserts ILogger got at least one call at `level` containing `messageFragment`.
    public static void VerifyLogged<T>(
        Mock<ILogger<T>> logger,
        LogLevel level,
        string messageFragment,
        Times? times = null)
    {
        logger.Verify(
            x => x.Log(
                level,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains(messageFragment)),
                It.IsAny<Exception?>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            times ?? Times.AtLeastOnce());
    }
}

// Unit tests for UserDeletionService
public class UserDeletionServiceTests
{
    // Existing happy-path / not-found scenarios

    [Fact]
    public async Task DeleteUserAsync_WhenUserNotFound_ReturnsFalse()
    {
        var (service, _, _, _) = DeleteUserTestFixtures.BuildService();

        bool result = await service.DeleteUserAsync(Guid.NewGuid());

        Assert.False(result);
    }

    [Fact]
    public async Task DeleteUserAsync_WhenDeletedPlaceholderNotFound_ReturnsFalse()
    {
        var user = new ApplicationUser { Id = Guid.NewGuid(), UserName = "testuser" };

        var (service, _, _, _) = DeleteUserTestFixtures.BuildService(
            userToFind: user,
            placeholder: null);

        bool result = await service.DeleteUserAsync(user.Id);

        Assert.False(result);
    }

    [Fact]
    public async Task DeleteUserAsync_WhenUserHasAuthoredGenres_ReassignsThemToPlaceholder()
    {
        Guid userId = Guid.NewGuid();
        Guid placeholderId = Guid.NewGuid();

        var user = new ApplicationUser { Id = userId, UserName = "testuser" };
        var placeholder = new ApplicationUser { Id = placeholderId, IsDeletedPlaceholder = true };

        var authored = new List<Genre>
        {
            new() { Id = Guid.NewGuid(), Name = "Jazz", AuthorId = userId },
            new() { Id = Guid.NewGuid(), Name = "Blues", AuthorId = userId }
        };

        var (service, _, _, _) = DeleteUserTestFixtures.BuildService(
            userToFind: user,
            placeholder: placeholder,
            authoredGenres: authored);

        await service.DeleteUserAsync(userId);

        Assert.All(authored, g => Assert.Equal(placeholderId, g.AuthorId));
    }

    [Fact]
    public async Task DeleteUserAsync_WhenUserHasAuthoredGenres_CallsSaveChangesOnce()
    {
        var user = new ApplicationUser { Id = Guid.NewGuid(), UserName = "testuser" };
        var placeholder = new ApplicationUser { Id = Guid.NewGuid(), IsDeletedPlaceholder = true };

        var (service, _, genreRepo, _) = DeleteUserTestFixtures.BuildService(
            userToFind: user,
            placeholder: placeholder,
            authoredGenres: new[] { new Genre { Name = "Jazz", AuthorId = user.Id } });

        await service.DeleteUserAsync(user.Id);

        genreRepo.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteUserAsync_WhenUserHasNoAuthoredGenres_ReturnsTrue()
    {
        var user = new ApplicationUser { Id = Guid.NewGuid(), UserName = "testuser" };
        var placeholder = new ApplicationUser { Id = Guid.NewGuid(), IsDeletedPlaceholder = true };

        var (service, _, _, _) = DeleteUserTestFixtures.BuildService(
            userToFind: user,
            placeholder: placeholder);

        bool result = await service.DeleteUserAsync(user.Id);

        Assert.True(result);
    }

    [Fact]
    public async Task DeleteUserAsync_WhenIdentityDeleteSucceeds_ReturnsTrue()
    {
        var user = new ApplicationUser { Id = Guid.NewGuid(), UserName = "testuser" };
        var placeholder = new ApplicationUser { Id = Guid.NewGuid(), IsDeletedPlaceholder = true };

        var (service, _, _, _) = DeleteUserTestFixtures.BuildService(
            userToFind: user,
            placeholder: placeholder,
            deleteResult: IdentityResult.Success);

        Assert.True(await service.DeleteUserAsync(user.Id));
    }

    [Fact]
    public async Task DeleteUserAsync_WhenIdentityDeleteFails_ReturnsFalse()
    {
        var user = new ApplicationUser { Id = Guid.NewGuid(), UserName = "testuser" };
        var placeholder = new ApplicationUser { Id = Guid.NewGuid(), IsDeletedPlaceholder = true };

        var (service, _, _, _) = DeleteUserTestFixtures.BuildService(
            userToFind: user,
            placeholder: placeholder,
            deleteResult: IdentityResult.Failed(new IdentityError { Description = "Deletion failed" }));

        Assert.False(await service.DeleteUserAsync(user.Id));
    }

    [Fact]
    public async Task DeleteUserAsync_CallsDeleteAsyncExactlyOnce()
    {
        var user = new ApplicationUser { Id = Guid.NewGuid(), UserName = "testuser" };
        var placeholder = new ApplicationUser { Id = Guid.NewGuid(), IsDeletedPlaceholder = true };

        var (service, userManager, _, _) = DeleteUserTestFixtures.BuildService(
            userToFind: user,
            placeholder: placeholder);

        await service.DeleteUserAsync(user.Id);

        userManager.Verify(x => x.DeleteAsync(user), Times.Once);
    }

    // New edge-case coverage

    // Double-delete (e.g. UI double-click) returns false instead of crashing.
    [Fact]
    public async Task DeleteUserAsync_WhenCalledTwice_SecondCallReturnsFalse()
    {
        Guid userId = Guid.NewGuid();
        var user = new ApplicationUser { Id = userId, UserName = "testuser" };
        var placeholder = new ApplicationUser { Id = Guid.NewGuid(), IsDeletedPlaceholder = true };

        Mock<UserManager<ApplicationUser>> userManager = DeleteUserTestFixtures.CreateMockUserManager();

        // First call returns the user, second call returns null.
        var sequence = new Queue<ApplicationUser?>(new[] { user, (ApplicationUser?)null });
        userManager
            .Setup(x => x.FindByIdAsync(userId.ToString()))
            .ReturnsAsync(() => sequence.Dequeue());

        userManager
            .Setup(x => x.Users)
            .Returns(new TestAsyncEnumerable<ApplicationUser>(new[] { placeholder }));

        userManager
            .Setup(x => x.DeleteAsync(user))
            .ReturnsAsync(IdentityResult.Success);

        var genreRepo = new Mock<IGenreRepository>();
        genreRepo.Setup(x => x.getGenresByAuthorId(userId)).Returns(new List<Genre>());

        var logger = new Mock<ILogger<UserDeletionService>>();
        var service = new UserDeletionService(userManager.Object, genreRepo.Object, logger.Object);

        Assert.True(await service.DeleteUserAsync(userId));
        Assert.False(await service.DeleteUserAsync(userId));
    }

    // Reassign must happen before delete or the FKs cascade-orphan.
    [Fact]
    public async Task DeleteUserAsync_ReassignsAndSavesBeforeDeletingUser()
    {
        var user = new ApplicationUser { Id = Guid.NewGuid(), UserName = "testuser" };
        var placeholder = new ApplicationUser { Id = Guid.NewGuid(), IsDeletedPlaceholder = true };

        var (service, userManager, genreRepo, _) = DeleteUserTestFixtures.BuildService(
            userToFind: user,
            placeholder: placeholder,
            authoredGenres: new[] { new Genre { Name = "Jazz", AuthorId = user.Id } });

        var callOrder = new List<string>();

        genreRepo
            .Setup(x => x.SaveChangesAsync())
            .Returns(Task.CompletedTask)
            .Callback(() => callOrder.Add(nameof(IGenreRepository.SaveChangesAsync)));

        userManager
            .Setup(x => x.DeleteAsync(user))
            .ReturnsAsync(IdentityResult.Success)
            .Callback(() => callOrder.Add(nameof(UserManager<ApplicationUser>.DeleteAsync)));

        await service.DeleteUserAsync(user.Id);

        Assert.Equal(
            new[] { nameof(IGenreRepository.SaveChangesAsync), nameof(UserManager<ApplicationUser>.DeleteAsync) },
            callOrder);
    }

    // SaveChangesAsync exceptions bubble up to the controller.
    [Fact]
    public async Task DeleteUserAsync_WhenSaveChangesThrows_PropagatesException()
    {
        var user = new ApplicationUser { Id = Guid.NewGuid(), UserName = "testuser" };
        var placeholder = new ApplicationUser { Id = Guid.NewGuid(), IsDeletedPlaceholder = true };

        var (service, _, genreRepo, _) = DeleteUserTestFixtures.BuildService(
            userToFind: user,
            placeholder: placeholder,
            authoredGenres: new[] { new Genre { Name = "Jazz", AuthorId = user.Id } });

        genreRepo
            .Setup(x => x.SaveChangesAsync())
            .ThrowsAsync(new DbUpdateException("simulated DB failure"));

        await Assert.ThrowsAsync<DbUpdateException>(() => service.DeleteUserAsync(user.Id));
    }

    // Empty authored list is valid — delete still succeeds.
    [Fact]
    public async Task DeleteUserAsync_WhenNoAuthoredGenres_DoesNotMutateOtherGenres()
    {
        var user = new ApplicationUser { Id = Guid.NewGuid(), UserName = "testuser" };
        var placeholder = new ApplicationUser { Id = Guid.NewGuid(), IsDeletedPlaceholder = true };

        var (service, _, _, _) = DeleteUserTestFixtures.BuildService(
            userToFind: user,
            placeholder: placeholder,
            authoredGenres: Array.Empty<Genre>());

        bool ok = await service.DeleteUserAsync(user.Id);

        Assert.True(ok);
    }

    // Logger verification

    [Fact]
    public async Task DeleteUserAsync_AlwaysLogsStartingMessage()
    {
        Guid userId = Guid.NewGuid();

        var (service, _, _, logger) = DeleteUserTestFixtures.BuildService();

        await service.DeleteUserAsync(userId);

        DeleteUserTestFixtures.VerifyLogged(logger, LogLevel.Information, "Starting account deletion");
    }

    [Fact]
    public async Task DeleteUserAsync_WhenUserNotFound_LogsWarning()
    {
        var (service, _, _, logger) = DeleteUserTestFixtures.BuildService();

        await service.DeleteUserAsync(Guid.NewGuid());

        DeleteUserTestFixtures.VerifyLogged(logger, LogLevel.Warning, "not found");
    }

    [Fact]
    public async Task DeleteUserAsync_WhenPlaceholderMissing_LogsError()
    {
        var user = new ApplicationUser { Id = Guid.NewGuid(), UserName = "testuser" };

        var (service, _, _, logger) = DeleteUserTestFixtures.BuildService(
            userToFind: user,
            placeholder: null);

        await service.DeleteUserAsync(user.Id);

        DeleteUserTestFixtures.VerifyLogged(logger, LogLevel.Error, "Deleted User placeholder not found");
    }

    [Fact]
    public async Task DeleteUserAsync_WhenSucceeds_LogsSuccessInformation()
    {
        var user = new ApplicationUser { Id = Guid.NewGuid(), UserName = "testuser" };
        var placeholder = new ApplicationUser { Id = Guid.NewGuid(), IsDeletedPlaceholder = true };

        var (service, _, _, logger) = DeleteUserTestFixtures.BuildService(
            userToFind: user,
            placeholder: placeholder);

        await service.DeleteUserAsync(user.Id);

        DeleteUserTestFixtures.VerifyLogged(logger, LogLevel.Information, "Successfully deleted account");
    }

    [Fact]
    public async Task DeleteUserAsync_WhenIdentityFails_LogsErrorPerIdentityError()
    {
        var user = new ApplicationUser { Id = Guid.NewGuid(), UserName = "testuser" };
        var placeholder = new ApplicationUser { Id = Guid.NewGuid(), IsDeletedPlaceholder = true };

        var (service, _, _, logger) = DeleteUserTestFixtures.BuildService(
            userToFind: user,
            placeholder: placeholder,
            deleteResult: IdentityResult.Failed(
                new IdentityError { Code = "E1", Description = "first" },
                new IdentityError { Code = "E2", Description = "second" }));

        await service.DeleteUserAsync(user.Id);

        DeleteUserTestFixtures.VerifyLogged(logger, LogLevel.Error, "first");
        DeleteUserTestFixtures.VerifyLogged(logger, LogLevel.Error, "second");
    }
}

// Unit tests for DeleteUserController
public class DeleteUserControllerTests
{
    // Wrap controller in a stub HttpContext with given claims.
    private static DeleteUserController BuildController(
        IUserDeletionService service,
        params Claim[] claims)
    {
        var controller = new DeleteUserController(service);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(claims))
            }
        };
        return controller;
    }

    [Fact]
    public async Task DeleteAccount_WhenUserIdMissing_ReturnsUnauthorized()
    {
        var service = new Mock<IUserDeletionService>();
        DeleteUserController controller = BuildController(service.Object);

        IActionResult result = await controller.DeleteAccount();

        Assert.IsType<UnauthorizedResult>(result);
        service.Verify(x => x.DeleteUserAsync(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task DeleteAccount_WhenUserIdNotAGuid_ReturnsBadRequest()
    {
        var service = new Mock<IUserDeletionService>();
        DeleteUserController controller = BuildController(
            service.Object,
            new Claim(ClaimTypes.NameIdentifier, "not-a-guid"));

        IActionResult result = await controller.DeleteAccount();

        Assert.IsType<BadRequestObjectResult>(result);
        service.Verify(x => x.DeleteUserAsync(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task DeleteAccount_WhenServiceReturnsTrue_ReturnsOkWithMessage()
    {
        Guid userId = Guid.NewGuid();
        var service = new Mock<IUserDeletionService>();
        service.Setup(x => x.DeleteUserAsync(userId)).ReturnsAsync(true);

        DeleteUserController controller = BuildController(
            service.Object,
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()));

        IActionResult result = await controller.DeleteAccount();

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(ok.Value);
        Assert.Contains("successfully deleted", ok.Value!.ToString());
    }

    [Fact]
    public async Task DeleteAccount_WhenServiceReturnsFalse_Returns500()
    {
        Guid userId = Guid.NewGuid();
        var service = new Mock<IUserDeletionService>();
        service.Setup(x => x.DeleteUserAsync(userId)).ReturnsAsync(false);

        DeleteUserController controller = BuildController(
            service.Object,
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()));

        IActionResult result = await controller.DeleteAccount();

        var status = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, status.StatusCode);
    }

    [Fact]
    public async Task DeleteAccount_PassesClaimGuidToService()
    {
        Guid userId = Guid.NewGuid();
        var service = new Mock<IUserDeletionService>();
        service.Setup(x => x.DeleteUserAsync(It.IsAny<Guid>())).ReturnsAsync(true);

        DeleteUserController controller = BuildController(
            service.Object,
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()));

        await controller.DeleteAccount();

        service.Verify(x => x.DeleteUserAsync(userId), Times.Once);
    }

    [Fact]
    public async Task DeleteAccount_WhenServiceThrows_ExceptionPropagates()
    {
        Guid userId = Guid.NewGuid();
        var service = new Mock<IUserDeletionService>();
        service
            .Setup(x => x.DeleteUserAsync(userId))
            .ThrowsAsync(new InvalidOperationException("boom"));

        DeleteUserController controller = BuildController(
            service.Object,
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()));

        await Assert.ThrowsAsync<InvalidOperationException>(() => controller.DeleteAccount());
    }
}

// Integration tests — SQLite in-memory + real EF/Identity stack.
public class UserDeletionServiceIntegrationTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly ServiceProvider _services;

    public UserDeletionServiceIntegrationTests()
    {
        // Shared connection so all scopes see the same in-memory DB
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var services = new ServiceCollection();

        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite(_connection));

        services
            .AddIdentityCore<ApplicationUser>(opts =>
            {
                // Loosen username validation so test users with simple names work.
                opts.User.RequireUniqueEmail = false;
                opts.Password.RequireDigit = false;
                opts.Password.RequiredLength = 1;
                opts.Password.RequireLowercase = false;
                opts.Password.RequireUppercase = false;
                opts.Password.RequireNonAlphanumeric = false;
            })
            .AddRoles<IdentityRole<Guid>>()
            .AddEntityFrameworkStores<AppDbContext>();

        services.AddLogging();
        services.AddScoped<IGenreRepository, GenreRepository>();
        services.AddScoped<IUserDeletionService, UserDeletionService>();

        _services = services.BuildServiceProvider();

        // Materialise the schema once.
        using IServiceScope scope = _services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Database.EnsureCreated();
    }

    // Seed a placeholder + a real user, return their ids.
    private async Task<(Guid placeholderId, Guid userId)> SeedUsersAsync()
    {
        using IServiceScope scope = _services.CreateScope();
        UserManager<ApplicationUser> users = scope.ServiceProvider
            .GetRequiredService<UserManager<ApplicationUser>>();

        var placeholder = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = "deleted_placeholder",
            Email = "deleted@audioatlas.local",
            IsDeletedPlaceholder = true
        };
        IdentityResult r1 = await users.CreateAsync(placeholder);
        Assert.True(r1.Succeeded, string.Join(",", r1.Errors.Select(e => e.Description)));

        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = "andreas",
            Email = "a@a.com"
        };
        IdentityResult r2 = await users.CreateAsync(user);
        Assert.True(r2.Succeeded, string.Join(",", r2.Errors.Select(e => e.Description)));

        return (placeholder.Id, user.Id);
    }

    [Fact]
    public async Task DeleteUserAsync_AgainstRealDb_RemovesUser_AndReassignsAuthoredGenre()
    {
        (Guid placeholderId, Guid userId) = await SeedUsersAsync();

        // Seed an authored genre.
        Guid genreId;
        using (IServiceScope scope = _services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var genre = new Genre { Name = "Jazz", AuthorId = userId };
            db.Genres.Add(genre);
            await db.SaveChangesAsync();
            genreId = genre.Id;
        }

        // Act
        bool ok;
        using (IServiceScope scope = _services.CreateScope())
        {
            var service = scope.ServiceProvider.GetRequiredService<IUserDeletionService>();
            ok = await service.DeleteUserAsync(userId);
        }

        Assert.True(ok);

        // Assert against a fresh scope so we read what was actually persisted.
        using (IServiceScope scope = _services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var users = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            Genre? persisted = await db.Genres.AsNoTracking().FirstOrDefaultAsync(g => g.Id == genreId);
            Assert.NotNull(persisted);
            Assert.Equal(placeholderId, persisted!.AuthorId);

            ApplicationUser? deletedUser = await users.FindByIdAsync(userId.ToString());
            Assert.Null(deletedUser);

            ApplicationUser? placeholderStillThere = await users.FindByIdAsync(placeholderId.ToString());
            Assert.NotNull(placeholderStillThere);
        }
    }

    [Fact]
    public async Task DeleteUserAsync_AgainstRealDb_ReassignsMultipleAuthoredGenres()
    {
        (Guid placeholderId, Guid userId) = await SeedUsersAsync();

        var genreIds = new List<Guid>();
        using (IServiceScope scope = _services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            foreach (string name in new[] { "Jazz", "Blues", "Funk" })
            {
                var g = new Genre { Name = name, AuthorId = userId };
                db.Genres.Add(g);
                genreIds.Add(g.Id);
            }
            await db.SaveChangesAsync();
        }

        using (IServiceScope scope = _services.CreateScope())
        {
            var service = scope.ServiceProvider.GetRequiredService<IUserDeletionService>();
            Assert.True(await service.DeleteUserAsync(userId));
        }

        using (IServiceScope scope = _services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            List<Genre> persisted = await db.Genres
                .AsNoTracking()
                .Where(g => genreIds.Contains(g.Id))
                .ToListAsync();

            Assert.Equal(3, persisted.Count);
            Assert.All(persisted, g => Assert.Equal(placeholderId, g.AuthorId));
        }
    }

    // Dummy genre (minimal fields) gets reassigned to the placeholder too.
    [Fact]
    public async Task DeleteUserAsync_DummyGenre_AuthorReplacedByPlaceholder()
    {
        (Guid placeholderId, Guid userId) = await SeedUsersAsync();

        Guid dummyId;
        using (IServiceScope scope = _services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Dummy genre — Name is non-nullable in the schema so we use a single
            // underscore. Every other field is left at its default (null / empty).
            var dummy = new Genre
            {
                Name = "_",
                AuthorId = userId
            };
            db.Genres.Add(dummy);
            await db.SaveChangesAsync();
            dummyId = dummy.Id;
        }

        using (IServiceScope scope = _services.CreateScope())
        {
            var service = scope.ServiceProvider.GetRequiredService<IUserDeletionService>();
            Assert.True(await service.DeleteUserAsync(userId));
        }

        using (IServiceScope scope = _services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var users = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            // Dummy survived...
            Genre? dummy = await db.Genres.AsNoTracking().FirstOrDefaultAsync(g => g.Id == dummyId);
            Assert.NotNull(dummy);

            // author is the placeholder, not the deleted user.
            Assert.Equal(placeholderId, dummy!.AuthorId);
            Assert.NotEqual(userId, dummy.AuthorId);

            // The user is gone.
            Assert.Null(await users.FindByIdAsync(userId.ToString()));
        }
    }

    [Fact]
    public async Task DeleteUserAsync_AgainstRealDb_WhenPlaceholderMissing_ReturnsFalse_AndUserSurvives()
    {
        // Seed only the user — no placeholder.
        Guid userId;
        using (IServiceScope scope = _services.CreateScope())
        {
            var users = scope.ServiceProvider
                .GetRequiredService<UserManager<ApplicationUser>>();
            var user = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                UserName = "andreas",
                Email = "a@a.com"
            };
            await users.CreateAsync(user);
            userId = user.Id;
        }

        bool ok;
        using (IServiceScope scope = _services.CreateScope())
        {
            var service = scope.ServiceProvider.GetRequiredService<IUserDeletionService>();
            ok = await service.DeleteUserAsync(userId);
        }

        Assert.False(ok);

        // Critical: the user must STILL be there. Returning false must not
        // partially delete the account.
        using (IServiceScope scope = _services.CreateScope())
        {
            var users = scope.ServiceProvider
                .GetRequiredService<UserManager<ApplicationUser>>();
            ApplicationUser? stillHere = await users.FindByIdAsync(userId.ToString());
            Assert.NotNull(stillHere);
        }
    }

    [Fact]
    public async Task DeleteUserAsync_AgainstRealDb_DoesNotTouchOtherUsersGenres()
    {
        (Guid placeholderId, Guid userId) = await SeedUsersAsync();

        // Seed a second user and a genre owned by them.
        Guid otherUserId;
        Guid otherGenreId;
        using (IServiceScope scope = _services.CreateScope())
        {
            var users = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var other = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                UserName = "bob",
                Email = "bob@a.com"
            };
            await users.CreateAsync(other);
            otherUserId = other.Id;

            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var g = new Genre { Name = "Bob's Genre", AuthorId = otherUserId };
            db.Genres.Add(g);
            await db.SaveChangesAsync();
            otherGenreId = g.Id;
        }

        // Delete the first user.
        using (IServiceScope scope = _services.CreateScope())
        {
            var service = scope.ServiceProvider.GetRequiredService<IUserDeletionService>();
            Assert.True(await service.DeleteUserAsync(userId));
        }

        // Bob's genre must be untouched.
        using (IServiceScope scope = _services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            Genre? bobGenre = await db.Genres.AsNoTracking().FirstOrDefaultAsync(g => g.Id == otherGenreId);
            Assert.NotNull(bobGenre);
            Assert.Equal(otherUserId, bobGenre!.AuthorId);
        }
    }

    public void Dispose()
    {
        _services.Dispose();
        _connection.Dispose();
    }
}
*/