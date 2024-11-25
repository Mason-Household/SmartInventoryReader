using Inventory.Data;
using Inventory.Models;
using Inventory.Services;
using Microsoft.EntityFrameworkCore;
using FluentAssertions;
using Inventory.Commands.Consigners;

namespace Inventory.Tests.Commands;

public class LoadConsignerHaulCommandTests
{
    private readonly AppDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private LoadConsignerHaulCommandHandler Sut => new(_context);

    public LoadConsignerHaulCommandTests()
    {
        _currentUserService = new TestCurrentUserService();
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
            .Options;
        _context = new AppDbContext(options, _currentUserService);
    }

    [Fact]
    public async Task ExecuteAsync_WithValidConsignerAndItems_ShouldLoadItemsAndUpdateBalance()
    {
        // Arrange
        var consigner = new Consigner 
        { 
            Id = 1,
            Name = "Test Consigner",
            CommissionRate = 0.7m,
            UnpaidBalance = 0
        };
        await _context.Consigners.AddAsync(consigner);
        await _context.SaveChangesAsync();

        var items = new List<Item>
        {
            new() { ActualPrice = 100m },
            new() { ActualPrice = 200m }
        };

        // Act
        var result = await Sut.Handle(new LoadConsignerHaulCommand { ConsignerId = consigner.Id, Items = items }, default);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        
        var loadedItems = await _context.Items.ToListAsync();
        loadedItems.Should().HaveCount(2);
        loadedItems.All(i => i.ConsignerId == consigner.Id).Should().BeTrue();
        loadedItems.All(i => i.SuggestedPrice == i.ActualPrice).Should().BeTrue();

        var updatedConsigner = await _context.Consigners.FindAsync(consigner.Id);
        updatedConsigner.UnpaidBalance.Should().Be(300m * 0.7m); // (100 + 200) * 0.7
    }

    [Fact]
    public async Task ExecuteAsync_WithNonExistentConsigner_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        var items = new List<Item>
        {
            new() { ActualPrice = 100m }
        };

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => 
            Sut.Handle(new LoadConsignerHaulCommand { ConsignerId = 999, Items = items }, default));
    }

    [Fact]
    public async Task ExecuteAsync_WithEmptyItemsList_ShouldNotUpdateBalance()
    {
        // Arrange
        var consigner = new Consigner 
        { 
            Id = 1,
            Name = "Test Consigner",
            CommissionRate = 0.7m,
            UnpaidBalance = 50m // Initial balance
        };
        await _context.Consigners.AddAsync(consigner);
        await _context.SaveChangesAsync();

        // Act
        var result = await Sut.Handle(new LoadConsignerHaulCommand { ConsignerId = consigner.Id, Items = [] }, default);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
        
        var updatedConsigner = await _context.Consigners.FindAsync(consigner.Id);
        updatedConsigner.UnpaidBalance.Should().Be(50m); // Should remain unchanged
    }

    [Fact]
    public async Task ExecuteAsync_ShouldSetCorrectConsignerIdForAllItems()
    {
        // Arrange
        var consigner = new Consigner 
        { 
            Id = 1,
            Name = "Test Consigner",
            CommissionRate = 0.7m
        };
        await _context.Consigners.AddAsync(consigner);
        await _context.SaveChangesAsync();

        var items = new List<Item>
        {
            new() { ActualPrice = 100m, ConsignerId = null },
            new() { ActualPrice = 200m, ConsignerId = 999 } // Different consigner ID
        };

        // Act
        await Sut.Handle(new LoadConsignerHaulCommand { ConsignerId = consigner.Id, Items = items }, default);

        // Assert
        var loadedItems = await _context.Items.ToListAsync();
        loadedItems.Should().HaveCount(2);
        loadedItems.All(i => i.ConsignerId == consigner.Id).Should().BeTrue();
    }

    private class TestCurrentUserService : ICurrentUserService
    {
        public string? GetUserEmail() => "test@example.com";
        public Guid? GetCurrentUserId() => Guid.Parse("00000000-0000-0000-0000-000000000001");
        public long? GetCurrentOrganizationId() => 1;
    }
}
