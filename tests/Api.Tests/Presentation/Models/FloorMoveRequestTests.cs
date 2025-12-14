using ElevatorControl.Api.Presentation.Models;
using Xunit;

namespace ElevatorControl.Api.Tests.Presentation.Models;

public class FloorMoveRequestTests
{
    [Fact]
    public void Constructor_SetsFloorProperty()
    {
        // Arrange & Act
        var request = new FloorMoveRequest(10);

        // Assert
        Assert.Equal(10, request.Floor);
    }

    [Fact]
    public void Equality_ReturnsTrueForSameFloor()
    {
        // Arrange
        var request1 = new FloorMoveRequest(7);
        var request2 = new FloorMoveRequest(7);

        // Act & Assert
        Assert.Equal(request1, request2);
    }

    [Fact]
    public void Equality_ReturnsFalseForDifferentFloors()
    {
        // Arrange
        var request1 = new FloorMoveRequest(3);
        var request2 = new FloorMoveRequest(8);

        // Act & Assert
        Assert.NotEqual(request1, request2);
    }

    [Fact]
    public void GetHashCode_ReturnsSameValueForEqualObjects()
    {
        // Arrange
        var request1 = new FloorMoveRequest(12);
        var request2 = new FloorMoveRequest(12);

        // Act & Assert
        Assert.Equal(request1.GetHashCode(), request2.GetHashCode());
    }

    [Fact]
    public void ToString_ContainsFloorNumber()
    {
        // Arrange
        var request = new FloorMoveRequest(20);

        // Act
        var result = request.ToString();

        // Assert
        Assert.Contains("20", result);
    }

    [Fact]
    public void Constructor_AcceptsNegativeFloor()
    {
        // Arrange & Act
        var request = new FloorMoveRequest(-1);

        // Assert
        Assert.Equal(-1, request.Floor);
    }

    [Fact]
    public void Constructor_AcceptsZeroFloor()
    {
        // Arrange & Act
        var request = new FloorMoveRequest(0);

        // Assert
        Assert.Equal(0, request.Floor);
    }

    [Fact]
    public void With_CreatesNewInstanceWithModifiedFloor()
    {
        // Arrange
        var request = new FloorMoveRequest(15);

        // Act
        var modified = request with { Floor = 20 };

        // Assert
        Assert.Equal(15, request.Floor);
        Assert.Equal(20, modified.Floor);
    }
}
