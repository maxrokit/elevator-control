using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElevatorControl.Api.Domain.Entities;
using ElevatorControl.Api.Domain.Enums;
using ElevatorControl.Api.Infrastructure.Repositories;

namespace ElevatorControl.Api.Application.Services;

public class ElevatorService
{
    private readonly IElevatorRepository _repository;

    public ElevatorService(IElevatorRepository repository)
    {
        _repository = repository;
    }

    public Task<Elevator> CreateElevatorAsync()
    {
        return _repository.CreateAsync();
    }

    public async Task<IEnumerable<object>> ListAsync()
    {
        var elevators = await _repository.GetAllAsync();
        return elevators.Select(e => new
        {
            e.Id,
            e.CurrentFloor,
            CurrentDirection = e.CurrentDirection.ToString(),
            FloorRequests = e.FloorDestinations.Select(f => new { Floor = f }).ToArray(),
            FloorCalls = e.FloorCalls.Select(kvp => new { floor = kvp.Key, isUpCalled = kvp.Value.IsUpCalled, isDownCalled = kvp.Value.IsDownCalled }).ToArray()
        });
    }

    public async Task<bool> AddFloorDestinationAsync(int elevatorId, int floor)
    {
        var elevator = await _repository.GetByIdAsync(elevatorId);
        if (elevator is null) return false;
        elevator.AddFloorDestination(floor);
        return true;
    }

    public async Task<int[]?> GetFloorDestinationsAsync(int elevatorId)
    {
        var elevator = await _repository.GetByIdAsync(elevatorId);
        if (elevator is null) return null;
        lock (elevator.FloorDestinations)
        {
            return elevator.FloorDestinations.ToArray();
        }
    }

    public async Task<bool> AddFloorCallAsync(int elevatorId, int floor, ElevatorDirection direction)
    {
        var elevator = await _repository.GetByIdAsync(elevatorId);
        if (elevator is null) return false;
        elevator.AddFloorCall(floor, direction);
        return true;
    }

    public async Task<int?> GetNextFloorAsync(int elevatorId)
    {
        var elevator = await _repository.GetByIdAsync(elevatorId);
        if (elevator is null) return null;
        return elevator.GetNextFloor();
    }

    public async Task MoveToFloorAsync(int elevatorId, int targetFloor)
    {
        var elevator = await _repository.GetByIdAsync(elevatorId);
        if (elevator is null) return;
        elevator.MoveToFloor(targetFloor);
    }
}
