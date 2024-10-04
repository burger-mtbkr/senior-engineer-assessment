namespace Event.Generator.Models;

public class EventData
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string EventType { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string Payload { get; set; } = string.Empty;
}
