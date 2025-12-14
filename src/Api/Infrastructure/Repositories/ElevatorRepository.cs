using System.Collections.Concurrent;
using System.Threading.Tasks;
using ElevatorControl.Api.Domain.Entities;

namespace ElevatorControl.Api.Infrastructure.Repositories;

public class ElevatorRepository : IElevatorRepository
{
    private readonly ConcurrentDictionary<int, Elevator> _elevators = new();
    private int _nextId = 1;

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
        var id = Interlocked.Increment(ref _nextId);
        var newElevator = new Elevator { Id = id, CurrentFloor = 1 };
        _elevators[newElevator.Id] = newElevator;
        return Task.FromResult(newElevator);
    }
}
