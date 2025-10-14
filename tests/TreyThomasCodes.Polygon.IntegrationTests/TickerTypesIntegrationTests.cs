// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TreyThomasCodes.Polygon.RestClient.Extensions;
using TreyThomasCodes.Polygon.RestClient.Services;
using TreyThomasCodes.Polygon.Models.Reference;

namespace TreyThomasCodes.Polygon.IntegrationTests;

/// <summary>
/// Integration tests for fetching ticker types data from Polygon.io.
/// These tests verify the client library functionality, not the API itself.
/// </summary>
public class TickerTypesIntegrationTests : IDisposable
{
    private readonly IHost _host;
    private readonly IPolygonClient _polygonClient;
    private readonly string _apiKey;
    private bool disposedValue;

    /// <summary>
    /// Initializes a new instance of the TickerTypesIntegrationTests class.
    /// Sets up the host with dependency injection and Polygon client configuration.
    /// </summary>
    public TickerTypesIntegrationTests()
    {
        var builder = Host.CreateApplicationBuilder();
        builder.Configuration.AddUserSecrets<TickerTypesIntegrationTests>();
        var apiKey = builder.Configuration["Polygon:ApiKey"];

        // Skip all tests in this class if no API key is configured
        Assert.SkipUnless(!string.IsNullOrEmpty(apiKey), "Polygon API key not configured in user secrets. Use: dotnet user-secrets set \"Polygon:ApiKey\" \"your-api-key-here\"");

        _apiKey = apiKey!; // Safe to use ! since we skip if null or empty

        builder.Services.AddPolygonClient(options =>
        {
            options.ApiKey = _apiKey;
        });

        _host = builder.Build();
        _polygonClient = _host.Services.GetRequiredService<IPolygonClient>();
    }

    /// <summary>
    /// Tests fetching ticker types from Polygon.io.
    /// Verifies that the client can successfully call the endpoint and deserialize the response.
    /// </summary>
    [Fact]
    public async Task GetTickerTypes_ShouldReturnValidResponse()
    {
        // Arrange
        var referenceDataService = _polygonClient.ReferenceData;

        // Act
        var tickerTypesResponse = await referenceDataService.GetTickerTypesAsync(TestContext.Current.CancellationToken);

        // Assert - Verify client successfully made the call and deserialized the response
        Assert.NotNull(tickerTypesResponse);
        Assert.Equal("OK", tickerTypesResponse.Status);
        Assert.NotEmpty(tickerTypesResponse.RequestId);
        Assert.True(tickerTypesResponse.Count > 0);
        Assert.NotNull(tickerTypesResponse.Results);
        Assert.True(tickerTypesResponse.Results.Count > 0);
        Assert.Equal(tickerTypesResponse.Count, tickerTypesResponse.Results.Count);
    }

    /// <summary>
    /// Tests that the ticker types data structure is properly typed.
    /// Verifies that the client correctly deserializes all response properties.
    /// </summary>
    [Fact]
    public async Task GetTickerTypes_ShouldHaveCorrectDataTypes()
    {
        // Arrange
        var referenceDataService = _polygonClient.ReferenceData;

        // Act
        var tickerTypesResponse = await referenceDataService.GetTickerTypesAsync(TestContext.Current.CancellationToken);

        // Assert - Verify client deserialized the response correctly
        Assert.NotNull(tickerTypesResponse);
        Assert.IsType<TickerTypesResponse>(tickerTypesResponse);

        // Verify property types
        Assert.IsType<List<TickerType>>(tickerTypesResponse.Results);
        Assert.IsType<int>(tickerTypesResponse.Count);
        Assert.IsType<string>(tickerTypesResponse.Status);
        Assert.IsType<string>(tickerTypesResponse.RequestId);

        // Verify at least one ticker type has proper structure
        var firstType = tickerTypesResponse.Results.First();
        Assert.IsType<string>(firstType.Code);
        Assert.IsType<string>(firstType.Description);
        Assert.IsType<string>(firstType.AssetClass);
        Assert.IsType<string>(firstType.Locale);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                _host.Dispose();
            }

            disposedValue = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
