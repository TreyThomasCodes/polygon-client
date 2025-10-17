## Project Overview

TreyThomasCodes.Polygon is a .NET client library for the Polygon.io REST API, providing strongly-typed access to stock market data. The solution consists of two main projects and multiple test projects:

**Main Projects:**
- **TreyThomasCodes.Polygon.Models** - Shared models and DTOs for API responses
- **TreyThomasCodes.Polygon.RestClient** - Main client library with HTTP services and dependency injection

**Test Projects:**
- **TreyThomasCodes.Polygon.Models.Tests** - XUnit tests for the models library
- **TreyThomasCodes.Polygon.RestClient.Tests** - XUnit tests for the REST client library
- **TreyThomasCodes.Polygon.IntegrationTests** - Integration tests for the complete system
- **TreyThomasCodes.Polygon.TestApp** - Console application for testing and development

## Build and Test Commands

```bash
# Build the entire solution
dotnet build

# Build specific projects
dotnet build src/TreyThomasCodes.Polygon.Models/TreyThomasCodes.Polygon.Models.csproj
dotnet build src/TreyThomasCodes.Polygon.RestClient/TreyThomasCodes.Polygon.RestClient.csproj

# Run tests
dotnet test

# Run specific test projects
dotnet test tests/TreyThomasCodes.Polygon.Models.Tests/TreyThomasCodes.Polygon.Models.Tests.csproj
dotnet test tests/TreyThomasCodes.Polygon.RestClient.Tests/TreyThomasCodes.Polygon.RestClient.Tests.csproj
dotnet test tests/TreyThomasCodes.Polygon.IntegrationTests/TreyThomasCodes.Polygon.IntegrationTests.csproj

# Run test app
dotnet run --project tests/TreyThomasCodes.Polygon.TestApp/TreyThomasCodes.Polygon.TestApp.csproj

# Create NuGet packages (automatically generated on build)
dotnet pack
```

## Architecture Overview

### Client Architecture
The client uses a layered architecture:

1. **API Layer** (`/Api/`) - Refit interfaces defining HTTP endpoints
   - `IPolygonStocksApi` - Stock trades, quotes, snapshots, and OHLC aggregates
   - `IPolygonReferenceApi` - Ticker reference data and market status
   - `IPolygonOptionsApi` - Options contract data (infrastructure ready for endpoint implementation)

2. **Service Layer** (`/Services/`) - Business logic and orchestration
   - `IPolygonClient` - Main facade providing access to all services
   - `IStocksService`, `IReferenceDataService`, `IOptionsService` - Domain-specific services

3. **Extensions Layer** (`/Extensions/`) - Extension methods for enhanced usability
   - `OptionsServiceExtensions` - Extension methods for `IOptionsService` providing:
     - Component-based methods (e.g., `GetContractByComponentsAsync`, `GetSnapshotByComponentsAsync`, `GetLastTradeByComponentsAsync`, `GetBarsByComponentsAsync`)
     - OptionsTicker-based overloads for all major Options API calls
     - Discovery helpers (`GetAvailableStrikesAsync`, `GetExpirationDatesAsync`)

4. **Configuration** (`/Configuration/`) - Options pattern for settings
   - `PolygonOptions` - API key, base URL, timeout, retry settings

5. **Authentication** (`/Authentication/`) - HTTP message handlers
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
- **Options Ticker Helpers** (`/Options/`)
  - `OptionType` - Enum for Call and Put option types
  - `OptionsTicker` - Class for creating, parsing, and validating OCC format options tickers
    - Static methods: `Create()`, `Parse()`, `TryParse()`
    - Instance methods: `ToString()`, `Equals()`, `GetHashCode()`
    - Properties: `Underlying`, `ExpirationDate`, `Type`, `Strike`
  - `OptionsTickerBuilder` - Fluent builder for constructing options tickers
    - Methods: `WithUnderlying()`, `WithExpiration()`, `AsCall()`, `AsPut()`, `WithType()`, `WithStrike()`, `Build()`, `BuildTicker()`, `Reset()`

### Testing Structure
- **TreyThomasCodes.Polygon.Models.Tests** - Unit tests for models using XUnit
  - `/Json/` - JSON converter tests
  - `/Options/` - Options ticker and builder tests
  - `/Reference/` - Reference data model tests
  - `/Stocks/` - Stock data model tests
- **TreyThomasCodes.Polygon.RestClient.Tests** - Unit tests for REST client services, organized by service and method
  - `/Authentication/` - Authentication handler tests
  - `/Services/Options/` - Options service tests split by method (GetBars, GetChainSnapshot, GetContractDetails, GetDailyOpenClose, GetLastTrade, GetPreviousDayBar, GetQuotes, GetSnapshot, GetTrades)
  - `/Services/ReferenceData/` - Reference data service tests split by method (GetConditionCodes, GetExchanges, GetMarketStatus, GetTickerTypes)
  - `/Services/Stocks/` - Stocks service tests split by method (GetLastQuote, GetLastTrade, GetSnapshot)
- **TreyThomasCodes.Polygon.IntegrationTests** - Integration tests for complete API workflows
- **TreyThomasCodes.Polygon.TestApp** - Console application for manual testing and development
- Uses `coverlet.collector` for code coverage across all test projects
- Moq framework available for mocking dependencies
- Test files follow the naming convention: `{ServiceName}_{MethodName}Tests.cs` for unit tests

### Package Generation
- Both RestClient and Models projects generate NuGet packages on build
- Current version 0.0.1 (development phase)
- MPL 2.0 license with comprehensive package metadata
- Documentation XML generation enabled for both projects

## Coding Style
- All classes and interfaces **MUST** have comprehensive vsdoc comments