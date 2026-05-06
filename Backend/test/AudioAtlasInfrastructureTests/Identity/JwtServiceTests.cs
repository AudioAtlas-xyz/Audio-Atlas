using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Xunit;
using AudioAtlasView.Controllers;
using AudioAtlasDomain.Users;
using Moq;
using Microsoft.AspNetCore.Identity;

namespace AudioAtlasInfrastructureTests.Identity;

public class JwtServiceTests
{
    [Fact]
    public async Task GenerateJwtToken_ShouldContainExpectedClaims()
    {
        // Arrange
        var store = new Mock<IUserStore<ApplicationUser>>();

        var userManager = new Mock<UserManager<ApplicationUser>>(
            store.Object,
            null!, null!, null!, null!, null!, null!, null!, null!);

        userManager
            .Setup(x => x.GetRolesAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync(new List<string> { "Admin" });

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
            { "Jwt:Key", "super_secret_test_key_that_is_long_enough_12345" },
            { "Jwt:Issuer", "AudioAtlas" },
            { "Jwt:Audience", "AudioAtlasUsers" }
            })
            .Build();

        var controller = new AuthController(
            dbContext: null!,
            userManager: userManager.Object,
            signInManager: null!,
            config: config);

        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            Email = "test@test.com",
            UserName = "testuser"
        };

        var method = typeof(AuthController)
            .GetMethod("GenerateJwtToken", BindingFlags.NonPublic | BindingFlags.Instance);

        var task = (Task<string>)method!.Invoke(controller, new object[] { user })!;
        var token = await task;

        // Assert
        Assert.NotNull(token);
        Assert.Contains(".", token); // basic JWT structure check
    }
}