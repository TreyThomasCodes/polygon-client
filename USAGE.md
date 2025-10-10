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

// Get daily bars for the last month
var dailyBars = await _client.Stocks.GetBarsAsync(
    ticker: "AAPL",
    multiplier: 1,
    timespan: AggregateInterval.Day,
    from: "2025-09-01",
    to: "2025-09-30",
    adjusted: true,
    sort: SortOrder.Ascending,
    limit: 5000
);

foreach (var bar in dailyBars.Results)
{
    Console.WriteLine($"Date: {bar.Timestamp}, O: {bar.Open}, H: {bar.High}, L: {bar.Low}, C: {bar.Close}, V: {bar.Volume}");
}

// Get 5-minute bars for intraday analysis
var intradayBars = await _client.Stocks.GetBarsAsync(
    ticker: "TSLA",
    multiplier: 5,
    timespan: AggregateInterval.Minute,
    from: "2025-09-15",
    to: "2025-09-15",
    adjusted: true
);

// Get hourly bars
var hourlyBars = await _client.Stocks.GetBarsAsync(
    ticker: "MSFT",
    multiplier: 1,
    timespan: AggregateInterval.Hour,
    from: "2025-09-01",
    to: "2025-09-30"
);

// Get weekly bars
var weeklyBars = await _client.Stocks.GetBarsAsync(
    ticker: "GOOGL",
    multiplier: 1,
    timespan: AggregateInterval.Week,
    from: "2025-01-01",
    to: "2025-09-30"
);

// Get monthly bars for long-term analysis
var monthlyBars = await _client.Stocks.GetBarsAsync(
    ticker: "SPY",
    multiplier: 1,
    timespan: AggregateInterval.Month,
    from: "2020-01-01",
    to: "2025-09-30"
);
```

#### GetPreviousCloseAsync - Get Previous Day's Close

Get the OHLC data for the previous trading day.

```csharp
var previousClose = await _client.Stocks.GetPreviousCloseAsync(
    ticker: "AAPL",
    adjusted: true
);

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
// Get all tickers for a specific date
var groupedDaily = await _client.Stocks.GetGroupedDailyAsync(
    date: "2025-09-15",
    adjusted: true,
    includeOtc: false
);

Console.WriteLine($"Found {groupedDaily.Results?.Count ?? 0} tickers");

foreach (var bar in groupedDaily.Results?.Take(10) ?? Enumerable.Empty<StockBar>())
{
    Console.WriteLine($"{bar.Ticker}: Open=${bar.Open}, Close=${bar.Close}, Volume={bar.Volume}");
}

// Include OTC securities
var groupedDailyWithOtc = await _client.Stocks.GetGroupedDailyAsync(
    date: "2025-09-15",
    adjusted: true,
    includeOtc: true
);
```

#### GetDailyOpenCloseAsync - Get Daily Open/Close Prices

Get detailed open and close prices for a specific day, including pre-market and after-hours activity.

```csharp
var dailyOpenClose = await _client.Stocks.GetDailyOpenCloseAsync(
    ticker: "AAPL",
    date: "2025-09-15",
    adjusted: true
);

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
// Get trades for a specific time range
var trades = await _client.Stocks.GetTradesAsync(
    ticker: "AAPL",
    timestampGte: "2025-09-15T09:30:00.000000000Z",
    timestampLte: "2025-09-15T16:00:00.000000000Z",
    limit: 1000,
    sort: "timestamp"
);

foreach (var trade in trades.Results ?? Enumerable.Empty<StockTrade>())
{
    Console.WriteLine($"Trade at {trade.ParticipantTimestamp}: {trade.Size} shares @ ${trade.Price}");
    Console.WriteLine($"  Exchange: {trade.Exchange}, Conditions: {string.Join(",", trade.Conditions ?? new List<int>())}");
}

// Get the most recent trades (descending order)
var recentTrades = await _client.Stocks.GetTradesAsync(
    ticker: "TSLA",
    limit: 100,
    sort: "timestamp.desc"
);

// Get trades for a specific timestamp
var specificTrades = await _client.Stocks.GetTradesAsync(
    ticker: "MSFT",
    timestamp: "1694782800000000000",
    limit: 10
);

