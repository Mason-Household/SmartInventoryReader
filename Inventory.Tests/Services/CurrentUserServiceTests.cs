using Moq;
using FluentAssertions;
using Inventory.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Inventory.Tests.Services;

public class CurrentUserServiceTests
{
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock = new();
    private CurrentUserService Sut => new(_httpContextAccessorMock.Object);

    [Fact]
    public void GetCurrentOrganizationId_WithValidClaim_ReturnsId()
    {
        var organizationId = 123L;
        var claims = new List<Claim>
        {
            new("OrganizationId", organizationId.ToString())
        };
        var identity = new ClaimsIdentity(claims);
        var principal = new ClaimsPrincipal(identity);
        var context = new DefaultHttpContext
        {
            User = principal
        };
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(context);

        var result = Sut.GetCurrentOrganizationId();

        result.Should().Be(organizationId);
    }

    [Fact]
    public void GetCurrentOrganizationId_WithoutClaim_ReturnsNull()
    {
        var claims = new List<Claim>();
        var identity = new ClaimsIdentity(claims);
        var principal = new ClaimsPrincipal(identity);
        var context = new DefaultHttpContext
        {
            User = principal
        };
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(context);

        var result = Sut.GetCurrentOrganizationId();

        result.Should().BeNull();
    }

    [Fact]
    public void GetCurrentOrganizationId_WithInvalidClaim_ReturnsNull()
    {
        var claims = new List<Claim>
        {
            new("OrganizationId", "invalid")
        };
        var identity = new ClaimsIdentity(claims);
        var principal = new ClaimsPrincipal(identity);
        var context = new DefaultHttpContext
        {
            User = principal
        };
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(context);

        var result = Sut.GetCurrentOrganizationId();

        result.Should().BeNull();
    }

    [Fact]
    public void GetCurrentOrganizationId_WithNullContext_ReturnsNull()
    {
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns((HttpContext)null);

        var result = Sut.GetCurrentOrganizationId();

        result.Should().BeNull();
    }

    [Fact]
    public void GetCurrentOrganizationId_WithNullUser_ReturnsNull()
    {
        var context = new DefaultHttpContext
        {
            User = null!
        };
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(context);

        var result = Sut.GetCurrentOrganizationId();

        result.Should().BeNull();
    }
}
