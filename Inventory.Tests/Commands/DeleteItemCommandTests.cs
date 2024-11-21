using FluentValidation.TestHelper;
using Inventory.Commands;
using Inventory.Models;
using Inventory.Repositories;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Inventory.Tests.Commands;

public class DeleteItemCommandTests
{
    private readonly Mock<IRepository<Item>> _mockRepository;
    private readonly Mock<ILogger<DeleteItemCommandHandler>> _mockLogger;
    private readonly DeleteItemCommandHandler _handler;
    private readonly DeleteItemCommandValidator _validator;

    public DeleteItemCommandTests()
    {
        _mockRepository = new Mock<IRepository<Item>>();
        _mockLogger = new Mock<ILogger<DeleteItemCommandHandler>>();
        _handler = new DeleteItemCommandHandler(_mockRepository.Object, _mockLogger.Object);
        _validator = new DeleteItemCommandValidator();
    }

    [Fact]
    public async Task Handle_WithExistingItem_ShouldSoftDeleteAndReturnTrue()
    {
        // Arrange
        var itemId = 1L;
        var item = new Item { Id = itemId, IsDeleted = false };
        var command = new DeleteItemCommand(itemId);

        _mockRepository.Setup(r => r.GetByIdAsync(itemId))
            .ReturnsAsync(item);
        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Item>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result);
        Assert.True(item.IsDeleted);
        _mockRepository.Verify(r => r.UpdateAsync(item), Times.Once);
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains($"Item with ID {itemId} soft deleted")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WithNonExistentItem_ShouldReturnFalse()
    {
        // Arrange
        var itemId = 1L;
        var command = new DeleteItemCommand(itemId);

        _mockRepository.Setup(r => r.GetByIdAsync(itemId))
            .ReturnsAsync((Item)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result);
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Item>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenExceptionOccurs_ShouldLogErrorAndReturnFalse()
    {
        // Arrange
        var itemId = 1L;
        var command = new DeleteItemCommand(itemId);
        var expectedException = new Exception("Test exception");

        _mockRepository.Setup(r => r.GetByIdAsync(itemId))
            .ThrowsAsync(expectedException);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result);
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains($"Error deleting item with ID {itemId}")),
                expectedException,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_WithInvalidId_ShouldHaveValidationError(long invalidId)
    {
        // Arrange
        var command = new DeleteItemCommand(invalidId);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void Validate_WithValidId_ShouldNotHaveValidationError()
    {
        // Arrange
        var command = new DeleteItemCommand(1);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Id);
    }
}
