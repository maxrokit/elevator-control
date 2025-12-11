using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ElevatorControl.Api.Domain.Enums;

/// <summary>
/// JSON converter that allows ElevatorDirection to be serialized/deserialized as numeric values (1 for Up, -1 for Down).
/// </summary>
public class ElevatorDirectionJsonConverter : JsonConverter<ElevatorDirection>
{
    public override ElevatorDirection Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Number)
        {
            var value = reader.GetInt32();
            return value == 1 ? ElevatorDirection.Up : ElevatorDirection.Down;
        }

        if (reader.TokenType == JsonTokenType.String)
        {
            var stringValue = reader.GetString();
            if (!string.IsNullOrEmpty(stringValue))
            {
                return Enum.Parse<ElevatorDirection>(stringValue, ignoreCase: true);
            }
        }

        throw new JsonException($"Unable to convert value to ElevatorDirection");
    }

    public override void Write(Utf8JsonWriter writer, ElevatorDirection value, JsonSerializerOptions options)
    {
        // Write as string for responses (Up/Down)
        writer.WriteStringValue(value.ToString());
    }
}
