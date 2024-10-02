## senior-engineer-assessment

### Requirements:

    **Event Generator Console Application:**
        Simulate the generation of various events, both recognized and unrecognized.
        Send events to the Event Processor application.

   ** Event Processor Application:**
        Receive events from the Event Generator.
        Process recognized events and perform appropriate actions.
        Handle unrecognized events gracefully (implementation is up to you).
        Log each processed event to a console feed for monitoring.
        Keep track of processed events using the JsonFlatFileDataStore package.
        Upon restart, the application should resume processing from the last unprocessed event without duplicating work.

   ** Dockerization:**
        Both applications should be containerized using Docker.
        Use Docker Compose to define and run multi-container Docker applications.
