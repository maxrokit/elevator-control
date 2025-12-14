using ElevatorControl.Api.Application.Commands;
using ElevatorControl.Api.Application.Handlers;
using ElevatorControl.Api.Domain.Entities;
using ElevatorControl.Api.Infrastructure.Repositories;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ElevatorControl.Api.Tests.Application.Handlers;

public class CreateElevatorCommandHandlerTests
{
    [Fact]
    public async Task HandleAsync_CreatesElevator_AndReturnsDto()
    {
        // Arrange
        var mockRepo = new Mock<IElevatorRepository>();
        var mockLogger = new Mock<ILogger<CreateElevatorCommandHandler>>();
        var expectedElevator = new Elevator { Id = 1, CurrentFloor = 1 };
        
        mockRepo.Setup(r => r.CreateAsync()).ReturnsAsync(expectedElevator);
        
        var handler = new CreateElevatorCommandHandler(mockRepo.Object, mockLogger.Object);
        var command = new CreateElevatorCommand();

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal(1, result.CurrentFloor);
        mockRepo.Verify(r => r.CreateAsync(), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_LogsElevatorCreation()
    {
        // Arrange
        var mockRepo = new Mock<IElevatorRepository>();
        var mockLogger = new Mock<ILogger<CreateElevatorCommandHandler>>();
        var expectedElevator = new Elevator { Id = 5, CurrentFloor = 1 };
        
        mockRepo.Setup(r => r.CreateAsync()).ReturnsAsync(expectedElevator);
        
        var handler = new CreateElevatorCommandHandler(mockRepo.Object, mockLogger.Object);
        var command = new CreateElevatorCommand();

        // Act
        await handler.HandleAsync(command);

        // Assert
        mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Created elevator 5")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
