using ElevatorControl.Api.Application.Commands;
using ElevatorControl.Api.Application.Handlers;
using ElevatorControl.Api.Domain.Entities;
using ElevatorControl.Api.Infrastructure.Repositories;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ElevatorControl.Api.Tests.Application.Handlers;

public class AddFloorDestinationCommandHandlerTests
{
    [Fact]
    public async Task HandleAsync_ReturnsFalse_WhenElevatorNotFound()
    {
        // Arrange
        var mockRepo = new Mock<IElevatorRepository>();
        var mockLogger = new Mock<ILogger<AddFloorDestinationCommandHandler>>();
        
        mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Elevator?)null);
        
        var handler = new AddFloorDestinationCommandHandler(mockRepo.Object, mockLogger.Object);
        var command = new AddFloorDestinationCommand(999, 5);

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task HandleAsync_AddsDestination_WhenElevatorExists()
    {
        // Arrange
        var mockRepo = new Mock<IElevatorRepository>();
        var mockLogger = new Mock<ILogger<AddFloorDestinationCommandHandler>>();
        var elevator = new Elevator { Id = 1, CurrentFloor = 1 };
        
        mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(elevator);
        
        var handler = new AddFloorDestinationCommandHandler(mockRepo.Object, mockLogger.Object);
        var command = new AddFloorDestinationCommand(1, 8);

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        Assert.True(result);
        Assert.Contains(8, elevator.FloorDestinations);
    }

    [Fact]
    public async Task HandleAsync_LogsDestinationAddition()
    {
        // Arrange
        var mockRepo = new Mock<IElevatorRepository>();
        var mockLogger = new Mock<ILogger<AddFloorDestinationCommandHandler>>();
        var elevator = new Elevator { Id = 3, CurrentFloor = 1 };
        
        mockRepo.Setup(r => r.GetByIdAsync(3)).ReturnsAsync(elevator);
        
        var handler = new AddFloorDestinationCommandHandler(mockRepo.Object, mockLogger.Object);
        var command = new AddFloorDestinationCommand(3, 12);

        // Act
        await handler.HandleAsync(command);

        // Assert
        mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Added floor destination 12")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
