# TreyThomasCodes.Polygon - .NET Client Library for Polygon.io

A reusable .NET 8+ library for connecting to and retrieving data from the Polygon.io API.

## Installation

```bash
dotnet add package TreyThomasCodes.Polygon.RestClient
```

## Quick Start

### 1. Register the Client with Dependency Injection

The library provides extension methods for `IServiceCollection` to register the Polygon client. There are three ways to configure it:

#### Option A: Using Configuration (appsettings.json)

Add the Polygon configuration to your `appsettings.json`:

```json
{
  "Polygon": {
    "ApiKey": "your-api-key-here",
    "BaseUrl": "https://api.polygon.io",
    "TimeoutSeconds": 30,
    "MaxRetries": 3
  }
}
```

Register the client in your `Program.cs` or `Startup.cs`:

```csharp
using TreyThomasCodes.Polygon.RestClient;

var builder = WebApplication.CreateBuilder(args);

// Register Polygon client using configuration
builder.Services.AddPolygonClient(builder.Configuration);
```

#### Option B: Using API Key Directly

```csharp
using TreyThomasCodes.Polygon.RestClient;

var builder = WebApplication.CreateBuilder(args);

// Register Polygon client with API key
builder.Services.AddPolygonClient("your-api-key-here");
```

#### Option C: Using Configuration Action

```csharp
using TreyThomasCodes.Polygon.RestClient;

var builder = WebApplication.CreateBuilder(args);

// Register Polygon client with configuration action
builder.Services.AddPolygonClient(options =>
{
    options.ApiKey = "your-api-key-here";
    options.BaseUrl = "https://api.polygon.io";
    options.TimeoutSeconds = 30;
    options.MaxRetries = 3;
});
```

### 2. Inject and Use the Client

Once registered, inject `IPolygonClient` into your services or controllers:

```csharp
using TreyThomasCodes.Polygon.RestClient.Services;

public class StockDataService
{
    private readonly IPolygonClient _polygonClient;

    public StockDataService(IPolygonClient polygonClient)
    {
        _polygonClient = polygonClient;
    }

    public async Task<decimal?> GetLastTradePrice(string ticker)
    {
        var request = new GetLastTradeRequest { Ticker = ticker };
        var response = await _polygonClient.Stocks.GetLastTradeAsync(request);
        return response?.Results?.Price;
    }

    public async Task<decimal?> GetLastQuoteBidPrice(string ticker)
    {
        var request = new GetLastQuoteRequest { Ticker = ticker };
        var response = await _polygonClient.Stocks.GetLastQuoteAsync(request);
        return response?.Results?.Bid?.Price;
    }

    public async Task<bool> IsMarketOpen()
    {
        var request = new GetMarketStatusRequest();
        var response = await _polygonClient.ReferenceData.GetMarketStatusAsync(request);
        return response?.Market == "open";
    }
}
```

## API Usage Patterns

This library provides **three different patterns** for making API calls. Choose the one that best fits your coding style:

### Pattern 1: Request Objects (Primary Pattern)

The most explicit pattern using strongly-typed request objects:

```csharp
using TreyThomasCodes.Polygon.RestClient.Requests.Stocks;

var request = new GetBarsRequest
{
    Ticker = "AAPL",
    Multiplier = 1,
    Timespan = AggregateInterval.Day,
    From = "2025-01-01",
    To = "2025-01-31",
    Adjusted = true
};
var bars = await _polygonClient.Stocks.GetBarsAsync(request);
```

### Pattern 2: Simple Parameter Overloads (Convenience Pattern)

For simple, single-parameter operations:

```csharp
// Get snapshot with just the ticker
var snapshot = await _polygonClient.Stocks.GetSnapshotAsync("AAPL");

// Get last trade with just the ticker
var lastTrade = await _polygonClient.Stocks.GetLastTradeAsync("MSFT");

// Get ticker details with just the ticker
var details = await _polygonClient.ReferenceData.GetTickerDetailsAsync("TSLA");
```

