namespace ElevatorControl.Api.Application.Commands;

public sealed record AddFloorDestinationCommand(int ElevatorId, int Floor);