// Get trades with greater than filter
var tradesAfter = await _client.Stocks.GetTradesAsync(
    ticker: "GOOGL",
    timestampGt: "2025-09-15T10:00:00.000000000Z",
    limit: 500
);
```

#### GetLastTradeAsync - Get Most Recent Trade

Get the most recent trade for a ticker.

```csharp
var lastTrade = await _client.Stocks.GetLastTradeAsync("AAPL");

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
// Get quotes for a specific time range
var quotes = await _client.Stocks.GetQuotesAsync(
    ticker: "AAPL",
    timestampGte: "2025-09-15T09:30:00.000000000Z",
    timestampLte: "2025-09-15T16:00:00.000000000Z",
    limit: 1000,
    sort: "timestamp"
);

foreach (var quote in quotes.Results ?? Enumerable.Empty<StockQuote>())
{
    Console.WriteLine($"Quote at {quote.ParticipantTimestamp}:");
    Console.WriteLine($"  Bid: ${quote.BidPrice} x {quote.BidSize} on {quote.BidExchange}");
    Console.WriteLine($"  Ask: ${quote.AskPrice} x {quote.AskSize} on {quote.AskExchange}");
}

// Get recent quotes in descending order
var recentQuotes = await _client.Stocks.GetQuotesAsync(
    ticker: "TSLA",
    limit: 100,
    sort: "timestamp.desc"
);

// Get quotes for a specific timestamp
var specificQuotes = await _client.Stocks.GetQuotesAsync(
    ticker: "MSFT",
    timestamp: "1694782800000000000"
);
```

#### GetLastQuoteAsync - Get Most Recent NBBO Quote

Get the most recent National Best Bid and Offer (NBBO) quote.

```csharp
var lastQuote = await _client.Stocks.GetLastQuoteAsync("AAPL");

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
// Get snapshot for all tickers
var marketSnapshot = await _client.Stocks.GetMarketSnapshotAsync(
    includeOtc: false
);

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
var marketSnapshotWithOtc = await _client.Stocks.GetMarketSnapshotAsync(
    includeOtc: true
);
```

#### GetSnapshotAsync - Get Snapshot for a Single Ticker

Get current market data for a specific ticker.

```csharp
var snapshot = await _client.Stocks.GetSnapshotAsync("AAPL");

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

// Search by name
var appleTickersearch = await _client.ReferenceData.GetTickersAsync(
    search: "Apple",
    active: true,
    limit: 10
);

foreach (var ticker in appleTickers.Results ?? Enumerable.Empty<StockTicker>())
{
    Console.WriteLine($"{ticker.Ticker}: {ticker.Name} ({ticker.Type})");
}

// Filter by ticker type (common stock)
var commonStocks = await _client.ReferenceData.GetTickersAsync(
    type: "CS",
    active: true,
    market: Market.Stocks,
    limit: 100
);

// Filter by exchange
var nasdaqStocks = await _client.ReferenceData.GetTickersAsync(
    exchange: "XNAS",
    active: true,
    limit: 100,
    sort: TickerSortFields.Ticker,
    order: SortOrder.Ascending
);

// Get tickers alphabetically in a range
var tickersAtoC = await _client.ReferenceData.GetTickersAsync(
    tickerGte: "A",
    tickerLt: "D",
    active: true,
    limit: 1000
);

// Filter by specific ticker
var exactTicker = await _client.ReferenceData.GetTickersAsync(
    ticker: "AAPL"
);

// Filter by CUSIP
var tickerByCusip = await _client.ReferenceData.GetTickersAsync(
    cusip: "037833100"
);

// Filter by CIK
var tickerByCik = await _client.ReferenceData.GetTickersAsync(
    cik: "0000320193"
);

// Get historical ticker info for a specific date
var historicalTicker = await _client.ReferenceData.GetTickersAsync(
    ticker: "AAPL",
    date: "2020-01-01"
);

// Search ETFs
var etfs = await _client.ReferenceData.GetTickersAsync(
    type: "ETF",
    active: true,
    search: "S&P 500",
    limit: 20
);

// Sort by various fields
var tickersByName = await _client.ReferenceData.GetTickersAsync(
    active: true,
    sort: TickerSortFields.Name,
    order: SortOrder.Ascending,
    limit: 100
);

