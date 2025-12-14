using ElevatorControl.Api.Domain.Entities;
using ElevatorControl.Api.Domain.Enums;
using Xunit;

namespace ElevatorControl.Api.Tests.Domain.Entities;

public class ElevatorTests
{
    #region AddFloorDestination Tests

    [Fact]
    public void AddFloorDestination_AddsNewFloorToList()
    {
        // Arrange
        var elevator = new Elevator { Id = 1, CurrentFloor = 1 };

        // Act
        elevator.AddFloorDestination(5);

        // Assert
        Assert.Contains(5, elevator.FloorDestinations);
    }

    [Fact]
    public void AddFloorDestination_DoesNotAddDuplicateFloor()
    {
        // Arrange
        var elevator = new Elevator { Id = 1, CurrentFloor = 1 };
        elevator.AddFloorDestination(5);

        // Act
        elevator.AddFloorDestination(5);

        // Assert
        Assert.Single(elevator.FloorDestinations);
        Assert.Equal(5, elevator.FloorDestinations[0]);
    }

    [Fact]
    public void AddFloorDestination_AddsMultipleFloors()
    {
        // Arrange
        var elevator = new Elevator { Id = 1, CurrentFloor = 1 };

        // Act
        elevator.AddFloorDestination(3);
        elevator.AddFloorDestination(7);
        elevator.AddFloorDestination(5);

        // Assert
        Assert.Equal(3, elevator.FloorDestinations.Count);
        Assert.Contains(3, elevator.FloorDestinations);
        Assert.Contains(7, elevator.FloorDestinations);
        Assert.Contains(5, elevator.FloorDestinations);
    }

    #endregion

    #region AddFloorCall Tests

    [Fact]
    public void AddFloorCall_AddsUpCallForNewFloor()
    {
        // Arrange
        var elevator = new Elevator { Id = 1, CurrentFloor = 1 };

        // Act
        elevator.AddFloorCall(5, ElevatorDirection.Up);

        // Assert
        Assert.True(elevator.FloorCalls.ContainsKey(5));
        Assert.True(elevator.FloorCalls[5].IsUpCalled);
        Assert.False(elevator.FloorCalls[5].IsDownCalled);
    }

    [Fact]
    public void AddFloorCall_AddsDownCallForNewFloor()
    {
        // Arrange
        var elevator = new Elevator { Id = 1, CurrentFloor = 1 };

        // Act
        elevator.AddFloorCall(5, ElevatorDirection.Down);

        // Assert
        Assert.True(elevator.FloorCalls.ContainsKey(5));
        Assert.False(elevator.FloorCalls[5].IsUpCalled);
        Assert.True(elevator.FloorCalls[5].IsDownCalled);
    }

    [Fact]
    public void AddFloorCall_MergesBothDirectionsForSameFloor()
    {
        // Arrange
        var elevator = new Elevator { Id = 1, CurrentFloor = 1 };

        // Act
        elevator.AddFloorCall(5, ElevatorDirection.Up);
        elevator.AddFloorCall(5, ElevatorDirection.Down);

        // Assert
        Assert.Single(elevator.FloorCalls);
        Assert.True(elevator.FloorCalls[5].IsUpCalled);
        Assert.True(elevator.FloorCalls[5].IsDownCalled);
    }

    [Fact]
    public void AddFloorCall_PreservesExistingDirectionWhenAddingSameDirection()
    {
        // Arrange
        var elevator = new Elevator { Id = 1, CurrentFloor = 1 };
        elevator.AddFloorCall(5, ElevatorDirection.Up);

        // Act
        elevator.AddFloorCall(5, ElevatorDirection.Up);

        // Assert
        Assert.Single(elevator.FloorCalls);
        Assert.True(elevator.FloorCalls[5].IsUpCalled);
        Assert.False(elevator.FloorCalls[5].IsDownCalled);
    }

