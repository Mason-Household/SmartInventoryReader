using Moq;
using MediatR;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Inventory.Controllers;
using Inventory.Commands;
using Inventory.Queries;
using Inventory.Models;
using Xunit;

namespace Inventory.Tests.Controllers;

public class ItemsControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly ItemsController _controller;

    public ItemsControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _controller = new ItemsController(_mediatorMock.Object);
    }

    [Fact]
    public async Task SaveItem_ValidCommand_ReturnsOkResult()
    {
        // Arrange
        var command = new SaveItemCommand
        {
            Name = "Test Item",
            ActualPrice = 10.99m,
            StockQuantity = 5,
            CategoryId = 1
        };
        var expectedItem = new Item { Id = 1, Name = "Test Item" };
        _mediatorMock.Setup(m => m.Send(command, default))
            .ReturnsAsync(expectedItem);

        // Act
        var result = await _controller.SaveItem(command);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedItem = okResult.Value.Should().BeOfType<Item>().Subject;
        returnedItem.Should().BeEquivalentTo(expectedItem);
        _mediatorMock.Verify(m => m.Send(command, default), Times.Once);
    }

    [Fact]
    public async Task GetItems_ValidQuery_ReturnsOkResult()
    {
        // Arrange
        long organizationId = 1;
        long userId = 1;
        int page = 1;
        int pageSize = 10;
        var expectedItems = new List<Item> { new() { Id = 1, Name = "Test Item" } };
        
        _mediatorMock.Setup(m => m.Send(It.IsAny<GetItemsQuery>(), default))
            .ReturnsAsync(expectedItems);

        // Act
        var result = await _controller.GetItems(organizationId, userId, page, pageSize);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedItems = okResult.Value.Should().BeOfType<List<Item>>().Subject;
        returnedItems.Should().BeEquivalentTo(expectedItems);
        _mediatorMock.Verify(m => m.Send(It.Is<GetItemsQuery>(q => 
            q.OrganizationId == organizationId &&
            q.UserId == userId &&
            q.Page == page &&
            q.PageSize == pageSize), default), Times.Once);
    }
}
