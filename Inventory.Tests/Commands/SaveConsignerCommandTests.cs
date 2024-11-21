using Inventory.Commands;
using Inventory.Data;
using Inventory.Models;
using Inventory.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;
using FluentAssertions;

namespace Inventory.Tests.Commands;

public class SaveConsignerCommandTests
{
    private readonly AppDbContext _context;
    private readonly UpsertConsignerCommand _command;
    private readonly ICurrentUserService _currentUserService;

    public SaveConsignerCommandTests()
    {
        _currentUserService = new TestCurrentUserService();
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
            .Options;
        _context = new AppDbContext(options, _currentUserService);
        _command = new SaveConsignerCommand(_context);
    }

    [Fact]
    public async Task ExecuteAsync_WithNewConsigner_ShouldCreateConsigner()
    {
        // Arrange
        var consigner = new Consigner
        {
            Name = "New Consigner",
            Email = "new@example.com",
            Phone = "123-456-7890",
            PaymentDetails = "PayPal: new@example.com",
            CommissionRate = 0.7m,
            Notes = "Test notes",
            IsActive = true
        };

        // Act
        var result = await _command.ExecuteAsync(consigner);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBe(0);

        var savedConsigner = await _context.Consigners.FindAsync(result.Id);
        savedConsigner.Should().NotBeNull();
        savedConsigner.Name.Should().Be("New Consigner");
        savedConsigner.Email.Should().Be("new@example.com");
        savedConsigner.Phone.Should().Be("123-456-7890");
        savedConsigner.PaymentDetails.Should().Be("PayPal: new@example.com");
        savedConsigner.CommissionRate.Should().Be(0.7m);
        savedConsigner.Notes.Should().Be("Test notes");
        savedConsigner.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task ExecuteAsync_WithExistingConsigner_ShouldUpdateConsigner()
    {
        // Arrange
        var existingConsigner = new Consigner
        {
            Name = "Original Name",
            Email = "original@example.com",
            Phone = "111-111-1111",
            PaymentDetails = "Original payment details",
            CommissionRate = 0.6m,
            Notes = "Original notes",
            IsActive = true
        };
        _context.Consigners.Add(existingConsigner);
        await _context.SaveChangesAsync();

        var updatedConsigner = new Consigner
        {
            Id = existingConsigner.Id,
            Name = "Updated Name",
            Email = "updated@example.com",
            Phone = "222-222-2222",
            PaymentDetails = "Updated payment details",
            CommissionRate = 0.75m,
            Notes = "Updated notes",
            IsActive = false
        };

        // Act
        var result = await _command.ExecuteAsync(updatedConsigner);

        // Assert
        result.Should().NotBeNull();
        
        var savedConsigner = await _context.Consigners.FindAsync(existingConsigner.Id);
        savedConsigner.Should().NotBeNull();
        savedConsigner.Name.Should().Be("Updated Name");
        savedConsigner.Email.Should().Be("updated@example.com");
        savedConsigner.Phone.Should().Be("222-222-2222");
        savedConsigner.PaymentDetails.Should().Be("Updated payment details");
        savedConsigner.CommissionRate.Should().Be(0.75m);
        savedConsigner.Notes.Should().Be("Updated notes");
        savedConsigner.IsActive.Should().BeFalse();
    }

    [Fact]
    public async Task ExecuteAsync_WithNonExistentConsigner_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        var nonExistentConsigner = new Consigner
        {
            Id = 999,
            Name = "Non-existent"
        };

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => 
            _command.ExecuteAsync(nonExistentConsigner));
    }

    [Fact]
    public async Task ExecuteAsync_ShouldPreserveUnmodifiedFields()
    {
        // Arrange
        var existingConsigner = new Consigner
        {
            Name = "Original Name",
            Email = "original@example.com",
            Phone = "111-111-1111",
            PaymentDetails = "Original payment details",
            CommissionRate = 0.6m,
            Notes = "Original notes",
            IsActive = true,
            UnpaidBalance = 1000m,
            TotalPaidOut = 500m
        };
        _context.Consigners.Add(existingConsigner);
        await _context.SaveChangesAsync();

        var updatedConsigner = new Consigner
        {
            Id = existingConsigner.Id,
            Name = "Updated Name",
            Email = "updated@example.com",
            Phone = "222-222-2222",
            PaymentDetails = "Updated payment details",
            CommissionRate = 0.75m,
            Notes = "Updated notes",
            IsActive = false,
            // Not setting UnpaidBalance and TotalPaidOut
        };

        // Act
        await _command.ExecuteAsync(updatedConsigner);

        // Assert
        var savedConsigner = await _context.Consigners.FindAsync(existingConsigner.Id);
        savedConsigner.Should().NotBeNull();
        savedConsigner.UnpaidBalance.Should().Be(1000m); // Should remain unchanged
        savedConsigner.TotalPaidOut.Should().Be(500m); // Should remain unchanged
    }

    private class TestCurrentUserService : ICurrentUserService
    {
        public string? GetUserEmail() => "test@example.com";
        public Guid? GetCurrentUserId() => Guid.Parse("00000000-0000-0000-0000-000000000001");
        public long? GetCurrentOrganizationId() => 1;
    }
}
