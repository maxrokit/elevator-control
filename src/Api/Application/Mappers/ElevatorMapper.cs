using System.Linq;
using ElevatorControl.Api.Application.DTOs;
using ElevatorControl.Api.Domain.Entities;

namespace ElevatorControl.Api.Application.Mappers;

public static class ElevatorMapper
{
    public static ElevatorDto ToDto(this Elevator elevator)
    {
        return new ElevatorDto(
            elevator.Id,
            elevator.CurrentFloor,
            (int)elevator.CurrentDirection,
            elevator.FloorDestinations.Select(f => new FloorRequestDto(f)).ToArray(),
            elevator.FloorCalls.Select(kvp => new FloorCallDto(
                kvp.Key,
                kvp.Value.IsUpCalled,
                kvp.Value.IsDownCalled
            )).ToArray()
        );
    }
}