    [Fact]
    public void AddFloorCall_HandlesMultipleFloorsWithDifferentDirections()
    {
        // Arrange
        var elevator = new Elevator { Id = 1, CurrentFloor = 1 };

        // Act
        elevator.AddFloorCall(3, ElevatorDirection.Up);
        elevator.AddFloorCall(7, ElevatorDirection.Down);
        elevator.AddFloorCall(5, ElevatorDirection.Up);

        // Assert
        Assert.Equal(3, elevator.FloorCalls.Count);
        Assert.True(elevator.FloorCalls[3].IsUpCalled);
        Assert.True(elevator.FloorCalls[7].IsDownCalled);
        Assert.True(elevator.FloorCalls[5].IsUpCalled);
    }

    #endregion

    #region GetNextFloor Tests

    [Fact]
    public void GetNextFloor_ReturnsNullWhenNoRequests()
    {
        // Arrange
        var elevator = new Elevator { Id = 1, CurrentFloor = 5 };

        // Act
        var nextFloor = elevator.GetNextFloor();

        // Assert
        Assert.Null(nextFloor);
    }

    [Fact]
    public void GetNextFloor_MovingUp_ReturnsNearestFloorAbove()
    {
        // Arrange
        var elevator = new Elevator { Id = 1, CurrentFloor = 3, CurrentDirection = ElevatorDirection.Up };
        elevator.AddFloorDestination(7);
        elevator.AddFloorDestination(5);
        elevator.AddFloorDestination(9);

        // Act
        var nextFloor = elevator.GetNextFloor();

        // Assert
        Assert.Equal(5, nextFloor);
    }

    [Fact]
    public void GetNextFloor_MovingUp_SwitchesToDownWhenNoFloorsAbove()
    {
        // Arrange
        var elevator = new Elevator { Id = 1, CurrentFloor = 5, CurrentDirection = ElevatorDirection.Up };
        elevator.AddFloorDestination(2);
        elevator.AddFloorDestination(3);

        // Act
        var nextFloor = elevator.GetNextFloor();

        // Assert
        Assert.Equal(3, nextFloor);
        Assert.Equal(ElevatorDirection.Down, elevator.CurrentDirection);
    }

    [Fact]
    public void GetNextFloor_MovingDown_ReturnsNearestFloorBelow()
    {
        // Arrange
        var elevator = new Elevator { Id = 1, CurrentFloor = 7, CurrentDirection = ElevatorDirection.Down };
        elevator.AddFloorDestination(2);
        elevator.AddFloorDestination(5);
        elevator.AddFloorDestination(3);

        // Act
        var nextFloor = elevator.GetNextFloor();

        // Assert
        Assert.Equal(5, nextFloor);
    }

    [Fact]
    public void GetNextFloor_MovingDown_SwitchesToUpWhenNoFloorsBelow()
    {
        // Arrange
        var elevator = new Elevator { Id = 1, CurrentFloor = 3, CurrentDirection = ElevatorDirection.Down };
        elevator.AddFloorDestination(5);
        elevator.AddFloorDestination(7);

        // Act
        var nextFloor = elevator.GetNextFloor();

        // Assert
        Assert.Equal(5, nextFloor);
        Assert.Equal(ElevatorDirection.Up, elevator.CurrentDirection);
    }

    [Fact]
    public void GetNextFloor_CombinesInternalDestinationsAndExternalCalls()
    {
        // Arrange
        var elevator = new Elevator { Id = 1, CurrentFloor = 3, CurrentDirection = ElevatorDirection.Up };
        elevator.AddFloorDestination(7);
        elevator.AddFloorCall(5, ElevatorDirection.Up);

        // Act
        var nextFloor = elevator.GetNextFloor();

        // Assert
        Assert.Equal(5, nextFloor);
    }

