# Dockerfile para AdmBeachApp - .NET MAUI
# Este Dockerfile é otimizado para desenvolvimento e CI/CD

# Stage 1: Build environment
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build-env
WORKDIR /app

# Install additional tools for MAUI development
RUN apt-get update && apt-get install -y \
    curl \
    git \
    unzip \
    && rm -rf /var/lib/apt/lists/*

# Copy csproj and restore dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY . ./

# Build the application (focusing on the core libraries that can be containerized)
# Note: MAUI apps typically need platform-specific builds, but this builds the shared libraries
RUN dotnet build --configuration Release --no-restore

# Stage 2: Runtime environment for development services
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Install development tools
RUN apt-get update && apt-get install -y \
    curl \
    wget \
    vim \
    git \
    && rm -rf /var/lib/apt/lists/*

# Copy built application
COPY --from=build-env /app/bin/Release/ ./

# Expose ports for development services
EXPOSE 80
EXPOSE 443
EXPOSE 5000
EXPOSE 5001

# Default command for development
CMD ["sleep", "infinity"]

# Stage 3: Development environment with full SDK
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS development
WORKDIR /app

# Install development dependencies
RUN apt-get update && apt-get install -y \
    curl \
    wget \
    vim \
    git \
    unzip \
    build-essential \
    && rm -rf /var/lib/apt/lists/*

# Install .NET MAUI workload
RUN dotnet workload install maui

# Copy source code
COPY . ./

# Restore packages
RUN dotnet restore

# Keep container running for development
CMD ["sleep", "infinity"]
