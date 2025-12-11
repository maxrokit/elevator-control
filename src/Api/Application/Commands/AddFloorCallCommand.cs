using ElevatorControl.Api.Domain.Enums;

namespace ElevatorControl.Api.Application.Commands;

public sealed record AddFloorCallCommand(int ElevatorId, int Floor, ElevatorDirection Direction);