    [Fact]
    public void GetNextFloor_HandlesMixedCallsAndDestinations()
    {
        // Arrange
        var elevator = new Elevator { Id = 1, CurrentFloor = 5, CurrentDirection = ElevatorDirection.Up };
        elevator.AddFloorDestination(8);
        elevator.AddFloorCall(6, ElevatorDirection.Down);
        elevator.AddFloorCall(10, ElevatorDirection.Up);

        // Act
        var nextFloor = elevator.GetNextFloor();

        // Assert
        Assert.Equal(6, nextFloor);
    }

    [Fact]
    public void GetNextFloor_IgnoresDuplicateBetweenCallsAndDestinations()
    {
        // Arrange
        var elevator = new Elevator { Id = 1, CurrentFloor = 3, CurrentDirection = ElevatorDirection.Up };
        elevator.AddFloorDestination(5);
        elevator.AddFloorCall(5, ElevatorDirection.Up);
        elevator.AddFloorDestination(7);

        // Act
        var nextFloor = elevator.GetNextFloor();

        // Assert
        Assert.Equal(5, nextFloor);
    }

    [Fact]
    public void GetNextFloor_HandlesBothDirectionCallsOnSameFloor()
    {
        // Arrange
        var elevator = new Elevator { Id = 1, CurrentFloor = 5, CurrentDirection = ElevatorDirection.Up };
        elevator.AddFloorCall(7, ElevatorDirection.Up);
        elevator.AddFloorCall(7, ElevatorDirection.Down);

        // Act
        var nextFloor = elevator.GetNextFloor();

        // Assert
        Assert.Equal(7, nextFloor);
    }

    [Fact]
    public void GetNextFloor_CompleteUpThenDownSequence()
    {
        // Arrange
        var elevator = new Elevator { Id = 1, CurrentFloor = 5, CurrentDirection = ElevatorDirection.Up };
        elevator.AddFloorDestination(8);
        elevator.AddFloorDestination(3);
        elevator.AddFloorCall(7, ElevatorDirection.Up);
        elevator.AddFloorCall(2, ElevatorDirection.Down);

        // Act - Going up first
        var next1 = elevator.GetNextFloor();
        Assert.Equal(7, next1);

        elevator.MoveToFloor(7);
        var next2 = elevator.GetNextFloor();
        Assert.Equal(8, next2);

        // Should switch to down after no more up requests
        elevator.MoveToFloor(8);
        var next3 = elevator.GetNextFloor();
        Assert.Equal(3, next3);
        Assert.Equal(ElevatorDirection.Down, elevator.CurrentDirection);

        elevator.MoveToFloor(3);
        var next4 = elevator.GetNextFloor();
        Assert.Equal(2, next4);
    }

    #endregion

    #region MoveToFloor Tests

    [Fact]
    public void MoveToFloor_UpdatesCurrentFloor()
    {
        // Arrange
        var elevator = new Elevator { Id = 1, CurrentFloor = 3 };

        // Act
        elevator.MoveToFloor(7);

        // Assert
        Assert.Equal(7, elevator.CurrentFloor);
    }

    [Fact]
    public void MoveToFloor_RemovesInternalDestination()
    {
        // Arrange
        var elevator = new Elevator { Id = 1, CurrentFloor = 3 };
        elevator.AddFloorDestination(5);
        elevator.AddFloorDestination(7);

        // Act
        elevator.MoveToFloor(5);

        // Assert
        Assert.DoesNotContain(5, elevator.FloorDestinations);
        Assert.Contains(7, elevator.FloorDestinations);
    }

    [Fact]
    public void MoveToFloor_MovingUp_ClearsUpCallOnly()
    {
        // Arrange
        var elevator = new Elevator { Id = 1, CurrentFloor = 3, CurrentDirection = ElevatorDirection.Up };
        elevator.AddFloorCall(5, ElevatorDirection.Up);
        elevator.AddFloorCall(5, ElevatorDirection.Down);

        // Act
        elevator.MoveToFloor(5);

        // Assert
        Assert.True(elevator.FloorCalls.ContainsKey(5));
        Assert.False(elevator.FloorCalls[5].IsUpCalled);
        Assert.True(elevator.FloorCalls[5].IsDownCalled);
    }

