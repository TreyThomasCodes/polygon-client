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
The client uses a layered architecture with a clear separation between internal implementation details and public API surface:

#### Internal Implementation Layers (Not Exposed to Consumers)

1. **API Layer** (`/Api/`) - **INTERNAL** Refit interfaces defining HTTP endpoints with primitive parameters
   - `IPolygonStocksApi` - Low-level stock API with primitive parameters (ticker string, multiplier int, etc.)
   - `IPolygonReferenceApi` - Low-level reference data API with primitive parameters
   - `IPolygonOptionsApi` - Low-level options API with primitive parameters
   - These interfaces are marked `internal` and are not part of the public API surface
   - Used internally by service implementations to make HTTP calls via Refit

#### Public API Layers (Exposed to Consumers)

2. **Request Layer** (`/Requests/`) - **PUBLIC** Strongly-typed request objects for API calls
   - `/Stocks/` - Request objects for stock market data (e.g., `GetBarsRequest`, `GetTradesRequest`, `GetQuotesRequest`)
   - `/Options/` - Request objects for options data (e.g., `GetContractDetailsRequest`, `GetBarsRequest`, `GetSnapshotRequest`)
   - `/Reference/` - Request objects for reference data (e.g., `GetTickersRequest`, `GetMarketStatusRequest`)
   - Each request object encapsulates all parameters for a specific API call
   - Provides a clean, discoverable API with IntelliSense support

3. **Validation Layer** (`/Validators/`) - **INTERNAL** FluentValidation validators for request objects
   - `/Stocks/` - Validators for stock requests (e.g., `GetBarsRequestValidator`, `GetTradesRequestValidator`)
   - `/Options/` - Validators for options requests (e.g., `GetContractDetailsRequestValidator`, `GetBarsRequestValidator`)
   - `/Reference/` - Validators for reference data requests (e.g., `GetTickersRequestValidator`, `GetMarketStatusRequestValidator`)
   - Validators enforce parameter requirements, formats, and constraints before API calls
   - Automatically integrated into service methods via dependency injection
   - Validation happens transparently; consumers receive `PolygonValidationException` if validation fails

4. **Service Layer** (`/Services/`) - **PUBLIC INTERFACES**, internal implementations
   - `IPolygonClient` - **PUBLIC** Main facade providing access to all services
   - `IStocksService`, `IReferenceDataService`, `IOptionsService` - **PUBLIC** Domain-specific service interfaces
   - `StocksService`, `ReferenceDataService`, `OptionsService` - **INTERNAL** Service implementations
   - All public service methods accept request objects and automatically validate them using FluentValidation
   - Service implementations orchestrate validation, API calls, and exception handling
   - `IOptionsService` includes convenience methods for enhanced usability:
     - Component-based methods (e.g., `GetContractByComponentsAsync`, `GetSnapshotByComponentsAsync`, `GetChainSnapshotByComponentsAsync`, `GetLastTradeByComponentsAsync`, `GetBarsByComponentsAsync`)
     - OptionsTicker-based overloads for all major Options API calls
     - Discovery helpers (`GetAvailableStrikesAsync`, `GetExpirationDatesAsync`)

5. **Configuration** (`/Configuration/`) - **PUBLIC** Options pattern for settings
   - `PolygonOptions` - API key, base URL, timeout, retry settings

6. **Authentication** (`/Authentication/`) - **INTERNAL** HTTP message handlers
   - `PolygonAuthenticationHandler` - Adds API key to requests

7. **Fluent API Layer** (`/Fluent/`) - **PUBLIC** Optional fluent query builders for expressive API usage
   - **Extension Methods** (`*ServiceFluentExtensions.cs`) - Extension methods on service interfaces
     - `StocksServiceFluentExtensions` - Entry points for Stocks fluent builders
     - `OptionsServiceFluentExtensions` - Entry points for Options fluent builders
     - `ReferenceDataServiceFluentExtensions` - Entry points for Reference fluent builders
   - **Query Builders** (`/Builders/`) - Progressive fluent builders for each API endpoint
     - `/Builders/Stocks/` - 10 builders for stocks API (BarsQueryBuilder, TradesQueryBuilder, QuotesQueryBuilder, etc.)
     - `/Builders/Options/` - 9 builders for options API (OptionBarsQueryBuilder, ChainSnapshotQueryBuilder, etc.)
     - `/Builders/Reference/` - 6 builders for reference API (TickersQueryBuilder, ConditionCodesQueryBuilder, etc.)
   - Accessed via separate namespace: `using TreyThomasCodes.Polygon.RestClient.Fluent;`
   - Opt-in design prevents IntelliSense pollution for users who prefer request objects
   - All builders delegate to existing request-based methods for validation consistency

### Public API Surface

Consumers of this library can use **three different patterns** for making API calls:

#### Pattern 1: Request Objects (Primary Pattern)
- **Request objects** in the `/Requests/` folder for specifying API parameters
- **Service interfaces** (`IPolygonClient`, `IStocksService`, `IOptionsService`, `IReferenceDataService`) for making API calls
- Most explicit and discoverable via IntelliSense
- All validation rules are enforced before API calls

#### Pattern 2: Simple Parameter Overloads (Convenience Pattern)
- Selected simple methods with direct parameter passing (e.g., `GetSnapshotAsync(string ticker)`)
- Available for the most common single-parameter operations
- Internally creates request objects and delegates to Pattern 1

