using Moq;
using FluentAssertions;
using Inventory.Models;
using Inventory.Commands;
using Inventory.Services;
using Inventory.Repositories;
using System.Linq.Expressions;
using FluentValidation.TestHelper;

namespace Inventory.Tests.Commands;

public class SaveItemCommandValidatorTests
{
    private readonly SaveItemCommandValidator _validator = new();

    [Fact]
    public void Validate_EmptyName_ShouldHaveError()
    {
        var command = new SaveItemCommand { Name = "", CategoryId = 1, ActualPrice = 10.99m };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Validate_InvalidCategoryId_ShouldHaveError()
    {
        var command = new SaveItemCommand { Name = "Test", CategoryId = 0, ActualPrice = 10.99m };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.CategoryId);
    }

    [Fact]
    public void Validate_InvalidActualPrice_ShouldHaveError()
    {
        var command = new SaveItemCommand { Name = "Test", CategoryId = 1, ActualPrice = 0 };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.ActualPrice);
    }

    [Fact]
    public void Validate_NegativeStockQuantity_ShouldHaveError()
    {
        var command = new SaveItemCommand { Name = "Test", CategoryId = 1, ActualPrice = 10.99m, StockQuantity = -1 };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.StockQuantity);
    }
}

public class SaveItemCommandHandlerTests
{
    private readonly SaveItemCommandHandler _handler;
    private readonly Mock<IRepository<Tag>> _tagRepositoryMock;
    private readonly Mock<IRepository<Item>> _itemRepositoryMock;
    private readonly Mock<ICurrentUserService> _currentUserServiceMock;
    private readonly Mock<IRepository<Organization>> _organizationRepositoryMock;

    public SaveItemCommandHandlerTests()
    {
        _itemRepositoryMock = new Mock<IRepository<Item>>();
        _tagRepositoryMock = new Mock<IRepository<Tag>>();
        _currentUserServiceMock = new Mock<ICurrentUserService>();
        _organizationRepositoryMock = new Mock<IRepository<Organization>>();
        _handler = new SaveItemCommandHandler(
            _itemRepositoryMock.Object,
            _tagRepositoryMock.Object,
            _organizationRepositoryMock.Object,
            _currentUserServiceMock.Object
        );
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldCreateItem()
    {
        var organizationId = 1L;
        var organization = new Organization { Id = organizationId };
        var command = new SaveItemCommand
        {
            Name = "Test Item",
            ActualPrice = 10.99m,
            StockQuantity = 5,
            CategoryId = 1,
            TagNames = ["tag1"]
        };

        _currentUserServiceMock.Setup(s => s.GetCurrentOrganizationId())
            .Returns(organizationId);
        _organizationRepositoryMock.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Organization, bool>>>()))
            .ReturnsAsync([organization]);
        _tagRepositoryMock.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Tag, bool>>>()))
            .ReturnsAsync([]);
        _tagRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Tag>()))
            .ReturnsAsync((Tag t) => t);
        _itemRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Item>()))
            .ReturnsAsync((Item i) => i);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Name.Should().Be(command.Name);
        result.ActualPrice.Should().Be(command.ActualPrice);
        result.Organization.Should().Be(organization);
        _itemRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Item>()), Times.Once);
    }

    [Fact]
    public async Task Handle_NoOrganization_ShouldThrowUnauthorizedAccess()
    {
        _currentUserServiceMock.Setup(s => s.GetCurrentOrganizationId())
            .Returns((long?)null);

        var command = new SaveItemCommand { Name = "Test" };
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _handler.Handle(command, CancellationToken.None)
        );
    }

    [Fact]
    public async Task Handle_OrganizationNotFound_ShouldThrowInvalidOperation()
    {
        _currentUserServiceMock.Setup(s => s.GetCurrentOrganizationId())
            .Returns(1L);
        _organizationRepositoryMock.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Organization, bool>>>()))
            .ReturnsAsync(new List<Organization>());

        var command = new SaveItemCommand { Name = "Test" };
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(command, CancellationToken.None)
        );
    }

    [Fact]
    public async Task Handle_ExistingTag_ShouldReuseTag()
    {
        var organizationId = 1L;
        var organization = new Organization { Id = organizationId };
        var existingTag = new Tag { Id = 1, Name = "tag1" };
        var command = new SaveItemCommand
        {
            Name = "Test Item",
            ActualPrice = 10.99m,
            TagNames = new List<string> { "tag1" }
        };

        _currentUserServiceMock.Setup(s => s.GetCurrentOrganizationId())
            .Returns(organizationId);
        _organizationRepositoryMock.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Organization, bool>>>()))
            .ReturnsAsync(new List<Organization> { organization });
        _tagRepositoryMock.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Tag, bool>>>()))
            .ReturnsAsync(new List<Tag> { existingTag });
        _itemRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Item>()))
            .ReturnsAsync((Item i) => i);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Tags.Should().Contain(existingTag);
        _tagRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Tag>()), Times.Never);
    }
}
