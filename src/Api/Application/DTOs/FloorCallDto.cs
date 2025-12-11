namespace ElevatorControl.Api.Application.DTOs;

public sealed record FloorCallDto(int Floor, bool IsUpCalled, bool IsDownCalled);