### Pattern 3: Fluent API (Optional Pattern)

For expressive, chainable queries (requires `using TreyThomasCodes.Polygon.RestClient.Fluent;`):

```csharp
using TreyThomasCodes.Polygon.RestClient.Fluent;

// Get stock bars with a fluent, progressive builder
var bars = await _polygonClient.Stocks
    .Bars("AAPL")
    .From("2025-01-01")
    .To("2025-01-31")
    .Daily()
    .Adjusted()
    .Limit(100)
    .ExecuteAsync();

// Search tickers with fluent filters
var tickers = await _polygonClient.ReferenceData
    .Tickers()
    .Search("Apple")
    .ActiveOnly()
    .OfType("CS")
    .Limit(10)
    .ExecuteAsync();

// Get options chain with fluent filtering
var chain = await _polygonClient.Options
    .ChainSnapshot("SPY")
    .CallsOnly()
    .ExpiringBetween("2025-12-01", "2025-12-31")
    .AtStrike(650m)
    .Limit(100)
    .ExecuteAsync();
```

The fluent API provides:
- **25 query builders** (10 for Stocks, 9 for Options, 6 for Reference Data)
- **Interval helpers** like `Daily()`, `Hourly(4)`, `Minutely(15)`
- **Semantic filters** like `CallsOnly()`, `ExpiringBetween()`, `ActiveOnly()`
- **Progressive building** - set parameters step by step
- **Same validation** - delegates to request objects internally

All patterns produce the same results and use the same validation. Choose based on your preference!

## Available Services

The `IPolygonClient` provides access to the following services:

### Stocks Service (`IStocksService`)

Accessed via `IPolygonClient.Stocks`

#### Aggregates (OHLC Bars)
- **`GetBarsAsync`** - Get aggregate OHLC bars for a stock over a date range
- **`GetPreviousCloseAsync`** - Get the previous day's OHLC data
- **`GetGroupedDailyAsync`** - Get daily OHLC data for all tickers on a specific date
- **`GetDailyOpenCloseAsync`** - Get the open and close prices for a specific day

#### Trades & Quotes
- **`GetTradesAsync`** - Get tick-level trade data for a stock
- **`GetQuotesAsync`** - Get tick-level quote (bid/ask) data for a stock
- **`GetLastTradeAsync`** - Get the most recent trade for a stock
- **`GetLastQuoteAsync`** - Get the most recent NBBO quote for a stock

#### Snapshots
- **`GetMarketSnapshotAsync`** - Get snapshot data for all available tickers
- **`GetSnapshotAsync`** - Get snapshot data for a specific ticker

### Reference Data Service (`IReferenceDataService`)

Accessed via `IPolygonClient.ReferenceData`

#### Ticker Information
- **`GetTickersAsync`** - Get a list of tickers with various filter options
- **`GetTickerDetailsAsync`** - Get detailed information for a specific ticker
- **`GetTickerTypesAsync`** - Get list of all supported ticker types

#### Market Information
- **`GetMarketStatusAsync`** - Get current market trading status
- **`GetExchangesAsync`** - Get list of exchanges and market centers
- **`GetConditionCodesAsync`** - Get trade and quote condition codes

### Options Service (`IOptionsService`)

Accessed via `IPolygonClient.Options`

#### Options Contracts
- **`GetContractDetailsAsync`** - Get detailed information about a specific options contract by its ticker symbol

#### Options Market Data
- **`GetSnapshotAsync`** - Get a snapshot of current market data for a specific options contract, including Greeks, implied volatility, and underlying asset information
- **`GetChainSnapshotAsync`** - Get snapshots for all options contracts for a given underlying asset with filtering by strike price, contract type, and expiration date
- **`GetLastTradeAsync`** - Get the most recent trade for a specific options contract
- **`GetQuotesAsync`** - Get historical bid/ask quote data for a specific options contract with time-based filtering and pagination
- **`GetTradesAsync`** - Get historical trade data for a specific options contract with time-based filtering and pagination
- **`GetBarsAsync`** - Get aggregate OHLC (bar/candle) data for an options contract over a specified time range with configurable intervals
- **`GetPreviousDayBarAsync`** - Get the previous trading day's OHLC bar data for a specific options contract
- **`GetDailyOpenCloseAsync`** - Get daily open, high, low, close summary for a specific options contract including pre-market and after-hours prices

