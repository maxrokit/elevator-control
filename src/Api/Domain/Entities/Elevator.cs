using System.Collections.Generic;
using System.Linq;
using ElevatorControl.Api.Domain.Enums;

namespace ElevatorControl.Api.Domain.Entities;

public class Elevator
{
    private readonly int _minFloor;
    private readonly int _maxFloor;
    private readonly object _lock = new();

    public int Id { get; init; }
    public int CurrentFloor { get; set; }
    /// <summary>
    /// The current direction the elevator is traveling (Up or Down).
    /// Starts at Up and switches based on requests.
    /// </summary>
    public ElevatorDirection CurrentDirection { get; set; } = ElevatorDirection.Up;
    /// <summary>
    /// List of internal floor destinations made from inside the elevator.
    /// </summary>
    public List<int> FloorDestinations { get; } = new();
    /// <summary>
    /// Tracks the directions (up/down) that each floor was called from externally.
    /// Key: floor number, Value: (IsUpCalled, IsDownCalled) tuple indicating which directions were called.
    /// </summary>
    public Dictionary<int, (bool IsUpCalled, bool IsDownCalled)> FloorCalls { get; } = new();

    /// <summary>
    /// Initializes a new instance of the Elevator class.
    /// </summary>
    /// <param name="minFloor">The minimum floor number (default: 1)</param>
    /// <param name="maxFloor">The maximum floor number (default: 100)</param>
    public Elevator(int minFloor = 1, int maxFloor = 100)
    {
        if (minFloor >= maxFloor)
            throw new ArgumentException("Minimum floor must be less than maximum floor");

        _minFloor = minFloor;
        _maxFloor = maxFloor;
    }


    /// <summary>
    /// Adds a floor destination request from inside the elevator.
    /// </summary>
    /// 
    public void AddFloorDestination(int floor)
    {
        ValidateFloor(floor, nameof(floor));
        lock (_lock)
        {
            if (!FloorDestinations.Contains(floor)) FloorDestinations.Add(floor);
        }
    }

    /// <summary>
    /// Adds a floor call request from outside the elevator either going up or down.
    /// </summary>
    public void AddFloorCall(int floor, ElevatorDirection direction)
    {
        ValidateFloor(floor, nameof(floor));
        ValidateDirectionForFloor(floor, direction);

        lock (_lock)
        {
            if (FloorCalls.TryGetValue(floor, out var existing))
            {
                // Merge the new direction with existing
                var updated = (
                    IsUpCalled: existing.IsUpCalled || direction == ElevatorDirection.Up,
                    IsDownCalled: existing.IsDownCalled || direction == ElevatorDirection.Down
                );
                FloorCalls[floor] = updated;
            }
            else
            {
                // New floor call
                FloorCalls[floor] = (
                    IsUpCalled: direction == ElevatorDirection.Up,
                    IsDownCalled: direction == ElevatorDirection.Down
                );
            }
        }
    }

    /// <summary>
    /// Determines the next floor the elevator should visit and updates direction accordingly.
    /// Goes up to satisfy all up requests, then down to satisfy all down requests.
    /// </summary>
    public int? GetNextFloor()
    {
        lock (_lock)
        {
            var floorsAbove = FloorDestinations.Concat(FloorCalls.Keys)
                .Where(f => f > CurrentFloor)
                .Distinct()
                .OrderBy(f => f)
                .ToList();

            var floorsBelow = FloorDestinations.Concat(FloorCalls.Keys)
                .Where(f => f < CurrentFloor)
                .Distinct()
                .OrderByDescending(f => f)
                .ToList();

            // If moving up and there are requests above, go to the nearest
            if (CurrentDirection == ElevatorDirection.Up)
            {
                var nextUp = floorsAbove.FirstOrDefault();
                if (nextUp > 0) return nextUp;

                // No more up requests, switch to down
                if (floorsBelow.Any())
                {
                    CurrentDirection = ElevatorDirection.Down;
                    return floorsBelow.FirstOrDefault();
                }
            }
            else // Moving down
            {
                var nextDown = floorsBelow.FirstOrDefault();
                if (nextDown > 0) return nextDown;

                // No more down requests, switch to up
                if (floorsAbove.Any())
                {
                    CurrentDirection = ElevatorDirection.Up;
                    return floorsAbove.FirstOrDefault();
                }
            }

            return null;
        }
    }

    /// <summary>
    /// Move elevator to a specific floor and clear external calls for that floor in the direction of travel.
    /// </summary>
    public void MoveToFloor(int floor)
    {
        ValidateFloor(floor, nameof(floor));

        if (floor == CurrentFloor)
            return; // Already there

        lock (_lock)
        {
            var previousFloor = CurrentFloor;

            // Determine the direction we moved to reach this floor
            var movementDirection = floor > previousFloor
                ? ElevatorDirection.Up
                : ElevatorDirection.Down;

            // Update current floor first
            CurrentFloor = floor;

            // Remove internal request for this floor
            FloorDestinations.Remove(floor);

            // Clear external calls only for the target floor in the direction we moved
            ClearFloorCallInDirection(floor, movementDirection);

            var nextFloor = GetNextFloor();
            if (nextFloor.HasValue)
            {
                CurrentDirection = nextFloor.Value > CurrentFloor
                    ? ElevatorDirection.Up
                    : ElevatorDirection.Down;
            }
        }
    }

    /// <summary>
    /// Clears the external call for the specified floor in the given direction.
    /// </summary>
    private void ClearFloorCallInDirection(int floor, ElevatorDirection direction)
    {
        ValidateFloor(floor, nameof(floor));
        if (FloorCalls.TryGetValue(floor, out var calls))
        {
            var updated = (
                IsUpCalled: calls.IsUpCalled && direction != ElevatorDirection.Up,
                IsDownCalled: calls.IsDownCalled && direction != ElevatorDirection.Down
            );

            if (updated.IsUpCalled || updated.IsDownCalled)
            {
                FloorCalls[floor] = updated;
            }
            else
            {
                FloorCalls.Remove(floor);
            }
        }
    }

    /// <summary>
    /// Validates that the given floor is within acceptable bounds.
    /// </summary>
    private void ValidateFloor(int floor, string paramName)
    {
        if (floor < _minFloor || floor > _maxFloor)
            throw new ArgumentOutOfRangeException(paramName,
                $"Floor must be between {_minFloor} and {_maxFloor}");
    }

    /// <summary>
    /// Validates that the direction is valid for the given floor.
    /// </summary>
    private void ValidateDirectionForFloor(int floor, ElevatorDirection direction)
    {
        if (floor == _minFloor && direction == ElevatorDirection.Down)
            throw new ArgumentException($"Cannot call elevator to go down from the bottom floor ({_minFloor})", nameof(direction));

        if (floor == _maxFloor && direction == ElevatorDirection.Up)
            throw new ArgumentException($"Cannot call elevator to go up from the top floor ({_maxFloor})", nameof(direction));
    }
}