    [Fact]
    public void MoveToFloor_MovingDown_ClearsDownCallOnly()
    {
        // Arrange
        var elevator = new Elevator { Id = 1, CurrentFloor = 7, CurrentDirection = ElevatorDirection.Down };
        elevator.AddFloorCall(5, ElevatorDirection.Up);
        elevator.AddFloorCall(5, ElevatorDirection.Down);

        // Act
        elevator.MoveToFloor(5);

        // Assert
        Assert.True(elevator.FloorCalls.ContainsKey(5));
        Assert.True(elevator.FloorCalls[5].IsUpCalled);
        Assert.False(elevator.FloorCalls[5].IsDownCalled);
    }

    [Fact]
    public void MoveToFloor_RemovesFloorCallCompletelyWhenOnlyOneDirection()
    {
        // Arrange
        var elevator = new Elevator { Id = 1, CurrentFloor = 3, CurrentDirection = ElevatorDirection.Up };
        elevator.AddFloorCall(5, ElevatorDirection.Up);

        // Act
        elevator.MoveToFloor(5);

        // Assert
        Assert.False(elevator.FloorCalls.ContainsKey(5));
    }

    [Fact]
    public void MoveToFloor_HandlesFloorWithNoCallsOrDestinations()
    {
        // Arrange
        var elevator = new Elevator { Id = 1, CurrentFloor = 3 };

        // Act
        elevator.MoveToFloor(5);

        // Assert
        Assert.Equal(5, elevator.CurrentFloor);
        Assert.False(elevator.FloorCalls.ContainsKey(5));
    }

    [Fact]
    public void MoveToFloor_ClearsOnlyCurrentDirectionCall()
    {
        // Arrange
        var elevator = new Elevator { Id = 1, CurrentFloor = 5, CurrentDirection = ElevatorDirection.Up };
        elevator.AddFloorCall(7, ElevatorDirection.Up);

        // Act
        elevator.MoveToFloor(7);

        // Assert
        Assert.False(elevator.FloorCalls.ContainsKey(7));
    }

    #endregion

    #region Integration Scenarios

    [Fact]
    public void Scenario_ElevatorServicesMultipleFloorCallsInOrder()
    {
        // Arrange: Elevator at floor 1, multiple calls from different floors
        var elevator = new Elevator { Id = 1, CurrentFloor = 1, CurrentDirection = ElevatorDirection.Up };

        // Floor 3 calls up, Floor 5 calls down, Floor 7 internal destination
        elevator.AddFloorCall(3, ElevatorDirection.Up);
        elevator.AddFloorCall(5, ElevatorDirection.Down);
        elevator.AddFloorDestination(7);

        // Act & Assert: Should go up to 3, then 5, then 7, then back down to handle floor 5 down call
        var next1 = elevator.GetNextFloor();
        Assert.Equal(3, next1);
        elevator.MoveToFloor(3);

        var next2 = elevator.GetNextFloor();
        Assert.Equal(5, next2);
        elevator.MoveToFloor(5);
        // Floor 5 up direction cleared, but down call still pending
        Assert.True(elevator.FloorCalls.ContainsKey(5));

        var next3 = elevator.GetNextFloor();
        Assert.Equal(7, next3);
        elevator.MoveToFloor(7);

        // Now should switch to down and service floor 5 down call
        var next4 = elevator.GetNextFloor();
        Assert.Equal(5, next4);
        Assert.Equal(ElevatorDirection.Down, elevator.CurrentDirection);
    }

