using System;
using System.IO;
using System.Threading.Tasks;
using Event.Generator.Models;
using Newtonsoft.Json;

namespace Event.Generator
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Event Generator Started...");

            // Determine if running in Docker
            bool isDocker = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";

            string eventsDirectory;

            if (isDocker)
            {
                eventsDirectory = Environment.GetEnvironmentVariable("EVENTS_DIRECTORY") ?? "/events";
            }
            else
            {
                // Calculate the solution root path
                string solutionRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
                eventsDirectory = Path.Combine(solutionRoot, "Events");
            }

            // Ensure the directory exists
            Directory.CreateDirectory(eventsDirectory);

            // Display the full path for verification
            Console.WriteLine($"Writing events to directory: {eventsDirectory}");

            // Generate 20 events and then stop
            for (int i = 0; i < 20; i++)
            {
                // Generate a random event
                var eventData = GenerateRandomEvent();

                // Write the event to a file
                await WriteEventToFileAsync(eventData, eventsDirectory);

                // Wait for a short interval
                await Task.Delay(500); // Adjust delay as needed
            }

            Console.WriteLine("Event Generator has finished generating events.");
        }

        private static EventData GenerateRandomEvent()
        {
            var eventTypes = new[] { "EventTypeA", "EventTypeB", "UnknownEventType" };
            var random = new Random();
            var eventType = eventTypes[random.Next(eventTypes.Length)];

            return new EventData
            {
                Id = Guid.NewGuid().ToString(),
                EventType = eventType,
                Timestamp = DateTime.UtcNow,
                Payload = $"Sample payload for {eventType}"
            };
        }

        private static async Task WriteEventToFileAsync(EventData eventData, string directory)
        {
            string fileName = Path.Combine(directory, $"{eventData.Id}.json");
            string jsonData = JsonConvert.SerializeObject(eventData);

            try
            {
                await File.WriteAllTextAsync(fileName, jsonData);
                Console.WriteLine($"Generated event: {eventData.EventType} at {eventData.Timestamp}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error writing event to file: {ex.Message}");
            }
        }
    }
}