#### Options Ticker Construction Helpers

Working with options tickers can be complex due to the OCC format requirements. The library provides helper classes to simplify ticker construction:

**`OptionsTicker`** - Static factory methods for creating and parsing OCC format options tickers:

```csharp
using TreyThomasCodes.Polygon.Models.Options;

// Create an options ticker from components
string ticker = OptionsTicker.Create(
    underlying: "UBER",
    expirationDate: new DateTime(2022, 1, 21),
    type: OptionType.Call,
    strike: 50m
);
// Returns: "O:UBER220121C00050000"

// Parse an existing ticker into components
var parsed = OptionsTicker.Parse("O:UBER220121C00050000");
Console.WriteLine($"Underlying: {parsed.Underlying}");      // UBER
Console.WriteLine($"Expiration: {parsed.ExpirationDate}");  // 2022-01-21
Console.WriteLine($"Type: {parsed.Type}");                  // Call
Console.WriteLine($"Strike: {parsed.Strike}");              // 50
```

**`OptionsTickerBuilder`** - Fluent API for building options tickers:

```csharp
using TreyThomasCodes.Polygon.Models.Options;

var ticker = new OptionsTickerBuilder()
    .WithUnderlying("SPY")
    .WithExpiration(2025, 12, 19)
    .AsCall()
    .WithStrike(650m)
    .Build();
// Returns: "O:SPY251219C00650000"
```

**Convenience Methods** - Simplified methods for common operations:

```csharp
using TreyThomasCodes.Polygon.Models.Options;
using TreyThomasCodes.Polygon.Models.Common;

// Method 1: Use components directly (simplest approach)
var contract = await _polygonClient.Options.GetContractByComponentsAsync(
    underlying: "UBER",
    expirationDate: new DateTime(2022, 1, 21),
    type: OptionType.Call,
    strike: 50m
);

var snapshot = await _polygonClient.Options.GetSnapshotByComponentsAsync(
    underlying: "SPY",
    expirationDate: new DateTime(2025, 12, 19),
    type: OptionType.Call,
    strike: 650m
);

var lastTrade = await _polygonClient.Options.GetLastTradeByComponentsAsync(
    underlying: "TSLA",
    expirationDate: new DateTime(2026, 3, 20),
    type: OptionType.Call,
    strike: 700m
);

var bars = await _polygonClient.Options.GetBarsByComponentsAsync(
    underlying: "SPY",
    expirationDate: new DateTime(2025, 12, 19),
    type: OptionType.Call,
    strike: 650m,
    multiplier: 1,
    timespan: AggregateInterval.Day,
    from: "2025-11-01",
    to: "2025-11-30"
);

var chainSnapshot = await _polygonClient.Options.GetChainSnapshotByComponentsAsync(
    underlyingAsset: "SPY",
    type: OptionType.Call,
    expirationDateGte: new DateTime(2025, 12, 1),
    expirationDateLte: new DateTime(2025, 12, 31),
    limit: 100
);

// Method 2: Use OptionsTicker objects for reusability
var ticker = OptionsTicker.Parse("O:SPY251219C00650000");
var contractDetails = await _polygonClient.Options.GetContractDetailsAsync(ticker);
var marketSnapshot = await _polygonClient.Options.GetSnapshotAsync(ticker);
var trade = await _polygonClient.Options.GetLastTradeAsync(ticker);
var quotes = await _polygonClient.Options.GetQuotesAsync(ticker, timestamp: "2024-12-01", limit: 100);
var trades = await _polygonClient.Options.GetTradesAsync(ticker, timestamp: "2021-09-03", limit: 100);
var dailyBars = await _polygonClient.Options.GetBarsAsync(ticker, 1, AggregateInterval.Day, "2025-11-01", "2025-11-30");
var dailyOHLC = await _polygonClient.Options.GetDailyOpenCloseAsync(ticker, "2023-01-09");
var previousDay = await _polygonClient.Options.GetPreviousDayBarAsync(ticker);

// Helper methods: Discover available strikes and expiration dates
var strikes = await _polygonClient.Options.GetAvailableStrikesAsync(
    underlying: "SPY",
    type: OptionType.Call,
    expirationDateGte: "2025-12-01",
    expirationDateLte: "2025-12-31"
);

var expirations = await _polygonClient.Options.GetExpirationDatesAsync(
    underlying: "UBER",
    type: OptionType.Put,
    strikePrice: 50m  // Optional: filter by strike
);
```

