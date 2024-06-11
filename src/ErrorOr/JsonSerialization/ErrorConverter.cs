using System.Text.Json;
using System.Text.Json.Serialization;

namespace ErrorOr.JsonSerialization;

public sealed class ErrorConverter : JsonConverter<Error>
{
    public override Error Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException("Expected StartObject token");
        }

        var code = string.Empty;
        var description = string.Empty;
        ErrorType? type = default;
        Dictionary<string, object>? metadata = null;

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                if (string.IsNullOrWhiteSpace(code))
                {
                    throw new JsonException("Expected value for code");
                }

                if (string.IsNullOrWhiteSpace(description))
                {
                    throw new JsonException("Expected value for code");
                }

                if (type is null)
                {
                    throw new JsonException("Expected value for type");
                }

                return new Error(code!, description!, type.Value, metadata);
            }

            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                throw new JsonException("Expected PropertyName token");
            }

            var propertyName = reader.GetString();
            reader.Read();

            switch (propertyName)
            {
                case "code":
                    code = reader.GetString();
                    break;
                case "description":
                    description = reader.GetString();
                    break;
                case "type":
                    var typeValue = reader.GetInt32();
                    if (typeValue is < (int)ErrorType.Failure or > (int)ErrorType.GatewayTimeout)
                    {
                        throw new JsonException("The specified error type is invalid");
                    }

                    type = (ErrorType)typeValue;

                    break;
                case "metadata":
                    metadata = DeserializeMetadata(ref reader);
                    break;
                default:
                    throw new JsonException($"Unknown property: {propertyName}");
            }
        }

        throw new JsonException("Expected EndObject token");
    }

    public override void Write(Utf8JsonWriter writer, Error value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WriteString("code", value.Code);
        writer.WriteString("description", value.Description);
        writer.WriteNumber("type", (int)value.Type);

        if (value.Metadata.Count > 0)
        {
            writer.WritePropertyName("metadata");
            writer.WriteStartObject();

            foreach (var kvp in value.Metadata)
            {
                if (kvp.Value is string stringValue)
                {
                    writer.WriteString(kvp.Key, stringValue);
                }
                else if (kvp.Value is int intValue)
                {
                    writer.WriteNumber(kvp.Key, intValue);
                }
                else if (kvp.Value is bool boolValue)
                {
                    writer.WriteBoolean(kvp.Key, boolValue);
                }
            }

            writer.WriteEndObject();
        }

        writer.WriteEndObject();
    }

    private static Dictionary<string, object> DeserializeMetadata(ref Utf8JsonReader reader)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException("Expected StartObject token");
        }

        Dictionary<string, object> metadata = new();
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                return metadata;
            }

            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                throw new JsonException("Expected PropertyName token");
            }

            var propertyName = reader.GetString();
            if (propertyName is null)
            {
                throw new JsonException("Expected PropertyName token");
            }

            reader.Read();

            switch (reader.TokenType)
            {
                case JsonTokenType.String:
                    metadata.Add(propertyName, reader.GetString()!);
                    break;
                case JsonTokenType.Number:
                    metadata.Add(propertyName, reader.GetInt32());
                    break;
                case JsonTokenType.True:
                case JsonTokenType.False:
                    metadata.Add(propertyName, reader.GetBoolean());
                    break;
                default:
                    throw new JsonException($"Unknown token type: {reader.TokenType}");
            }
        }

        throw new JsonException("Expected EndObject token");
    }
}
