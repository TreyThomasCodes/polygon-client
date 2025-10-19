# TreyThomasCodes.Polygon - Complete API Usage Guide

This guide provides comprehensive examples for every API call implemented in the TreyThomasCodes.Polygon library.

## Table of Contents

- [Setup](#setup)
- [Stocks Service](#stocks-service)
  - [Aggregates (OHLC Bars)](#aggregates-ohlc-bars)
  - [Trades](#trades)
  - [Quotes](#quotes)
  - [Snapshots](#snapshots)
- [Reference Data Service](#reference-data-service)
  - [Tickers](#tickers)
  - [Market Information](#market-information)
- [Options Service](#options-service)
- [Common Patterns](#common-patterns)

## Setup

### Dependency Injection Registration

```csharp
using TreyThomasCodes.Polygon.RestClient;

var builder = WebApplication.CreateBuilder(args);

// Option 1: Using configuration from appsettings.json
builder.Services.AddPolygonClient(builder.Configuration);

// Option 2: Using API key directly
builder.Services.AddPolygonClient("your-api-key-here");

// Option 3: Using configuration action
builder.Services.AddPolygonClient(options =>
{
    options.ApiKey = "your-api-key-here";
    options.BaseUrl = "https://api.polygon.io";
    options.TimeoutSeconds = 30;
    options.MaxRetries = 3;
});
```

### Injecting the Client

```csharp
using TreyThomasCodes.Polygon.RestClient.Services;

public class MyService
{
    private readonly IPolygonClient _client;

    public MyService(IPolygonClient client)
    {
        _client = client;
    }
}
```

## Stocks Service

Access the stocks service via `IPolygonClient.Stocks`.

### Aggregates (OHLC Bars)

#### GetBarsAsync - Get Historical OHLC Data

Retrieve aggregate bars for a ticker over a date range with configurable intervals.

```csharp
using TreyThomasCodes.Polygon.Models.Common;
using TreyThomasCodes.Polygon.RestClient.Requests.Stocks;

// Get daily bars for the last month
var dailyBarsRequest = new GetBarsRequest
{
    Ticker = "AAPL",
    Multiplier = 1,
    Timespan = AggregateInterval.Day,
    From = "2025-09-01",
    To = "2025-09-30",
    Adjusted = true,
    Sort = SortOrder.Ascending,
    Limit = 5000
};
var dailyBars = await _client.Stocks.GetBarsAsync(dailyBarsRequest);

foreach (var bar in dailyBars.Results)
{
    Console.WriteLine($"Date: {bar.Timestamp}, O: {bar.Open}, H: {bar.High}, L: {bar.Low}, C: {bar.Close}, V: {bar.Volume}");
}

// Get 5-minute bars for intraday analysis
var intradayBarsRequest = new GetBarsRequest
{
    Ticker = "TSLA",
    Multiplier = 5,
    Timespan = AggregateInterval.Minute,
    From = "2025-09-15",
    To = "2025-09-15",
    Adjusted = true
};
var intradayBars = await _client.Stocks.GetBarsAsync(intradayBarsRequest);

// Get hourly bars
var hourlyBarsRequest = new GetBarsRequest
{
    Ticker = "MSFT",
    Multiplier = 1,
    Timespan = AggregateInterval.Hour,
    From = "2025-09-01",
    To = "2025-09-30"
};
var hourlyBars = await _client.Stocks.GetBarsAsync(hourlyBarsRequest);

// Get weekly bars
var weeklyBarsRequest = new GetBarsRequest
{
    Ticker = "GOOGL",
    Multiplier = 1,
    Timespan = AggregateInterval.Week,
    From = "2025-01-01",
    To = "2025-09-30"
};
var weeklyBars = await _client.Stocks.GetBarsAsync(weeklyBarsRequest);

// Get monthly bars for long-term analysis
var monthlyBarsRequest = new GetBarsRequest
{
    Ticker = "SPY",
    Multiplier = 1,
    Timespan = AggregateInterval.Month,
    From = "2020-01-01",
    To = "2025-09-30"
};
var monthlyBars = await _client.Stocks.GetBarsAsync(monthlyBarsRequest);
```

#### GetPreviousCloseAsync - Get Previous Day's Close

Get the OHLC data for the previous trading day.

```csharp
using TreyThomasCodes.Polygon.RestClient.Requests.Stocks;

var request = new GetPreviousCloseRequest
{
    Ticker = "AAPL",
    Adjusted = true
};
var previousClose = await _client.Stocks.GetPreviousCloseAsync(request);

if (previousClose.Results?.Count > 0)
{
    var bar = previousClose.Results[0];
    Console.WriteLine($"Previous Close: ${bar.Close}");
    Console.WriteLine($"Volume: {bar.Volume}");
    Console.WriteLine($"VWAP: ${bar.VolumeWeightedAveragePrice}");
}
```

#### GetGroupedDailyAsync - Get All Tickers for a Date

Get daily OHLC data for all available tickers on a specific date.

```csharp
using TreyThomasCodes.Polygon.RestClient.Requests.Stocks;

// Get all tickers for a specific date
var request = new GetGroupedDailyRequest
{
    Date = "2025-09-15",
    Adjusted = true,
    IncludeOtc = false
};
var groupedDaily = await _client.Stocks.GetGroupedDailyAsync(request);

Console.WriteLine($"Found {groupedDaily.Results?.Count ?? 0} tickers");

foreach (var bar in groupedDaily.Results?.Take(10) ?? Enumerable.Empty<StockBar>())
{
    Console.WriteLine($"{bar.Ticker}: Open=${bar.Open}, Close=${bar.Close}, Volume={bar.Volume}");
}

// Include OTC securities
var requestWithOtc = new GetGroupedDailyRequest
{
    Date = "2025-09-15",
    Adjusted = true,
    IncludeOtc = true
};
var groupedDailyWithOtc = await _client.Stocks.GetGroupedDailyAsync(requestWithOtc);
```

#### GetDailyOpenCloseAsync - Get Daily Open/Close Prices

Get detailed open and close prices for a specific day, including pre-market and after-hours activity.

```csharp
using TreyThomasCodes.Polygon.RestClient.Requests.Stocks;

var request = new GetDailyOpenCloseRequest
{
    Ticker = "AAPL",
    Date = "2025-09-15",
    Adjusted = true
};
var dailyOpenClose = await _client.Stocks.GetDailyOpenCloseAsync(request);

if (dailyOpenClose.Results?.Count > 0)
{
    var data = dailyOpenClose.Results[0];
    Console.WriteLine($"Open: ${data.Open}");
    Console.WriteLine($"High: ${data.High}");
    Console.WriteLine($"Low: ${data.Low}");
    Console.WriteLine($"Close: ${data.Close}");
    Console.WriteLine($"Volume: {data.Volume}");
    Console.WriteLine($"Pre-market: ${data.PreMarket}");
    Console.WriteLine($"After-hours: ${data.AfterHours}");
}
```

### Trades

#### GetTradesAsync - Get Tick-Level Trade Data

Retrieve detailed trade-by-trade data for a ticker.

```csharp
using TreyThomasCodes.Polygon.RestClient.Requests.Stocks;

// Get trades for a specific time range
var tradesRequest = new GetTradesRequest
{
    Ticker = "AAPL",
    TimestampGte = "2025-09-15T09:30:00.000000000Z",
    TimestampLte = "2025-09-15T16:00:00.000000000Z",
    Limit = 1000,
    Sort = "timestamp"
};
var trades = await _client.Stocks.GetTradesAsync(tradesRequest);

foreach (var trade in trades.Results ?? Enumerable.Empty<StockTrade>())
{
    Console.WriteLine($"Trade at {trade.ParticipantTimestamp}: {trade.Size} shares @ ${trade.Price}");
    Console.WriteLine($"  Exchange: {trade.Exchange}, Conditions: {string.Join(",", trade.Conditions ?? new List<int>())}");
}

// Get the most recent trades (descending order)
var recentTradesRequest = new GetTradesRequest
{
    Ticker = "TSLA",
    Limit = 100,
    Sort = "timestamp.desc"
};
var recentTrades = await _client.Stocks.GetTradesAsync(recentTradesRequest);

// Get trades for a specific timestamp
var specificTradesRequest = new GetTradesRequest
{
    Ticker = "MSFT",
    Timestamp = "1694782800000000000",
    Limit = 10
};
var specificTrades = await _client.Stocks.GetTradesAsync(specificTradesRequest);

// Get trades with greater than filter
var tradesAfterRequest = new GetTradesRequest
{
    Ticker = "GOOGL",
    TimestampGt = "2025-09-15T10:00:00.000000000Z",
    Limit = 500
};
var tradesAfter = await _client.Stocks.GetTradesAsync(tradesAfterRequest);
```

#### GetLastTradeAsync - Get Most Recent Trade

Get the most recent trade for a ticker.

```csharp
using TreyThomasCodes.Polygon.RestClient.Requests.Stocks;

var request = new GetLastTradeRequest { Ticker = "AAPL" };
var lastTrade = await _client.Stocks.GetLastTradeAsync(request);

if (lastTrade.Results != null)
{
    Console.WriteLine($"Last trade price: ${lastTrade.Results.Price}");
    Console.WriteLine($"Trade size: {lastTrade.Results.Size} shares");
    Console.WriteLine($"Exchange: {lastTrade.Results.Exchange}");
    Console.WriteLine($"Timestamp: {lastTrade.Results.ParticipantTimestamp}");
}
```

### Quotes

#### GetQuotesAsync - Get Tick-Level Quote Data

Retrieve detailed bid/ask quote data for a ticker.

```csharp
using TreyThomasCodes.Polygon.RestClient.Requests.Stocks;

// Get quotes for a specific time range
var quotesRequest = new GetQuotesRequest
{
    Ticker = "AAPL",
    TimestampGte = "2025-09-15T09:30:00.000000000Z",
    TimestampLte = "2025-09-15T16:00:00.000000000Z",
    Limit = 1000,
    Sort = "timestamp"
};
var quotes = await _client.Stocks.GetQuotesAsync(quotesRequest);

foreach (var quote in quotes.Results ?? Enumerable.Empty<StockQuote>())
{
    Console.WriteLine($"Quote at {quote.ParticipantTimestamp}:");
    Console.WriteLine($"  Bid: ${quote.BidPrice} x {quote.BidSize} on {quote.BidExchange}");
    Console.WriteLine($"  Ask: ${quote.AskPrice} x {quote.AskSize} on {quote.AskExchange}");
}

// Get recent quotes in descending order
var recentQuotesRequest = new GetQuotesRequest
{
    Ticker = "TSLA",
    Limit = 100,
    Sort = "timestamp.desc"
};
var recentQuotes = await _client.Stocks.GetQuotesAsync(recentQuotesRequest);

// Get quotes for a specific timestamp
var specificQuotesRequest = new GetQuotesRequest
{
    Ticker = "MSFT",
    Timestamp = "1694782800000000000"
};
var specificQuotes = await _client.Stocks.GetQuotesAsync(specificQuotesRequest);
```

#### GetLastQuoteAsync - Get Most Recent NBBO Quote

Get the most recent National Best Bid and Offer (NBBO) quote.

```csharp
using TreyThomasCodes.Polygon.RestClient.Requests.Stocks;

var request = new GetLastQuoteRequest { Ticker = "AAPL" };
var lastQuote = await _client.Stocks.GetLastQuoteAsync(request);

if (lastQuote.Results != null)
{
    Console.WriteLine($"Bid: ${lastQuote.Results.Bid.Price} x {lastQuote.Results.Bid.Size}");
    Console.WriteLine($"Ask: ${lastQuote.Results.Ask.Price} x {lastQuote.Results.Ask.Size}");
    Console.WriteLine($"Spread: ${lastQuote.Results.Ask.Price - lastQuote.Results.Bid.Price}");
    Console.WriteLine($"Mid-point: ${(lastQuote.Results.Bid.Price + lastQuote.Results.Ask.Price) / 2}");
}
```

### Snapshots

#### GetMarketSnapshotAsync - Get Snapshot of All Tickers

Get current market data for all available tickers.

```csharp
using TreyThomasCodes.Polygon.RestClient.Requests.Stocks;

// Get snapshot for all tickers
var request = new GetMarketSnapshotRequest
{
    IncludeOtc = false
};
var marketSnapshot = await _client.Stocks.GetMarketSnapshotAsync(request);

Console.WriteLine($"Total tickers: {marketSnapshot.Results?.Count ?? 0}");

// Display top gainers
var gainers = marketSnapshot.Results?
    .Where(s => s.TodaysChangePercentage.HasValue)
    .OrderByDescending(s => s.TodaysChangePercentage)
    .Take(10);

Console.WriteLine("\nTop Gainers:");
foreach (var snapshot in gainers ?? Enumerable.Empty<StockSnapshot>())
{
    Console.WriteLine($"{snapshot.Ticker}: {snapshot.TodaysChangePercentage:F2}% " +
                     $"(${snapshot.Day?.Close} from ${snapshot.PreviousDay?.Close})");
}

// Display top losers
var losers = marketSnapshot.Results?
    .Where(s => s.TodaysChangePercentage.HasValue)
    .OrderBy(s => s.TodaysChangePercentage)
    .Take(10);

Console.WriteLine("\nTop Losers:");
foreach (var snapshot in losers ?? Enumerable.Empty<StockSnapshot>())
{
    Console.WriteLine($"{snapshot.Ticker}: {snapshot.TodaysChangePercentage:F2}% " +
                     $"(${snapshot.Day?.Close} from ${snapshot.PreviousDay?.Close})");
}

// Include OTC securities
var requestWithOtc = new GetMarketSnapshotRequest
{
    IncludeOtc = true
};
var marketSnapshotWithOtc = await _client.Stocks.GetMarketSnapshotAsync(requestWithOtc);
```

#### GetSnapshotAsync - Get Snapshot for a Single Ticker

Get current market data for a specific ticker.

```csharp
using TreyThomasCodes.Polygon.RestClient.Requests.Stocks;

var request = new GetSnapshotRequest { Ticker = "AAPL" };
var snapshot = await _client.Stocks.GetSnapshotAsync(request);

if (snapshot.Ticker != null)
{
    Console.WriteLine($"Ticker: {snapshot.Ticker.Ticker}");
    Console.WriteLine($"Day High: ${snapshot.Ticker.Day?.High}");
    Console.WriteLine($"Day Low: ${snapshot.Ticker.Day?.Low}");
    Console.WriteLine($"Day Open: ${snapshot.Ticker.Day?.Open}");
    Console.WriteLine($"Day Close: ${snapshot.Ticker.Day?.Close}");
    Console.WriteLine($"Day Volume: {snapshot.Ticker.Day?.Volume}");
    Console.WriteLine($"Day VWAP: ${snapshot.Ticker.Day?.VolumeWeightedAveragePrice}");
    Console.WriteLine($"Previous Close: ${snapshot.Ticker.PreviousDay?.Close}");
    Console.WriteLine($"Today's Change: {snapshot.Ticker.TodaysChange}");
    Console.WriteLine($"Today's Change %: {snapshot.Ticker.TodaysChangePercentage:F2}%");

    // Last trade info
    if (snapshot.Ticker.LastTrade != null)
    {
        Console.WriteLine($"\nLast Trade:");
        Console.WriteLine($"  Price: ${snapshot.Ticker.LastTrade.Price}");
        Console.WriteLine($"  Size: {snapshot.Ticker.LastTrade.Size}");
        Console.WriteLine($"  Time: {snapshot.Ticker.LastTrade.ParticipantTimestamp}");
    }

    // Last quote info
    if (snapshot.Ticker.LastQuote != null)
    {
        Console.WriteLine($"\nLast Quote:");
        Console.WriteLine($"  Bid: ${snapshot.Ticker.LastQuote.BidPrice} x {snapshot.Ticker.LastQuote.BidSize}");
        Console.WriteLine($"  Ask: ${snapshot.Ticker.LastQuote.AskPrice} x {snapshot.Ticker.LastQuote.AskSize}");
    }
}
```

## Reference Data Service

Access the reference data service via `IPolygonClient.ReferenceData`.

### Tickers

#### GetTickersAsync - Search and Filter Tickers

Search for tickers with various filter options.

```csharp
using TreyThomasCodes.Polygon.Models.Common;
using TreyThomasCodes.Polygon.Models.Reference;
using TreyThomasCodes.Polygon.RestClient.Requests.Reference;

// Search by name
var appleTickersRequest = new GetTickersRequest
{
    Search = "Apple",
    Active = true,
    Limit = 10
};
var appleTickers = await _client.ReferenceData.GetTickersAsync(appleTickersRequest);

foreach (var ticker in appleTickers.Results ?? Enumerable.Empty<StockTicker>())
{
    Console.WriteLine($"{ticker.Ticker}: {ticker.Name} ({ticker.Type})");
}

// Filter by ticker type (common stock)
var commonStocksRequest = new GetTickersRequest
{
    Type = "CS",
    Active = true,
    Market = Market.Stocks,
    Limit = 100
};
var commonStocks = await _client.ReferenceData.GetTickersAsync(commonStocksRequest);

// Filter by exchange
var nasdaqStocksRequest = new GetTickersRequest
{
    Exchange = "XNAS",
    Active = true,
    Limit = 100,
    Sort = TickerSortFields.Ticker,
    Order = SortOrder.Ascending
};
var nasdaqStocks = await _client.ReferenceData.GetTickersAsync(nasdaqStocksRequest);

// Get tickers alphabetically in a range
var tickersAtoCRequest = new GetTickersRequest
{
    TickerGte = "A",
    TickerLt = "D",
    Active = true,
    Limit = 1000
};
var tickersAtoC = await _client.ReferenceData.GetTickersAsync(tickersAtoCRequest);

// Filter by specific ticker
var exactTickerRequest = new GetTickersRequest
{
    Ticker = "AAPL"
};
var exactTicker = await _client.ReferenceData.GetTickersAsync(exactTickerRequest);

// Filter by CUSIP
var tickerByCusipRequest = new GetTickersRequest
{
    Cusip = "037833100"
};
var tickerByCusip = await _client.ReferenceData.GetTickersAsync(tickerByCusipRequest);

// Filter by CIK
var tickerByCikRequest = new GetTickersRequest
{
    Cik = "0000320193"
};
var tickerByCik = await _client.ReferenceData.GetTickersAsync(tickerByCikRequest);

// Get historical ticker info for a specific date
var historicalTickerRequest = new GetTickersRequest
{
    Ticker = "AAPL",
    Date = "2020-01-01"
};
var historicalTicker = await _client.ReferenceData.GetTickersAsync(historicalTickerRequest);

// Search ETFs
var etfsRequest = new GetTickersRequest
{
    Type = "ETF",
    Active = true,
    Search = "S&P 500",
    Limit = 20
};
var etfs = await _client.ReferenceData.GetTickersAsync(etfsRequest);

// Sort by various fields
var tickersByNameRequest = new GetTickersRequest
{
    Active = true,
    Sort = TickerSortFields.Name,
    Order = SortOrder.Ascending,
    Limit = 100
};
var tickersByName = await _client.ReferenceData.GetTickersAsync(tickersByNameRequest);

var tickersByVolumeRequest = new GetTickersRequest
{
    Active = true,
    Sort = TickerSortFields.LastUpdatedUtc,
    Order = SortOrder.Descending,
    Limit = 100
};
var tickersByVolume = await _client.ReferenceData.GetTickersAsync(tickersByVolumeRequest);
```

#### GetTickerDetailsAsync - Get Detailed Ticker Information

Get comprehensive information about a specific ticker.

```csharp
using TreyThomasCodes.Polygon.RestClient.Requests.Reference;

// Get current ticker details
var request = new GetTickerDetailsRequest { Ticker = "AAPL" };
var tickerDetails = await _client.ReferenceData.GetTickerDetailsAsync(request);

if (tickerDetails.Results != null)
{
    var ticker = tickerDetails.Results;
    Console.WriteLine($"Ticker: {ticker.Ticker}");
    Console.WriteLine($"Name: {ticker.Name}");
    Console.WriteLine($"Market: {ticker.Market}");
    Console.WriteLine($"Locale: {ticker.Locale}");
    Console.WriteLine($"Primary Exchange: {ticker.PrimaryExchange}");
    Console.WriteLine($"Type: {ticker.Type}");
    Console.WriteLine($"Active: {ticker.Active}");
    Console.WriteLine($"Currency: {ticker.CurrencyName}");
    Console.WriteLine($"CIK: {ticker.Cik}");
    Console.WriteLine($"Composite FIGI: {ticker.CompositeFigi}");
    Console.WriteLine($"Share Class FIGI: {ticker.ShareClassFigi}");
    Console.WriteLine($"Market Cap: ${ticker.MarketCap}");
    Console.WriteLine($"Phone Number: {ticker.PhoneNumber}");
    Console.WriteLine($"Description: {ticker.Description}");
    Console.WriteLine($"Homepage: {ticker.HomepageUrl}");
    Console.WriteLine($"Total Employees: {ticker.TotalEmployees}");
    Console.WriteLine($"List Date: {ticker.ListDate}");
    Console.WriteLine($"Share Class Shares Outstanding: {ticker.ShareClassSharesOutstanding}");
    Console.WriteLine($"Weighted Shares Outstanding: {ticker.WeightedSharesOutstanding}");
    Console.WriteLine($"Round Lot: {ticker.RoundLot}");
}

// Get historical ticker details for a specific date
var historicalRequest = new GetTickerDetailsRequest
{
    Ticker = "AAPL",
    Date = "2020-01-01"
};
var historicalDetails = await _client.ReferenceData.GetTickerDetailsAsync(historicalRequest);
```

#### GetTickerTypesAsync - Get All Ticker Types

Get a list of all supported ticker types.

```csharp
using TreyThomasCodes.Polygon.RestClient.Requests.Reference;

var request = new GetTickerTypesRequest();
var tickerTypes = await _client.ReferenceData.GetTickerTypesAsync(request);

Console.WriteLine("Available Ticker Types:");
foreach (var type in tickerTypes.Results ?? Enumerable.Empty<TickerType>())
{
    Console.WriteLine($"{type.Code}: {type.Description}");
    Console.WriteLine($"  Asset Class: {type.AssetClass}");
    Console.WriteLine($"  Locale: {type.Locale}");
}

// Example output:
// CS: Common Stock
//   Asset Class: stocks
//   Locale: us
// ETF: Exchange Traded Fund
//   Asset Class: stocks
//   Locale: us
// PFD: Preferred Stock
//   Asset Class: stocks
//   Locale: us
```

### Market Information

#### GetMarketStatusAsync - Check Current Market Status

Get the current trading status of exchanges and markets.

```csharp
using TreyThomasCodes.Polygon.RestClient.Requests.Reference;

var request = new GetMarketStatusRequest();
var marketStatus = await _client.ReferenceData.GetMarketStatusAsync(request);

Console.WriteLine($"Market: {marketStatus.Market}");
Console.WriteLine($"Server Time: {marketStatus.ServerTime}");

// Check specific exchange statuses
Console.WriteLine("\nExchange Status:");
foreach (var exchange in marketStatus.Exchanges ?? new Dictionary<string, string>())
{
    Console.WriteLine($"  {exchange.Key}: {exchange.Value}");
}

// Check currency status
Console.WriteLine("\nCurrency Markets:");
foreach (var currency in marketStatus.Currencies ?? new Dictionary<string, string>())
{
    Console.WriteLine($"  {currency.Key}: {currency.Value}");
}

// Check if it's early hours trading
Console.WriteLine($"\nEarly Hours: {marketStatus.EarlyHours}");

// Check if it's after hours trading
Console.WriteLine($"After Hours: {marketStatus.AfterHours}");

// Simple market open check
bool isMarketOpen = marketStatus.Market == "open";
Console.WriteLine($"\nIs Market Open: {isMarketOpen}");
```

#### GetExchangesAsync - Get List of Exchanges

Get a comprehensive list of exchanges and market centers.

```csharp
using TreyThomasCodes.Polygon.Models.Common;
using TreyThomasCodes.Polygon.RestClient.Requests.Reference;

// Get all exchanges
var allExchangesRequest = new GetExchangesRequest();
var allExchanges = await _client.ReferenceData.GetExchangesAsync(allExchangesRequest);

Console.WriteLine("All Exchanges:");
foreach (var exchange in allExchanges.Results ?? Enumerable.Empty<Exchange>())
{
    Console.WriteLine($"{exchange.OperatingMic} - {exchange.Name}");
    Console.WriteLine($"  Type: {exchange.Type}");
    Console.WriteLine($"  Market: {exchange.Market}");
    Console.WriteLine($"  Locale: {exchange.Locale}");
    Console.WriteLine($"  MIC: {exchange.Mic}, Operating MIC: {exchange.OperatingMic}");
    Console.WriteLine($"  Participant ID: {exchange.ParticipantId}");
    Console.WriteLine($"  URL: {exchange.Url}");
}

// Filter by asset class
var stockExchangesRequest = new GetExchangesRequest
{
    AssetClass = AssetClass.Stocks
};
var stockExchanges = await _client.ReferenceData.GetExchangesAsync(stockExchangesRequest);

var cryptoExchangesRequest = new GetExchangesRequest
{
    AssetClass = AssetClass.Crypto
};
var cryptoExchanges = await _client.ReferenceData.GetExchangesAsync(cryptoExchangesRequest);

var forexExchangesRequest = new GetExchangesRequest
{
    AssetClass = AssetClass.Fx
};
var forexExchanges = await _client.ReferenceData.GetExchangesAsync(forexExchangesRequest);

// Filter by locale
var usExchangesRequest = new GetExchangesRequest
{
    Locale = Locale.Us
};
var usExchanges = await _client.ReferenceData.GetExchangesAsync(usExchangesRequest);

var globalExchangesRequest = new GetExchangesRequest
{
    Locale = Locale.Global
};
var globalExchanges = await _client.ReferenceData.GetExchangesAsync(globalExchangesRequest);

// Combine filters
var usStockExchangesRequest = new GetExchangesRequest
{
    AssetClass = AssetClass.Stocks,
    Locale = Locale.Us
};
var usStockExchanges = await _client.ReferenceData.GetExchangesAsync(usStockExchangesRequest);
```

#### GetConditionCodesAsync - Get Trade and Quote Condition Codes

Get condition codes that provide context for market data events.

```csharp
using TreyThomasCodes.Polygon.Models.Common;
using TreyThomasCodes.Polygon.Models.Reference;
using TreyThomasCodes.Polygon.RestClient.Requests.Reference;

// Get all condition codes
var allConditionsRequest = new GetConditionCodesRequest
{
    Limit = 1000
};
var allConditions = await _client.ReferenceData.GetConditionCodesAsync(allConditionsRequest);

foreach (var condition in allConditions.Results ?? Enumerable.Empty<ConditionCode>())
{
    Console.WriteLine($"ID {condition.Id}: {condition.Name}");
    Console.WriteLine($"  Asset Class: {condition.AssetClass}");
    Console.WriteLine($"  Data Types: {string.Join(", ", condition.DataTypes ?? new List<string>())}");
    Console.WriteLine($"  Description: {condition.Description}");
    Console.WriteLine($"  Abbreviation: {condition.Abbreviation}");
    Console.WriteLine($"  Legacy: {condition.Legacy}");
}

// Filter by asset class
var stockConditionsRequest = new GetConditionCodesRequest
{
    AssetClass = AssetClass.Stocks,
    Limit = 100
};
var stockConditions = await _client.ReferenceData.GetConditionCodesAsync(stockConditionsRequest);

// Filter by data type
var tradeConditionsRequest = new GetConditionCodesRequest
{
    DataType = DataType.Trade,
    Limit = 100
};
var tradeConditions = await _client.ReferenceData.GetConditionCodesAsync(tradeConditionsRequest);

var quoteConditionsRequest = new GetConditionCodesRequest
{
    DataType = DataType.Quote,
    Limit = 100
};
var quoteConditions = await _client.ReferenceData.GetConditionCodesAsync(quoteConditionsRequest);

// Get specific condition code
var specificConditionRequest = new GetConditionCodesRequest
{
    Id = "1"
};
var specificCondition = await _client.ReferenceData.GetConditionCodesAsync(specificConditionRequest);

// Get multiple condition codes
var multipleConditionsRequest = new GetConditionCodesRequest
{
    Id = "1,2,3,4,5"
};
var multipleConditions = await _client.ReferenceData.GetConditionCodesAsync(multipleConditionsRequest);

// Filter by SIP mapping
var consolidatedConditionsRequest = new GetConditionCodesRequest
{
    SipMapping = SipMappingType.Consolidated,
    Limit = 100
};
var consolidatedConditions = await _client.ReferenceData.GetConditionCodesAsync(consolidatedConditionsRequest);

var participant1ConditionsRequest = new GetConditionCodesRequest
{
    SipMapping = SipMappingType.Participant1,
    Limit = 100
};
var participant1Conditions = await _client.ReferenceData.GetConditionCodesAsync(participant1ConditionsRequest);

// Sort results
var sortedConditionsRequest = new GetConditionCodesRequest
{
    AssetClass = AssetClass.Stocks,
    Sort = ConditionCodeSortFields.Name,
    Order = SortOrder.Ascending,
    Limit = 100
};
var sortedConditions = await _client.ReferenceData.GetConditionCodesAsync(sortedConditionsRequest);

// Combine multiple filters
var stockTradeConditionsRequest = new GetConditionCodesRequest
{
    AssetClass = AssetClass.Stocks,
    DataType = DataType.Trade,
    SipMapping = SipMappingType.Consolidated,
    Sort = ConditionCodeSortFields.Id,
    Order = SortOrder.Ascending,
    Limit = 100
};
var stockTradeConditions = await _client.ReferenceData.GetConditionCodesAsync(stockTradeConditionsRequest);
```

## Options Service

Access the options service via `IPolygonClient.Options`.

### Options Contracts

#### GetContractDetailsAsync - Get Options Contract Information

Retrieve detailed information about a specific options contract by its ticker symbol.

```csharp
using TreyThomasCodes.Polygon.RestClient.Requests.Options;
using TreyThomasCodes.Polygon.RestClient.Requests.Stocks;

// Get contract details for a call option on SPY
var contractRequest = new GetContractDetailsRequest
{
    OptionsTicker = "O:SPY251219C00650000"
};
var contract = await _client.Options.GetContractDetailsAsync(contractRequest);

if (contract.Results != null)
{
    var details = contract.Results;
    Console.WriteLine($"Ticker: {details.Ticker}");
    Console.WriteLine($"Underlying: {details.UnderlyingTicker}");
    Console.WriteLine($"Contract Type: {details.ContractType}");
    Console.WriteLine($"Strike Price: ${details.StrikePrice}");
    Console.WriteLine($"Expiration Date: {details.ExpirationDate}");
    Console.WriteLine($"Exercise Style: {details.ExerciseStyle}");
    Console.WriteLine($"Shares Per Contract: {details.SharesPerContract}");
    Console.WriteLine($"Primary Exchange: {details.PrimaryExchange}");
    Console.WriteLine($"CFI Code: {details.Cfi}");
}

// Get contract details for a put option on AAPL
var putContractRequest = new GetContractDetailsRequest
{
    OptionsTicker = "O:AAPL251219P00200000"
};
var putContract = await _client.Options.GetContractDetailsAsync(putContractRequest);

if (putContract.Results != null)
{
    var details = putContract.Results;
    Console.WriteLine($"\n{details.ContractType?.ToUpper()} Option on {details.UnderlyingTicker}");
    Console.WriteLine($"Strike: ${details.StrikePrice}");
    Console.WriteLine($"Expires: {details.ExpirationDate}");
    Console.WriteLine($"Style: {details.ExerciseStyle}");
}

// Example: Calculate intrinsic value (requires current stock price)
var optionContractRequest = new GetContractDetailsRequest
{
    OptionsTicker = "O:TSLA251219C00250000"
};
var optionContract = await _client.Options.GetContractDetailsAsync(optionContractRequest);

var stockSnapshotRequest = new GetSnapshotRequest { Ticker = "TSLA" };
var stockSnapshot = await _client.Stocks.GetSnapshotAsync(stockSnapshotRequest);

if (optionContract.Results != null && stockSnapshot.Ticker?.LastTrade?.Price != null)
{
    var strikePrice = optionContract.Results.StrikePrice ?? 0;
    var currentPrice = stockSnapshot.Ticker.LastTrade.Price;
    var intrinsicValue = Math.Max(0, currentPrice - strikePrice);

    Console.WriteLine($"\nOption Analysis:");
    Console.WriteLine($"Current Stock Price: ${currentPrice}");
    Console.WriteLine($"Strike Price: ${strikePrice}");
    Console.WriteLine($"Intrinsic Value: ${intrinsicValue}");
    Console.WriteLine($"Status: {(intrinsicValue > 0 ? "In the Money" : "Out of the Money")}");
}
```

**Options Ticker Format:** Options tickers follow the OCC (Options Clearing Corporation) format:
- Prefix: `O:` (indicates options)
- Underlying ticker: e.g., `SPY`, `AAPL`, `TSLA`
- Expiration date: `YYMMDD` format
- Contract type: `C` for call, `P` for put
- Strike price: 8 digits with implied decimals (e.g., `00650000` = $650.00)

**Example:** `O:SPY251219C00650000`
- `O:` - Options prefix
- `SPY` - Underlying ticker (SPDR S&P 500 ETF)
- `251219` - Expiration date (December 19, 2025)
- `C` - Call option
- `00650000` - Strike price ($650.00)

### Options Ticker Construction Helpers

Working with OCC format options tickers can be complex and error-prone. The library provides helper classes and extension methods to simplify ticker construction and API calls.

#### OptionsTicker Class - Create and Parse Tickers

The `OptionsTicker` class provides static factory methods for creating and parsing OCC format ticker strings.

```csharp
using TreyThomasCodes.Polygon.Models.Options;

// Create a ticker from components
string callTicker = OptionsTicker.Create(
    underlying: "UBER",
    expirationDate: new DateTime(2022, 1, 21),
    type: OptionType.Call,
    strike: 50m
);
Console.WriteLine(callTicker); // Output: O:UBER220121C00050000

// Create a put option ticker
string putTicker = OptionsTicker.Create(
    underlying: "F",
    expirationDate: new DateTime(2021, 11, 19),
    type: OptionType.Put,
    strike: 14m
);
Console.WriteLine(putTicker); // Output: O:F211119P00014000

// Parse an existing ticker into components
var parsed = OptionsTicker.Parse("O:UBER220121C00050000");
Console.WriteLine($"Underlying: {parsed.Underlying}");      // UBER
Console.WriteLine($"Expiration: {parsed.ExpirationDate}");  // 2022-01-21
Console.WriteLine($"Type: {parsed.Type}");                  // Call
Console.WriteLine($"Strike: ${parsed.Strike}");             // 50

// Try parsing with validation
if (OptionsTicker.TryParse("O:SPY251219C00650000", out var ticker))
{
    Console.WriteLine($"Successfully parsed: Strike ${ticker.Strike}");
}
else
{
    Console.WriteLine("Invalid ticker format");
}

// Create OptionsTicker instance and convert to string
var tickerObj = new OptionsTicker("SPY", new DateTime(2025, 12, 19), OptionType.Call, 650m);
string formattedTicker = tickerObj.ToString(); // O:SPY251219C00650000
```

#### OptionsTickerBuilder - Fluent API

The `OptionsTickerBuilder` class provides a fluent, discoverable API for building options tickers.

```csharp
using TreyThomasCodes.Polygon.Models.Options;

// Build a call option ticker using fluent API
var callTicker = new OptionsTickerBuilder()
    .WithUnderlying("SPY")
    .WithExpiration(2025, 12, 19)
    .AsCall()
    .WithStrike(650m)
    .Build();
Console.WriteLine(callTicker); // O:SPY251219C00650000

// Build a put option ticker
var putTicker = new OptionsTickerBuilder()
    .WithUnderlying("TSLA")
    .WithExpiration(new DateTime(2026, 3, 20))
    .AsPut()
    .WithStrike(700m)
    .Build();
Console.WriteLine(putTicker); // O:TSLA260320P00700000

// Build and get OptionsTicker object instead of string
var tickerObject = new OptionsTickerBuilder()
    .WithUnderlying("AAPL")
    .WithExpiration(2025, 1, 17)
    .WithType(OptionType.Put)
    .WithStrike(150m)
    .BuildTicker(); // Returns OptionsTicker instance

// Reuse builder with Reset()
var builder = new OptionsTickerBuilder();

var ticker1 = builder
    .WithUnderlying("NVDA")
    .WithExpiration(2025, 6, 20)
    .AsCall()
    .WithStrike(800m)
    .Build();

builder.Reset();

var ticker2 = builder
    .WithUnderlying("AMZN")
    .WithExpiration(2025, 9, 19)
    .AsPut()
    .WithStrike(180m)
    .Build();
```

#### Extension Methods - Simplified API Calls

The library provides extension methods that accept components or `OptionsTicker` objects directly, eliminating the need to manually construct ticker strings.

```csharp
using TreyThomasCodes.Polygon.RestClient.Extensions;
using TreyThomasCodes.Polygon.Models.Options;
using TreyThomasCodes.Polygon.Models.Common;

// Method 1: Use components directly
var contract = await _client.Options.GetContractByComponentsAsync(
    underlying: "UBER",
    expirationDate: new DateTime(2022, 1, 21),
    type: OptionType.Call,
    strike: 50m
);

var snapshot = await _client.Options.GetSnapshotByComponentsAsync(
    underlying: "SPY",
    expirationDate: new DateTime(2025, 12, 19),
    type: OptionType.Call,
    strike: 650m
);

var lastTrade = await _client.Options.GetLastTradeByComponentsAsync(
    underlying: "TSLA",
    expirationDate: new DateTime(2026, 3, 20),
    type: OptionType.Call,
    strike: 700m
);

var bars = await _client.Options.GetBarsByComponentsAsync(
    underlying: "SPY",
    expirationDate: new DateTime(2025, 12, 19),
    type: OptionType.Call,
    strike: 650m,
    multiplier: 1,
    timespan: AggregateInterval.Day,
    from: "2025-11-01",
    to: "2025-11-30"
);

var chainSnapshot = await _client.Options.GetChainSnapshotByComponentsAsync(
    underlyingAsset: "SPY",
    type: OptionType.Call,
    expirationDateGte: new DateTime(2025, 12, 1),
    expirationDateLte: new DateTime(2025, 12, 31),
    limit: 100
);

// Method 2: Use OptionsTicker objects for reusability
var ticker = OptionsTicker.Parse("O:SPY251219C00650000");

// All major options API calls support OptionsTicker objects
var contractDetails = await _client.Options.GetContractDetailsAsync(ticker);
var marketSnapshot = await _client.Options.GetSnapshotAsync(ticker);
var trade = await _client.Options.GetLastTradeAsync(ticker);
var quotes = await _client.Options.GetQuotesAsync(ticker, timestamp: "2024-12-01", limit: 100);
var trades = await _client.Options.GetTradesAsync(ticker, timestamp: "2021-09-03", limit: 100);
var dailyBars = await _client.Options.GetBarsAsync(ticker, 1, AggregateInterval.Day, "2025-11-01", "2025-11-30");
var dailyOHLC = await _client.Options.GetDailyOpenCloseAsync(ticker, "2023-01-09");
var previousDay = await _client.Options.GetPreviousDayBarAsync(ticker);

// Method 3: Helper methods for discovery
// Discover available strike prices for an underlying
var strikes = await _client.Options.GetAvailableStrikesAsync(
    underlying: "SPY",
    type: OptionType.Call,
    expirationDateGte: "2025-12-01",
    expirationDateLte: "2025-12-31"
);

foreach (var strike in strikes)
{
    Console.WriteLine($"Available strike: ${strike}");
}

// Discover available expiration dates
var expirations = await _client.Options.GetExpirationDatesAsync(
    underlying: "UBER",
    type: OptionType.Put,
    strikePrice: 50m  // Optional: filter by specific strike
);

foreach (var expiration in expirations)
{
    Console.WriteLine($"Available expiration: {expiration:yyyy-MM-dd}");
}

// Discover expiration dates for a specific strike
var expirationsForStrike = await _client.Options.GetExpirationDatesAsync(
    underlying: "AAPL",
    type: OptionType.Call,
    strikePrice: 180m
);
```

**Benefits of Using Helpers:**
- **Type Safety** - Compile-time validation of parameters
- **Validation** - Automatic validation of ticker components (e.g., strike price can't be negative)
- **Readability** - Clear, self-documenting code
- **Flexibility** - Choose between string tickers, components, or OptionsTicker objects
- **Discovery** - Find available strikes and expiration dates without manual API exploration

### Options Market Data

#### GetSnapshotAsync - Get Options Contract Snapshot

Retrieve a comprehensive snapshot of current market data for an options contract, including Greeks, implied volatility, last trade, last quote, and underlying asset information.

```csharp
using TreyThomasCodes.Polygon.RestClient.Requests.Options;

// Get snapshot for a call option on SPY
var snapshotRequest = new GetSnapshotRequest
{
    UnderlyingAsset = "SPY",
    OptionContract = "SPY251219C00650000"
};
var snapshot = await _client.Options.GetSnapshotAsync(snapshotRequest);

if (snapshot.Results != null)
{
    var data = snapshot.Results;

    // Contract details
    Console.WriteLine("Contract Details:");
    Console.WriteLine($"Ticker: {data.Details?.Ticker}");
    Console.WriteLine($"Type: {data.Details?.ContractType}");
    Console.WriteLine($"Strike: ${data.Details?.StrikePrice}");
    Console.WriteLine($"Expiration: {data.Details?.ExpirationDate}");
    Console.WriteLine($"Style: {data.Details?.ExerciseStyle}");

    // Pricing information
    Console.WriteLine("\nPricing:");
    Console.WriteLine($"Break-Even Price: ${data.BreakEvenPrice}");
    Console.WriteLine($"Implied Volatility: {data.ImpliedVolatility:P2}");
    Console.WriteLine($"Open Interest: {data.OpenInterest}");

    // Daily data
    if (data.Day != null)
    {
        Console.WriteLine("\nDaily Data:");
        Console.WriteLine($"Open: ${data.Day.Open}");
        Console.WriteLine($"High: ${data.Day.High}");
        Console.WriteLine($"Low: ${data.Day.Low}");
        Console.WriteLine($"Close: ${data.Day.Close}");
        Console.WriteLine($"Volume: {data.Day.Volume}");
        Console.WriteLine($"Change: ${data.Day.Change} ({data.Day.ChangePercent:F2}%)");
        Console.WriteLine($"VWAP: ${data.Day.Vwap}");
    }

    // Greeks
    if (data.Greeks != null)
    {
        Console.WriteLine("\nGreeks:");
        Console.WriteLine($"Delta: {data.Greeks.Delta:F4}");
        Console.WriteLine($"Gamma: {data.Greeks.Gamma:F4}");
        Console.WriteLine($"Theta: {data.Greeks.Theta:F4}");
        Console.WriteLine($"Vega: {data.Greeks.Vega:F4}");
    }

    // Last quote
    if (data.LastQuote != null)
    {
        Console.WriteLine("\nLast Quote:");
        Console.WriteLine($"Bid: ${data.LastQuote.Bid} x {data.LastQuote.BidSize} on exchange {data.LastQuote.BidExchange}");
        Console.WriteLine($"Ask: ${data.LastQuote.Ask} x {data.LastQuote.AskSize} on exchange {data.LastQuote.AskExchange}");
        Console.WriteLine($"Midpoint: ${data.LastQuote.Midpoint}");
        Console.WriteLine($"Timeframe: {data.LastQuote.Timeframe}");
    }

    // Last trade
    if (data.LastTrade != null)
    {
        Console.WriteLine("\nLast Trade:");
        Console.WriteLine($"Price: ${data.LastTrade.Price}");
        Console.WriteLine($"Size: {data.LastTrade.Size} contracts");
        Console.WriteLine($"Exchange: {data.LastTrade.Exchange}");
        Console.WriteLine($"Conditions: {string.Join(", ", data.LastTrade.Conditions ?? new List<int>())}");
        Console.WriteLine($"Timeframe: {data.LastTrade.Timeframe}");
    }

    // Underlying asset
    if (data.UnderlyingAsset != null)
    {
        Console.WriteLine("\nUnderlying Asset:");
        Console.WriteLine($"Ticker: {data.UnderlyingAsset.Ticker}");
        Console.WriteLine($"Price: ${data.UnderlyingAsset.Price}");
        Console.WriteLine($"Change to Break-Even: ${data.UnderlyingAsset.ChangeToBreakEven}");
        Console.WriteLine($"Timeframe: {data.UnderlyingAsset.Timeframe}");
    }
}

// Get snapshot for a put option on AAPL
var putSnapshotRequest = new GetSnapshotRequest
{
    UnderlyingAsset = "AAPL",
    OptionContract = "AAPL250117P00150000"
};
var putSnapshot = await _client.Options.GetSnapshotAsync(putSnapshotRequest);

if (putSnapshot.Results != null)
{
    var put = putSnapshot.Results;
    Console.WriteLine($"\n{put.Details?.ContractType?.ToUpper()} Option Analysis:");
    Console.WriteLine($"Current Premium: ${put.Day?.Close}");
    Console.WriteLine($"Intrinsic Value: ${Math.Max(0, (put.Details?.StrikePrice ?? 0) - (put.UnderlyingAsset?.Price ?? 0))}");
    Console.WriteLine($"Time Value: ${(put.Day?.Close ?? 0) - Math.Max(0, (put.Details?.StrikePrice ?? 0) - (put.UnderlyingAsset?.Price ?? 0))}");

    if (put.Greeks?.Delta.HasValue == true)
    {
        Console.WriteLine($"Delta: {put.Greeks.Delta:F4} (Put options have negative delta)");
    }
}

// Example: Compare multiple strike prices
var strikes = new[] { "SPY251219C00600000", "SPY251219C00650000", "SPY251219C00700000" };

Console.WriteLine("\nComparing Strike Prices:");
foreach (var strike in strikes)
{
    var strikeSnapshotRequest = new GetSnapshotRequest
    {
        UnderlyingAsset = "SPY",
        OptionContract = strike
    };
    var optSnapshot = await _client.Options.GetSnapshotAsync(strikeSnapshotRequest);

    if (optSnapshot.Results != null)
    {
        var opt = optSnapshot.Results;
        Console.WriteLine($"\nStrike ${opt.Details?.StrikePrice}:");
        Console.WriteLine($"  Premium: ${opt.Day?.Close}");
        Console.WriteLine($"  Delta: {opt.Greeks?.Delta:F4}");
        Console.WriteLine($"  Gamma: {opt.Greeks?.Gamma:F4}");
        Console.WriteLine($"  IV: {opt.ImpliedVolatility:P2}");
        Console.WriteLine($"  Open Interest: {opt.OpenInterest}");
        Console.WriteLine($"  Volume: {opt.Day?.Volume}");
    }
}

// Example: Calculate probability of profit using delta
var callSnapshotRequest = new GetSnapshotRequest
{
    UnderlyingAsset = "SPY",
    OptionContract = "SPY251219C00650000"
};
var callSnapshot = await _client.Options.GetSnapshotAsync(callSnapshotRequest);

if (callSnapshot.Results?.Greeks?.Delta.HasValue == true)
{
    var delta = callSnapshot.Results.Greeks.Delta.Value;
    var probProfit = Math.Abs(delta) * 100; // Approximation

    Console.WriteLine($"\nProbability Analysis:");
    Console.WriteLine($"Delta: {delta:F4}");
    Console.WriteLine($"Approx. Probability ITM at Expiration: {probProfit:F2}%");
    Console.WriteLine($"Approx. Probability OTM at Expiration: {(100 - probProfit):F2}%");
}
```

**Parameters:**
- `underlyingAsset` - The ticker symbol of the underlying asset (e.g., "SPY", "AAPL")
- `optionContract` - The options contract identifier in OCC format WITHOUT the "O:" prefix (e.g., "SPY251219C00650000")

#### GetChainSnapshotAsync - Get Option Chain Snapshot

Retrieve snapshots for all options contracts for a given underlying asset. Returns comprehensive market data for each contract including Greeks, implied volatility, last trade, last quote, and underlying asset information. Supports filtering and pagination.

```csharp
using TreyThomasCodes.Polygon.RestClient.Requests.Options;

// Get all options contracts for MSTR with a limit
var chainSnapshotRequest = new GetChainSnapshotRequest
{
    UnderlyingAsset = "MSTR",
    Limit = 10
};
var chainSnapshot = await _client.Options.GetChainSnapshotAsync(chainSnapshotRequest);

if (chainSnapshot.Results != null)
{
    Console.WriteLine($"Found {chainSnapshot.Results.Count} contracts");

    foreach (var contract in chainSnapshot.Results)
    {
        Console.WriteLine($"\n{contract.Details?.Ticker}:");
        Console.WriteLine($"  Type: {contract.Details?.ContractType}");
        Console.WriteLine($"  Strike: ${contract.Details?.StrikePrice}");
        Console.WriteLine($"  Expiration: {contract.Details?.ExpirationDate}");
        Console.WriteLine($"  Last Price: ${contract.Day?.Close}");
        Console.WriteLine($"  Volume: {contract.Day?.Volume}");
        Console.WriteLine($"  Open Interest: {contract.OpenInterest}");
        Console.WriteLine($"  IV: {contract.ImpliedVolatility:P2}");

        if (contract.Greeks != null)
        {
            Console.WriteLine($"  Delta: {contract.Greeks.Delta:F4}");
            Console.WriteLine($"  Gamma: {contract.Greeks.Gamma:F6}");
            Console.WriteLine($"  Theta: {contract.Greeks.Theta:F4}");
            Console.WriteLine($"  Vega: {contract.Greeks.Vega:F4}");
        }
    }

    // Check for pagination
    if (!string.IsNullOrEmpty(chainSnapshot.NextUrl))
    {
        Console.WriteLine($"\nMore results available: {chainSnapshot.NextUrl}");
    }
}

// Filter by contract type (calls only)
var callsOnlyRequest = new GetChainSnapshotRequest
{
    UnderlyingAsset = "SPY",
    ContractType = "call",
    Limit = 20
};
var callsOnly = await _client.Options.GetChainSnapshotAsync(callsOnlyRequest);

Console.WriteLine($"\nCall Options for SPY:");
foreach (var call in callsOnly.Results ?? Enumerable.Empty<OptionSnapshot>())
{
    Console.WriteLine($"Strike ${call.Details?.StrikePrice}: ${call.Day?.Close}");
}

// Filter by contract type (puts only)
var putsOnlyRequest = new GetChainSnapshotRequest
{
    UnderlyingAsset = "SPY",
    ContractType = "put",
    Limit = 20
};
var putsOnly = await _client.Options.GetChainSnapshotAsync(putsOnlyRequest);

Console.WriteLine($"\nPut Options for SPY:");
foreach (var put in putsOnly.Results ?? Enumerable.Empty<OptionSnapshot>())
{
    Console.WriteLine($"Strike ${put.Details?.StrikePrice}: ${put.Day?.Close}");
}

// Filter by specific strike price
var strikeSnapshotRequest = new GetChainSnapshotRequest
{
    UnderlyingAsset = "SPY",
    StrikePrice = 650m,
    Limit = 10
};
var strikeSnapshot = await _client.Options.GetChainSnapshotAsync(strikeSnapshotRequest);

Console.WriteLine($"\nAll contracts at $650 strike:");
foreach (var contract in strikeSnapshot.Results ?? Enumerable.Empty<OptionSnapshot>())
{
    Console.WriteLine($"{contract.Details?.Ticker}:");
    Console.WriteLine($"  Type: {contract.Details?.ContractType}");
    Console.WriteLine($"  Expiration: {contract.Details?.ExpirationDate}");
    Console.WriteLine($"  Price: ${contract.Day?.Close}");
}

// Filter by expiration date range
var expirationRangeRequest = new GetChainSnapshotRequest
{
    UnderlyingAsset = "AAPL",
    ExpirationDateGte = "2025-10-01",
    ExpirationDateLte = "2025-12-31",
    Limit = 50
};
var expirationRange = await _client.Options.GetChainSnapshotAsync(expirationRangeRequest);

Console.WriteLine($"\nAAPL options expiring between Oct-Dec 2025:");
foreach (var contract in expirationRange.Results ?? Enumerable.Empty<OptionSnapshot>())
{
    Console.WriteLine($"{contract.Details?.Ticker} expires {contract.Details?.ExpirationDate}");
}

// Sort by strike price in ascending order
var sortedByStrikeRequest = new GetChainSnapshotRequest
{
    UnderlyingAsset = "TSLA",
    ContractType = "call",
    Order = "asc",
    Sort = "strike_price",
    Limit = 20
};
var sortedByStrike = await _client.Options.GetChainSnapshotAsync(sortedByStrikeRequest);

Console.WriteLine($"\nTSLA calls sorted by strike price:");
foreach (var contract in sortedByStrike.Results ?? Enumerable.Empty<OptionSnapshot>())
{
    Console.WriteLine($"Strike ${contract.Details?.StrikePrice}: ${contract.Day?.Close}, Delta: {contract.Greeks?.Delta:F4}");
}

// Combine multiple filters for specific analysis
var nearMoneyPutsRequest = new GetChainSnapshotRequest
{
    UnderlyingAsset = "SPY",
    ContractType = "put",
    ExpirationDateGte = "2025-11-01",
    ExpirationDateLte = "2025-11-30",
    Order = "asc",
    Sort = "strike_price",
    Limit = 50
};
var nearMoneyPuts = await _client.Options.GetChainSnapshotAsync(nearMoneyPutsRequest);

Console.WriteLine($"\nSPY November 2025 puts:");
foreach (var put in nearMoneyPuts.Results ?? Enumerable.Empty<OptionSnapshot>())
{
    if (put.Details?.StrikePrice.HasValue == true && put.UnderlyingAsset?.Price.HasValue == true)
    {
        var distance = put.UnderlyingAsset.Price.Value - put.Details.StrikePrice.Value;
        var status = distance > 0 ? "OTM" : "ITM";

        Console.WriteLine($"Strike ${put.Details.StrikePrice} ({status}):");
        Console.WriteLine($"  Premium: ${put.Day?.Close}");
        Console.WriteLine($"  Delta: {put.Greeks?.Delta:F4}");
        Console.WriteLine($"  Open Interest: {put.OpenInterest}");
        Console.WriteLine($"  Volume: {put.Day?.Volume}");
    }
}

// Example: Find options with high volume
var highVolumeRequest = new GetChainSnapshotRequest
{
    UnderlyingAsset = "NVDA",
    Limit = 100
};
var highVolume = await _client.Options.GetChainSnapshotAsync(highVolumeRequest);

var highVolumeContracts = highVolume.Results?
    .Where(c => c.Day?.Volume.HasValue == true && c.Day.Volume > 1000)
    .OrderByDescending(c => c.Day.Volume)
    .Take(10);

Console.WriteLine($"\nNVDA options with highest volume:");
foreach (var contract in highVolumeContracts ?? Enumerable.Empty<OptionSnapshot>())
{
    Console.WriteLine($"{contract.Details?.Ticker}:");
    Console.WriteLine($"  Type: {contract.Details?.ContractType}");
    Console.WriteLine($"  Strike: ${contract.Details?.StrikePrice}");
    Console.WriteLine($"  Volume: {contract.Day?.Volume}");
    Console.WriteLine($"  Open Interest: {contract.OpenInterest}");
    Console.WriteLine($"  Price: ${contract.Day?.Close}");
}

// Example: Build an options chain display
var optionsChainRequest = new GetChainSnapshotRequest
{
    UnderlyingAsset = "SPY",
    ExpirationDateGte = "2025-12-15",
    ExpirationDateLte = "2025-12-20",
    Order = "asc",
    Sort = "strike_price",
    Limit = 100
};
var optionsChain = await _client.Options.GetChainSnapshotAsync(optionsChainRequest);

// Group by strike price
var chainByStrike = optionsChain.Results?
    .GroupBy(c => c.Details?.StrikePrice)
    .OrderBy(g => g.Key);

Console.WriteLine($"\nSPY Options Chain (Dec 2025):");
Console.WriteLine($"{"Strike",-10} {"Call Bid",-12} {"Call Ask",-12} {"Put Bid",-12} {"Put Ask",-12}");
Console.WriteLine(new string('-', 60));

foreach (var strikeGroup in chainByStrike ?? Enumerable.Empty<IGrouping<decimal?, OptionSnapshot>>())
{
    var call = strikeGroup.FirstOrDefault(c => c.Details?.ContractType == "call");
    var put = strikeGroup.FirstOrDefault(c => c.Details?.ContractType == "put");

    var callBid = call?.LastQuote?.Bid?.ToString("F2") ?? "-";
    var callAsk = call?.LastQuote?.Ask?.ToString("F2") ?? "-";
    var putBid = put?.LastQuote?.Bid?.ToString("F2") ?? "-";
    var putAsk = put?.LastQuote?.Ask?.ToString("F2") ?? "-";

    Console.WriteLine($"{strikeGroup.Key,-10} {callBid,-12} {callAsk,-12} {putBid,-12} {putAsk,-12}");
}

// Example: Calculate implied volatility smile
var ivSmileRequest = new GetChainSnapshotRequest
{
    UnderlyingAsset = "AAPL",
    ContractType = "call",
    ExpirationDateGte = "2025-11-15",
    ExpirationDateLte = "2025-11-20",
    Order = "asc",
    Sort = "strike_price",
    Limit = 50
};
var ivSmile = await _client.Options.GetChainSnapshotAsync(ivSmileRequest);

Console.WriteLine($"\nImplied Volatility Smile:");
Console.WriteLine($"{"Strike",-10} {"IV",-10} {"Delta",-10}");
Console.WriteLine(new string('-', 30));

foreach (var contract in ivSmile.Results ?? Enumerable.Empty<OptionSnapshot>())
{
    if (contract.Details?.StrikePrice.HasValue == true && contract.ImpliedVolatility.HasValue)
    {
        Console.WriteLine($"{contract.Details.StrikePrice,-10} {contract.ImpliedVolatility:P2,-10} {contract.Greeks?.Delta:F4,-10}");
    }
}

// Example: Pagination through large result sets
var allContracts = new List<OptionSnapshot>();
var paginationRequest = new GetChainSnapshotRequest
{
    UnderlyingAsset = "SPY",
    Limit = 100
};
var response = await _client.Options.GetChainSnapshotAsync(paginationRequest);

if (response.Results != null)
{
    allContracts.AddRange(response.Results);
}

// Note: To implement full pagination, you would need to parse the cursor from NextUrl
// and pass it to the cursor parameter in subsequent calls
if (!string.IsNullOrEmpty(response.NextUrl))
{
    Console.WriteLine($"\nMore results available. Use the cursor parameter for pagination.");
    Console.WriteLine($"Next page URL: {response.NextUrl}");
}

Console.WriteLine($"\nRetrieved {allContracts.Count} option contracts");
```

**Parameters:**
- `underlyingAsset` (required) - The ticker symbol of the underlying asset (e.g., "SPY", "AAPL", "MSTR")
- `strikePrice` (optional) - Filter by exact strike price
- `contractType` (optional) - Filter by contract type ("call" or "put")
- `expirationDateGte` (optional) - Filter contracts expiring on or after this date (YYYY-MM-DD format)
- `expirationDateLte` (optional) - Filter contracts expiring on or before this date (YYYY-MM-DD format)
- `limit` (optional) - Maximum number of results to return (varies by plan)
- `order` (optional) - Sort order ("asc" or "desc")
- `sort` (optional) - Field to sort by (e.g., "ticker", "strike_price", "expiration_date")
- `cursor` (optional) - Pagination cursor from previous response's `next_url`

**Response:** Returns a `PolygonResponse<List<OptionSnapshot>>` containing:
- A list of option snapshots, each with the same structure as the individual `GetSnapshotAsync` response
- `NextUrl` property for pagination if more results are available
- Standard response metadata (`Status`, `RequestId`)

**Use Cases:**
- Build complete options chains with calls and puts at all strikes
- Analyze implied volatility across different strikes (volatility smile/skew)
- Find high-volume or high open interest contracts
- Compare options at different expiration dates
- Screen for specific trading opportunities (e.g., ITM/OTM contracts)
- Calculate synthetic positions and spreads
- Monitor the full options market for an underlying asset

#### GetLastTradeAsync - Get Most Recent Option Trade

Retrieve the most recent trade for a specific options contract.

```csharp
using TreyThomasCodes.Polygon.RestClient.Requests.Options;

// Get last trade for a call option on TSLA
var lastTradeRequest = new GetLastTradeRequest
{
    OptionsTicker = "O:TSLA260320C00700000"
};
var lastTrade = await _client.Options.GetLastTradeAsync(lastTradeRequest);

if (lastTrade.Results != null)
{
    var trade = lastTrade.Results;
    Console.WriteLine($"Ticker: {trade.Ticker}");
    Console.WriteLine($"Last trade price: ${trade.Price}");
    Console.WriteLine($"Trade size: {trade.Size} contracts");
    Console.WriteLine($"Exchange: {trade.Exchange}");
    Console.WriteLine($"Timestamp: {trade.Timestamp}");
    Console.WriteLine($"Sequence: {trade.Sequence}");
    Console.WriteLine($"Condition codes: {string.Join(", ", trade.Conditions ?? new List<int>())}");

    // Display market timestamp (converted to Eastern Time)
    if (trade.MarketTimestamp.HasValue)
    {
        Console.WriteLine($"Market time: {trade.MarketTimestamp.Value}");
    }
}

// Get last trade for a put option on SPY
var putTradeRequest = new GetLastTradeRequest
{
    OptionsTicker = "O:SPY251219P00650000"
};
var putTrade = await _client.Options.GetLastTradeAsync(putTradeRequest);

if (putTrade.Results != null)
{
    Console.WriteLine($"\nLast trade for {putTrade.Results.Ticker}:");
    Console.WriteLine($"Price: ${putTrade.Results.Price}");
    Console.WriteLine($"Size: {putTrade.Results.Size}");
}

// Example: Compare last trade prices across different strikes
var strikes = new[]
{
    "O:SPY251219C00600000",
    "O:SPY251219C00650000",
    "O:SPY251219C00700000"
};

Console.WriteLine("\nLast Trade Prices by Strike:");
foreach (var strike in strikes)
{
    var strikeTradeRequest = new GetLastTradeRequest { OptionsTicker = strike };
    var trade = await _client.Options.GetLastTradeAsync(strikeTradeRequest);

    if (trade.Results != null)
    {
        Console.WriteLine($"Strike ${strike.Substring(strike.Length - 8)}: ${trade.Results.Price} (Size: {trade.Results.Size})");
    }
}

// Example: Calculate bid-ask spread using snapshot and last trade
var tradeSnapshotRequest = new GetSnapshotRequest
{
    UnderlyingAsset = "AAPL",
    OptionContract = "AAPL250117C00150000"
};
var snapshot = await _client.Options.GetSnapshotAsync(tradeSnapshotRequest);

var tradeRequest = new GetLastTradeRequest
{
    OptionsTicker = "O:AAPL250117C00150000"
};
var trade = await _client.Options.GetLastTradeAsync(tradeRequest);

if (snapshot.Results?.LastQuote != null && trade.Results != null)
{
    var bid = snapshot.Results.LastQuote.Bid ?? 0;
    var ask = snapshot.Results.LastQuote.Ask ?? 0;
    var last = trade.Results.Price ?? 0;
    var spread = ask - bid;
    var midpoint = (bid + ask) / 2;

    Console.WriteLine($"\nPrice Analysis for {trade.Results.Ticker}:");
    Console.WriteLine($"Bid: ${bid}");
    Console.WriteLine($"Ask: ${ask}");
    Console.WriteLine($"Spread: ${spread}");
    Console.WriteLine($"Midpoint: ${midpoint}");
    Console.WriteLine($"Last trade: ${last}");
    Console.WriteLine($"Trade vs Midpoint: ${last - midpoint}");
}
```

**Parameters:**
- `optionsTicker` (required) - The options ticker symbol in OCC format (e.g., "O:TSLA260320C00700000"). Must include the "O:" prefix.

**Response:** Returns a `PolygonResponse<OptionTrade>` containing:
- `Ticker` - The options ticker symbol
- `Price` - The price at which the trade was executed
- `Size` - The number of contracts traded
- `Exchange` - The exchange where the trade was executed (numeric code)
- `Timestamp` - When the trade was executed (nanoseconds since Unix epoch)
- `Sequence` - Sequence number for ordering trades at the same timestamp
- `Conditions` - List of condition codes that apply to the trade
- `Id` - Unique identifier for the trade
- `MarketTimestamp` - Computed property that converts the timestamp to Eastern Time

**Use Cases:**
- Get the most recent execution price for an options contract
- Monitor real-time options trading activity
- Compare last trade prices across different strikes or expirations
- Analyze trade execution relative to bid/ask spreads
- Track sequential trades for market microstructure analysis
- Validate option pricing models against actual trades

#### GetQuotesAsync - Get Historical Option Quotes

Retrieve historical bid/ask quote data for a specific options contract. Returns tick-level quote data including bid and ask prices, sizes, exchange information, and timestamps. Supports time-based filtering and pagination.

```csharp
using TreyThomasCodes.Polygon.RestClient.Requests.Options;

// Get recent quotes for an option with a limit
var quotesRequest = new GetQuotesRequest
{
    OptionsTicker = "O:SPY241220P00720000",
    Limit = 10
};
var quotes = await _client.Options.GetQuotesAsync(quotesRequest);

if (quotes.Results != null)
{
    Console.WriteLine($"Found {quotes.Results.Count} quotes");

    foreach (var quote in quotes.Results)
    {
        Console.WriteLine($"\nQuote at sequence {quote.SequenceNumber}:");
        Console.WriteLine($"  Bid: ${quote.BidPrice} x {quote.BidSize} on exchange {quote.BidExchange}");
        Console.WriteLine($"  Ask: ${quote.AskPrice} x {quote.AskSize} on exchange {quote.AskExchange}");
        Console.WriteLine($"  Timestamp: {quote.SipTimestamp}");

        // Display market timestamp (converted to Eastern Time)
        if (quote.MarketTimestamp.HasValue)
        {
            Console.WriteLine($"  Market time: {quote.MarketTimestamp.Value}");
        }
    }

    // Check for pagination
    if (!string.IsNullOrEmpty(quotes.NextUrl))
    {
        Console.WriteLine($"\nMore results available: {quotes.NextUrl}");
    }
}

// Get quotes for a specific time range
var timeRangeQuotesRequest = new GetQuotesRequest
{
    OptionsTicker = "O:TSLA260320C00700000",
    Timestamp = "2022-03-07T14:30:00",
    TimestampLte = "2022-03-07T16:00:00",
    Limit = 100
};
var timeRangeQuotes = await _client.Options.GetQuotesAsync(timeRangeQuotesRequest);

Console.WriteLine($"\nQuotes between 2:30 PM and 4:00 PM:");
foreach (var quote in timeRangeQuotes.Results ?? Enumerable.Empty<OptionQuote>())
{
    var spread = (quote.AskPrice ?? 0) - (quote.BidPrice ?? 0);
    Console.WriteLine($"Bid: ${quote.BidPrice}, Ask: ${quote.AskPrice}, Spread: ${spread}");
}

// Get quotes after a specific timestamp
var quotesAfterRequest = new GetQuotesRequest
{
    OptionsTicker = "O:AAPL250117C00150000",
    TimestampGt = "2022-03-07",
    Order = "asc",
    Limit = 50
};
var quotesAfter = await _client.Options.GetQuotesAsync(quotesAfterRequest);

// Get quotes before a specific timestamp
var quotesBeforeRequest = new GetQuotesRequest
{
    OptionsTicker = "O:MSFT250321P00400000",
    TimestampLt = "2022-03-08",
    Order = "desc",
    Limit = 50
};
var quotesBefore = await _client.Options.GetQuotesAsync(quotesBeforeRequest);

// Get quotes in a specific range with sorting
var sortedQuotesRequest = new GetQuotesRequest
{
    OptionsTicker = "O:SPY241220P00720000",
    TimestampGte = "2022-03-07T09:30:00",
    TimestampLte = "2022-03-07T16:00:00",
    Order = "asc",
    Sort = "timestamp",
    Limit = 1000
};
var sortedQuotes = await _client.Options.GetQuotesAsync(sortedQuotesRequest);

Console.WriteLine($"\nQuotes sorted by timestamp:");
foreach (var quote in sortedQuotes.Results ?? Enumerable.Empty<OptionQuote>())
{
    Console.WriteLine($"{quote.SipTimestamp}: Bid ${quote.BidPrice} x {quote.BidSize}, Ask ${quote.AskPrice} x {quote.AskSize}");
}

// Example: Calculate average bid-ask spread
if (quotes.Results?.Count > 0)
{
    var spreads = quotes.Results
        .Where(q => q.AskPrice.HasValue && q.BidPrice.HasValue)
        .Select(q => q.AskPrice!.Value - q.BidPrice!.Value);

    var avgSpread = spreads.Average();
    var minSpread = spreads.Min();
    var maxSpread = spreads.Max();

    Console.WriteLine($"\nSpread Analysis:");
    Console.WriteLine($"  Average: ${avgSpread:F2}");
    Console.WriteLine($"  Minimum: ${minSpread:F2}");
    Console.WriteLine($"  Maximum: ${maxSpread:F2}");
}

// Example: Track quote updates over time
var quoteHistoryRequest = new GetQuotesRequest
{
    OptionsTicker = "O:SPY241220P00720000",
    TimestampGte = "2022-03-07T14:00:00",
    TimestampLte = "2022-03-07T14:05:00",
    Order = "asc",
    Limit = 100
};
var quoteHistory = await _client.Options.GetQuotesAsync(quoteHistoryRequest);

Console.WriteLine($"\nQuote changes over 5 minutes:");
OptionQuote? previousQuote = null;
foreach (var quote in quoteHistory.Results ?? Enumerable.Empty<OptionQuote>())
{
    if (previousQuote != null)
    {
        var bidChange = (quote.BidPrice ?? 0) - (previousQuote.BidPrice ?? 0);
        var askChange = (quote.AskPrice ?? 0) - (previousQuote.AskPrice ?? 0);

        if (bidChange != 0 || askChange != 0)
        {
            Console.WriteLine($"Sequence {quote.SequenceNumber}:");
            Console.WriteLine($"  Bid change: ${bidChange:F2}");
            Console.WriteLine($"  Ask change: ${askChange:F2}");
        }
    }
    previousQuote = quote;
}

// Example: Find best bid and ask across time
var allQuotesRequest = new GetQuotesRequest
{
    OptionsTicker = "O:AAPL250117C00150000",
    TimestampGte = "2022-03-07",
    Limit = 1000
};
var allQuotes = await _client.Options.GetQuotesAsync(allQuotesRequest);

if (allQuotes.Results?.Count > 0)
{
    var bestBid = allQuotes.Results
        .Where(q => q.BidPrice.HasValue)
        .MaxBy(q => q.BidPrice!.Value);

    var bestAsk = allQuotes.Results
        .Where(q => q.AskPrice.HasValue)
        .MinBy(q => q.AskPrice!.Value);

    Console.WriteLine($"\nBest prices observed:");
    Console.WriteLine($"  Best Bid: ${bestBid?.BidPrice} x {bestBid?.BidSize} (Seq: {bestBid?.SequenceNumber})");
    Console.WriteLine($"  Best Ask: ${bestAsk?.AskPrice} x {bestAsk?.AskSize} (Seq: {bestAsk?.SequenceNumber})");
}

// Example: Pagination through large result sets
var allOptionQuotes = new List<OptionQuote>();
var quotesPaginationRequest = new GetQuotesRequest
{
    OptionsTicker = "O:SPY241220P00720000",
    TimestampGte = "2022-03-07",
    Limit = 100
};
var response = await _client.Options.GetQuotesAsync(quotesPaginationRequest);

if (response.Results != null)
{
    allOptionQuotes.AddRange(response.Results);
}

// Note: To implement full pagination, you would need to parse the cursor from NextUrl
// and pass it to the cursor parameter in subsequent calls
if (!string.IsNullOrEmpty(response.NextUrl))
{
    Console.WriteLine($"\nMore results available. Use the cursor parameter for pagination.");
    Console.WriteLine($"Next page URL: {response.NextUrl}");
}

Console.WriteLine($"\nRetrieved {allOptionQuotes.Count} option quotes");
```

**Parameters:**
- `optionsTicker` (required) - The options ticker symbol in OCC format (e.g., "O:SPY241220P00720000"). Must include the "O:" prefix.
- `timestamp` (optional) - Query for quotes at or after this timestamp. Can be a date (YYYY-MM-DD), datetime (YYYY-MM-DDTHH:MM:SS), or nanosecond timestamp.
- `timestampLt` (optional) - Query for quotes before this timestamp
- `timestampLte` (optional) - Query for quotes at or before this timestamp
- `timestampGt` (optional) - Query for quotes after this timestamp
- `timestampGte` (optional) - Query for quotes at or after this timestamp
- `order` (optional) - Sort order ("asc" or "desc") by timestamp
- `limit` (optional) - Maximum number of results to return (varies by plan)
- `sort` (optional) - Sort field (defaults to "timestamp")
- `cursor` (optional) - Pagination cursor from previous response's `next_url`

**Response:** Returns a `PolygonResponse<List<OptionQuote>>` containing:
- A list of option quotes with the following properties per quote:
  - `AskPrice` - The ask price for the option
  - `AskSize` - The number of contracts offered at the ask price
  - `AskExchange` - The exchange offering the ask (numeric code)
  - `BidPrice` - The bid price for the option
  - `BidSize` - The number of contracts bid for at the bid price
  - `BidExchange` - The exchange offering the bid (numeric code)
  - `SequenceNumber` - Sequence number for ordering quotes at the same timestamp
  - `SipTimestamp` - When the quote was generated (nanoseconds since Unix epoch)
  - `MarketTimestamp` - Computed property that converts SipTimestamp to Eastern Time
- `NextUrl` property for pagination if more results are available
- Standard response metadata (`Status`, `RequestId`)

**Use Cases:**
- Monitor historical bid/ask spreads for options contracts
- Analyze market liquidity and quote depth over time
- Track quote changes and market maker activity
- Calculate time-weighted average bid/ask spreads
- Identify periods of wide or narrow spreads
- Build order book reconstructions for specific time periods
- Compare quote patterns across different strike prices or expirations
- Detect and analyze quote volatility and market microstructure

#### GetTradesAsync - Get Historical Option Trades

Retrieve historical trade data for a specific options contract. Returns tick-level trade data including price, size, exchange, conditions, and timestamps. Supports time-based filtering and pagination.

```csharp
using TreyThomasCodes.Polygon.RestClient.Requests.Options;

// Get recent trades for an option with a limit
var tradesRequest = new GetTradesRequest
{
    OptionsTicker = "O:TSLA210903C00700000",
    Limit = 10
};
var trades = await _client.Options.GetTradesAsync(tradesRequest);

if (trades.Results != null)
{
    Console.WriteLine($"Found {trades.Results.Count} trades");

    foreach (var trade in trades.Results)
    {
        Console.WriteLine($"\nTrade:");
        Console.WriteLine($"  Price: ${trade.Price}");
        Console.WriteLine($"  Size: {trade.Size} contracts");
        Console.WriteLine($"  Exchange: {trade.Exchange}");
        Console.WriteLine($"  Sequence: {trade.SequenceNumber}");
        Console.WriteLine($"  SIP Timestamp: {trade.SipTimestamp}");

        // Display market timestamp (converted to Eastern Time)
        if (trade.MarketTimestamp.HasValue)
        {
            Console.WriteLine($"  Market time: {trade.MarketTimestamp.Value}");
        }

        // Display participant timestamp
        if (trade.MarketParticipantTimestamp.HasValue)
        {
            Console.WriteLine($"  Participant time: {trade.MarketParticipantTimestamp.Value}");
        }

        // Display condition codes
        if (trade.Conditions != null && trade.Conditions.Count > 0)
        {
            Console.WriteLine($"  Conditions: {string.Join(", ", trade.Conditions)}");
        }
    }

    // Check for pagination
    if (!string.IsNullOrEmpty(trades.NextUrl))
    {
        Console.WriteLine($"\nMore results available: {trades.NextUrl}");
    }
}

// Get trades for a specific time range
var timeRangeTradesRequest = new GetTradesRequest
{
    OptionsTicker = "O:TSLA210903C00700000",
    Timestamp = "2021-07-23T15:42:22",
    TimestampLte = "2021-07-23T16:00:00",
    Limit = 100
};
var timeRangeTrades = await _client.Options.GetTradesAsync(timeRangeTradesRequest);

Console.WriteLine($"\nTrades between 3:42 PM and 4:00 PM:");
foreach (var trade in timeRangeTrades.Results ?? Enumerable.Empty<OptionTradeV3>())
{
    Console.WriteLine($"${trade.Price} x {trade.Size} contracts @ {trade.MarketTimestamp}");
}

// Get trades after a specific timestamp
var tradesAfterRequest = new GetTradesRequest
{
    OptionsTicker = "O:SPY241220P00720000",
    TimestampGt = "2021-07-23",
    Order = "asc",
    Limit = 50
};
var tradesAfter = await _client.Options.GetTradesAsync(tradesAfterRequest);

// Get trades before a specific timestamp
var tradesBeforeRequest = new GetTradesRequest
{
    OptionsTicker = "O:AAPL250117C00150000",
    TimestampLt = "2021-07-24",
    Order = "desc",
    Limit = 50
};
var tradesBefore = await _client.Options.GetTradesAsync(tradesBeforeRequest);

// Get trades in a specific range with sorting
var sortedTradesRequest = new GetTradesRequest
{
    OptionsTicker = "O:TSLA210903C00700000",
    TimestampGte = "2021-07-23T09:30:00",
    TimestampLte = "2021-07-23T16:00:00",
    Order = "asc",
    Sort = "timestamp",
    Limit = 1000
};
var sortedTrades = await _client.Options.GetTradesAsync(sortedTradesRequest);

Console.WriteLine($"\nTrades sorted by timestamp:");
foreach (var trade in sortedTrades.Results ?? Enumerable.Empty<OptionTradeV3>())
{
    Console.WriteLine($"{trade.SipTimestamp}: ${trade.Price} x {trade.Size} contracts on exchange {trade.Exchange}");
}

// Example: Calculate volume-weighted average price (VWAP)
if (trades.Results?.Count > 0)
{
    var totalValue = trades.Results
        .Where(t => t.Price.HasValue && t.Size.HasValue)
        .Sum(t => t.Price!.Value * t.Size!.Value);

    var totalVolume = trades.Results
        .Where(t => t.Size.HasValue)
        .Sum(t => t.Size!.Value);

    if (totalVolume > 0)
    {
        var vwap = totalValue / totalVolume;
        Console.WriteLine($"\nVWAP: ${vwap:F2}");
        Console.WriteLine($"Total Volume: {totalVolume} contracts");
        Console.WriteLine($"Total Value: ${totalValue:F2}");
    }
}

// Example: Analyze trade sizes
if (trades.Results?.Count > 0)
{
    var sizes = trades.Results
        .Where(t => t.Size.HasValue)
        .Select(t => t.Size!.Value);

    Console.WriteLine($"\nTrade Size Analysis:");
    Console.WriteLine($"  Average: {sizes.Average():F2} contracts");
    Console.WriteLine($"  Minimum: {sizes.Min()} contracts");
    Console.WriteLine($"  Maximum: {sizes.Max()} contracts");
}

// Example: Track price movement over time
var priceHistoryRequest = new GetTradesRequest
{
    OptionsTicker = "O:TSLA210903C00700000",
    TimestampGte = "2021-07-23T14:00:00",
    TimestampLte = "2021-07-23T14:05:00",
    Order = "asc",
    Limit = 100
};
var priceHistory = await _client.Options.GetTradesAsync(priceHistoryRequest);

Console.WriteLine($"\nPrice movement over 5 minutes:");
OptionTradeV3? previousTrade = null;
foreach (var trade in priceHistory.Results ?? Enumerable.Empty<OptionTradeV3>())
{
    if (previousTrade != null && trade.Price.HasValue && previousTrade.Price.HasValue)
    {
        var priceChange = trade.Price.Value - previousTrade.Price.Value;

        if (priceChange != 0)
        {
            Console.WriteLine($"Sequence {trade.SequenceNumber}:");
            Console.WriteLine($"  Price: ${trade.Price:F2}");
            Console.WriteLine($"  Change: ${priceChange:F2}");
        }
    }
    previousTrade = trade;
}

// Example: Find largest trade
var allTradesRequest = new GetTradesRequest
{
    OptionsTicker = "O:TSLA210903C00700000",
    TimestampGte = "2021-07-23",
    Limit = 1000
};
var allTrades = await _client.Options.GetTradesAsync(allTradesRequest);

if (allTrades.Results?.Count > 0)
{
    var largestTrade = allTrades.Results
        .Where(t => t.Size.HasValue)
        .MaxBy(t => t.Size!.Value);

    Console.WriteLine($"\nLargest trade:");
    Console.WriteLine($"  Size: {largestTrade?.Size} contracts");
    Console.WriteLine($"  Price: ${largestTrade?.Price}");
    Console.WriteLine($"  Value: ${(largestTrade?.Price ?? 0) * (largestTrade?.Size ?? 0)}");
}

// Example: Pagination through large result sets
var allOptionTrades = new List<OptionTradeV3>();
var tradesPaginationRequest = new GetTradesRequest
{
    OptionsTicker = "O:TSLA210903C00700000",
    TimestampGte = "2021-07-23",
    Limit = 100
};
var response = await _client.Options.GetTradesAsync(tradesPaginationRequest);

if (response.Results != null)
{
    allOptionTrades.AddRange(response.Results);
}

// Note: To implement full pagination, you would need to parse the cursor from NextUrl
// and pass it to the cursor parameter in subsequent calls
if (!string.IsNullOrEmpty(response.NextUrl))
{
    Console.WriteLine($"\nMore results available. Use the cursor parameter for pagination.");
    Console.WriteLine($"Next page URL: {response.NextUrl}");
}

Console.WriteLine($"\nRetrieved {allOptionTrades.Count} option trades");
```

**Parameters:**
- `optionsTicker` (required) - The options ticker symbol in OCC format (e.g., "O:TSLA210903C00700000"). Must include the "O:" prefix.
- `timestamp` (optional) - Query for trades at or after this timestamp. Can be a date (YYYY-MM-DD), datetime (YYYY-MM-DDTHH:MM:SS), or nanosecond timestamp.
- `timestampLt` (optional) - Query for trades before this timestamp
- `timestampLte` (optional) - Query for trades at or before this timestamp
- `timestampGt` (optional) - Query for trades after this timestamp
- `timestampGte` (optional) - Query for trades at or after this timestamp
- `order` (optional) - Sort order ("asc" or "desc") by timestamp
- `limit` (optional) - Maximum number of results to return (varies by plan)
- `sort` (optional) - Sort field (defaults to "timestamp")
- `cursor` (optional) - Pagination cursor from previous response's `next_url`

**Response:** Returns a `PolygonResponse<List<OptionTradeV3>>` containing:
- A list of option trades with the following properties per trade:
  - `Price` - The price at which the trade was executed (per contract)
  - `Size` - The number of contracts traded
  - `Exchange` - The exchange where the trade was executed (numeric code)
  - `Conditions` - List of condition codes providing additional context about the trade
  - `SipTimestamp` - When the trade was reported to the SIP (nanoseconds since Unix epoch)
  - `ParticipantTimestamp` - When the trade was reported by the exchange participant (nanoseconds since Unix epoch)
  - `SequenceNumber` - Sequence number for ordering trades at the same timestamp
  - `Id` - Trade identifier
  - `MarketTimestamp` - Computed property that converts SipTimestamp to Eastern Time
  - `MarketParticipantTimestamp` - Computed property that converts ParticipantTimestamp to Eastern Time
- `NextUrl` property for pagination if more results are available
- Standard response metadata (`Status`, `RequestId`)

**Use Cases:**
- Analyze historical trading activity for options contracts
- Calculate volume-weighted average price (VWAP) for options
- Track trade execution patterns and size distribution
- Identify large block trades or unusual trading activity
- Monitor price movement and volatility throughout the trading day
- Compare trade execution quality across exchanges
- Build time-and-sales data for options contracts
- Analyze market microstructure and trade flow

#### GetBarsAsync - Get Historical Option Bars

Retrieve aggregate OHLC (bar/candle) data for an options contract over a specified time range. Returns historical pricing data aggregated by the specified time interval, useful for charting and technical analysis.

```csharp
using TreyThomasCodes.Polygon.Models.Common;
using TreyThomasCodes.Polygon.RestClient.Requests.Options;

// Get daily bars for an SPY call option
var dailyBarsRequest = new GetBarsRequest
{
    OptionsTicker = "O:SPY251219C00650000",
    Multiplier = 1,
    Timespan = AggregateInterval.Day,
    From = "2023-01-09",
    To = "2023-02-10"
};
var dailyBars = await _client.Options.GetBarsAsync(dailyBarsRequest);

if (dailyBars.Results != null)
{
    Console.WriteLine($"Found {dailyBars.Results.Count} bars");
    Console.WriteLine($"Ticker: {dailyBars.Ticker}");
    Console.WriteLine($"Adjusted: {dailyBars.Adjusted}");

    foreach (var bar in dailyBars.Results)
    {
        Console.WriteLine($"\nBar at {bar.Timestamp}:");
        Console.WriteLine($"  Open: ${bar.Open}");
        Console.WriteLine($"  High: ${bar.High}");
        Console.WriteLine($"  Low: ${bar.Low}");
        Console.WriteLine($"  Close: ${bar.Close}");
        Console.WriteLine($"  Volume: {bar.Volume} contracts");
        Console.WriteLine($"  VWAP: ${bar.VolumeWeightedAveragePrice}");
        Console.WriteLine($"  Transactions: {bar.NumberOfTransactions}");

        // Display market timestamp (converted to Eastern Time)
        if (bar.MarketTimestamp.HasValue)
        {
            Console.WriteLine($"  Market time: {bar.MarketTimestamp.Value}");
        }
    }
}

// Get 5-minute intraday bars
var intradayBarsRequest = new GetBarsRequest
{
    OptionsTicker = "O:TSLA260320C00700000",
    Multiplier = 5,
    Timespan = AggregateInterval.Minute,
    From = "2023-01-09",
    To = "2023-01-10",
    Limit = 100
};
var intradayBars = await _client.Options.GetBarsAsync(intradayBarsRequest);

Console.WriteLine($"\n5-minute intraday bars:");
foreach (var bar in intradayBars.Results ?? Enumerable.Empty<OptionBar>())
{
    Console.WriteLine($"{bar.MarketTimestamp}: O=${bar.Open}, H=${bar.High}, L=${bar.Low}, C=${bar.Close}, V={bar.Volume}");
}

// Get hourly bars
var hourlyBarsRequest = new GetBarsRequest
{
    OptionsTicker = "O:AAPL250117C00150000",
    Multiplier = 1,
    Timespan = AggregateInterval.Hour,
    From = "2023-01-09",
    To = "2023-01-13"
};
var hourlyBars = await _client.Options.GetBarsAsync(hourlyBarsRequest);

// Get weekly bars for long-term analysis
var weeklyBarsRequest = new GetBarsRequest
{
    OptionsTicker = "O:SPY251219C00650000",
    Multiplier = 1,
    Timespan = AggregateInterval.Week,
    From = "2023-01-01",
    To = "2023-03-31"
};
var weeklyBars = await _client.Options.GetBarsAsync(weeklyBarsRequest);

// With optional parameters
var customBarsRequest = new GetBarsRequest
{
    OptionsTicker = "O:SPY251219C00650000",
    Multiplier = 1,
    Timespan = AggregateInterval.Day,
    From = "2023-01-09",
    To = "2023-02-10",
    Adjusted = true,
    Sort = SortOrder.Ascending,
    Limit = 50
};
var customBars = await _client.Options.GetBarsAsync(customBarsRequest);

Console.WriteLine($"\nCustom bars with sorting and limit:");
foreach (var bar in customBars.Results ?? Enumerable.Empty<OptionBar>())
{
    Console.WriteLine($"Date: {bar.MarketTimestamp}, Close: ${bar.Close}, Volume: {bar.Volume}");
}

// Example: Calculate price movement and volatility
if (dailyBars.Results?.Count > 1)
{
    var firstBar = dailyBars.Results.First();
    var lastBar = dailyBars.Results.Last();

    if (firstBar.Close.HasValue && lastBar.Close.HasValue)
    {
        var priceChange = lastBar.Close.Value - firstBar.Close.Value;
        var percentChange = (priceChange / firstBar.Close.Value) * 100;

        Console.WriteLine($"\nPrice Movement Analysis:");
        Console.WriteLine($"Start Price: ${firstBar.Close}");
        Console.WriteLine($"End Price: ${lastBar.Close}");
        Console.WriteLine($"Change: ${priceChange:F2} ({percentChange:F2}%)");
    }

    // Calculate average daily volume
    var avgVolume = dailyBars.Results
        .Where(b => b.Volume.HasValue)
        .Average(b => (double)b.Volume!.Value);

    Console.WriteLine($"Average Daily Volume: {avgVolume:F0} contracts");

    // Calculate price volatility (standard deviation of daily returns)
    var returns = new List<decimal>();
    for (int i = 1; i < dailyBars.Results.Count; i++)
    {
        if (dailyBars.Results[i].Close.HasValue && dailyBars.Results[i - 1].Close.HasValue)
        {
            var dailyReturn = (dailyBars.Results[i].Close!.Value - dailyBars.Results[i - 1].Close!.Value)
                            / dailyBars.Results[i - 1].Close!.Value;
            returns.Add(dailyReturn);
        }
    }

    if (returns.Count > 0)
    {
        var avgReturn = returns.Average();
        var variance = returns.Sum(r => Math.Pow((double)(r - avgReturn), 2)) / returns.Count;
        var stdDev = Math.Sqrt(variance);

        Console.WriteLine($"\nVolatility Analysis:");
        Console.WriteLine($"Avg Daily Return: {avgReturn:P2}");
        Console.WriteLine($"Std Dev: {stdDev:P2}");
        Console.WriteLine($"Annualized Volatility: {stdDev * Math.Sqrt(252):P2}");
    }
}

// Example: Find highest and lowest prices
if (dailyBars.Results?.Count > 0)
{
    var highestBar = dailyBars.Results
        .Where(b => b.High.HasValue)
        .OrderByDescending(b => b.High!.Value)
        .FirstOrDefault();

    var lowestBar = dailyBars.Results
        .Where(b => b.Low.HasValue)
        .OrderBy(b => b.Low!.Value)
        .FirstOrDefault();

    Console.WriteLine($"\nPrice Range:");
    Console.WriteLine($"Highest: ${highestBar?.High} on {highestBar?.MarketTimestamp}");
    Console.WriteLine($"Lowest: ${lowestBar?.Low} on {lowestBar?.MarketTimestamp}");
}

// Example: Calculate total trading volume
if (dailyBars.Results?.Count > 0)
{
    var totalVolume = dailyBars.Results
        .Where(b => b.Volume.HasValue)
        .Sum(b => (decimal)b.Volume!.Value);

    var avgVwap = dailyBars.Results
        .Where(b => b.VolumeWeightedAveragePrice.HasValue && b.Volume.HasValue)
        .Sum(b => b.VolumeWeightedAveragePrice!.Value * b.Volume!.Value) / totalVolume;

    Console.WriteLine($"\nTrading Statistics:");
    Console.WriteLine($"Total Volume: {totalVolume} contracts");
    Console.WriteLine($"Average VWAP: ${avgVwap:F2}");
}

// Example: Compare multiple option contracts
var strikes = new[] { "O:SPY251219C00600000", "O:SPY251219C00650000", "O:SPY251219C00700000" };

Console.WriteLine($"\nComparing Multiple Strikes:");
foreach (var strike in strikes)
{
    var barsRequest = new GetBarsRequest
    {
        OptionsTicker = strike,
        Multiplier = 1,
        Timespan = AggregateInterval.Day,
        From = "2023-01-09",
        To = "2023-02-10"
    };
    var bars = await _client.Options.GetBarsAsync(barsRequest);

    if (bars.Results?.Count > 0)
    {
        var lastBar = bars.Results.Last();
        var avgVolume = bars.Results.Average(b => (double)(b.Volume ?? 0));

        Console.WriteLine($"\n{bars.Ticker}:");
        Console.WriteLine($"  Last Close: ${lastBar.Close}");
        Console.WriteLine($"  Avg Volume: {avgVolume:F0} contracts");
        Console.WriteLine($"  Bars: {bars.Results.Count}");
    }
}
```

**Parameters:**
- `optionsTicker` (required) - The options ticker symbol in OCC format (e.g., "O:SPY251219C00650000"). Must include the "O:" prefix.
- `multiplier` (required) - The number of timespan units to aggregate (e.g., 1 for 1 day, 5 for 5 minutes, 15 for 15 minutes)
- `timespan` (required) - The size of the time window for each aggregate (minute, hour, day, week, month, quarter, year)
- `from` (required) - Start date for the aggregate window in YYYY-MM-DD format
- `to` (required) - End date for the aggregate window in YYYY-MM-DD format
- `adjusted` (optional) - Whether to adjust for splits. Defaults to true if not specified. Note that options contracts are not typically adjusted for underlying stock splits.
- `sort` (optional) - Sort order for results (asc for ascending, desc for descending by timestamp)
- `limit` (optional) - Limit the number of aggregate results returned. Maximum value varies by plan.

**Response:** Returns a `PolygonResponse<List<OptionBar>>` containing:
- A list of option bars with the following properties per bar:
  - `Volume` - The number of contracts traded during the aggregate window
  - `VolumeWeightedAveragePrice` - The average price weighted by volume
  - `Open` - The opening price for the time period
  - `Close` - The closing price for the time period
  - `High` - The highest price reached during the time period
  - `Low` - The lowest price reached during the time period
  - `Timestamp` - The timestamp for the start of the aggregate window (milliseconds since Unix epoch)
  - `NumberOfTransactions` - The count of individual trades aggregated into this bar
  - `MarketTimestamp` - Computed property that converts the timestamp to Eastern Time
- Response metadata including:
  - `Ticker` - The options ticker symbol
  - `QueryCount` - Number of queries made
  - `ResultsCount` - Number of results returned
  - `Adjusted` - Whether the results are adjusted
  - `Count` - Total count of results
  - `Status` - Response status ("OK" for success)
  - `RequestId` - Unique request identifier

**Use Cases:**
- Build candlestick charts for options contracts
- Analyze historical price movements and trends
- Calculate technical indicators (moving averages, Bollinger Bands, etc.)
- Monitor options premium changes over time
- Compare price action across different strike prices or expirations
- Identify support and resistance levels
- Calculate historical volatility from price bars
- Backtest options trading strategies
- Track volume patterns and liquidity
- Analyze intraday trading patterns with minute or hour bars
- Monitor long-term trends with weekly or monthly bars

**Note:** For real-time or near real-time data, consider using the GetTradesAsync or GetQuotesAsync endpoints. The bars endpoint provides aggregated historical data which is ideal for charting and technical analysis.

#### GetPreviousDayBarAsync - Get Previous Trading Day Bar

Retrieve the previous trading day's OHLC bar data for a specific options contract. Returns the most recent completed trading session's open, high, low, close, volume, and volume-weighted average price data. This is useful for calculating daily price changes and percentage movements.

```csharp
using TreyThomasCodes.Polygon.RestClient.Requests.Options;

// Get previous day bar data for an SPY call option
var previousDayBarRequest = new GetPreviousDayBarRequest
{
    OptionsTicker = "O:SPY251219C00650000"
};
var previousDayBar = await _client.Options.GetPreviousDayBarAsync(previousDayBarRequest);

if (previousDayBar?.Results != null && previousDayBar.Results.Count > 0)
{
    var bar = previousDayBar.Results[0];
    Console.WriteLine($"Ticker: {previousDayBar.Ticker}");
    Console.WriteLine($"Adjusted: {previousDayBar.Adjusted}");
    Console.WriteLine($"\nPrevious Day Summary:");
    Console.WriteLine($"  Open: ${bar.Open}");
    Console.WriteLine($"  High: ${bar.High}");
    Console.WriteLine($"  Low: ${bar.Low}");
    Console.WriteLine($"  Close: ${bar.Close}");
    Console.WriteLine($"  Volume: {bar.Volume} contracts");
    Console.WriteLine($"  VWAP: ${bar.VolumeWeightedAveragePrice}");
    Console.WriteLine($"  Transactions: {bar.NumberOfTransactions}");

    // Calculate daily price change
    if (bar.Open.HasValue && bar.Close.HasValue)
    {
        var change = bar.Close.Value - bar.Open.Value;
        var changePercent = (change / bar.Open.Value) * 100;
        Console.WriteLine($"\nDaily Change: ${change:F2} ({changePercent:F2}%)");
    }

    // Convert timestamp to readable date
    if (bar.MarketTimestamp.HasValue)
    {
        Console.WriteLine($"Date: {bar.MarketTimestamp.Value.ToString("yyyy-MM-dd", null)}");
    }
}

// Get previous day bar data with adjusted parameter
var adjustedBarRequest = new GetPreviousDayBarRequest
{
    OptionsTicker = "O:AAPL250117P00150000",
    Adjusted = true
};
var adjustedBar = await _client.Options.GetPreviousDayBarAsync(adjustedBarRequest);

if (adjustedBar?.Results != null && adjustedBar.Results.Count > 0)
{
    var bar = adjustedBar.Results[0];
    Console.WriteLine($"\n{adjustedBar.Ticker} Previous Day:");
    Console.WriteLine($"Close: ${bar.Close} | Volume: {bar.Volume} | VWAP: ${bar.VolumeWeightedAveragePrice}");
}

// Example: Compare multiple options contracts
var optionTickers = new[]
{
    "O:SPY251219C00650000",
    "O:SPY251219C00700000",
    "O:SPY251219C00750000"
};

Console.WriteLine("\nPrevious Day Performance Comparison:");
foreach (var ticker in optionTickers)
{
    try
    {
        var previousDayRequest = new GetPreviousDayBarRequest { OptionsTicker = ticker };
        var previousDay = await _client.Options.GetPreviousDayBarAsync(previousDayRequest);

        if (previousDay?.Results != null && previousDay.Results.Count > 0)
        {
            var bar = previousDay.Results[0];
            if (bar.Open.HasValue && bar.Close.HasValue)
            {
                var changePercent = ((bar.Close.Value - bar.Open.Value) / bar.Open.Value) * 100;
                Console.WriteLine($"{ticker}: {changePercent:F2}% | Vol: {bar.Volume}");
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"{ticker}: Error - {ex.Message}");
    }
}
```

**Common Use Cases:**
- Calculate daily price changes for options contracts
- Monitor previous day's trading activity
- Compare performance across multiple strike prices or expirations
- Quick access to yesterday's closing prices for algorithmic trading
- Track volume and liquidity from the previous trading session

**Note:** This endpoint returns the most recent completed trading day's data. For specific historical dates, use GetDailyOpenCloseAsync instead.

#### GetDailyOpenCloseAsync - Get Daily Open/Close Summary

Retrieve the daily open, high, low, close (OHLC) summary for a specific options contract on a given date. Returns comprehensive daily trading data including pre-market and after-hours prices.

```csharp
using TreyThomasCodes.Polygon.RestClient.Requests.Options;

// Get daily open/close data for an SPY call option
var dailyDataRequest = new GetDailyOpenCloseRequest
{
    OptionsTicker = "O:SPY251219C00650000",
    Date = "2023-01-09"
};
var dailyData = await _client.Options.GetDailyOpenCloseAsync(dailyDataRequest);

if (dailyData != null)
{
    Console.WriteLine($"Symbol: {dailyData.Symbol}");
    Console.WriteLine($"Date: {dailyData.From}");
    Console.WriteLine($"\nDaily Summary:");
    Console.WriteLine($"  Open: ${dailyData.Open}");
    Console.WriteLine($"  High: ${dailyData.High}");
    Console.WriteLine($"  Low: ${dailyData.Low}");
    Console.WriteLine($"  Close: ${dailyData.Close}");
    Console.WriteLine($"  Volume: {dailyData.Volume} contracts");

    if (dailyData.PreMarket.HasValue)
    {
        Console.WriteLine($"  Pre-Market: ${dailyData.PreMarket}");
    }

    if (dailyData.AfterHours.HasValue)
    {
        Console.WriteLine($"  After-Hours: ${dailyData.AfterHours}");
    }

    // Calculate daily price change
    if (dailyData.Open.HasValue && dailyData.Close.HasValue)
    {
        var change = dailyData.Close.Value - dailyData.Open.Value;
        var changePercent = (change / dailyData.Open.Value) * 100;
        Console.WriteLine($"\nDaily Change: ${change:F2} ({changePercent:F2}%)");
    }

    // Calculate trading range
    if (dailyData.High.HasValue && dailyData.Low.HasValue)
    {
        var range = dailyData.High.Value - dailyData.Low.Value;
        Console.WriteLine($"Trading Range: ${range:F2}");
    }
}

// Get daily data for a put option
var putDailyDataRequest = new GetDailyOpenCloseRequest
{
    OptionsTicker = "O:AAPL250117P00150000",
    Date = "2023-01-15"
};
var putDailyData = await _client.Options.GetDailyOpenCloseAsync(putDailyDataRequest);

if (putDailyData != null)
{
    Console.WriteLine($"\n{putDailyData.Symbol} Daily Summary:");
    Console.WriteLine($"Open to Close: ${putDailyData.Open} → ${putDailyData.Close}");
    Console.WriteLine($"Range: ${putDailyData.Low} - ${putDailyData.High}");
    Console.WriteLine($"Volume: {putDailyData.Volume} contracts");
}

// Example: Track daily option premium changes
var dates = new[] { "2023-01-09", "2023-01-10", "2023-01-11", "2023-01-12", "2023-01-13" };
var optionTicker = "O:TSLA260320C00700000";

Console.WriteLine("\nDaily Premium Tracking:");
foreach (var date in dates)
{
    try
    {
        var dailyRequest = new GetDailyOpenCloseRequest
        {
            OptionsTicker = optionTicker,
            Date = date
        };
        var daily = await _client.Options.GetDailyOpenCloseAsync(dailyRequest);

        if (daily != null)
        {
            Console.WriteLine($"{date}: Open=${daily.Open}, Close=${daily.Close}, " +
                            $"Volume={daily.Volume}");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"{date}: No data available ({ex.Message})");
    }
}

// Example: Compare pre-market, regular hours, and after-hours trading
var compareDataRequest = new GetDailyOpenCloseRequest
{
    OptionsTicker = "O:SPY251219C00650000",
    Date = "2023-01-09"
};
var compareData = await _client.Options.GetDailyOpenCloseAsync(compareDataRequest);

if (compareData != null)
{
    Console.WriteLine("\nTrading Session Analysis:");

    if (compareData.PreMarket.HasValue && compareData.Open.HasValue)
    {
        var preMarketMove = compareData.Open.Value - compareData.PreMarket.Value;
        Console.WriteLine($"Pre-Market → Open: ${preMarketMove:F2}");
    }

    if (compareData.Open.HasValue && compareData.Close.HasValue)
    {
        var regularHoursMove = compareData.Close.Value - compareData.Open.Value;
        Console.WriteLine($"Open → Close: ${regularHoursMove:F2}");
    }

    if (compareData.Close.HasValue && compareData.AfterHours.HasValue)
    {
        var afterHoursMove = compareData.AfterHours.Value - compareData.Close.Value;
        Console.WriteLine($"Close → After-Hours: ${afterHoursMove:F2}");
    }
}
```

**Parameters:**
- `optionsTicker` (required) - The options ticker symbol in OCC format (e.g., "O:SPY251219C00650000"). Must include the "O:" prefix.
- `date` (required) - The date of the requested daily data in YYYY-MM-DD format (e.g., "2023-01-09")

**Response:** Returns an `OptionDailyOpenClose` object containing:
- `Status` - Response status ("OK" for success)
- `From` - The date for this daily summary in YYYY-MM-DD format
- `Symbol` - The options contract ticker symbol in OCC format
- `Open` - The opening price for the trading day
- `High` - The highest price reached during the trading day
- `Low` - The lowest price reached during the trading day
- `Close` - The closing price for the trading day
- `Volume` - The total trading volume for the day (number of contracts)
- `PreMarket` - The last trade price from the pre-market trading session
- `AfterHours` - The last trade price from the after-hours trading session

**Use Cases:**
- Get a quick daily snapshot of an options contract's performance
- Track daily premium changes for specific options
- Analyze pre-market and after-hours trading activity
- Calculate daily price changes and ranges
- Monitor daily volume for liquidity assessment
- Compare multiple days of daily summaries for trend analysis
- Verify daily high/low levels for technical analysis
- Track options during earnings announcements or major events

**Note:** This endpoint provides a summary of the entire trading day in a single response. For more granular intraday data, use the GetBarsAsync endpoint with minute or hour intervals.

## Common Patterns

### Error Handling

```csharp
try
{
    var response = await _client.Stocks.GetLastTradeAsync("AAPL");

    if (response?.Results != null)
    {
        Console.WriteLine($"Price: ${response.Results.Price}");
    }
    else
    {
        Console.WriteLine("No data returned");
    }
}
catch (HttpRequestException ex)
{
    Console.WriteLine($"HTTP Error: {ex.Message}");
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}
```

### Pagination

For endpoints that support pagination, use the `limit` parameter and handle multiple requests:

```csharp
var allTickers = new List<StockTicker>();
var limit = 1000; // Maximum allowed
string? nextUrl = null;

do
{
    var response = await _client.ReferenceData.GetTickersAsync(
        active: true,
        limit: limit
    );

    if (response.Results != null)
    {
        allTickers.AddRange(response.Results);
    }

    nextUrl = response.NextUrl;

    // Note: Current implementation doesn't support continuation tokens yet
    // This pattern is prepared for future pagination support

} while (nextUrl != null);

Console.WriteLine($"Retrieved {allTickers.Count} tickers");
```

### Cancellation

Use `CancellationToken` for long-running operations:

```csharp
using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));

try
{
    var response = await _client.Stocks.GetTradesAsync(
        ticker: "AAPL",
        timestampGte: "2025-09-15T09:30:00.000000000Z",
        timestampLte: "2025-09-15T16:00:00.000000000Z",
        limit: 50000,
        cancellationToken: cts.Token
    );
}
catch (OperationCanceledException)
{
    Console.WriteLine("Operation timed out");
}
```

### Async/Await Best Practices

```csharp
// Parallel requests for different tickers
var tasks = new[]
{
    _client.Stocks.GetLastTradeAsync("AAPL"),
    _client.Stocks.GetLastTradeAsync("MSFT"),
    _client.Stocks.GetLastTradeAsync("GOOGL")
};

var results = await Task.WhenAll(tasks);

foreach (var result in results)
{
    if (result?.Results != null)
    {
        Console.WriteLine($"Price: ${result.Results.Price}");
    }
}
```

### Using Enums vs String Values

The library provides enums for type safety, but they convert to the correct string values:

```csharp
// Using enum (recommended)
var bars = await _client.Stocks.GetBarsAsync(
    ticker: "AAPL",
    multiplier: 1,
    timespan: AggregateInterval.Day, // Type-safe enum
    from: "2025-09-01",
    to: "2025-09-30"
);

// Available AggregateInterval values:
// - AggregateInterval.Minute
// - AggregateInterval.Hour
// - AggregateInterval.Day
// - AggregateInterval.Week
// - AggregateInterval.Month
// - AggregateInterval.Quarter
// - AggregateInterval.Year

// Available SortOrder values:
// - SortOrder.Ascending
// - SortOrder.Descending

// Available Market values:
// - Market.Stocks
// - Market.Crypto
// - Market.Forex

// Available AssetClass values:
// - AssetClass.Stocks
// - AssetClass.Options
// - AssetClass.Crypto
// - AssetClass.Fx

// Available DataType values:
// - DataType.Trade
// - DataType.Quote

// Available Locale values:
// - Locale.Us
// - Locale.Global

// Available SipMappingType values:
// - SipMappingType.Consolidated
// - SipMappingType.Participant1
// - SipMappingType.Participant2
// - SipMappingType.Participant3

// Use constants for sort fields
var tickers = await _client.ReferenceData.GetTickersAsync(
    active: true,
    sort: TickerSortFields.Name, // Type-safe constant
    order: SortOrder.Ascending
);

// Available TickerSortFields values:
// - TickerSortFields.Ticker
// - TickerSortFields.Name
// - TickerSortFields.Market
// - TickerSortFields.Locale
// - TickerSortFields.PrimaryExchange
// - TickerSortFields.Type
// - TickerSortFields.Active
// - TickerSortFields.CurrencySymbol
// - TickerSortFields.Cik
// - TickerSortFields.CompositeFigi
// - TickerSortFields.ShareClassFigi
// - TickerSortFields.LastUpdatedUtc

// Available ConditionCodeSortFields values:
// - ConditionCodeSortFields.AssetClass
// - ConditionCodeSortFields.DataType
// - ConditionCodeSortFields.Id
// - ConditionCodeSortFields.Name
// - ConditionCodeSortFields.Type
```

### Working with Timestamps

Polygon.io uses nanosecond timestamps. The library handles these automatically:

```csharp
// Using ISO 8601 date strings (recommended for clarity)
var trades = await _client.Stocks.GetTradesAsync(
    ticker: "AAPL",
    timestampGte: "2025-09-15T09:30:00.000000000Z",
    timestampLte: "2025-09-15T16:00:00.000000000Z"
);

// Using nanosecond epoch timestamps
var tradesEpoch = await _client.Stocks.GetTradesAsync(
    ticker: "AAPL",
    timestampGte: "1694782800000000000",
    timestampLte: "1694806400000000000"
);
```

### Response Structure

All API responses follow a consistent pattern:

```csharp
var response = await _client.Stocks.GetBarsAsync(
    ticker: "AAPL",
    multiplier: 1,
    timespan: AggregateInterval.Day,
    from: "2025-09-01",
    to: "2025-09-30"
);

// Common response properties
Console.WriteLine($"Status: {response.Status}");
Console.WriteLine($"Request ID: {response.RequestId}");
Console.WriteLine($"Count: {response.Count}");
Console.WriteLine($"Results Count: {response.Results?.Count ?? 0}");

// Some responses include pagination info
if (!string.IsNullOrEmpty(response.NextUrl))
{
    Console.WriteLine($"Next URL: {response.NextUrl}");
}

// Process results
foreach (var bar in response.Results ?? Enumerable.Empty<StockBar>())
{
    // Work with individual results
}
```

## Additional Resources

- [Polygon.io API Documentation](https://polygon.io/docs)
- [GitHub Repository](https://github.com/treycodescodes/TreyThomasCodes.Polygon)
- [NuGet Package](https://www.nuget.org/packages/TreyThomasCodes.Polygon.RestClient)
