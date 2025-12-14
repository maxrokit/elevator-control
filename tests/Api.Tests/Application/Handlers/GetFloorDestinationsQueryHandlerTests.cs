using ElevatorControl.Api.Application.Handlers;
using ElevatorControl.Api.Application.Queries;
using ElevatorControl.Api.Domain.Entities;
using ElevatorControl.Api.Infrastructure.Repositories;
using Moq;
using Xunit;

namespace ElevatorControl.Api.Tests.Application.Handlers;

public class GetFloorDestinationsQueryHandlerTests
{
    [Fact]
    public async Task HandleAsync_ReturnsNull_WhenElevatorNotFound()
    {
        // Arrange
        var mockRepo = new Mock<IElevatorRepository>();
        
        mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Elevator?)null);
        
        var handler = new GetFloorDestinationsQueryHandler(mockRepo.Object);
        var query = new GetFloorDestinationsQuery(999);

        // Act
        var result = await handler.HandleAsync(query);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task HandleAsync_ReturnsEmptyArray_WhenNoDestinations()
    {
        // Arrange
        var mockRepo = new Mock<IElevatorRepository>();
        var elevator = new Elevator { Id = 1, CurrentFloor = 1 };
        
        mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(elevator);
        
        var handler = new GetFloorDestinationsQueryHandler(mockRepo.Object);
        var query = new GetFloorDestinationsQuery(1);

        // Act
        var result = await handler.HandleAsync(query);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task HandleAsync_ReturnsDestinations_WhenTheyExist()
    {
        // Arrange
        var mockRepo = new Mock<IElevatorRepository>();
        var elevator = new Elevator { Id = 1, CurrentFloor = 1 };
        elevator.AddFloorDestination(5);
        elevator.AddFloorDestination(8);
        elevator.AddFloorDestination(3);
        
        mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(elevator);
        
        var handler = new GetFloorDestinationsQueryHandler(mockRepo.Object);
        var query = new GetFloorDestinationsQuery(1);

        // Act
        var result = (await handler.HandleAsync(query))!.ToArray();

        // Assert
        Assert.Equal(3, result.Length);
        Assert.Contains(5, result);
        Assert.Contains(8, result);
        Assert.Contains(3, result);
    }
}
