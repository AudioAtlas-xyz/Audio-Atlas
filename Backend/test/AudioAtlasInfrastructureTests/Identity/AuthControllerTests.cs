using System.Reflection;
using AudioAtlasDomain.Users;
using AudioAtlasView.Controllers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;
using AudioAtlasInfrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace AudioAtlasInfrastructureTests.Identity;

public class AuthControllerTests
{
    [Theory]
    [InlineData("abc", true)]
    [InlineData("valid_name", true)]
    [InlineData("ab", false)]
    [InlineData("name-with-dash", false)]
    [InlineData("name with space", false)]
    [InlineData("this_username_is_way_too_long", false)]
    public void IsValidUsername_ShouldValidateCorrectly(string username, bool expected)
    {
        // Arrange
        var method = typeof(AuthController)
            .GetMethod("IsValidUsername", BindingFlags.NonPublic | BindingFlags.Static);

        // Act
        var result = (bool)method!.Invoke(null, new object[] { username })!;

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void BuildSuggestedUsername_ShouldCleanInput()
    {
        // Arrange
        var method = typeof(AuthController)
            .GetMethod("BuildSuggestedUsername", BindingFlags.NonPublic | BindingFlags.Static);

        // Act
        var result = (string)method!.Invoke(null, new object[] { "Cool User!!!" })!;

        // Assert
        Assert.Equal("Cool_User", result);
    }

    [Fact]
    public async Task CheckUsername_WhenUsernameIsAvailable_ReturnsAvailableTrue()
    {
        // Arrange
        var store = new Mock<IUserStore<ApplicationUser>>();

        var userManager = new Mock<UserManager<ApplicationUser>>(
            store.Object,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!);

        userManager
            .Setup(x => x.FindByNameAsync("newuser"))
            .Returns(Task.FromResult<ApplicationUser?>(null));

        var controller = new AuthController(
            dbContext: null!,
            userManager: userManager.Object,
            signInManager: null!,
            config: new ConfigurationBuilder().Build());

        // Act
        var result = await controller.CheckUsername("newuser");

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);

        Assert.NotNull(okResult.Value);

        var responseText = okResult.Value.ToString();

        Assert.Contains("True", responseText);
    }

    [Fact]
    public async Task CheckUsername_WhenUsernameExists_ReturnsAvailableFalse()
    {
        // Arrange
        var store = new Mock<IUserStore<ApplicationUser>>();

        var userManager = new Mock<UserManager<ApplicationUser>>(
            store.Object,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!);

        userManager
            .Setup(x => x.FindByNameAsync("takenuser"))
            .Returns(Task.FromResult<ApplicationUser?>(
                new ApplicationUser
                {
                    Id = Guid.NewGuid(),
                    UserName = "takenuser"
                }));

        var controller = new AuthController(
            dbContext: null!,
            userManager: userManager.Object,
            signInManager: null!,
            config: new ConfigurationBuilder().Build());

        // Act
        var result = await controller.CheckUsername("takenuser");

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);

        Assert.NotNull(okResult.Value);

        var responseText = okResult.Value.ToString();

