using ElevatorControl.Api.Application.Commands;
using ElevatorControl.Api.Application.Handlers;
using ElevatorControl.Api.Domain.Entities;
using ElevatorControl.Api.Domain.Enums;
using ElevatorControl.Api.Infrastructure.Repositories;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ElevatorControl.Api.Tests.Application.Handlers;

public class AddFloorCallCommandHandlerTests
{
    [Fact]
    public async Task HandleAsync_ReturnsFalse_WhenElevatorNotFound()
    {
        // Arrange
        var mockRepo = new Mock<IElevatorRepository>();
        var mockLogger = new Mock<ILogger<AddFloorCallCommandHandler>>();
        
        mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Elevator?)null);
        
        var handler = new AddFloorCallCommandHandler(mockRepo.Object, mockLogger.Object);
        var command = new AddFloorCallCommand(999, 5, ElevatorDirection.Up);

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task HandleAsync_AddsFloorCall_WhenElevatorExists()
    {
        // Arrange
        var mockRepo = new Mock<IElevatorRepository>();
        var mockLogger = new Mock<ILogger<AddFloorCallCommandHandler>>();
        var elevator = new Elevator { Id = 1, CurrentFloor = 1 };
        
        mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(elevator);
        
        var handler = new AddFloorCallCommandHandler(mockRepo.Object, mockLogger.Object);
        var command = new AddFloorCallCommand(1, 5, ElevatorDirection.Up);

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        Assert.True(result);
        Assert.True(elevator.FloorCalls.ContainsKey(5));
        Assert.True(elevator.FloorCalls[5].IsUpCalled);
    }

    [Fact]
    public async Task HandleAsync_LogsFloorCall()
    {
        // Arrange
        var mockRepo = new Mock<IElevatorRepository>();
        var mockLogger = new Mock<ILogger<AddFloorCallCommandHandler>>();
        var elevator = new Elevator { Id = 2, CurrentFloor = 1 };
        
        mockRepo.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(elevator);
        
        var handler = new AddFloorCallCommandHandler(mockRepo.Object, mockLogger.Object);
        var command = new AddFloorCallCommand(2, 7, ElevatorDirection.Down);

        // Act
        await handler.HandleAsync(command);

        // Assert
        mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Added floor call from floor 7")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ThrowsException_WhenFloorOutOfRange()
    {
        // Arrange
        var mockRepo = new Mock<IElevatorRepository>();
        var mockLogger = new Mock<ILogger<AddFloorCallCommandHandler>>();
        var elevator = new Elevator(minFloor: 1, maxFloor: 10) { Id = 1, CurrentFloor = 5 };
        
        mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(elevator);
        
        var handler = new AddFloorCallCommandHandler(mockRepo.Object, mockLogger.Object);
        var command = new AddFloorCallCommand(1, 15, ElevatorDirection.Up);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => handler.HandleAsync(command));
    }

    [Fact]
    public async Task HandleAsync_ThrowsException_WhenDirectionInvalidForFloor()
    {
        // Arrange
        var mockRepo = new Mock<IElevatorRepository>();
        var mockLogger = new Mock<ILogger<AddFloorCallCommandHandler>>();
        var elevator = new Elevator(minFloor: 1, maxFloor: 10) { Id = 1, CurrentFloor = 5 };
        
        mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(elevator);
        
        var handler = new AddFloorCallCommandHandler(mockRepo.Object, mockLogger.Object);
        var command = new AddFloorCallCommand(1, 1, ElevatorDirection.Down);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => handler.HandleAsync(command));
    }
}
