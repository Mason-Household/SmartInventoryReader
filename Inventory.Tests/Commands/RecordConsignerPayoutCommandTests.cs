using Inventory.Commands;
using Inventory.Data;
using Inventory.Models;
using Inventory.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;
using FluentAssertions;

namespace Inventory.Tests.Commands;

public class RecordConsignerPayoutCommandTests
{
    private readonly AppDbContext _context;
    private readonly RecordConsignerPayoutCommand _command;
    private readonly ICurrentUserService _currentUserService;

    public RecordConsignerPayoutCommandTests()
    {
        _currentUserService = new TestCurrentUserService();
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
            .Options;
        _context = new AppDbContext(options, _currentUserService);
        _command = new RecordConsignerPayoutCommand(_context);
    }

    [Fact]
    public async Task ExecuteAsync_WithValidPayout_ShouldRecordPayoutAndUpdateBalances()
    {
        // Arrange
        var consigner = new Consigner 
        { 
            Id = 1,
            Name = "Test Consigner",
            UnpaidBalance = 1000m,
            TotalPaidOut = 500m
        };
        await _context.Consigners.AddAsync(consigner);
        await _context.SaveChangesAsync();

        var payout = new ConsignerPayout
        {
            ConsignerId = consigner.Id,
            Amount = 300m,
            PaymentMethod = "Check"
        };

        // Act
        var result = await _command.ExecuteAsync(payout);

        // Assert
        result.Should().NotBeNull();
        result.PayoutDate.Should().NotBe(default);
        
        var updatedConsigner = await _context.Consigners.FindAsync(consigner.Id);
        updatedConsigner.UnpaidBalance.Should().Be(700m); // 1000 - 300
        updatedConsigner.TotalPaidOut.Should().Be(800m); // 500 + 300

        var savedPayout = await _context.ConsignerPayouts.FirstOrDefaultAsync();
        savedPayout.Should().NotBeNull();
        savedPayout.Amount.Should().Be(300m);
        savedPayout.ConsignerId.Should().Be(consigner.Id);
    }

    [Fact]
    public async Task ExecuteAsync_WithNonExistentConsigner_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        var payout = new ConsignerPayout
        {
            ConsignerId = 999,
            Amount = 100m
        };

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => 
            _command.ExecuteAsync(payout));
    }

    [Fact]
    public async Task ExecuteAsync_WithSpecifiedPayoutDate_ShouldKeepOriginalDate()
    {
        // Arrange
        var consigner = new Consigner { Id = 1, Name = "Test Consigner" };
        await _context.Consigners.AddAsync(consigner);
        await _context.SaveChangesAsync();

        var specificDate = new DateTime(2024, 1, 1);
        var payout = new ConsignerPayout
        {
            ConsignerId = consigner.Id,
            Amount = 100m,
            PayoutDate = specificDate
        };

        // Act
        var result = await _command.ExecuteAsync(payout);

        // Assert
        result.PayoutDate.Should().Be(specificDate);
    }

    [Fact]
    public async Task CalculateUnpaidBalanceAsync_ShouldCalculateCorrectly()
    {
        // Arrange
        var consigner = new Consigner 
        { 
            Id = 1,
            Name = "Test Consigner",
            CommissionRate = 0.7m
        };
        await _context.Consigners.AddAsync(consigner);

        var items = new List<Item>
        {
            new Item { ConsignerId = consigner.Id, ActualPrice = 100m },
            new Item { ConsignerId = consigner.Id, ActualPrice = 200m }
        };
        await _context.Items.AddRangeAsync(items);

        var existingPayout = new ConsignerPayout
        {
            ConsignerId = consigner.Id,
            Amount = 50m
        };
        await _context.ConsignerPayouts.AddAsync(existingPayout);

        await _context.SaveChangesAsync();

        // Act
        var result = await _command.CalculateUnpaidBalanceAsync(consigner.Id);

        // Assert
        // Total sales = 300, commission = 0.7, so share = 210
        // Already paid = 50, so unpaid = 160
        result.Should().Be(160m);
    }

    [Fact]
    public async Task CalculateUnpaidBalanceAsync_WithNonExistentConsigner_ShouldThrowKeyNotFoundException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => 
            _command.CalculateUnpaidBalanceAsync(999));
    }

    [Fact]
    public async Task CalculateUnpaidBalanceAsync_WithNoItems_ShouldReturnNegativeOfPaidAmount()
    {
        // Arrange
        var consigner = new Consigner 
        { 
            Id = 1,
            Name = "Test Consigner",
            CommissionRate = 0.7m
        };
        await _context.Consigners.AddAsync(consigner);

        var existingPayout = new ConsignerPayout
        {
            ConsignerId = consigner.Id,
            Amount = 50m
        };
        await _context.ConsignerPayouts.AddAsync(existingPayout);

        await _context.SaveChangesAsync();

        // Act
        var result = await _command.CalculateUnpaidBalanceAsync(consigner.Id);

        // Assert
        result.Should().Be(-50m); // No sales, but 50 already paid
    }

    private class TestCurrentUserService : ICurrentUserService
    {
        public string? GetUserEmail() => "test@example.com";
        public Guid? GetCurrentUserId() => Guid.Parse("00000000-0000-0000-0000-000000000001");
        public long? GetCurrentOrganizationId() => 1;
    }
}
