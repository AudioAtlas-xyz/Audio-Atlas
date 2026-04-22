using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;
using AudioAtlasView.Controllers;
using AudioAtlasDomain.Users;
using AudioAtlasInfrastructure.Database;

namespace AudioAtlasInfrastructureTests.Identity;

public class JwtServiceTests
{
    [Fact]
    public void GenerateJwtToken_ShouldContainExpectedClaims()
    {
        // Arrange
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "Jwt:Key", "super_secret_test_key_that_is_long_enough_12345" },
                { "Jwt:Issuer", "AudioAtlas" },
                { "Jwt:Audience", "AudioAtlasUsers" },
                { "Frontend:BaseUrl", "http://localhost:3000" }
            })
            .Build();

        var controller = new AuthController(
            dbContext: null!,
            userManager: null!,
            signInManager: null!,
            config: config);

       var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            Email = "test@test.com",
            UserName = "awesomesauce"
        };

        var method = typeof(AuthController)
            .GetMethod("GenerateJwtToken", BindingFlags.NonPublic | BindingFlags.Instance);

        // Act
        var token = (string)method!.Invoke(controller, new object[] { user })!;

        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

        // Assert
        Assert.Contains(jwt.Claims, c =>
            c.Type == System.Security.Claims.ClaimTypes.Email &&
            c.Value == "test@test.com");

        Assert.Contains(jwt.Claims, c =>
            c.Type == System.Security.Claims.ClaimTypes.Name &&
            c.Value == "awesomesauce");

        Assert.Contains(jwt.Claims, c =>
            c.Type == System.Security.Claims.ClaimTypes.NameIdentifier &&
            c.Value == user.Id.ToString());
    }
}