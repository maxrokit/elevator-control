using ElevatorControl.Api.Domain.Enums;

namespace ElevatorControl.Api.Presentation.Models;

public sealed record FloorCallRequest(int Floor, ElevatorDirection Direction = ElevatorDirection.Up);