These helpers eliminate the need to manually construct OCC format ticker strings and provide type-safe, validated ticker generation.

## Configuration Options

| Option | Description | Default |
|--------|-------------|---------|
| `ApiKey` | Your Polygon.io API key (required) | - |
| `BaseUrl` | Base URL for Polygon API | `https://api.polygon.io` |
| `TimeoutSeconds` | HTTP request timeout in seconds | `30` |
| `MaxRetries` | Maximum number of retry attempts | `3` |

## Documentation

For comprehensive examples and detailed usage instructions for every API call, see [USAGE.md](USAGE.md).

### Quick Examples

```csharp
using TreyThomasCodes.Polygon.Models.Common;
using TreyThomasCodes.Polygon.RestClient.Requests.Stocks;
using TreyThomasCodes.Polygon.RestClient.Requests.Options;
using TreyThomasCodes.Polygon.RestClient.Requests.Reference;

// Get historical price data
var barsRequest = new GetBarsRequest
{
    Ticker = "AAPL",
    Multiplier = 1,
    Timespan = AggregateInterval.Day,
    From = "2025-09-01",
    To = "2025-09-30",
    Adjusted = true
};
var bars = await _polygonClient.Stocks.GetBarsAsync(barsRequest);

// Get the latest trade
var lastTradeRequest = new GetLastTradeRequest { Ticker = "AAPL" };
var lastTrade = await _polygonClient.Stocks.GetLastTradeAsync(lastTradeRequest);
Console.WriteLine($"Last trade price: ${lastTrade.Results.Price}");

// Search for tickers
var tickersRequest = new GetTickersRequest
{
    Search = "Apple",
    Active = true,
    Limit = 10
};
var tickers = await _polygonClient.ReferenceData.GetTickersAsync(tickersRequest);

// Check market status
var statusRequest = new GetMarketStatusRequest();
var status = await _polygonClient.ReferenceData.GetMarketStatusAsync(statusRequest);
Console.WriteLine($"Market is {status.Market}");

// Get option contract details
var contractRequest = new GetContractDetailsRequest
{
    OptionsTicker = "O:SPY251219C00650000"
};
var optionContract = await _polygonClient.Options.GetContractDetailsAsync(contractRequest);
Console.WriteLine($"Strike: ${optionContract.Results.StrikePrice}, Expiration: {optionContract.Results.ExpirationDate}");

// Get option bars (historical OHLC data)
var optionBarsRequest = new Options.GetBarsRequest
{
    OptionsTicker = "O:SPY251219C00650000",
    Multiplier = 1,
    Timespan = AggregateInterval.Day,
    From = "2023-01-09",
    To = "2023-02-10"
};
var optionBars = await _polygonClient.Options.GetBarsAsync(optionBarsRequest);
Console.WriteLine($"Found {optionBars.Results?.Count} option bars");
```

See [USAGE.md](USAGE.md) for detailed examples of all available API calls.

## License

MPL 2.0 License - see LICENSE file for details.