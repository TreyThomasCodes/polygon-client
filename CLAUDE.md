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

2. **Request Layer** (`/Requests/`) - Strongly-typed request objects for API calls
   - `/Stocks/` - Request objects for stock market data (e.g., `GetBarsRequest`, `GetTradesRequest`, `GetQuotesRequest`)
   - `/Options/` - Request objects for options data (e.g., `GetContractDetailsRequest`, `GetBarsRequest`, `GetSnapshotRequest`)
   - `/Reference/` - Request objects for reference data (e.g., `GetTickersRequest`, `GetMarketStatusRequest`)
   - Each request object encapsulates all parameters for a specific API call

3. **Validation Layer** (`/Validators/`) - FluentValidation validators for request objects
   - `/Stocks/` - Validators for stock requests (e.g., `GetBarsRequestValidator`, `GetTradesRequestValidator`)
   - `/Options/` - Validators for options requests (e.g., `GetContractDetailsRequestValidator`, `GetBarsRequestValidator`)
   - `/Reference/` - Validators for reference data requests (e.g., `GetTickersRequestValidator`, `GetMarketStatusRequestValidator`)
   - Validators enforce parameter requirements, formats, and constraints before API calls
   - Automatically integrated into service methods via dependency injection

4. **Service Layer** (`/Services/`) - Business logic and orchestration
   - `IPolygonClient` - Main facade providing access to all services
   - `IStocksService`, `IReferenceDataService`, `IOptionsService` - Domain-specific services
   - All service methods accept request objects and automatically validate them using FluentValidation

5. **Extensions Layer** (`/Extensions/`) - Extension methods for enhanced usability
   - `OptionsServiceExtensions` - Extension methods for `IOptionsService` providing:
     - Component-based methods (e.g., `GetContractByComponentsAsync`, `GetSnapshotByComponentsAsync`, `GetLastTradeByComponentsAsync`, `GetBarsByComponentsAsync`)
     - OptionsTicker-based overloads for all major Options API calls
     - Discovery helpers (`GetAvailableStrikesAsync`, `GetExpirationDatesAsync`)

6. **Configuration** (`/Configuration/`) - Options pattern for settings
   - `PolygonOptions` - API key, base URL, timeout, retry settings

7. **Authentication** (`/Authentication/`) - HTTP message handlers
   - `PolygonAuthenticationHandler` - Adds API key to requests

### Dependency Injection Setup
Registration is handled through `ServiceCollectionExtensions.AddPolygonClient()` with multiple overloads:
- Configuration-based (appsettings.json)
- Direct API key
- Action-based configuration

### Key Dependencies
- **Refit** - HTTP client generation from interfaces
- **FluentValidation** - Request object validation with automatic error handling
- **Microsoft.Extensions.*** - Configuration, DI, HTTP client factory, logging
- **NodaTime** - Date/time handling (Models project)
- **XUnit** - Testing framework with Moq for mocking
- **Serilog** - Structured logging (TestApp project)

### Package Management
- Uses **Central Package Management** via `Directory.Packages.props`
- All package versions are centrally managed at the solution level
- Projects reference packages without version numbers

## Development Patterns

### Request Objects and Validation
The library uses a request object pattern with FluentValidation for all API calls:

**Request Objects** (`/Requests/`) - Strongly-typed classes encapsulating API parameters:
- Organized by service area: `/Stocks/`, `/Options/`, `/Reference/`
- Each request class contains all parameters for a specific API endpoint
- Example: `GetBarsRequest` contains `Ticker`, `Multiplier`, `Timespan`, `From`, `To`, `Adjusted`, `Sort`, `Limit`

**Validators** (`/Validators/`) - FluentValidation validators for each request type:
- Organized by service area matching request structure
- Enforce parameter requirements, formats, and constraints
- Example: `GetBarsRequestValidator` validates date formats, ticker length, multiplier range, etc.
- Validators are automatically registered via dependency injection
- Service methods call `ValidateAndThrowAsync` before making API requests

**Usage Pattern**:
```csharp
// Create a request object
var request = new GetBarsRequest
{
    Ticker = "AAPL",
    Multiplier = 1,
    Timespan = AggregateInterval.Day,
    From = "2025-09-01",
    To = "2025-09-30",
    Adjusted = true
};

// Pass to service method (validation happens automatically)
var bars = await _client.Stocks.GetBarsAsync(request);
```

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
- **TreyThomasCodes.Polygon.IntegrationTests** - Integration tests for complete API workflows, organized by service area
  - `IntegrationTestBase.cs` - Base class providing DI setup, Polygon client configuration, and resource disposal for all integration tests
  - `/Options/` - Integration tests for Options API methods (GetBars, GetChainSnapshot, GetContractDetails, GetDailyOpenClose, GetLastTrade, GetPreviousDayBar, GetQuotes, GetSnapshot, GetTrades)
  - `/OptionsExtensions/` - Integration tests for Options extension methods (ComponentBasedExtensions, OptionsTickerExtensions, DiscoveryHelpers)
  - `/Stocks/` - Integration tests for Stocks API methods (GetBars, GetLastQuote, GetLastTrade, GetSnapshot)
  - `/ReferenceData/` - Integration tests for Reference Data API methods (GetConditionCodes, GetExchanges, GetMarketStatus, GetTickerTypes)
- **TreyThomasCodes.Polygon.TestApp** - Console application for manual testing and development
- Uses `coverlet.collector` for code coverage across all test projects
- Moq framework available for mocking dependencies
- Test files follow the naming convention: `{MethodName}Tests.cs` for unit tests and `{MethodName}IntegrationTests.cs` for integration tests

### Package Generation
- Both RestClient and Models projects generate NuGet packages on build
- Current version 0.0.1 (development phase)
- MPL 2.0 license with comprehensive package metadata
- Documentation XML generation enabled for both projects

## Coding Style
- All classes and interfaces **MUST** have comprehensive vsdoc comments