var tickersByVolume = await _client.ReferenceData.GetTickersAsync(
    active: true,
    sort: TickerSortFields.LastUpdatedUtc,
    order: SortOrder.Descending,
    limit: 100
);
```

#### GetTickerDetailsAsync - Get Detailed Ticker Information

Get comprehensive information about a specific ticker.

```csharp
// Get current ticker details
var tickerDetails = await _client.ReferenceData.GetTickerDetailsAsync("AAPL");

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
var historicalDetails = await _client.ReferenceData.GetTickerDetailsAsync(
    ticker: "AAPL",
    date: "2020-01-01"
);
```

#### GetTickerTypesAsync - Get All Ticker Types

Get a list of all supported ticker types.

```csharp
var tickerTypes = await _client.ReferenceData.GetTickerTypesAsync();

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
var marketStatus = await _client.ReferenceData.GetMarketStatusAsync();

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

// Get all exchanges
var allExchanges = await _client.ReferenceData.GetExchangesAsync();

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
var stockExchanges = await _client.ReferenceData.GetExchangesAsync(
    assetClass: AssetClass.Stocks
);

var cryptoExchanges = await _client.ReferenceData.GetExchangesAsync(
    assetClass: AssetClass.Crypto
);

var forexExchanges = await _client.ReferenceData.GetExchangesAsync(
    assetClass: AssetClass.Fx
);

// Filter by locale
var usExchanges = await _client.ReferenceData.GetExchangesAsync(
    locale: Locale.Us
);

var globalExchanges = await _client.ReferenceData.GetExchangesAsync(
    locale: Locale.Global
);

// Combine filters
var usStockExchanges = await _client.ReferenceData.GetExchangesAsync(
    assetClass: AssetClass.Stocks,
    locale: Locale.Us
);
```

#### GetConditionCodesAsync - Get Trade and Quote Condition Codes

Get condition codes that provide context for market data events.

```csharp
using TreyThomasCodes.Polygon.Models.Common;
using TreyThomasCodes.Polygon.Models.Reference;

// Get all condition codes
var allConditions = await _client.ReferenceData.GetConditionCodesAsync(
    limit: 1000
);

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
var stockConditions = await _client.ReferenceData.GetConditionCodesAsync(
    assetClass: AssetClass.Stocks,
    limit: 100
);

// Filter by data type
var tradeConditions = await _client.ReferenceData.GetConditionCodesAsync(
    dataType: DataType.Trade,
    limit: 100
);

var quoteConditions = await _client.ReferenceData.GetConditionCodesAsync(
    dataType: DataType.Quote,
    limit: 100
);

// Get specific condition code
var specificCondition = await _client.ReferenceData.GetConditionCodesAsync(
    id: "1"
);

// Get multiple condition codes
var multipleConditions = await _client.ReferenceData.GetConditionCodesAsync(
    id: "1,2,3,4,5"
);

// Filter by SIP mapping
var consolidatedConditions = await _client.ReferenceData.GetConditionCodesAsync(
    sipMapping: SipMappingType.Consolidated,
    limit: 100
);

var participant1Conditions = await _client.ReferenceData.GetConditionCodesAsync(
    sipMapping: SipMappingType.Participant1,
    limit: 100
);

// Sort results
var sortedConditions = await _client.ReferenceData.GetConditionCodesAsync(
    assetClass: AssetClass.Stocks,
    sort: ConditionCodeSortFields.Name,
    order: SortOrder.Ascending,
    limit: 100
);

// Combine multiple filters
var stockTradeConditions = await _client.ReferenceData.GetConditionCodesAsync(
    assetClass: AssetClass.Stocks,
    dataType: DataType.Trade,
    sipMapping: SipMappingType.Consolidated,
    sort: ConditionCodeSortFields.Id,
    order: SortOrder.Ascending,
    limit: 100
);
```

## Options Service

Access the options service via `IPolygonClient.Options`.

### Options Contracts

#### GetContractDetailsAsync - Get Options Contract Information

Retrieve detailed information about a specific options contract by its ticker symbol.

```csharp
// Get contract details for a call option on SPY
var contract = await _client.Options.GetContractDetailsAsync("O:SPY251219C00650000");

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
var putContract = await _client.Options.GetContractDetailsAsync("O:AAPL251219P00200000");

