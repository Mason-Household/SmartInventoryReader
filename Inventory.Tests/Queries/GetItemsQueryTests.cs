using Moq;
using FluentAssertions;
using FluentValidation.TestHelper;
using Inventory.Queries;
using Inventory.Models;
using Inventory.Services;
using Inventory.Repositories;
using System.Linq.Expressions;

namespace Inventory.Tests.Queries;

public class GetItemsQueryValidatorTests
{
    private readonly GetItemsQueryValidator _validator;

    public GetItemsQueryValidatorTests()
    {
        _validator = new GetItemsQueryValidator();
    }

    [Fact]
    public void Validate_EmptyUserId_ShouldHaveError()
    {
        var query = new GetItemsQuery { OrganizationId = 1, Page = 1, PageSize = 10 };
        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(x => x.UserId);
    }

    [Fact]
    public void Validate_EmptyOrganizationId_ShouldHaveError()
    {
        var query = new GetItemsQuery { UserId = 1, Page = 1, PageSize = 10 };
        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(x => x.OrganizationId);
    }

    [Fact]
    public void Validate_InvalidPage_ShouldHaveError()
    {
        var query = new GetItemsQuery { UserId = 1, OrganizationId = 1, Page = 0, PageSize = 10 };
        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(x => x.Page);
    }

    [Fact]
    public void Validate_InvalidPageSize_ShouldHaveError()
    {
        var query = new GetItemsQuery { UserId = 1, OrganizationId = 1, Page = 1, PageSize = 0 };
        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(x => x.PageSize);
    }
}

public class GetItemsQueryHandlerTests
{
    private readonly Mock<IRepository<Item>> _itemRepositoryMock;
    private readonly Mock<IRepository<Organization>> _organizationRepositoryMock;
    private readonly Mock<ICurrentUserService> _currentUserServiceMock;
    private readonly GetItemsQueryHandler _handler;

    public GetItemsQueryHandlerTests()
    {
        _itemRepositoryMock = new Mock<IRepository<Item>>();
        _organizationRepositoryMock = new Mock<IRepository<Organization>>();
        _currentUserServiceMock = new Mock<ICurrentUserService>();
        _handler = new GetItemsQueryHandler(
            _itemRepositoryMock.Object,
            _organizationRepositoryMock.Object,
            _currentUserServiceMock.Object
        );
    }

    [Fact]
    public async Task Handle_ValidQuery_ShouldReturnItems()
    {
        // Arrange
        var organizationId = 1L;
        var organization = new Organization { Id = organizationId };
        var items = new List<Item>
        {
            new() { Id = 1, Name = "Item 1" },
            new() { Id = 2, Name = "Item 2" },
            new() { Id = 3, Name = "Item 3" }
        };

        var query = new GetItemsQuery
        {
            OrganizationId = organizationId,
            UserId = 1,
            Page = 1,
            PageSize = 2
        };

        _currentUserServiceMock.Setup(s => s.GetCurrentOrganizationId())
            .Returns(organizationId);
        _organizationRepositoryMock.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Organization, bool>>>()))
            .ReturnsAsync(new List<Organization> { organization });
        _itemRepositoryMock.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Item, bool>>>()))
            .ReturnsAsync(items);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Count.Should().Be(2); // PageSize = 2
        result.First().Name.Should().Be("Item 1");
    }

    [Fact]
    public async Task Handle_WithSearchTerm_ShouldFilterItems()
    {
        // Arrange
        var organizationId = 1L;
        var organization = new Organization { Id = organizationId };
        var items = new List<Item>
        {
            new() { Id = 1, Name = "Apple" },
            new() { Id = 2, Name = "Banana" },
            new() { Id = 3, Name = "Orange" }
        };

        var query = new GetItemsQuery
        {
            OrganizationId = organizationId,
            UserId = 1,
            Page = 1,
            PageSize = 10,
            SearchTerm = "an"
        };

        _currentUserServiceMock.Setup(s => s.GetCurrentOrganizationId())
            .Returns(organizationId);
        _organizationRepositoryMock.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Organization, bool>>>()))
            .ReturnsAsync(new List<Organization> { organization });
        _itemRepositoryMock.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Item, bool>>>()))
            .ReturnsAsync(items);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Count.Should().Be(2);
        result.All(i => i.Name.Contains("an")).Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WithSorting_ShouldOrderItems()
    {
        // Arrange
        var organizationId = 1L;
        var organization = new Organization { Id = organizationId };
        var items = new List<Item>
        {
            new() { Id = 1, Name = "Zebra" },
            new() { Id = 2, Name = "Apple" },
            new() { Id = 3, Name = "Banana" }
        };

        var query = new GetItemsQuery
        {
            OrganizationId = organizationId,
            UserId = 1,
            Page = 1,
            PageSize = 10,
            SortBy = "Name",
            SortAscending = true
        };

        _currentUserServiceMock.Setup(s => s.GetCurrentOrganizationId())
            .Returns(organizationId);
        _organizationRepositoryMock.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Organization, bool>>>()))
            .ReturnsAsync(new List<Organization> { organization });
        _itemRepositoryMock.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Item, bool>>>()))
            .ReturnsAsync(items);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Count.Should().Be(3);
        result.Select(i => i.Name).Should().BeInAscendingOrder();
    }

    [Fact]
    public async Task Handle_NoOrganization_ShouldThrowUnauthorizedAccess()
    {
        // Arrange
        _currentUserServiceMock.Setup(s => s.GetCurrentOrganizationId())
            .Returns((long?)null);

        var query = new GetItemsQuery { OrganizationId = 1, UserId = 1, Page = 1, PageSize = 10 };

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _handler.Handle(query, CancellationToken.None)
        );
    }

    [Fact]
    public async Task Handle_OrganizationNotFound_ShouldThrowInvalidOperation()
    {
        // Arrange
        _currentUserServiceMock.Setup(s => s.GetCurrentOrganizationId())
            .Returns(1L);
        _organizationRepositoryMock.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Organization, bool>>>()))
            .ReturnsAsync(new List<Organization>());

        var query = new GetItemsQuery { OrganizationId = 1, UserId = 1, Page = 1, PageSize = 10 };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(query, CancellationToken.None)
        );
    }
}
