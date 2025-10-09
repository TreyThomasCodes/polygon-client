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
        var response = await _polygonClient.Stocks.GetLastTradeAsync(ticker);
        return response?.Results?.Price;
    }

    public async Task<decimal?> GetLastQuoteBidPrice(string ticker)
    {
        var response = await _polygonClient.Stocks.GetLastQuoteAsync(ticker);
        return response?.Results?.Bid?.Price;
    }

    public async Task<bool> IsMarketOpen()
    {
        var response = await _polygonClient.ReferenceData.GetMarketStatusAsync();
        return response?.Market == "open";
    }
}
```

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

// Get historical price data
var bars = await _polygonClient.Stocks.GetBarsAsync(
    ticker: "AAPL",
    multiplier: 1,
    timespan: AggregateInterval.Day,
    from: "2025-09-01",
    to: "2025-09-30",
    adjusted: true
);

// Get the latest trade
var lastTrade = await _polygonClient.Stocks.GetLastTradeAsync("AAPL");
Console.WriteLine($"Last trade price: ${lastTrade.Results.Price}");

// Search for tickers
var tickers = await _polygonClient.ReferenceData.GetTickersAsync(
    search: "Apple",
    active: true,
    limit: 10
);

// Check market status
var status = await _polygonClient.ReferenceData.GetMarketStatusAsync();
Console.WriteLine($"Market is {status.Market}");
```

See [USAGE.md](USAGE.md) for detailed examples of all available API calls.

## License

MPL 2.0 License - see LICENSE file for details.