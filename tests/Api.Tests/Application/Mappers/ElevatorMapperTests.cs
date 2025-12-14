using ElevatorControl.Api.Application.Mappers;
using ElevatorControl.Api.Domain.Entities;
using ElevatorControl.Api.Domain.Enums;
using Xunit;

namespace ElevatorControl.Api.Tests.Application.Mappers;

public class ElevatorMapperTests
{
    [Fact]
    public void ToDto_MapsBasicProperties()
    {
        // Arrange
        var elevator = new Elevator 
        { 
            Id = 5, 
            CurrentFloor = 10,
            CurrentDirection = ElevatorDirection.Up
        };

        // Act
        var dto = elevator.ToDto();

        // Assert
        Assert.Equal(5, dto.Id);
        Assert.Equal(10, dto.CurrentFloor);
        Assert.Equal(1, dto.CurrentDirection);
    }

    [Fact]
    public void ToDto_MapsFloorDestinations()
    {
        // Arrange
        var elevator = new Elevator { Id = 1, CurrentFloor = 1 };
        elevator.AddFloorDestination(5);
        elevator.AddFloorDestination(8);
        elevator.AddFloorDestination(3);

        // Act
        var dto = elevator.ToDto();

        // Assert
        Assert.Equal(3, dto.FloorRequests.Length);
        Assert.Contains(dto.FloorRequests, f => f.Floor == 5);
        Assert.Contains(dto.FloorRequests, f => f.Floor == 8);
        Assert.Contains(dto.FloorRequests, f => f.Floor == 3);
    }

    [Fact]
    public void ToDto_MapsFloorCalls()
    {
        // Arrange
        var elevator = new Elevator { Id = 1, CurrentFloor = 1 };
        elevator.AddFloorCall(4, ElevatorDirection.Up);
        elevator.AddFloorCall(7, ElevatorDirection.Down);
        elevator.AddFloorCall(10, ElevatorDirection.Up);

        // Act
        var dto = elevator.ToDto();

        // Assert
        Assert.Equal(3, dto.FloorCalls.Length);
        
        var call4 = dto.FloorCalls.First(c => c.Floor == 4);
        Assert.True(call4.IsUpCalled);
        Assert.False(call4.IsDownCalled);
        
        var call7 = dto.FloorCalls.First(c => c.Floor == 7);
        Assert.False(call7.IsUpCalled);
        Assert.True(call7.IsDownCalled);
    }

    [Fact]
    public void ToDto_MapsBothDirectionsCalls()
    {
        // Arrange
        var elevator = new Elevator { Id = 1, CurrentFloor = 1 };
        elevator.AddFloorCall(5, ElevatorDirection.Up);
        elevator.AddFloorCall(5, ElevatorDirection.Down);

        // Act
        var dto = elevator.ToDto();

        // Assert
        Assert.Single(dto.FloorCalls);
        var call = dto.FloorCalls[0];
        Assert.Equal(5, call.Floor);
        Assert.True(call.IsUpCalled);
        Assert.True(call.IsDownCalled);
    }

    [Fact]
    public void ToDto_HandlesEmptyCollections()
    {
        // Arrange
        var elevator = new Elevator { Id = 1, CurrentFloor = 1 };

        // Act
        var dto = elevator.ToDto();

        // Assert
        Assert.Empty(dto.FloorRequests);
        Assert.Empty(dto.FloorCalls);
    }

    [Fact]
    public void ToDto_MapsDownDirection()
    {
        // Arrange
        var elevator = new Elevator 
        { 
            Id = 2, 
            CurrentFloor = 15,
            CurrentDirection = ElevatorDirection.Down
        };

        // Act
        var dto = elevator.ToDto();

        // Assert
        Assert.Equal(-1, dto.CurrentDirection);
    }
}
