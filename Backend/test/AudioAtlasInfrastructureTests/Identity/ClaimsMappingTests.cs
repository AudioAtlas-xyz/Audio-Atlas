using System.Security.Claims;
using Xunit;

namespace AudioAtlasInfrastructureTests.Identity;

public class ClaimsMappingTests
{
    [Fact]
    public void FindEmailClaim_ShouldReturnEmail()
    {
        var principal = new ClaimsPrincipal(
            new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Email, "test@test.com")
            }));

        var email = principal.FindFirst(ClaimTypes.Email)?.Value;

        Assert.Equal("test@test.com", email);
    }

    [Fact]
    public void FindEmailClaim_WhenMissing_ShouldReturnNull()
    {
        var principal = new ClaimsPrincipal(
            new ClaimsIdentity());

        var email = principal.FindFirst(ClaimTypes.Email)?.Value;

        Assert.Null(email);
    }
}