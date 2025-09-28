## Project Overview

TTC.Polygon is a .NET client library for the Polygon.io REST API, providing strongly-typed access to stock market data. The solution consists of two main projects and multiple test projects:

**Main Projects:**
- **TTC.Polygon.Models** - Shared models and DTOs for API responses
- **TTC.Polygon.RestClient** - Main client library with HTTP services and dependency injection

**Test Projects:**
- **TTC.Polygon.Models.Tests** - XUnit tests for the models library
- **TTC.Polygon.RestClient.Tests** - XUnit tests for the REST client library
- **TTC.Polygon.IntegrationTests** - Integration tests for the complete system
- **TTC.Polygon.TestApp** - Console application for testing and development

## Build and Test Commands

```bash
# Build the entire solution
dotnet build

# Build specific projects
dotnet build src/TTC.Polygon.Models/TTC.Polygon.Models.csproj
dotnet build src/TTC.Polygon.RestClient/TTC.Polygon.RestClient.csproj

# Run tests
dotnet test

# Run specific test projects
dotnet test tests/TTC.Polygon.Models.Tests/TTC.Polygon.Models.Tests.csproj
dotnet test tests/TTC.Polygon.RestClient.Tests/TTC.Polygon.RestClient.Tests.csproj
dotnet test tests/TTC.Polygon.IntegrationTests/TTC.Polygon.IntegrationTests.csproj

# Run test app
dotnet run --project tests/TTC.Polygon.TestApp/TTC.Polygon.TestApp.csproj

# Create NuGet packages (automatically generated on build)
dotnet pack
```

## Architecture Overview

### Client Architecture
The client uses a layered architecture:

1. **API Layer** (`/Api/`) - Refit interfaces defining HTTP endpoints
   - `IPolygonStocksApi` - Stock trades, quotes, snapshots, and OHLC aggregates
   - `IPolygonReferenceApi` - Ticker reference data and market status

2. **Service Layer** (`/Services/`) - Business logic and orchestration
   - `IPolygonClient` - Main facade providing access to all services
   - `IStocksService`, `IReferenceDataService` - Domain-specific services

3. **Configuration** (`/Configuration/`) - Options pattern for settings
   - `PolygonOptions` - API key, base URL, timeout, retry settings

4. **Authentication** (`/Authentication/`) - HTTP message handlers
   - `PolygonAuthenticationHandler` - Adds API key to requests

### Dependency Injection Setup
Registration is handled through `ServiceCollectionExtensions.AddPolygonClient()` with multiple overloads:
- Configuration-based (appsettings.json)
- Direct API key
- Action-based configuration

### Key Dependencies
- **Refit** - HTTP client generation from interfaces
- **Microsoft.Extensions.*** - Configuration, DI, HTTP client factory, logging
- **NodaTime** - Date/time handling (Models project)
- **XUnit** - Testing framework with Moq for mocking
- **Serilog** - Structured logging (TestApp project)

### Package Management
- Uses **Central Package Management** via `Directory.Packages.props`
- All package versions are centrally managed at the solution level
- Projects reference packages without version numbers

## Development Patterns

### Models Project
- Contains strongly-typed models for all API responses
- Uses `PolygonResponse<T>` as generic wrapper
- Targets .NET 8 and .NET 9

### Testing Structure
- **TTC.Polygon.Models.Tests** - Unit tests for models using XUnit
- **TTC.Polygon.RestClient.Tests** - Unit tests for REST client services
- **TTC.Polygon.IntegrationTests** - Integration tests for complete API workflows
- **TTC.Polygon.TestApp** - Console application for manual testing and development
- Uses `coverlet.collector` for code coverage across all test projects
- Moq framework available for mocking dependencies

### Package Generation
- Both RestClient and Models projects generate NuGet packages on build
- Current version 0.0.1 (development phase)
- MPL 2.0 license with comprehensive package metadata
- Documentation XML generation enabled for both projects

## Coding Style
- All classes and interfaces **MUST** have comprehensive vsdoc comments