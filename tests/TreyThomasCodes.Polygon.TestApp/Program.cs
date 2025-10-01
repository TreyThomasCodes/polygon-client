using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NodaTime;
using Serilog;
using Serilog.Exceptions;
using Serilog.Exceptions.Core;
using Serilog.Exceptions.Refit.Destructurers;
using TreyThomasCodes.Polygon.RestClient.Extensions;
using TreyThomasCodes.Polygon.RestClient.Services;
using TreyThomasCodes.Polygon.TestApp;
using TreyThomasCodes.Polygon.Models.Common;

Log.Logger = new LoggerConfiguration()
    .Enrich.WithExceptionDetails(new DestructuringOptionsBuilder()
    .WithDefaultDestructurers()
    .WithDestructurers([new ApiExceptionDestructurer(destructureHttpContent: true)]))
    .WriteTo.Console()
    .CreateLogger();

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration.AddUserSecrets<Program>();
builder.Services.AddSerilog();

var apiKey = builder.Configuration["Polygon:ApiKey"];
if (string.IsNullOrEmpty(apiKey))
{
    Console.WriteLine("Please set your Polygon API key using:");
    Console.WriteLine("dotnet user-secrets set \"Polygon:ApiKey\" \"your-api-key-here\"");
    return;
}

// Register the logging handler
builder.Services.AddTransient<LoggingHandler>();

// Add the Polygon client with logging
builder.Services.AddPolygonClient(options =>
{
    options.ApiKey = apiKey;
});

// Configure HTTP clients to use the logging handler
builder.Services.ConfigureAll<Microsoft.Extensions.Http.HttpClientFactoryOptions>(options =>
{
    options.HttpMessageHandlerBuilderActions.Add(builder =>
    {
        builder.AdditionalHandlers.Insert(0, builder.Services.GetRequiredService<LoggingHandler>());
    });
});

var host = builder.Build();

var polygonClient = host.Services.GetRequiredService<IPolygonClient>();

Log.Information("Fetching AAPL last day OHLC data...");

try
{
    var yesterday = DateOnly.FromDateTime(DateTime.Today.AddDays(-1));
    var stocksService = polygonClient.Stocks;

    var response = await stocksService.GetBarsAsync("AAPL", 1, AggregateInterval.Day, yesterday.ToString("yyyy-MM-dd"), yesterday.ToString("yyyy-MM-dd"));

    if (response?.Results != null && response?.ResultsCount > 0)
    {
        var bar = response.Results[0];
        Log.Information("AAPL OHLC for {Date}: Open=${Open:F2}, High=${High:F2}, Low=${Low:F2}, Close=${Close:F2}, Volume={Volume:N0}, VWAP=${VWAP:F2}",
            yesterday.ToString("yyyy-MM-dd"), bar.Open, bar.High, bar.Low, bar.Close, bar.Volume, bar.VolumeWeightedAveragePrice);
    }
    else
    {
        Log.Warning("No data found for the specified date. The market may have been closed.");
    }

    Log.Information("Fetching AAPL previous trading day data using previous day endpoint...");

    var previousDayResponse = await stocksService.GetPreviousCloseAsync("AAPL");

    if (previousDayResponse?.Results != null && previousDayResponse?.ResultsCount > 0)
    {
        var prevBar = previousDayResponse.Results[0];
        Log.Information("AAPL Previous Trading Day: Open=${Open:F2}, High=${High:F2}, Low=${Low:F2}, Close=${Close:F2}, Volume={Volume:N0}, VWAP=${VWAP:F2}",
            prevBar.Open, prevBar.High, prevBar.Low, prevBar.Close, prevBar.Volume, prevBar.VolumeWeightedAveragePrice);
    }
    else
    {
        Log.Warning("No previous trading day data found.");
    }

    Log.Information("Fetching ZZZZZZ previous trading day data (nonexistent stock)...");

    var zzzzzResponse = await stocksService.GetPreviousCloseAsync("ZZZZZZ");

    if (zzzzzResponse?.Results != null && zzzzzResponse?.Results?.Count > 0)
    {
        var zzzzzBar = zzzzzResponse.Results[0];
        Log.Information("ZZZZZZ Previous Trading Day: Open=${Open:F2}, High=${High:F2}, Low=${Low:F2}, Close=${Close:F2}, Volume={Volume:N0}, VWAP=${VWAP:F2}",
            zzzzzBar.Open, zzzzzBar.High, zzzzzBar.Low, zzzzzBar.Close, zzzzzBar.Volume, zzzzzBar.VolumeWeightedAveragePrice);
    }
    else
    {
        Log.Warning("No previous trading day data found for ZZZZZZ (expected for nonexistent stock).");
    }

    Log.Information("Fetching TSLA daily OHLC data for week of 9/15/2025...");

    // Week of 9/15/2025: September 15-19, 2025
    var weekStart = new DateOnly(2025, 9, 15);
    var weekEnd = new DateOnly(2025, 9, 19);

    var tslaWeekResponse = await stocksService.GetBarsAsync("TSLA", 1, AggregateInterval.Day, weekStart.ToString("yyyy-MM-dd"), weekEnd.ToString("yyyy-MM-dd"));

    if (tslaWeekResponse?.Results != null && tslaWeekResponse?.ResultsCount > 0)
    {
        Log.Information("TSLA Daily OHLC Data for week of {WeekStart} to {WeekEnd}:", weekStart.ToString("yyyy-MM-dd"), weekEnd.ToString("yyyy-MM-dd"));

        foreach (var bar in tslaWeekResponse.Results)
        {
            var tradeDate = bar.Timestamp.HasValue
                ? Instant.FromUnixTimeMilliseconds((long)bar.Timestamp.Value).InUtc().Date
                : SystemClock.Instance.GetCurrentInstant().InUtc().Date;
            Log.Information("  {Date}: Open=${Open:F2}, High=${High:F2}, Low=${Low:F2}, Close=${Close:F2}, Volume={Volume:N0}, VWAP=${VWAP:F2}",
                tradeDate.ToString("yyyy-MM-dd", null), bar.Open, bar.High, bar.Low, bar.Close, bar.Volume, bar.VolumeWeightedAveragePrice);
        }

        Log.Information("Total trading days found: {Count}", tslaWeekResponse.ResultsCount);
    }
    else
    {
        Log.Warning("No TSLA data found for the week of 9/15/2025. The market may have been closed or data may not be available yet.");
    }
}
catch (Exception ex)
{
    Log.Error(ex, "An error occurred while fetching stock data");
}
finally
{
    await Log.CloseAndFlushAsync();
}
