using ElevatorControl.Api.Domain.Enums;
using ElevatorControl.Api.Presentation.Models;
using Xunit;

namespace ElevatorControl.Api.Tests.Presentation.Models;

public class FloorCallRequestTests
{
    [Fact]
    public void Constructor_SetsFloorProperty()
    {
        // Arrange & Act
        var request = new FloorCallRequest(5, ElevatorDirection.Up);

        // Assert
        Assert.Equal(5, request.Floor);
    }

    [Fact]
    public void Constructor_SetsDirectionProperty()
    {
        // Arrange & Act
        var request = new FloorCallRequest(7, ElevatorDirection.Down);

        // Assert
        Assert.Equal(ElevatorDirection.Down, request.Direction);
    }

    [Fact]
    public void Constructor_UsesUpAsDefaultDirection()
    {
        // Arrange & Act
        var request = new FloorCallRequest(10);

        // Assert
        Assert.Equal(ElevatorDirection.Up, request.Direction);
    }

    [Fact]
    public void Equality_ReturnsTrueForSameValues()
    {
        // Arrange
        var request1 = new FloorCallRequest(5, ElevatorDirection.Up);
        var request2 = new FloorCallRequest(5, ElevatorDirection.Up);

        // Act & Assert
        Assert.Equal(request1, request2);
    }

    [Fact]
    public void Equality_ReturnsFalseForDifferentFloors()
    {
        // Arrange
        var request1 = new FloorCallRequest(5, ElevatorDirection.Up);
        var request2 = new FloorCallRequest(6, ElevatorDirection.Up);

        // Act & Assert
        Assert.NotEqual(request1, request2);
    }

    [Fact]
    public void Equality_ReturnsFalseForDifferentDirections()
    {
        // Arrange
        var request1 = new FloorCallRequest(5, ElevatorDirection.Up);
        var request2 = new FloorCallRequest(5, ElevatorDirection.Down);

        // Act & Assert
        Assert.NotEqual(request1, request2);
    }

    [Fact]
    public void GetHashCode_ReturnsSameValueForEqualObjects()
    {
        // Arrange
        var request1 = new FloorCallRequest(8, ElevatorDirection.Down);
        var request2 = new FloorCallRequest(8, ElevatorDirection.Down);

        // Act & Assert
        Assert.Equal(request1.GetHashCode(), request2.GetHashCode());
    }

    [Fact]
    public void ToString_ReturnsFormattedString()
    {
        // Arrange
        var request = new FloorCallRequest(12, ElevatorDirection.Up);

        // Act
        var result = request.ToString();

        // Assert
        Assert.Contains("12", result);
        Assert.Contains("Up", result);
    }
}
