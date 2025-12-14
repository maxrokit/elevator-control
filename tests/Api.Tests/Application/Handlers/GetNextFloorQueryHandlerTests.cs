using ElevatorControl.Api.Application.Handlers;
using ElevatorControl.Api.Application.Queries;
using ElevatorControl.Api.Domain.Entities;
using ElevatorControl.Api.Domain.Enums;
using ElevatorControl.Api.Infrastructure.Repositories;
using Moq;
using Xunit;

namespace ElevatorControl.Api.Tests.Application.Handlers;

public class GetNextFloorQueryHandlerTests
{
    [Fact]
    public async Task HandleAsync_ReturnsNull_WhenElevatorNotFound()
    {
        // Arrange
        var mockRepo = new Mock<IElevatorRepository>();
        
        mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Elevator?)null);
        
        var handler = new GetNextFloorQueryHandler(mockRepo.Object);
        var query = new GetNextFloorQuery(999);

        // Act
        var result = await handler.HandleAsync(query);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task HandleAsync_ReturnsNull_WhenNoRequests()
    {
        // Arrange
        var mockRepo = new Mock<IElevatorRepository>();
        var elevator = new Elevator { Id = 1, CurrentFloor = 5 };
        
        mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(elevator);
        
        var handler = new GetNextFloorQueryHandler(mockRepo.Object);
        var query = new GetNextFloorQuery(1);

        // Act
        var result = await handler.HandleAsync(query);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task HandleAsync_ReturnsNextFloor_WhenDestinationExists()
    {
        // Arrange
        var mockRepo = new Mock<IElevatorRepository>();
        var elevator = new Elevator 
        { 
            Id = 1, 
            CurrentFloor = 5,
            CurrentDirection = ElevatorDirection.Up
        };
        elevator.AddFloorDestination(8);
        
        mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(elevator);
        
        var handler = new GetNextFloorQueryHandler(mockRepo.Object);
        var query = new GetNextFloorQuery(1);

        // Act
        var result = await handler.HandleAsync(query);

        // Assert
        Assert.Equal(8, result);
    }

    [Fact]
    public async Task HandleAsync_ReturnsNextFloor_WhenFloorCallExists()
    {
        // Arrange
        var mockRepo = new Mock<IElevatorRepository>();
        var elevator = new Elevator 
        { 
            Id = 1, 
            CurrentFloor = 3,
            CurrentDirection = ElevatorDirection.Up
        };
        elevator.AddFloorCall(7, ElevatorDirection.Down);
        
        mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(elevator);
        
        var handler = new GetNextFloorQueryHandler(mockRepo.Object);
        var query = new GetNextFloorQuery(1);

        // Act
        var result = await handler.HandleAsync(query);

        // Assert
        Assert.Equal(7, result);
    }

    [Fact]
    public async Task HandleAsync_PrioritizesCurrentDirection()
    {
        // Arrange
        var mockRepo = new Mock<IElevatorRepository>();
        var elevator = new Elevator 
        { 
            Id = 1, 
            CurrentFloor = 5,
            CurrentDirection = ElevatorDirection.Up
        };
        elevator.AddFloorDestination(8); // Above
        elevator.AddFloorDestination(3); // Below
        
        mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(elevator);
        
        var handler = new GetNextFloorQueryHandler(mockRepo.Object);
        var query = new GetNextFloorQuery(1);

        // Act
        var result = await handler.HandleAsync(query);

        // Assert - Should go up first
        Assert.Equal(8, result);
    }
}
