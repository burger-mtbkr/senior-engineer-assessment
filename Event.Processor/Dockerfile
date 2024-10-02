# Use the .NET 8 SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the csproj and restore dependencies
COPY ["Event.Processor.csproj", "./"]
RUN dotnet restore "Event.Processor.csproj"

# Copy the rest of the application and build
COPY . .
RUN dotnet publish "Event.Processor.csproj" -c Release -o /app/publish

# Use the .NET runtime image
FROM mcr.microsoft.com/dotnet/runtime:8.0
WORKDIR /app

# Copy the published application
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "Event.Processor.dll"]
