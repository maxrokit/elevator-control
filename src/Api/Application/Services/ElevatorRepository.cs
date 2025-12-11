using System.Collections.Concurrent;
using System.Threading.Tasks;
using ElevatorControl.Api.Domain.Entities;

namespace ElevatorControl.Api.Application.Services;

public interface IElevatorRepository
{
    Task<Elevator?> GetByIdAsync(int id);
    Task<IEnumerable<Elevator>> GetAllAsync();
    Task<Elevator> CreateAsync();
}

public class ElevatorRepository : IElevatorRepository
{
    private readonly ConcurrentDictionary<int, Elevator> _elevators = new();

    public ElevatorRepository()
    {
        // Initialize with one elevator
        _ = CreateAsync();
    }

    public Task<Elevator?> GetByIdAsync(int id)
    {
        _elevators.TryGetValue(id, out var elevator);
        return Task.FromResult(elevator);
    }

    public Task<IEnumerable<Elevator>> GetAllAsync()
    {
        return Task.FromResult<IEnumerable<Elevator>>(_elevators.Values);
    }

    public Task<Elevator> CreateAsync()
    {
        var newElevator = new Elevator
        {
            Id = _elevators.Count + 1,
            CurrentFloor = 1,
            CurrentDirection = Domain.Enums.ElevatorDirection.Up
        };
        _elevators[newElevator.Id] = newElevator;
        return Task.FromResult(newElevator);
    }
}
