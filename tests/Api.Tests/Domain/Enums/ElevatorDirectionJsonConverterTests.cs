using System.Text.Json;
using ElevatorControl.Api.Domain.Enums;
using Xunit;

namespace ElevatorControl.Api.Tests.Domain.Enums;

public class ElevatorDirectionJsonConverterTests
{
    private readonly JsonSerializerOptions _options;

    public ElevatorDirectionJsonConverterTests()
    {
        _options = new JsonSerializerOptions
        {
            Converters = { new ElevatorDirectionJsonConverter() }
        };
    }

    [Fact]
    public void Write_SerializesUpAs1()
    {
        // Arrange
        var direction = ElevatorDirection.Up;

        // Act
        var json = JsonSerializer.Serialize(direction, _options);

        // Assert
        Assert.Equal("1", json);
    }

    [Fact]
    public void Write_SerializesDownAsNegative1()
    {
        // Arrange
        var direction = ElevatorDirection.Down;

        // Act
        var json = JsonSerializer.Serialize(direction, _options);

        // Assert
        Assert.Equal("-1", json);
    }

    [Fact]
    public void Read_Deserializes1AsUp()
    {
        // Arrange
        var json = "1";

        // Act
        var direction = JsonSerializer.Deserialize<ElevatorDirection>(json, _options);

        // Assert
        Assert.Equal(ElevatorDirection.Up, direction);
    }

    [Fact]
    public void Read_DeserializesNegative1AsDown()
    {
        // Arrange
        var json = "-1";

        // Act
        var direction = JsonSerializer.Deserialize<ElevatorDirection>(json, _options);

        // Assert
        Assert.Equal(ElevatorDirection.Down, direction);
    }

    [Fact]
    public void Read_DeserializesAnyNonOneValueAsDown()
    {
        // Arrange
        var json = "0";

        // Act
        var direction = JsonSerializer.Deserialize<ElevatorDirection>(json, _options);

        // Assert
        Assert.Equal(ElevatorDirection.Down, direction);
    }

    [Fact]
    public void Read_Deserializes5AsDown()
    {
        // Arrange
        var json = "5";

        // Act
        var direction = JsonSerializer.Deserialize<ElevatorDirection>(json, _options);

        // Assert
        Assert.Equal(ElevatorDirection.Down, direction);
    }

    [Fact]
    public void Read_ThrowsJsonException_WhenTokenIsNotNumber()
    {
        // Arrange
        var json = "\"Up\"";

        // Act & Assert
        var ex = Assert.Throws<JsonException>(() => 
            JsonSerializer.Deserialize<ElevatorDirection>(json, _options));
        Assert.Contains("Unable to convert value to ElevatorDirection", ex.Message);
    }

    [Fact]
    public void Read_ThrowsJsonException_WhenTokenIsNull()
    {
        // Arrange
        var json = "null";

        // Act & Assert
        Assert.Throws<JsonException>(() => 
            JsonSerializer.Deserialize<ElevatorDirection>(json, _options));
    }

    [Fact]
    public void RoundTrip_PreservesUpDirection()
    {
        // Arrange
        var original = ElevatorDirection.Up;

        // Act
        var json = JsonSerializer.Serialize(original, _options);
        var deserialized = JsonSerializer.Deserialize<ElevatorDirection>(json, _options);

        // Assert
        Assert.Equal(original, deserialized);
    }

    [Fact]
    public void RoundTrip_PreservesDownDirection()
    {
        // Arrange
        var original = ElevatorDirection.Down;

        // Act
        var json = JsonSerializer.Serialize(original, _options);
        var deserialized = JsonSerializer.Deserialize<ElevatorDirection>(json, _options);

        // Assert
        Assert.Equal(original, deserialized);
    }

    [Fact]
    public void Write_WorksInComplexObject()
    {
        // Arrange
        var obj = new { Direction = ElevatorDirection.Up, Floor = 5 };

        // Act
        var json = JsonSerializer.Serialize(obj, _options);

        // Assert
        Assert.Contains("\"Direction\":1", json);
    }

    [Fact]
    public void Read_WorksInComplexObject()
    {
        // Arrange
        var json = "{\"Direction\":1,\"Floor\":5}";

        // Act
        var result = JsonSerializer.Deserialize<TestObject>(json, _options);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(ElevatorDirection.Up, result.Direction);
        Assert.Equal(5, result.Floor);
    }

    private class TestObject
    {
        public ElevatorDirection Direction { get; set; }
        public int Floor { get; set; }
    }
}
