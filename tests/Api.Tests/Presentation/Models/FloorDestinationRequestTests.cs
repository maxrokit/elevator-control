using ElevatorControl.Api.Presentation.Models;
using Xunit;

namespace ElevatorControl.Api.Tests.Presentation.Models;

public class FloorDestinationRequestTests
{
    [Fact]
    public void Constructor_SetsFloorProperty()
    {
        // Arrange & Act
        var request = new FloorDestinationRequest(8);

        // Assert
        Assert.Equal(8, request.Floor);
    }

    [Fact]
    public void Equality_ReturnsTrueForSameFloor()
    {
        // Arrange
        var request1 = new FloorDestinationRequest(5);
        var request2 = new FloorDestinationRequest(5);

        // Act & Assert
        Assert.Equal(request1, request2);
    }

    [Fact]
    public void Equality_ReturnsFalseForDifferentFloors()
    {
        // Arrange
        var request1 = new FloorDestinationRequest(5);
        var request2 = new FloorDestinationRequest(10);

        // Act & Assert
        Assert.NotEqual(request1, request2);
    }

    [Fact]
    public void GetHashCode_ReturnsSameValueForEqualObjects()
    {
        // Arrange
        var request1 = new FloorDestinationRequest(7);
        var request2 = new FloorDestinationRequest(7);

        // Act & Assert
        Assert.Equal(request1.GetHashCode(), request2.GetHashCode());
    }

    [Fact]
    public void ToString_ContainsFloorNumber()
    {
        // Arrange
        var request = new FloorDestinationRequest(15);

        // Act
        var result = request.ToString();

        // Assert
        Assert.Contains("15", result);
    }

    [Fact]
    public void Constructor_AcceptsNegativeFloor()
    {
        // Arrange & Act
        var request = new FloorDestinationRequest(-2);

        // Assert
        Assert.Equal(-2, request.Floor);
    }

    [Fact]
    public void Constructor_AcceptsZeroFloor()
    {
        // Arrange & Act
        var request = new FloorDestinationRequest(0);

        // Assert
        Assert.Equal(0, request.Floor);
    }
}
