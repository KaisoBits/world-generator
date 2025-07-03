# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /src

# Copy project file and restore dependencies
COPY WorldGenerator/WorldGenerator.csproj WorldGenerator/
RUN dotnet restore WorldGenerator/WorldGenerator.csproj

# Copy source code and build
COPY . .
WORKDIR /src/WorldGenerator
RUN dotnet build WorldGenerator.csproj -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish WorldGenerator.csproj -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/runtime:8.0-alpine AS runtime

# Install curl for health checks
RUN apk update && apk add --no-cache \
    curl

# Set working directory
WORKDIR /app

# Copy the published application
COPY --from=publish /app/publish .

# Create a simple health check endpoint script
RUN echo '#!/bin/sh' > /app/healthcheck.sh && \
    echo 'curl -f http://localhost:1337/_health || exit 1' >> /app/healthcheck.sh && \
    chmod +x /app/healthcheck.sh

# Health check configuration using the ECS health check command
HEALTHCHECK --interval=30s --timeout=10s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:1337/_health || exit 1

# Set the entry point
ENTRYPOINT ["dotnet", "WorldGenerator.dll"]