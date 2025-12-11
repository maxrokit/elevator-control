namespace ElevatorControl.Api.Application.Commands;

public sealed record MoveToFloorCommand(int ElevatorId, int TargetFloor);