if (putContract.Results != null)
{
    var details = putContract.Results;
    Console.WriteLine($"\n{details.ContractType?.ToUpper()} Option on {details.UnderlyingTicker}");
    Console.WriteLine($"Strike: ${details.StrikePrice}");
    Console.WriteLine($"Expires: {details.ExpirationDate}");
    Console.WriteLine($"Style: {details.ExerciseStyle}");
}

// Example: Calculate intrinsic value (requires current stock price)
var optionContract = await _client.Options.GetContractDetailsAsync("O:TSLA251219C00250000");
var stockSnapshot = await _client.Stocks.GetSnapshotAsync("TSLA");

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

### Options Market Data

#### GetSnapshotAsync - Get Options Contract Snapshot

Retrieve a comprehensive snapshot of current market data for an options contract, including Greeks, implied volatility, last trade, last quote, and underlying asset information.

```csharp
// Get snapshot for a call option on SPY
var snapshot = await _client.Options.GetSnapshotAsync("SPY", "SPY251219C00650000");

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
var putSnapshot = await _client.Options.GetSnapshotAsync("AAPL", "AAPL250117P00150000");

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
    var optSnapshot = await _client.Options.GetSnapshotAsync("SPY", strike);

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
var callSnapshot = await _client.Options.GetSnapshotAsync("SPY", "SPY251219C00650000");

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
// Get all options contracts for MSTR with a limit
var chainSnapshot = await _client.Options.GetChainSnapshotAsync(
    underlyingAsset: "MSTR",
    limit: 10
);

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
var callsOnly = await _client.Options.GetChainSnapshotAsync(
    underlyingAsset: "SPY",
    contractType: "call",
    limit: 20
);

Console.WriteLine($"\nCall Options for SPY:");
foreach (var call in callsOnly.Results ?? Enumerable.Empty<OptionSnapshot>())
{
    Console.WriteLine($"Strike ${call.Details?.StrikePrice}: ${call.Day?.Close}");
}

// Filter by contract type (puts only)
var putsOnly = await _client.Options.GetChainSnapshotAsync(
    underlyingAsset: "SPY",
    contractType: "put",
    limit: 20
);

Console.WriteLine($"\nPut Options for SPY:");
foreach (var put in putsOnly.Results ?? Enumerable.Empty<OptionSnapshot>())
{
    Console.WriteLine($"Strike ${put.Details?.StrikePrice}: ${put.Day?.Close}");
}

// Filter by specific strike price
var strikeSnapshot = await _client.Options.GetChainSnapshotAsync(
    underlyingAsset: "SPY",
    strikePrice: 650m,
    limit: 10
);

Console.WriteLine($"\nAll contracts at $650 strike:");
foreach (var contract in strikeSnapshot.Results ?? Enumerable.Empty<OptionSnapshot>())
{
    Console.WriteLine($"{contract.Details?.Ticker}:");
    Console.WriteLine($"  Type: {contract.Details?.ContractType}");
    Console.WriteLine($"  Expiration: {contract.Details?.ExpirationDate}");
    Console.WriteLine($"  Price: ${contract.Day?.Close}");
}

// Filter by expiration date range
var expirationRange = await _client.Options.GetChainSnapshotAsync(
    underlyingAsset: "AAPL",
    expirationDateGte: "2025-10-01",
    expirationDateLte: "2025-12-31",
    limit: 50
);

Console.WriteLine($"\nAAPL options expiring between Oct-Dec 2025:");
foreach (var contract in expirationRange.Results ?? Enumerable.Empty<OptionSnapshot>())
{
    Console.WriteLine($"{contract.Details?.Ticker} expires {contract.Details?.ExpirationDate}");
}

// Sort by strike price in ascending order
var sortedByStrike = await _client.Options.GetChainSnapshotAsync(
    underlyingAsset: "TSLA",
    contractType: "call",
    order: "asc",
    sort: "strike_price",
    limit: 20
);

Console.WriteLine($"\nTSLA calls sorted by strike price:");
foreach (var contract in sortedByStrike.Results ?? Enumerable.Empty<OptionSnapshot>())
{
    Console.WriteLine($"Strike ${contract.Details?.StrikePrice}: ${contract.Day?.Close}, Delta: {contract.Greeks?.Delta:F4}");
}

// Combine multiple filters for specific analysis
var nearMoneyPuts = await _client.Options.GetChainSnapshotAsync(
    underlyingAsset: "SPY",
    contractType: "put",
    expirationDateGte: "2025-11-01",
    expirationDateLte: "2025-11-30",
    order: "asc",
    sort: "strike_price",
    limit: 50
);

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
var highVolume = await _client.Options.GetChainSnapshotAsync(
    underlyingAsset: "NVDA",
    limit: 100
);

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
var optionsChain = await _client.Options.GetChainSnapshotAsync(
    underlyingAsset: "SPY",
    expirationDateGte: "2025-12-15",
    expirationDateLte: "2025-12-20",
    order: "asc",
    sort: "strike_price",
    limit: 100
);

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
var ivSmile = await _client.Options.GetChainSnapshotAsync(
    underlyingAsset: "AAPL",
    contractType: "call",
    expirationDateGte: "2025-11-15",
    expirationDateLte: "2025-11-20",
    order: "asc",
    sort: "strike_price",
    limit: 50
);

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
var response = await _client.Options.GetChainSnapshotAsync(
    underlyingAsset: "SPY",
    limit: 100
);

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
// Get last trade for a call option on TSLA
var lastTrade = await _client.Options.GetLastTradeAsync("O:TSLA260320C00700000");

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
var putTrade = await _client.Options.GetLastTradeAsync("O:SPY251219P00650000");

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
    var trade = await _client.Options.GetLastTradeAsync(strike);

    if (trade.Results != null)
    {
        Console.WriteLine($"Strike ${strike.Substring(strike.Length - 8)}: ${trade.Results.Price} (Size: {trade.Results.Size})");
    }
}

// Example: Calculate bid-ask spread using snapshot and last trade
var snapshot = await _client.Options.GetSnapshotAsync("AAPL", "AAPL250117C00150000");
var trade = await _client.Options.GetLastTradeAsync("O:AAPL250117C00150000");

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
// Get recent quotes for an option with a limit
var quotes = await _client.Options.GetQuotesAsync(
    optionsTicker: "O:SPY241220P00720000",
    limit: 10
);

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
var timeRangeQuotes = await _client.Options.GetQuotesAsync(
    optionsTicker: "O:TSLA260320C00700000",
    timestamp: "2022-03-07T14:30:00",
    timestampLte: "2022-03-07T16:00:00",
    limit: 100
);

Console.WriteLine($"\nQuotes between 2:30 PM and 4:00 PM:");
foreach (var quote in timeRangeQuotes.Results ?? Enumerable.Empty<OptionQuote>())
{
    var spread = (quote.AskPrice ?? 0) - (quote.BidPrice ?? 0);
    Console.WriteLine($"Bid: ${quote.BidPrice}, Ask: ${quote.AskPrice}, Spread: ${spread}");
}

// Get quotes after a specific timestamp
var quotesAfter = await _client.Options.GetQuotesAsync(
    optionsTicker: "O:AAPL250117C00150000",
    timestampGt: "2022-03-07",
    order: "asc",
    limit: 50
);

// Get quotes before a specific timestamp
var quotesBefore = await _client.Options.GetQuotesAsync(
    optionsTicker: "O:MSFT250321P00400000",
    timestampLt: "2022-03-08",
    order: "desc",
    limit: 50
);

// Get quotes in a specific range with sorting
var sortedQuotes = await _client.Options.GetQuotesAsync(
    optionsTicker: "O:SPY241220P00720000",
    timestampGte: "2022-03-07T09:30:00",
    timestampLte: "2022-03-07T16:00:00",
    order: "asc",
    sort: "timestamp",
    limit: 1000
);

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
var quoteHistory = await _client.Options.GetQuotesAsync(
    optionsTicker: "O:SPY241220P00720000",
    timestampGte: "2022-03-07T14:00:00",
    timestampLte: "2022-03-07T14:05:00",
    order: "asc",
    limit: 100
);

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
var allQuotes = await _client.Options.GetQuotesAsync(
    optionsTicker: "O:AAPL250117C00150000",
    timestampGte: "2022-03-07",
    limit: 1000
);

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
var response = await _client.Options.GetQuotesAsync(
    optionsTicker: "O:SPY241220P00720000",
    timestampGte: "2022-03-07",
    limit: 100
);

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

**Note:** Additional options endpoints for aggregates will be added in upcoming releases.

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
