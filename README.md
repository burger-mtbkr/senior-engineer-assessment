# Instructions for Building the Event Processing Application

## Overview

You are tasked with building a simple event processing system consisting of two console applications:

1. **Event Generator**: Generates events and writes them to an `Events` folder. **This is already provided for you.**
2. **Event Processor**: Monitors the `Events` folder, processes known events, and stores processed events in a data store.

We have provided you with the Event Generator so you can familiar yourself with that application then you are required to build the processor application.  

#### Event Generator overview:

- **Functionality**:
  - use `dotnet run` to start the application.
  - It will generates a total of **20 events**. in a `Events` folder at the root of the solution.
  - Each event will  have the following properties:
    - `Id`: A unique identifier (string representation of a GUID).
    - `EventType`: One of the following values:
      - `"EventTypeA"`
      - `"EventTypeB"`
      - `"UnknownEventType"`
    - `Timestamp`: The UTC time when the event was generated.
    - `Payload`: A string containing a sample payload message.
  - Each event is written into a separate JSON file in the `Events` folder.
  - After generating 20 events, the application will exit.

- **Implementation Details**:
  - Uses `System.Text.Json` for serialization and deserialization.
  - The `Events` folder is located at the solution root.
  - Each event file uses the event's `Id` property as the file name (e.g., `eventId.json`).


## Event Processor Requirements

You are required to build the processor console application which monitors that `Events` folder for new events.  We will require the following functionality:

  - Monitor the `Events` folder for event files.
  - Upon startup, process all existing event files in the folder.
  - Process only events with `EventType` of `"EventTypeA"` or `"EventTypeB"`.
    - For these events:
      - Check if the event has already been processed by looking it up in a data store.
      - If not processed:
        - Process the event (placeholder logic is acceptable).
        - Store the processed event in a data store under a collection named `"processedEvents"`.
        - Delete the event file from the `Events` folder after processing.
  - Ignore events with any other `EventType` (e.g., `"UnknownEventType"`).
    - These event files should remain in the `Events` folder unprocessed.
  - Continue to monitor the `Events` folder for any new event files added after startup.

- **Implementation Details**:

  - Use `System.Text.Json` for serialization and deserialization.
  - The data store should be a JSON file (`processed_events.json`) located in a `Data` folder at the solution root.
    - Use `JsonFlatFileDataStore` or a similar nosql library for data storage.
  - Ensure that processed events are not reprocessed if the application restarts or an event id is reused.
  - Handle any exceptions gracefully and log appropriate error messages.
  - Do not process or delete event files that are of unknown event types.

### 3. Shared Requirements

- Both applications should use the same `Events` folder located at the solution root.
- Ensure proper synchronization to avoid file access issues.
  - When reading or writing to event files, handle possible exceptions due to file locks.
- All code should be written in C# targeting .NET 8.0 or newer

## Project Structure

Your solution should have the following structure:


```
SENIOR_ASSESSMENT/
├── Event.Generator/
│   ├── Program.cs
│   ├── Models/
│   │   └── EventData.cs
│   ├── Event.Generator.csproj
├── Event.Processor/
│   ├── Program.cs
│   ├── Models/
│   │   └── EventData.cs
│   ├── Event.Processor.csproj
├── Events/     # Shared Events folder
├── Data/       # Data folder for the Event Processor
└── SENIOR_ASSESSMENT.sln

```
## Testing

- Given testing is of critical importance to us, consider how you will test these processors, what test coverage you want to add.  Hint:  We want ot see some tests!

## Deliverables

- The complete solution with both projects implemented as per the requirements.
- **Tests**.
- Ensure that all source code is included and properly organized within the solution.
- Provide any necessary instructions to run and test the applications.
- Include any **Assumptions** you have made and any **improvements** that you would make in the future.
- Submit your application as a **GitHub repository**. Ideally with visible history in place.

## Bonus Deliverables:

- Add docker support.
- Use docker-compose to run the projects.

---

If you have any questions or need clarification on any of the requirements, please feel free to ask.
