using System.Security.Claims;
using Xunit;

namespace AudioAtlasInfrastructureTests.Identity;

public class AuthorizationTests
{
    [Fact]
    public void UserRole_ShouldNotBeAdmin()
    {
        // Arrange
        var principal = new ClaimsPrincipal(
            new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Role, "User")
            }));

        // Act
        var role = principal.FindFirst(ClaimTypes.Role)?.Value;

        // Assert
        Assert.NotEqual("Admin", role);
    }

    [Fact]
    public void AdminRole_ShouldBeAdmin()
    {
        // Arrange
        var principal = new ClaimsPrincipal(
            new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Role, "Admin")
            }));

        // Act
        var role = principal.FindFirst(ClaimTypes.Role)?.Value;

        // Assert
        Assert.Equal("Admin", role);
    }
}