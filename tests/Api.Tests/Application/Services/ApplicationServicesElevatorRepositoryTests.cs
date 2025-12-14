using ElevatorControl.Api.Application.Services;
using Xunit;

namespace ElevatorControl.Api.Tests.Application.Services;

public class ApplicationServicesElevatorRepositoryTests
{
    [Fact]
    public async Task Constructor_InitializesWithOneElevator()
    {
        // Arrange & Act
        var repository = new ElevatorControl.Api.Application.Services.ElevatorRepository();
        var elevators = await repository.GetAllAsync();

        // Assert
        Assert.Single(elevators);
    }

    [Fact]
    public async Task CreateAsync_CreatesElevatorWithIncrementedId()
    {
        // Arrange
        var repository = new ElevatorControl.Api.Application.Services.ElevatorRepository();

        // Act
        var elevator1 = await repository.CreateAsync();
        var elevator2 = await repository.CreateAsync();

        // Assert
        Assert.NotEqual(elevator1.Id, elevator2.Id);
        Assert.True(elevator2.Id > elevator1.Id);
    }

    [Fact]
    public async Task CreateAsync_SetsInitialFloorToOne()
    {
        // Arrange
        var repository = new ElevatorControl.Api.Application.Services.ElevatorRepository();

        // Act
        var elevator = await repository.CreateAsync();

        // Assert
        Assert.Equal(1, elevator.CurrentFloor);
    }

    [Fact]
    public async Task CreateAsync_SetsInitialDirectionToUp()
    {
        // Arrange
        var repository = new ElevatorControl.Api.Application.Services.ElevatorRepository();

        // Act
        var elevator = await repository.CreateAsync();

        // Assert
        Assert.Equal(ElevatorControl.Api.Domain.Enums.ElevatorDirection.Up, elevator.CurrentDirection);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsElevator_WhenExists()
    {
        // Arrange
        var repository = new ElevatorControl.Api.Application.Services.ElevatorRepository();
        var created = await repository.CreateAsync();

        // Act
        var retrieved = await repository.GetByIdAsync(created.Id);

        // Assert
        Assert.NotNull(retrieved);
        Assert.Equal(created.Id, retrieved.Id);
        Assert.Same(created, retrieved);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenNotExists()
    {
        // Arrange
        var repository = new ElevatorControl.Api.Application.Services.ElevatorRepository();

        // Act
        var result = await repository.GetByIdAsync(9999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllElevators()
    {
        // Arrange
        var repository = new ElevatorControl.Api.Application.Services.ElevatorRepository();
        
        // Constructor creates one, create two more
        var elevator2 = await repository.CreateAsync();
        var elevator3 = await repository.CreateAsync();

        // Act
        var elevators = (await repository.GetAllAsync()).ToList();

        // Assert
        Assert.Equal(3, elevators.Count);
        Assert.Contains(elevator2, elevators);
        Assert.Contains(elevator3, elevators);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsSharedElevatorInstances()
    {
        // Arrange
        var repository = new ElevatorControl.Api.Application.Services.ElevatorRepository();
        var created = await repository.CreateAsync();

        // Act
        var allElevators = (await repository.GetAllAsync()).ToList();
        var retrievedById = await repository.GetByIdAsync(created.Id);

        // Assert
        Assert.Contains(allElevators, e => ReferenceEquals(e, created));
        Assert.Same(created, retrievedById);
    }

    [Fact]
    public async Task CreateAsync_IdBasedOnCount_NotThreadSafe()
    {
        // Arrange
        var repository = new ElevatorControl.Api.Application.Services.ElevatorRepository();

        // Act - Create multiple elevators
        var elevator1 = await repository.CreateAsync();
        var elevator2 = await repository.CreateAsync();
        var elevator3 = await repository.CreateAsync();

        // Assert - IDs should be based on count + 1
        var allElevators = (await repository.GetAllAsync()).ToList();
        Assert.Equal(4, allElevators.Count); // Constructor creates 1, plus 3 more
    }
}
