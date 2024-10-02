using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using JsonFlatFileDataStore;
using Event.Processor.Models;

namespace Event.Processor
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Event Processor Started...");

            // Determine if running in Docker
            bool isDocker = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";

            string eventsDirectory;
            string dataStorePath;

            if (isDocker)
            {
                eventsDirectory = Environment.GetEnvironmentVariable("EVENTS_DIRECTORY") ?? "/events";
                dataStorePath = Environment.GetEnvironmentVariable("DATA_STORE_PATH") ?? "/data/data.json";
            }
            else
            {
                // Calculate the solution root path
                string solutionRoot = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..");
                eventsDirectory = Path.Combine(solutionRoot, "Events");
                dataStorePath = Path.Combine(solutionRoot, "Data", "data.json");
            }

            // Ensure the directories exist
            Directory.CreateDirectory(eventsDirectory);
            Directory.CreateDirectory(Path.GetDirectoryName(dataStorePath) ?? string.Empty);

            // Display the full path for verification
            Console.WriteLine($"Monitoring directory: {Path.GetFullPath(eventsDirectory)}");
            Console.WriteLine($"Data store path: {Path.GetFullPath(dataStorePath)}");

            // Initialize the data store
            var dataStore = new DataStore(dataStorePath);
            var processedEventsCollection = dataStore.GetCollection<EventData>("processedEvents");

            // Set up a FileSystemWatcher to monitor the directory
            using var watcher = new FileSystemWatcher(eventsDirectory, "*.json")
            {
                NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite,
                EnableRaisingEvents = true
            };

            watcher.Created += async (sender, e) => await OnNewEventAsync(e.FullPath, processedEventsCollection);

            // Keep the application running indefinitely
            await Task.Delay(Timeout.Infinite);
        }

        private static async Task OnNewEventAsync(string filePath, IDocumentCollection<EventData> processedEventsCollection)
        {
            try
            {
                // Read the event data
                string jsonData = await File.ReadAllTextAsync(filePath);

                var options = new JsonSerializerOptions
                {
                    Converters = { new JsonStringGuidConverter() }
                };
                var eventData = JsonSerializer.Deserialize<EventData>(jsonData, options);

                if (eventData == null)
                {
                    Console.WriteLine($"Invalid event data in file: {filePath}");
                    return;
                }

                Console.WriteLine($"Received event: {eventData.EventType}");

                // Check if the event has already been processed
                if (processedEventsCollection.AsQueryable().Any(e => e.Id == eventData.Id))
                {
                    Console.WriteLine($"Event already processed: {eventData.Id}");
                    // Optionally delete the file
                    File.Delete(filePath);
                    return;
                }

                // Process the event
                await ProcessEventAsync(eventData);

                // Store the event in the data store
                processedEventsCollection.InsertOne(eventData);

                // Delete the event file after processing
                File.Delete(filePath);

                // Log to console
                Console.WriteLine($"Processed event: {eventData.EventType} at {eventData.Timestamp}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing event file '{filePath}': {ex.Message}");
            }
        }

        private static Task ProcessEventAsync(EventData eventData)
        {
            return eventData.EventType switch
            {
                "EventTypeA" => HandleEventTypeA(eventData),
                "EventTypeB" => HandleEventTypeB(eventData),
                _ => HandleUnrecognizedEvent(eventData)
            };
        }

        private static Task HandleEventTypeA(EventData eventData)
        {
            // Placeholder processing logic for EventTypeA
            Console.WriteLine("Processing EventTypeA");
            return Task.CompletedTask;
        }

        private static Task HandleEventTypeB(EventData eventData)
        {
            // Placeholder processing logic for EventTypeB
            Console.WriteLine("Processing EventTypeB");
            return Task.CompletedTask;
        }

        private static Task HandleUnrecognizedEvent(EventData eventData)
        {
            Console.WriteLine($"Unrecognized event type: {eventData.EventType}");
            return Task.CompletedTask;
        }
    }
}