        Assert.Contains("False", responseText);
    }

        [Fact]
    public async Task CompleteOnboarding_WhenPoliciesNotAccepted_ReturnsBadRequest()
    {
        // Arrange
        var controller = new AuthController(
            dbContext: null!,
            userManager: null!,
            signInManager: null!,
            config: new ConfigurationBuilder().Build());

        var request = new CompleteOnboardingRequest
        {
            Username = "newuser",
            AcceptedContributionGuidelines = false,
            AcceptedPrivacyPolicy = true
        };

        // Act
        var result = await controller.CompleteOnboarding(request);

        // Assert
        var badRequest = Assert.IsType<Microsoft.AspNetCore.Mvc.BadRequestObjectResult>(result);
        Assert.Equal("You must accept both policies.", badRequest.Value);
    }

        [Fact]
    public async Task CompleteOnboarding_WhenUsernameIsInvalid_ReturnsBadRequest()
    {
        // Arrange
        var controller = new AuthController(
            dbContext: null!,
            userManager: null!,
            signInManager: null!,
            config: new ConfigurationBuilder().Build());

        var request = new CompleteOnboardingRequest
        {
            Username = "bad username!",
            AcceptedContributionGuidelines = true,
            AcceptedPrivacyPolicy = true
        };

        // Act
        var result = await controller.CompleteOnboarding(request);

        // Assert
        var badRequest = Assert.IsType<Microsoft.AspNetCore.Mvc.BadRequestObjectResult>(result);
        Assert.Equal("Invalid username format.", badRequest.Value);
    }

        [Fact]
    public async Task CompleteOnboarding_WhenUsernameAlreadyExists_ReturnsConflict()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using var dbContext = new AppDbContext(options);

        dbContext.PendingExternalRegistrations.Add(new PendingExternalRegistration
        {
            Id = Guid.NewGuid(),
            Email = "test@test.com",
            LoginProvider = "Google",
            ProviderKey = "123",
            ProviderDisplayName = "Google",
            SuggestedUsername = "takenuser",
            CreatedAtUtc = DateTime.UtcNow,
            ExpiresAtUtc = DateTime.UtcNow.AddMinutes(10)
        });

        await dbContext.SaveChangesAsync();

        var store = new Mock<IUserStore<ApplicationUser>>();

        var userManager = new Mock<UserManager<ApplicationUser>>(
            store.Object,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!);

        userManager
            .Setup(x => x.FindByNameAsync("takenuser"))
            .ReturnsAsync(new ApplicationUser
            {
                Id = Guid.NewGuid(),
                UserName = "takenuser"
            });

        var controller = new AuthController(
            dbContext,
            userManager.Object,
            null!,
            new ConfigurationBuilder().Build());

        var request = new CompleteOnboardingRequest
        {
            PendingRegistrationId = dbContext.PendingExternalRegistrations.First().Id,
            Username = "takenuser",
            AcceptedContributionGuidelines = true,
            AcceptedPrivacyPolicy = true
        };

        // Act
        var result = await controller.CompleteOnboarding(request);

        // Assert
        Assert.IsType<ConflictObjectResult>(result);
    }


        [Fact]
    public async Task CompleteOnboarding_WithValidRequest_ReturnsToken()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using var dbContext = new AppDbContext(options);

        var pendingId = Guid.NewGuid();

        dbContext.PendingExternalRegistrations.Add(new PendingExternalRegistration
        {
            Id = pendingId,
            Email = "test@test.com",
            LoginProvider = "Google",
            ProviderKey = "123",
            ProviderDisplayName = "Google",
            SuggestedUsername = "newuser",
            CreatedAtUtc = DateTime.UtcNow,
            ExpiresAtUtc = DateTime.UtcNow.AddMinutes(10)
        });

        await dbContext.SaveChangesAsync();

        var store = new Mock<IUserStore<ApplicationUser>>();

        var userManager = new Mock<UserManager<ApplicationUser>>(
            store.Object,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!);

        userManager
            .Setup(x => x.FindByNameAsync("newuser"))
            .ReturnsAsync((ApplicationUser?)null);

        userManager
            .Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync(IdentityResult.Success);

        userManager
            .Setup(x => x.AddLoginAsync(
                It.IsAny<ApplicationUser>(),
                It.IsAny<UserLoginInfo>()))
            .ReturnsAsync(IdentityResult.Success);

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "Jwt:Key", "super_secret_test_key_that_is_long_enough_12345" },
                { "Jwt:Issuer", "AudioAtlas" },
                { "Jwt:Audience", "AudioAtlasUsers" }
            })
            .Build();

        var controller = new AuthController(
            dbContext,
            userManager.Object,
            null!,
            config);

        var request = new CompleteOnboardingRequest
        {
            PendingRegistrationId = pendingId,
            Username = "newuser",
            AcceptedContributionGuidelines = true,
            AcceptedPrivacyPolicy = true
        };

        // Act
        var result = await controller.CompleteOnboarding(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);

        Assert.NotNull(okResult.Value);

        var responseText = okResult.Value.ToString();

        Assert.Contains("token", responseText);
    }






    [Fact]
    public async Task CompleteOnboarding_WhenPendingRegistrationDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using var dbContext = new AppDbContext(options);

        var controller = new AuthController(
            dbContext,
            null!,
            null!,
            new ConfigurationBuilder().Build());

        var request = new CompleteOnboardingRequest
        {
            PendingRegistrationId = Guid.NewGuid(),
            Username = "newuser",
            AcceptedContributionGuidelines = true,
            AcceptedPrivacyPolicy = true
        };

        // Act
        var result = await controller.CompleteOnboarding(request);

        // Assert
        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Pending registration not found.", notFound.Value);
    }

    [Fact]
    public async Task CompleteOnboarding_WhenPendingRegistrationIsExpired_ReturnsBadRequest()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using var dbContext = new AppDbContext(options);

        var pendingId = Guid.NewGuid();

        dbContext.PendingExternalRegistrations.Add(new PendingExternalRegistration
        {
            Id = pendingId,
            Email = "test@test.com",
            LoginProvider = "Google",
            ProviderKey = "123",
            ProviderDisplayName = "Google",
            SuggestedUsername = "newuser",
            CreatedAtUtc = DateTime.UtcNow.AddMinutes(-30),
            ExpiresAtUtc = DateTime.UtcNow.AddMinutes(-1)
        });

        await dbContext.SaveChangesAsync();

        var controller = new AuthController(
            dbContext,
            null!,
            null!,
            new ConfigurationBuilder().Build());

        var request = new CompleteOnboardingRequest
        {
            PendingRegistrationId = pendingId,
            Username = "newuser",
            AcceptedContributionGuidelines = true,
            AcceptedPrivacyPolicy = true
        };

        // Act
        var result = await controller.CompleteOnboarding(request);

        // Assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Registration expired.", badRequest.Value);
    }

    [Fact]
