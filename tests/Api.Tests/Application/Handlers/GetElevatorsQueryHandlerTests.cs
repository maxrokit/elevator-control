using ElevatorControl.Api.Application.Handlers;
using ElevatorControl.Api.Application.Queries;
using ElevatorControl.Api.Domain.Entities;
using ElevatorControl.Api.Infrastructure.Repositories;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ElevatorControl.Api.Tests.Application.Handlers;

public class GetElevatorsQueryHandlerTests
{
    [Fact]
    public async Task HandleAsync_ReturnsEmptyList_WhenNoElevators()
    {
        // Arrange
        var mockRepo = new Mock<IElevatorRepository>();
        var mockLogger = new Mock<ILogger<GetElevatorsQueryHandler>>();
        
        mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Elevator>());
        
        var handler = new GetElevatorsQueryHandler(mockRepo.Object, mockLogger.Object);
        var query = new GetElevatorsQuery();

        // Act
        var result = (await handler.HandleAsync(query)).ToList();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task HandleAsync_ReturnsAllElevators()
    {
        // Arrange
        var mockRepo = new Mock<IElevatorRepository>();
        var mockLogger = new Mock<ILogger<GetElevatorsQueryHandler>>();
        
        var elevators = new List<Elevator>
        {
            new Elevator { Id = 1, CurrentFloor = 1 },
            new Elevator { Id = 2, CurrentFloor = 5 },
            new Elevator { Id = 3, CurrentFloor = 10 }
        };
        
        mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(elevators);
        
        var handler = new GetElevatorsQueryHandler(mockRepo.Object, mockLogger.Object);
        var query = new GetElevatorsQuery();

        // Act
        var result = (await handler.HandleAsync(query)).ToList();

        // Assert
        Assert.Equal(3, result.Count);
        Assert.Contains(result, e => e.Id == 1 && e.CurrentFloor == 1);
        Assert.Contains(result, e => e.Id == 2 && e.CurrentFloor == 5);
        Assert.Contains(result, e => e.Id == 3 && e.CurrentFloor == 10);
    }

    [Fact]
    public async Task HandleAsync_LogsElevatorCount()
    {
        // Arrange
        var mockRepo = new Mock<IElevatorRepository>();
        var mockLogger = new Mock<ILogger<GetElevatorsQueryHandler>>();
        
        var elevators = new List<Elevator>
        {
            new Elevator { Id = 1, CurrentFloor = 1 },
            new Elevator { Id = 2, CurrentFloor = 5 }
        };
        
        mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(elevators);
        
        var handler = new GetElevatorsQueryHandler(mockRepo.Object, mockLogger.Object);
        var query = new GetElevatorsQuery();

        // Act
        await handler.HandleAsync(query);

        // Assert
        mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Retrieved 2 elevators")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
