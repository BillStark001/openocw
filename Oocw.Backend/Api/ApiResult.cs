


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Reflection;
using Oocw.Backend.Utils;

namespace Oocw.Backend.Api;


[JsonConverter(typeof(ApiResultJsonConverter))]
public sealed class ApiResult
{

    public const int CODE_SUCCESS = 0;
    public const int CODE_INTERNAL_ERROR = 1;

    public int Code { get; set; } = CODE_SUCCESS;
    public List<string>? Patch { get; set; }
    public object? Data { get; set; } = null!;
    public string? Message { get; set; }

    public ApiResult() {

    }

    public ApiResult((int, string?) codeDefinition) {
        (Code, Message) = codeDefinition;
    }
}

public class ApiResultJsonConverter : JsonConverter<ApiResult>
{

    public override ApiResult Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException("Deserialization is not implemented for this converter.");
    }

    string ConvertName(string name, JsonSerializerOptions options)
        => options.PropertyNamingPolicy?.ConvertName(name) ?? name;

    public override void Write(Utf8JsonWriter writer, ApiResult value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        var codeName = ConvertName(nameof(ApiResult.Code), options);
        var patchName = ConvertName(nameof(ApiResult.Patch), options);
        var msgName = ConvertName(nameof(ApiResult.Message), options);

        writer.WriteNumber(
            codeName,
            value.Code
        );

        if (value.Patch != null)
        {
            writer.WriteStartArray(patchName);
            foreach (var item in value.Patch)
            {
                writer.WriteStringValue(item);
            }
            writer.WriteEndArray();
        }

        if (value.Message != null)
        {
            writer.WriteString(msgName, value.Message);
        }

        // serialize data's properties
        if (value.Data != null)
        {
            // TODO handle situations if data is primitive, date time or enumerable
            using JsonDocument doc = JsonSerializer.SerializeToDocument(value.Data, options);
            foreach (JsonProperty property in doc.RootElement.EnumerateObject())
            {
                if (property.Name == codeName 
                    || (property.Name == patchName && value.Patch != null) 
                    || (property.Name == msgName && value.Message != null)
                ) {
                    continue;
                }
                property.WriteTo(writer);
            }
        }

        writer.WriteEndObject();
    }
}

public class ApiResultJsonConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert == typeof(ApiResult);
    }

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        return new ApiResultJsonConverter();
    }
}