using System.Text.Json.Serialization;

namespace ElevatorControl.Api.Domain.Enums;

/// <summary>
/// Represents the direction an elevator is traveling.
/// </summary>
[JsonConverter(typeof(ElevatorDirectionJsonConverter))]
public enum ElevatorDirection
{
    Up = 1,
    Down = -1
}
