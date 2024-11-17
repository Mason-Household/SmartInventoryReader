using FluentAssertions;
using Inventory.Models;

namespace Inventory.Tests.Models;

public class ModelTests
{
    [Fact]
    public void AuditableEntity_ShouldTrackDates()
    {
        // Arrange
        var item = new Item();

        // Act & Assert
        item.CreatedAt.Should().Be(default);
        item.UpdatedAt.Should().Be(default);
    }

    [Fact]
    public void TenantEntity_ShouldRequireOrganization()
    {
        // Arrange
        var item = new Item();
        var org = new Organization { Id = 1, Name = "Test Org" };

        // Act
        item.Organization = org;

        // Assert
        item.Organization.Should().NotBeNull();
        item.Organization.Id.Should().Be(1);
    }

    [Fact]
    public void UserOrganization_ShouldSetProperties()
    {
        // Arrange & Act
        var userOrg = new UserOrganization
        {
            UserId = 1,
            OrganizationId = 1,
            Role = "Admin",
            JoinedAt = DateTime.UtcNow
        };

        // Assert
        userOrg.UserId.Should().Be(1);
        userOrg.OrganizationId.Should().Be(1);
        userOrg.Role.Should().Be("Admin");
        userOrg.JoinedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Item_ShouldManageTagCollection()
    {
        // Arrange
        var item = new Item();
        var tag = new Tag { Name = "Test Tag" };

        // Act
        item.Tags.Add(tag);

        // Assert
        item.Tags.Should().ContainSingle();
        item.Tags.Should().Contain(tag);
    }

    [Fact]
    public void Item_ShouldTrackPriceHistory()
    {
        // Arrange
        var item = new Item { ActualPrice = 10.99m };
        var priceHistory = new PriceHistory
        {
            Price = 9.99m,
            EffectiveDate = DateTime.UtcNow,
            Item = item
        };

        // Act
        item.PriceHistory.Add(priceHistory);

        // Assert
        item.PriceHistory.Should().ContainSingle();
        item.PriceHistory.First().Price.Should().Be(9.99m);
    }

    [Fact]
    public void Item_ShouldManageImages()
    {
        // Arrange
        var item = new Item();
        var image = new ItemImage
        {
            Item = item
        };

        // Act
        item.Images.Add(image);
        item.PrimaryImageUrl = "test.jpg";

        // Assert
        item.Images.Should().ContainSingle();
        item.PrimaryImageUrl.Should().Be("test.jpg");
    }

    [Fact]
    public void Category_ShouldManageItems()
    {
        // Arrange
        var category = new Category { Name = "Test Category" };
        var item = new Item { Name = "Test Item" };

        // Act
        item.Category = category;

        // Assert
        item.Category.Should().Be(category);
        item.CategoryId.Should().Be(category.Id);
    }

    [Fact]
    public void Organization_ShouldManageUserOrganizations()
    {
        // Arrange
        var org = new Organization { Name = "Test Org" };
        var userId = Guid.NewGuid();
        var user = new ApplicationUser { Id = userId, FirstName = "Test", LastName = "User" };
        var userOrg = new UserOrganization
        {
            UserId = BitConverter.ToInt64(userId.ToByteArray(), 0),
            Role = "Member",
            JoinedAt = DateTime.UtcNow,
            Organization = org,
            User = user
        };

        // Assert
        userOrg.Organization.Should().Be(org);
        userOrg.User.Should().Be(user);
        userOrg.Role.Should().Be("Member");
    }

    [Fact]
    public void InventoryTransaction_ShouldTrackChanges()
    {
        // Arrange
        var item = new Item { Name = "Test Item" };
        var transaction = new InventoryTransaction
        {
            Item = item,
            Quantity = 5,
            Type = TransactionType.Purchase,
            Notes = "Initial Stock",
            Reference = "PO-001",
            UnitPrice = 10.99m,
            TransactionDate = DateTime.UtcNow
        };

        // Assert
        transaction.Item.Should().Be(item);
        transaction.Quantity.Should().Be(5);
        transaction.Type.Should().Be(TransactionType.Purchase);
        transaction.Notes.Should().Be("Initial Stock");
        transaction.Reference.Should().Be("PO-001");
        transaction.UnitPrice.Should().Be(10.99m);
        transaction.TransactionDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void ApplicationUser_ShouldSetProperties()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new ApplicationUser
        {
            Id = userId,
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com",
            CurrentOrganizationId = 1
        };

        // Assert
        user.Id.Should().Be(userId);
        user.FirstName.Should().Be("John");
        user.LastName.Should().Be("Doe");
        user.Email.Should().Be("john@example.com");
        user.CurrentOrganizationId.Should().Be(1);
    }
}
