using System.Text.Json.Serialization;

namespace Event.Generator.Models;

public class EventData
{
    [JsonConverter(typeof(JsonStringGuidConverter))]
    public Guid Id { get; set; } = Guid.NewGuid();
    public string EventType { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string Payload { get; set; } = string.Empty;
}
