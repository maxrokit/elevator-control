using ElevatorControl.Api.Domain.Entities;
using ElevatorControl.Api.Domain.Enums;
using Moq;
using Xunit;
using AppServicesIElevatorRepository = ElevatorControl.Api.Application.Services.IElevatorRepository;

namespace ElevatorControl.Api.Tests.Application.Services;

public class ElevatorServiceTests
{
    [Fact]
    public async Task CreateElevatorAsync_CallsRepositoryCreate()
    {
        // Arrange
        var mockRepo = new Mock<AppServicesIElevatorRepository>();
        var expectedElevator = new Elevator { Id = 1, CurrentFloor = 1 };
        mockRepo.Setup(r => r.CreateAsync()).ReturnsAsync(expectedElevator);
        
        var service = new ElevatorControl.Api.Application.Services.ElevatorService(mockRepo.Object);

        // Act
        var result = await service.CreateElevatorAsync();

        // Assert
        Assert.Equal(expectedElevator, result);
        mockRepo.Verify(r => r.CreateAsync(), Times.Once);
    }

    [Fact]
    public async Task ListAsync_ReturnsFormattedElevatorList()
    {
        // Arrange
        var mockRepo = new Mock<AppServicesIElevatorRepository>();
        var elevator = new Elevator { Id = 1, CurrentFloor = 5, CurrentDirection = ElevatorDirection.Up };
        elevator.AddFloorDestination(10);
        elevator.AddFloorCall(8, ElevatorDirection.Down);
        
        mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new[] { elevator });
        
        var service = new ElevatorControl.Api.Application.Services.ElevatorService(mockRepo.Object);

        // Act
        var result = (await service.ListAsync()).ToList();

        // Assert
        Assert.Single(result);
        var item = result[0];
        Assert.NotNull(item);
    }

    [Fact]
    public async Task AddFloorDestinationAsync_ReturnsFalse_WhenElevatorNotFound()
    {
        // Arrange
        var mockRepo = new Mock<AppServicesIElevatorRepository>();
        mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Elevator?)null);
        
        var service = new ElevatorControl.Api.Application.Services.ElevatorService(mockRepo.Object);

        // Act
        var result = await service.AddFloorDestinationAsync(999, 5);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task AddFloorDestinationAsync_AddsDestination_WhenElevatorExists()
    {
        // Arrange
        var mockRepo = new Mock<AppServicesIElevatorRepository>();
        var elevator = new Elevator { Id = 1, CurrentFloor = 1 };
        mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(elevator);
        
        var service = new ElevatorControl.Api.Application.Services.ElevatorService(mockRepo.Object);

        // Act
        var result = await service.AddFloorDestinationAsync(1, 8);

        // Assert
        Assert.True(result);
        Assert.Contains(8, elevator.FloorDestinations);
    }

    [Fact]
    public async Task GetFloorDestinationsAsync_ReturnsNull_WhenElevatorNotFound()
    {
        // Arrange
        var mockRepo = new Mock<AppServicesIElevatorRepository>();
        mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Elevator?)null);
        
        var service = new ElevatorControl.Api.Application.Services.ElevatorService(mockRepo.Object);

        // Act
        var result = await service.GetFloorDestinationsAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetFloorDestinationsAsync_ReturnsDestinations_WhenElevatorExists()
    {
        // Arrange
        var mockRepo = new Mock<AppServicesIElevatorRepository>();
        var elevator = new Elevator { Id = 1, CurrentFloor = 1 };
        elevator.AddFloorDestination(5);
        elevator.AddFloorDestination(8);
        mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(elevator);
        
        var service = new ElevatorControl.Api.Application.Services.ElevatorService(mockRepo.Object);

        // Act
        var result = await service.GetFloorDestinationsAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Length);
        Assert.Contains(5, result);
        Assert.Contains(8, result);
    }

    [Fact]
    public async Task AddFloorCallAsync_ReturnsFalse_WhenElevatorNotFound()
    {
        // Arrange
        var mockRepo = new Mock<AppServicesIElevatorRepository>();
        mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Elevator?)null);
        
        var service = new ElevatorControl.Api.Application.Services.ElevatorService(mockRepo.Object);

        // Act
        var result = await service.AddFloorCallAsync(999, 5, ElevatorDirection.Up);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task AddFloorCallAsync_AddsCall_WhenElevatorExists()
    {
        // Arrange
        var mockRepo = new Mock<AppServicesIElevatorRepository>();
        var elevator = new Elevator { Id = 1, CurrentFloor = 1 };
        mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(elevator);
        
        var service = new ElevatorControl.Api.Application.Services.ElevatorService(mockRepo.Object);

        // Act
        var result = await service.AddFloorCallAsync(1, 7, ElevatorDirection.Down);

        // Assert
        Assert.True(result);
        Assert.True(elevator.FloorCalls.ContainsKey(7));
        Assert.True(elevator.FloorCalls[7].IsDownCalled);
    }

    [Fact]
    public async Task GetNextFloorAsync_ReturnsNull_WhenElevatorNotFound()
    {
        // Arrange
        var mockRepo = new Mock<AppServicesIElevatorRepository>();
        mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Elevator?)null);
        
        var service = new ElevatorControl.Api.Application.Services.ElevatorService(mockRepo.Object);

        // Act
        var result = await service.GetNextFloorAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetNextFloorAsync_ReturnsNextFloor_WhenElevatorExists()
    {
        // Arrange
        var mockRepo = new Mock<AppServicesIElevatorRepository>();
        var elevator = new Elevator { Id = 1, CurrentFloor = 1, CurrentDirection = ElevatorDirection.Up };
        elevator.AddFloorDestination(10);
        mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(elevator);
        
        var service = new ElevatorControl.Api.Application.Services.ElevatorService(mockRepo.Object);

        // Act
        var result = await service.GetNextFloorAsync(1);

        // Assert
        Assert.Equal(10, result);
    }

    [Fact]
    public async Task MoveToFloorAsync_DoesNothing_WhenElevatorNotFound()
    {
        // Arrange
        var mockRepo = new Mock<AppServicesIElevatorRepository>();
        mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Elevator?)null);
        
        var service = new ElevatorControl.Api.Application.Services.ElevatorService(mockRepo.Object);

        // Act
        await service.MoveToFloorAsync(999, 5);

        // Assert
        mockRepo.Verify(r => r.GetByIdAsync(999), Times.Once);
    }

    [Fact]
    public async Task MoveToFloorAsync_MovesElevator_WhenElevatorExists()
    {
        // Arrange
        var mockRepo = new Mock<AppServicesIElevatorRepository>();
        var elevator = new Elevator { Id = 1, CurrentFloor = 1 };
        mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(elevator);
        
        var service = new ElevatorControl.Api.Application.Services.ElevatorService(mockRepo.Object);

        // Act
        await service.MoveToFloorAsync(1, 10);

        // Assert
        Assert.Equal(10, elevator.CurrentFloor);
    }
}