#### Pattern 3: Fluent API (Optional Pattern)
- **Fluent query builders** in the `/Fluent/` namespace
- Progressive building with method chaining
- Requires `using TreyThomasCodes.Polygon.RestClient.Fluent;` to opt-in
- Example: `_client.Stocks.Bars("AAPL").From("2025-01-01").To("2025-01-31").Daily().ExecuteAsync()`
- Internally creates request objects and delegates to Pattern 1

All patterns share:
- **Model classes** from the `TreyThomasCodes.Polygon.Models` package for response data
- **Exception types** (`PolygonException`, `PolygonValidationException`, `PolygonApiException`, `PolygonHttpException`) for error handling

The primitive API layer (Refit interfaces with individual parameters) is intentionally hidden to:
- Enforce consistent validation across all API calls
- Provide a cleaner, more discoverable API surface
- Enable future enhancements without breaking changes
- Encourage best practices through request objects

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

### Fluent API Patterns
The library provides an optional fluent API for more expressive query construction. This is a **Tier 3** API that sits on top of the request object pattern.

**Accessing the Fluent API:**
```csharp
// Add the fluent namespace to opt-in
using TreyThomasCodes.Polygon.RestClient.Fluent;

// Now fluent extension methods are available on service interfaces
var bars = await _client.Stocks
    .Bars("AAPL")
    .From("2025-01-01")
    .To("2025-01-31")
    .Daily()
    .Adjusted()
    .Limit(100)
    .ExecuteAsync();
```

**Key Characteristics:**
- **Opt-in via namespace** - Prevents IntelliSense clutter for users who prefer request objects
- **Progressive building** - Parameters set via fluent methods, validated at execution time
- **Verb-first naming** - Extension methods use verb-first names (e.g., `Bars()`, `Tickers()`, `OptionSnapshot()`)
- **Async-only execution** - All builders terminate with `.ExecuteAsync()`
- **Validation consistency** - All builders delegate to request-based methods for consistent validation
- **Interval helpers** - Convenient methods for time intervals (e.g., `Daily()`, `Hourly(4)`, `Minutely(15)`)
- **Filter helpers** - Semantic filtering methods (e.g., `AtStrike()`, `CallsOnly()`, `ExpiringBetween()`)

**Available Fluent Builders:**

**Stocks Service (10 builders):**
- `Bars()` - OHLC aggregates with interval helpers
- `Trades()` - Historical trade data with timestamp filters
- `Quotes()` - Historical quote data with timestamp filters
- `Snapshot()` - Ticker snapshot
- `LastTrade()` - Most recent trade
- `LastQuote()` - Most recent quote
- `PreviousClose()` - Previous day's close
- `GroupedDaily()` - All tickers for a date
- `DailyOpenClose()` - Daily open/close for a ticker and date
- `MarketSnapshot()` - Snapshot of all tickers

**Options Service (9 builders):**
- `ContractDetails()` - Contract details by ticker
- `OptionSnapshot()` - Option contract snapshot with smart ticker parsing
- `ChainSnapshot()` - Full options chain with extensive filtering
- `OptionBars()` - OHLC data with interval helpers
- `OptionTrades()` - Historical trade data
- `OptionQuotes()` - Historical quote data
- `OptionLastTrade()` - Most recent trade
- `OptionDailyOpenClose()` - Daily open/close data
- `OptionPreviousDayBar()` - Previous day bar

**Reference Service (6 builders):**
- `Tickers()` - Search/filter tickers with 15+ options
- `TickerDetails()` - Ticker details with optional date
- `MarketStatus()` - Current market status
- `TickerTypes()` - All ticker types
- `ConditionCodes()` - Condition codes with filters
- `Exchanges()` - Exchanges with asset class/locale filters

**Example Comparisons:**

Request Object Pattern:
```csharp
var request = new GetBarsRequest
{
    Ticker = "AAPL",
    Multiplier = 1,
    Timespan = AggregateInterval.Day,
    From = "2025-01-01",
    To = "2025-01-31",
    Adjusted = true,
    Limit = 100
};
var bars = await _client.Stocks.GetBarsAsync(request);
```

Fluent API Pattern:
```csharp
using TreyThomasCodes.Polygon.RestClient.Fluent;

var bars = await _client.Stocks
    .Bars("AAPL")
    .From("2025-01-01")
    .To("2025-01-31")
    .Daily()
    .Adjusted()
    .Limit(100)
    .ExecuteAsync();
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
  - `/Services/Stocks/` - Stocks service tests split by method (GetLastQuote, GetLastTrade, GetSnapshot, SimpleOverloads)
  - `/Fluent/` - Fluent API query builder tests (BarsQueryBuilder, OptionBarsQueryBuilder, TickersQueryBuilder)
- **TreyThomasCodes.Polygon.IntegrationTests** - Integration tests for complete API workflows, organized by service area
  - `IntegrationTestBase.cs` - Base class providing DI setup, Polygon client configuration, and resource disposal for all integration tests
  - `/Options/` - Integration tests for Options API methods (GetBars, GetChainSnapshot, GetContractDetails, GetDailyOpenClose, GetLastTrade, GetPreviousDayBar, GetQuotes, GetSnapshot, GetTrades)
  - `/OptionsExtensions/` - Integration tests for Options convenience methods (ComponentBasedMethods, OptionsTickerOverloads, DiscoveryHelpers)
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