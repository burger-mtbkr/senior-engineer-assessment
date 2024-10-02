using System.Text.Json.Serialization;

namespace Event.Processor.Models
{
    public class EventData
    {
        [JsonConverter(typeof(JsonStringGuidConverter))]
        public Guid Id { get; set; }
        public string EventType { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public string Payload { get; set; } = string.Empty;
    }
}
