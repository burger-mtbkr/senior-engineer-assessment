using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Event.Processor.Models;
using JsonFlatFileDataStore;
using Newtonsoft.Json;

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
                dataStorePath = Environment.GetEnvironmentVariable("DATA_STORE_PATH") ?? "/data/processed_events.json";
            }
            else
            {
                // Calculate the solution root path
                string solutionRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
                eventsDirectory = Path.Combine(solutionRoot, "Events");
                dataStorePath = Path.Combine(solutionRoot, "Data", "processed_events.json");
            }

            // Ensure the directories exist
            Directory.CreateDirectory(eventsDirectory);
            Directory.CreateDirectory(Path.GetDirectoryName(dataStorePath) ?? string.Empty);

            // Initialize the data store
            var dataStore = new DataStore(dataStorePath);
            var processedEventsCollection = dataStore.GetCollection<EventData>("processedEvents");

            // Display the full path for verification
            Console.WriteLine($"Monitoring directory: {eventsDirectory}");
            Console.WriteLine($"Data store path: {dataStorePath}");

            // Process existing events in the directory
            await ProcessExistingEvents(eventsDirectory, processedEventsCollection);

            // Set up a FileSystemWatcher to monitor the directory for new events
            using var watcher = new FileSystemWatcher(eventsDirectory, "*.json")
            {
                NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite,
                EnableRaisingEvents = true
            };

            watcher.Created += async (sender, e) => await OnNewEventAsync(e.FullPath, processedEventsCollection);

            // Keep the application running indefinitely
            await Task.Delay(Timeout.Infinite);
        }

        private static async Task ProcessExistingEvents(string directory, IDocumentCollection<EventData> processedEventsCollection)
        {
            var eventFiles = Directory.GetFiles(directory, "*.json");

            foreach (var filePath in eventFiles)
            {
                await OnNewEventAsync(filePath, processedEventsCollection);
            }
        }

        private static async Task OnNewEventAsync(string filePath, IDocumentCollection<EventData> processedEventsCollection)
        {
            try
            {
                // Read the event data
                string jsonData = await File.ReadAllTextAsync(filePath);
                var eventData = JsonConvert.DeserializeObject<EventData>(jsonData);

                if (eventData == null)
                {
                    Console.WriteLine($"Invalid event data in file: {filePath}");
                    return;
                }

                // Check if the event type is known
                if (eventData.EventType == "EventTypeA" || eventData.EventType == "EventTypeB")
                {
                    Console.WriteLine($"Received known event: {eventData.EventType}");

                    // Check if the event has already been processed
                    if (processedEventsCollection.AsQueryable().Any(e => e.Id == eventData.Id))
                    {
                        Console.WriteLine($"Event already processed: {eventData.Id}");
                        // Delete the file since it's a duplicate
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
                else
                {
                    // Unknown event type; do not process or delete
                    Console.WriteLine($"Unknown event type encountered: {eventData.EventType}. File will remain unprocessed.");
                    // Optionally, you can log or handle unknown event types differently
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing event file '{filePath}': {ex.Message}");
            }
        }

        private static Task ProcessEventAsync(EventData eventData)
        {
            // Implement processing logic for known event types
            return eventData.EventType switch
            {
                "EventTypeA" => HandleEventTypeA(eventData),
                "EventTypeB" => HandleEventTypeB(eventData),
                _ => Task.CompletedTask
            };
        }

        private static Task HandleEventTypeA(EventData eventData)
        {
            Console.WriteLine("Processing EventTypeA");
            // Add your custom processing logic here
            return Task.CompletedTask;
        }

        private static Task HandleEventTypeB(EventData eventData)
        {
            Console.WriteLine("Processing EventTypeB");
            // Add your custom processing logic here
            return Task.CompletedTask;
        }
    }
}

