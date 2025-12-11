namespace ElevatorControl.Api.Application.DTOs;

public sealed record ElevatorDto(
    int Id,
    int CurrentFloor,
    int CurrentDirection,
    FloorRequestDto[] FloorRequests,
    FloorCallDto[] FloorCalls
);
