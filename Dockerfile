# Stage 1: Build the application
# Use the official .NET SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the project file and restore dependencies
# This is done separately to leverage Docker layer caching
COPY LegacyOrderService.csproj .
RUN dotnet restore

# Copy the rest of the source code
COPY . .

# Build the application in Release mode
RUN dotnet build -c Release -o /app/build

# Stage 2: Publish the application
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

# Stage 3: Create the final runtime image
# Use the smaller runtime image (no SDK needed)
FROM mcr.microsoft.com/dotnet/runtime:8.0 AS final
WORKDIR /app

# Copy the published app from the publish stage
COPY --from=publish /app/publish .

# Copy appsettings.json
COPY appsettings.json .

# Run the application
ENTRYPOINT ["dotnet", "LegacyOrderService.dll"]
