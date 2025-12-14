using ElevatorControl.Api.Infrastructure.Repositories;
using Xunit;

namespace ElevatorControl.Api.Tests.Infrastructure.Repositories;

public class ElevatorRepositoryTests
{
    [Fact]
    public async Task CreateAsync_CreatesElevatorWithUniqueId()
    {
        // Arrange
        var repository = new ElevatorRepository();

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
        var repository = new ElevatorRepository();

        // Act
        var elevator = await repository.CreateAsync();

        // Assert
        Assert.Equal(1, elevator.CurrentFloor);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsElevator_WhenExists()
    {
        // Arrange
        var repository = new ElevatorRepository();
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
        var repository = new ElevatorRepository();

        // Act
        var result = await repository.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsEmptyList_WhenNoElevators()
    {
        // Arrange
        var repository = new ElevatorRepository();

        // Act
        var elevators = await repository.GetAllAsync();

        // Assert
        Assert.Empty(elevators);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllElevators()
    {
        // Arrange
        var repository = new ElevatorRepository();
        var elevator1 = await repository.CreateAsync();
        var elevator2 = await repository.CreateAsync();
        var elevator3 = await repository.CreateAsync();

        // Act
        var elevators = (await repository.GetAllAsync()).ToList();

        // Assert
        Assert.Equal(3, elevators.Count);
        Assert.Contains(elevator1, elevators);
        Assert.Contains(elevator2, elevators);
        Assert.Contains(elevator3, elevators);
    }

    [Fact]
    public async Task CreateAsync_IsThreadSafe_GeneratesUniqueIds()
    {
        // Arrange
        var repository = new ElevatorRepository();
        var tasks = new List<Task<ElevatorControl.Api.Domain.Entities.Elevator>>();

        // Act - Create 100 elevators concurrently
        for (int i = 0; i < 100; i++)
        {
            tasks.Add(repository.CreateAsync());
        }
        var elevators = await Task.WhenAll(tasks);

        // Assert - All IDs should be unique
        var ids = elevators.Select(e => e.Id).ToList();
        Assert.Equal(100, ids.Distinct().Count());
    }
}