public async Task CheckUsername_WhenUsernameFormatIsInvalid_ReturnsAvailableFalse()
{
    // Arrange
    var controller = new AuthController(
        dbContext: null!,
        userManager: null!,
        signInManager: null!,
        config: new ConfigurationBuilder().Build());

    // Act
    var result = await controller.CheckUsername("bad username!");

    // Assert
    var okResult = Assert.IsType<OkObjectResult>(result);

    Assert.NotNull(okResult.Value);

    var responseText = okResult.Value.ToString();

    Assert.Contains("False", responseText);
    Assert.Contains("Username must be 3-20 characters", responseText);
}

    [Fact]
public async Task ExternalCallback_WhenNoExternalLoginInfo_ReturnsUnauthorized()
{
    // Arrange
   var connection = new Microsoft.Data.Sqlite.SqliteConnection("Filename=:memory:");
    await connection.OpenAsync();

    var options = new DbContextOptionsBuilder<AppDbContext>()
        .UseSqlite(connection)
        .Options;

    using var dbContext = new AppDbContext(options);
    await dbContext.Database.EnsureCreatedAsync();

    var store = new Mock<IUserStore<ApplicationUser>>();

    var userManager = new Mock<UserManager<ApplicationUser>>(
        store.Object,
        null!,
        null!,
        null!,
        null!,
        null!,
        null!,
        null!,
        null!);

    var contextAccessor = new Mock<Microsoft.AspNetCore.Http.IHttpContextAccessor>();
    var claimsFactory = new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>();

    var signInManager = new Mock<SignInManager<ApplicationUser>>(
        userManager.Object,
        contextAccessor.Object,
        claimsFactory.Object,
        null!,
        null!,
        null!,
        null!);

    signInManager
        .Setup(x => x.GetExternalLoginInfoAsync(null))
        .ReturnsAsync((ExternalLoginInfo?)null);

    var controller = new AuthController(
        dbContext,
        userManager.Object,
        signInManager.Object,
        new ConfigurationBuilder().Build());

    // Act
    var result = await controller.ExternalCallback();

    // Assert
    Assert.IsType<UnauthorizedResult>(result);
}

   [Fact]
public async Task ExternalCallback_WhenUserAlreadyExists_ReturnsRedirectWithToken()
{
    // Arrange
    var connection = new Microsoft.Data.Sqlite.SqliteConnection("Filename=:memory:");
    await connection.OpenAsync();

    var options = new DbContextOptionsBuilder<AppDbContext>()
        .UseSqlite(connection)
        .Options;

    using var dbContext = new AppDbContext(options);
    await dbContext.Database.EnsureCreatedAsync();

    var existingUser = new ApplicationUser
    {
        Id = Guid.NewGuid(),
        Email = "test@test.com",
        UserName = "existinguser"
    };

    var store = new Mock<IUserStore<ApplicationUser>>();

    var userManager = new Mock<UserManager<ApplicationUser>>(
        store.Object,
        null!,
        null!,
        null!,
        null!,
        null!,
        null!,
        null!,
        null!);

    userManager
        .Setup(x => x.FindByLoginAsync("Google", "123"))
        .ReturnsAsync(existingUser);

    var contextAccessor = new Mock<IHttpContextAccessor>();
    var claimsFactory = new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>();

    var signInManager = new Mock<SignInManager<ApplicationUser>>(
        userManager.Object,
        contextAccessor.Object,
        claimsFactory.Object,
        null!,
        null!,
        null!,
        null!);

    var principal = new System.Security.Claims.ClaimsPrincipal(
        new System.Security.Claims.ClaimsIdentity(new[]
        {
            new System.Security.Claims.Claim(
                System.Security.Claims.ClaimTypes.Email,
                "test@test.com")
        }));

    var externalInfo = new ExternalLoginInfo(
        principal,
        "Google",
        "123",
        "Google");

    signInManager
        .Setup(x => x.GetExternalLoginInfoAsync(null))
        .ReturnsAsync(externalInfo);

    var config = new ConfigurationBuilder()
        .AddInMemoryCollection(new Dictionary<string, string?>
        {
            { "Frontend:BaseUrl", "http://localhost:3000" },
            { "Jwt:Key", "super_secret_test_key_that_is_long_enough_12345" },
            { "Jwt:Issuer", "AudioAtlas" },
            { "Jwt:Audience", "AudioAtlasUsers" }
        })
        .Build();

    var controller = new AuthController(
        dbContext,
        userManager.Object,
        signInManager.Object,
        config);

    var authService = new Mock<Microsoft.AspNetCore.Authentication.IAuthenticationService>();

    var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
    services.AddSingleton(authService.Object);

    controller.ControllerContext = new ControllerContext
    {
        HttpContext = new DefaultHttpContext
        {
            RequestServices = services.BuildServiceProvider()
        }
    };

    // Act
    var result = await controller.ExternalCallback();

    // Assert
    var redirect = Assert.IsType<RedirectResult>(result);

    Assert.NotNull(redirect.Url);
    Assert.Contains("http://localhost:3000/auth/callback", redirect.Url);
    Assert.Contains("newUser=false", redirect.Url);
    Assert.Contains("token=", redirect.Url);
}

}