    [Fact]
    public void Scenario_ElevatorHandlesDynamicRequestsWhileMoving()
    {
        // Arrange: Elevator moving up from floor 2
        var elevator = new Elevator { Id = 1, CurrentFloor = 2, CurrentDirection = ElevatorDirection.Up };
        elevator.AddFloorDestination(8);

        // Act: New call added while elevator is moving
        var next1 = elevator.GetNextFloor();
        Assert.Equal(8, next1);

        // Someone at floor 5 calls up
        elevator.AddFloorCall(5, ElevatorDirection.Up);

        // Should now stop at floor 5 first
        var next2 = elevator.GetNextFloor();
        Assert.Equal(5, next2);

        elevator.MoveToFloor(5);
        var next3 = elevator.GetNextFloor();
        Assert.Equal(8, next3);
    }

    [Fact]
    public void Scenario_ElevatorAtTopFloorWithDownwardRequests()
    {
        // Arrange: Elevator at floor 10 (top floor) moving up with only downward requests
        var elevator = new Elevator { Id = 1, CurrentFloor = 10, CurrentDirection = ElevatorDirection.Up };
        elevator.AddFloorCall(5, ElevatorDirection.Down);
        elevator.AddFloorDestination(3);

        // Act: Should switch to down direction
        var nextFloor = elevator.GetNextFloor();

        // Assert
        Assert.Equal(5, nextFloor);
        Assert.Equal(ElevatorDirection.Down, elevator.CurrentDirection);
    }

    [Fact]
    public void Scenario_MultiplePassengersEnterAndExit()
    {
        // Arrange: Elevator at floor 1
        var elevator = new Elevator { Id = 1, CurrentFloor = 1, CurrentDirection = ElevatorDirection.Up };

        // Floor 3 and 5 call up
        elevator.AddFloorCall(3, ElevatorDirection.Up);
        elevator.AddFloorCall(5, ElevatorDirection.Up);

        // Act: Move to floor 3
        var next1 = elevator.GetNextFloor();
        Assert.Equal(3, next1);
        elevator.MoveToFloor(3);

        // Passenger at 3 requests floor 7
        elevator.AddFloorDestination(7);

        // Move to floor 5
        var next2 = elevator.GetNextFloor();
        Assert.Equal(5, next2);
        elevator.MoveToFloor(5);

        // Passenger at 5 requests floor 8
        elevator.AddFloorDestination(8);

        // Should now go to 7, then 8
        var next3 = elevator.GetNextFloor();
        Assert.Equal(7, next3);
        elevator.MoveToFloor(7);

        var next4 = elevator.GetNextFloor();
        Assert.Equal(8, next4);
        elevator.MoveToFloor(8);

        // No more requests
        var next5 = elevator.GetNextFloor();
        Assert.Null(next5);
    }

    [Fact]
    public void Scenario_ElevatorHandlesOppositeDirectionCallsAfterCurrentDirection()
    {
        // Arrange: Elevator at floor 5 moving up
        var elevator = new Elevator { Id = 1, CurrentFloor = 5, CurrentDirection = ElevatorDirection.Up };

        // Floor 8 calls down, floor 3 calls up, internal request to floor 7
        elevator.AddFloorCall(8, ElevatorDirection.Down);
        elevator.AddFloorCall(3, ElevatorDirection.Up);
        elevator.AddFloorDestination(7);

        // Act & Assert: Should go up to 7, then 8
        var next1 = elevator.GetNextFloor();
        Assert.Equal(7, next1);
        elevator.MoveToFloor(7);

        var next2 = elevator.GetNextFloor();
        Assert.Equal(8, next2);
        elevator.MoveToFloor(8);
        // Floor 8 still has down call since we arrived going up
        Assert.True(elevator.FloorCalls.ContainsKey(8));
        Assert.True(elevator.FloorCalls[8].IsDownCalled);

        // Should now switch to down and go to floor 3 (nearest down request)
        var next3 = elevator.GetNextFloor();
        Assert.Equal(3, next3);
        Assert.Equal(ElevatorDirection.Down, elevator.CurrentDirection);

        elevator.MoveToFloor(3);
        // Floor 3's up call should still be pending because we arrived going down
        Assert.True(elevator.FloorCalls.ContainsKey(3));
        Assert.True(elevator.FloorCalls[3].IsUpCalled);

        // No more down requests, should switch back to up
        var next4 = elevator.GetNextFloor();
        Assert.NotNull(next4);
        Assert.Equal(ElevatorDirection.Up, elevator.CurrentDirection);
    }

