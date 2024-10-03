using Newtonsoft.Json;

namespace Event.Processor.Models
{
    public class EventData
    {
        public string Id { get; set; }
        public string EventType { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public string Payload { get; set; } = string.Empty;
    }
}
