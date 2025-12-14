using ElevatorControl.Api.Application.Commands;
using ElevatorControl.Api.Application.Handlers;
using ElevatorControl.Api.Domain.Entities;
using ElevatorControl.Api.Infrastructure.Repositories;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ElevatorControl.Api.Tests.Application.Handlers;

public class MoveToFloorCommandHandlerTests
{
    [Fact]
    public async Task HandleAsync_ReturnsFalse_WhenElevatorNotFound()
    {
        // Arrange
        var mockRepo = new Mock<IElevatorRepository>();
        var mockLogger = new Mock<ILogger<MoveToFloorCommandHandler>>();
        
        mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Elevator?)null);
        
        var handler = new MoveToFloorCommandHandler(mockRepo.Object, mockLogger.Object);
        var command = new MoveToFloorCommand(999, 5);

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task HandleAsync_MovesElevator_WhenElevatorExists()
    {
        // Arrange
        var mockRepo = new Mock<IElevatorRepository>();
        var mockLogger = new Mock<ILogger<MoveToFloorCommandHandler>>();
        var elevator = new Elevator { Id = 1, CurrentFloor = 1 };
        
        mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(elevator);
        
        var handler = new MoveToFloorCommandHandler(mockRepo.Object, mockLogger.Object);
        var command = new MoveToFloorCommand(1, 5);

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        Assert.True(result);
        Assert.Equal(5, elevator.CurrentFloor);
    }

    [Fact]
    public async Task HandleAsync_LogsMovement()
    {
        // Arrange
        var mockRepo = new Mock<IElevatorRepository>();
        var mockLogger = new Mock<ILogger<MoveToFloorCommandHandler>>();
        var elevator = new Elevator { Id = 2, CurrentFloor = 3 };
        
        mockRepo.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(elevator);
        
        var handler = new MoveToFloorCommandHandler(mockRepo.Object, mockLogger.Object);
        var command = new MoveToFloorCommand(2, 8);

        // Act
        await handler.HandleAsync(command);

        // Assert
        mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Moving elevator 2 to floor 8")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