    #endregion

    #region Constructor Validation Tests

    [Fact]
    public void Constructor_ThrowsArgumentException_WhenMinFloorEqualsMaxFloor()
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => new Elevator(5, 5));
        Assert.Contains("Minimum floor must be less than maximum floor", ex.Message);
    }

    [Fact]
    public void Constructor_ThrowsArgumentException_WhenMinFloorGreaterThanMaxFloor()
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => new Elevator(10, 5));
        Assert.Contains("Minimum floor must be less than maximum floor", ex.Message);
    }

    [Fact]
    public void Constructor_AcceptsValidMinMaxFloors()
    {
        // Act
        var elevator = new Elevator(minFloor: 0, maxFloor: 50);

        // Assert
        Assert.NotNull(elevator);
    }

    [Fact]
    public void Constructor_UsesDefaultFloorRange_WhenNoParametersProvided()
    {
        // Act
        var elevator = new Elevator { Id = 1, CurrentFloor = 1 };

        // Assert - Should accept floors 1-100 (default range)
        elevator.AddFloorDestination(1);
        elevator.AddFloorDestination(100);
        Assert.Contains(1, elevator.FloorDestinations);
        Assert.Contains(100, elevator.FloorDestinations);
    }

    #endregion

    #region Direction Validation Tests

    [Fact]
    public void AddFloorCall_ThrowsArgumentException_WhenCallingDownFromBottomFloor()
    {
        // Arrange
        var elevator = new Elevator(minFloor: 1, maxFloor: 10) { Id = 1, CurrentFloor = 5 };

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => 
            elevator.AddFloorCall(1, ElevatorDirection.Down));
        Assert.Contains("Cannot call elevator to go down from the bottom floor (1)", ex.Message);
    }

    [Fact]
    public void AddFloorCall_ThrowsArgumentException_WhenCallingUpFromTopFloor()
    {
        // Arrange
        var elevator = new Elevator(minFloor: 1, maxFloor: 10) { Id = 1, CurrentFloor = 5 };

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => 
            elevator.AddFloorCall(10, ElevatorDirection.Up));
        Assert.Contains("Cannot call elevator to go up from the top floor (10)", ex.Message);
    }

    [Fact]
    public void AddFloorCall_AcceptsUpCall_AtBottomFloor()
    {
        // Arrange
        var elevator = new Elevator(minFloor: 1, maxFloor: 10) { Id = 1, CurrentFloor = 5 };

        // Act
        elevator.AddFloorCall(1, ElevatorDirection.Up);

        // Assert
        Assert.True(elevator.FloorCalls.ContainsKey(1));
        Assert.True(elevator.FloorCalls[1].IsUpCalled);
    }

    [Fact]
    public void AddFloorCall_AcceptsDownCall_AtTopFloor()
    {
        // Arrange
        var elevator = new Elevator(minFloor: 1, maxFloor: 10) { Id = 1, CurrentFloor = 5 };

        // Act
        elevator.AddFloorCall(10, ElevatorDirection.Down);

        // Assert
        Assert.True(elevator.FloorCalls.ContainsKey(10));
        Assert.True(elevator.FloorCalls[10].IsDownCalled);
    }

    [Fact]
    public void AddFloorCall_WorksWithCustomFloorRange()
    {
        // Arrange - Building with floors -2 to 20
        var elevator = new Elevator(minFloor: -2, maxFloor: 20) { Id = 1, CurrentFloor = 0 };

        // Act & Assert - Bottom floor can only go up
        var ex1 = Assert.Throws<ArgumentException>(() => 
            elevator.AddFloorCall(-2, ElevatorDirection.Down));
        Assert.Contains("Cannot call elevator to go down from the bottom floor (-2)", ex1.Message);

        // Top floor can only go down
        var ex2 = Assert.Throws<ArgumentException>(() => 
            elevator.AddFloorCall(20, ElevatorDirection.Up));
        Assert.Contains("Cannot call elevator to go up from the top floor (20)", ex2.Message);

        // Valid calls should work
        elevator.AddFloorCall(-2, ElevatorDirection.Up);
        elevator.AddFloorCall(20, ElevatorDirection.Down);
        Assert.True(elevator.FloorCalls[-2].IsUpCalled);
        Assert.True(elevator.FloorCalls[20].IsDownCalled);
    }

    #endregion

    #region Floor Validation with Custom Ranges Tests

    [Fact]
    public void AddFloorDestination_ThrowsArgumentOutOfRangeException_WhenFloorBelowMinimum()
    {
        // Arrange
        var elevator = new Elevator(minFloor: 5, maxFloor: 20) { Id = 1, CurrentFloor = 10 };

        // Act & Assert
        var ex = Assert.Throws<ArgumentOutOfRangeException>(() => 
            elevator.AddFloorDestination(3));
        Assert.Contains("Floor must be between 5 and 20", ex.Message);
    }

    [Fact]
    public void AddFloorDestination_ThrowsArgumentOutOfRangeException_WhenFloorAboveMaximum()
    {
        // Arrange
        var elevator = new Elevator(minFloor: 5, maxFloor: 20) { Id = 1, CurrentFloor = 10 };

        // Act & Assert
        var ex = Assert.Throws<ArgumentOutOfRangeException>(() => 
            elevator.AddFloorDestination(25));
        Assert.Contains("Floor must be between 5 and 20", ex.Message);
    }

    [Fact]
    public void AddFloorDestination_AcceptsFloor_WithinCustomRange()
    {
        // Arrange
        var elevator = new Elevator(minFloor: 5, maxFloor: 20) { Id = 1, CurrentFloor = 10 };

        // Act
        elevator.AddFloorDestination(5);
        elevator.AddFloorDestination(20);
        elevator.AddFloorDestination(12);

        // Assert
        Assert.Contains(5, elevator.FloorDestinations);
        Assert.Contains(20, elevator.FloorDestinations);
        Assert.Contains(12, elevator.FloorDestinations);
    }

    [Fact]
    public void AddFloorCall_ThrowsArgumentOutOfRangeException_WhenFloorOutsideCustomRange()
    {
        // Arrange
        var elevator = new Elevator(minFloor: 0, maxFloor: 15) { Id = 1, CurrentFloor = 5 };

        // Act & Assert - Below minimum
        var ex1 = Assert.Throws<ArgumentOutOfRangeException>(() => 
            elevator.AddFloorCall(-1, ElevatorDirection.Up));
        Assert.Contains("Floor must be between 0 and 15", ex1.Message);

        // Above maximum
        var ex2 = Assert.Throws<ArgumentOutOfRangeException>(() => 
            elevator.AddFloorCall(16, ElevatorDirection.Down));
        Assert.Contains("Floor must be between 0 and 15", ex2.Message);
    }

    [Fact]
    public void MoveToFloor_ThrowsArgumentOutOfRangeException_WhenFloorOutsideCustomRange()
    {
        // Arrange
        var elevator = new Elevator(minFloor: 10, maxFloor: 30) { Id = 1, CurrentFloor = 15 };

        // Act & Assert - Below minimum
        var ex1 = Assert.Throws<ArgumentOutOfRangeException>(() => 
            elevator.MoveToFloor(5));
        Assert.Contains("Floor must be between 10 and 30", ex1.Message);

        // Above maximum
        var ex2 = Assert.Throws<ArgumentOutOfRangeException>(() => 
            elevator.MoveToFloor(35));
        Assert.Contains("Floor must be between 10 and 30", ex2.Message);
    }

    #endregion
}
