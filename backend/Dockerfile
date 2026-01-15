# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy csproj files and restore dependencies
COPY src/ECommerce.Domain/*.csproj ./src/ECommerce.Domain/
COPY src/ECommerce.Application/*.csproj ./src/ECommerce.Application/
COPY src/ECommerce.Infrastructure/*.csproj ./src/ECommerce.Infrastructure/
COPY src/ECommerce.API/*.csproj ./src/ECommerce.API/

WORKDIR /app/src/ECommerce.API
RUN dotnet restore

# Copy everything else and build
WORKDIR /app
COPY src/ ./src/
WORKDIR /app/src/ECommerce.API
RUN dotnet publish -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

# Expose port
EXPOSE 8080

# Set environment variable for ASP.NET to listen on port 8080
ENV ASPNETCORE_URLS=http://+:8080

# Run the application
ENTRYPOINT ["dotnet", "ECommerce.API.dll"]
