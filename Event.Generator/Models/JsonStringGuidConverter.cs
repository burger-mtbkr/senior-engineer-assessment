using System.Text.Json;
using System.Text.Json.Serialization;

namespace Event.Generator.Models;
public class JsonStringGuidConverter: JsonConverter<Guid>
{
    public override Guid Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var stringValue = reader.GetString();
        if (stringValue != null && Guid.TryParse(stringValue, out var guidValue))
        {
            return guidValue;
        }
        return Guid.Empty;
    }

    public override void Write(Utf8JsonWriter writer, Guid value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}