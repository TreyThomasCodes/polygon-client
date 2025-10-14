// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TreyThomasCodes.Polygon.RestClient.Extensions;
using TreyThomasCodes.Polygon.RestClient.Services;
using TreyThomasCodes.Polygon.Models.Reference;
using TreyThomasCodes.Polygon.Models.Common;

namespace TreyThomasCodes.Polygon.IntegrationTests;

/// <summary>
/// Integration tests for fetching condition codes data from Polygon.io.
/// These tests verify the client library functionality, not the API itself.
/// </summary>
public class ConditionCodesIntegrationTests : IDisposable
{
    private readonly IHost _host;
    private readonly IPolygonClient _polygonClient;
    private readonly string _apiKey;
    private bool disposedValue;

    /// <summary>
    /// Initializes a new instance of the ConditionCodesIntegrationTests class.
    /// Sets up the host with dependency injection and Polygon client configuration.
    /// </summary>
    public ConditionCodesIntegrationTests()
    {
        var builder = Host.CreateApplicationBuilder();
        builder.Configuration.AddUserSecrets<ConditionCodesIntegrationTests>();
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
    /// Tests fetching condition codes from Polygon.io with stocks asset class filter.
    /// Verifies that the client can successfully call the endpoint and deserialize the response.
    /// </summary>
    [Fact]
    public async Task GetConditionCodes_WithStocksFilter_ShouldReturnValidResponse()
    {
        // Arrange
        var referenceDataService = _polygonClient.ReferenceData;

        // Act
        var conditionCodesResponse = await referenceDataService.GetConditionCodesAsync(
            assetClass: AssetClass.Stocks,
            limit: 10,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert - Verify client successfully made the call and deserialized the response
        Assert.NotNull(conditionCodesResponse);
        Assert.Equal("OK", conditionCodesResponse.Status);
        Assert.NotEmpty(conditionCodesResponse.RequestId);
        Assert.True(conditionCodesResponse.Count > 0);
        Assert.NotNull(conditionCodesResponse.Results);
        Assert.True(conditionCodesResponse.Results.Count > 0);
    }

    /// <summary>
    /// Tests that the condition codes data structure is properly typed.
    /// Verifies that the client correctly deserializes all response properties.
    /// </summary>
    [Fact]
    public async Task GetConditionCodes_ShouldHaveCorrectDataTypes()
    {
        // Arrange
        var referenceDataService = _polygonClient.ReferenceData;

        // Act
        var conditionCodesResponse = await referenceDataService.GetConditionCodesAsync(
            assetClass: AssetClass.Stocks,
            limit: 5,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert - Verify client deserialized the response correctly
        Assert.NotNull(conditionCodesResponse);
        Assert.IsType<List<ConditionCode>>(conditionCodesResponse.Results);
        Assert.IsType<int>(conditionCodesResponse.Count);
        Assert.IsType<string>(conditionCodesResponse.Status);
        Assert.IsType<string>(conditionCodesResponse.RequestId);

        // Verify at least one condition code has proper structure
        var firstCode = conditionCodesResponse.Results.First();
        Assert.IsType<int>(firstCode.Id);
        Assert.IsType<string>(firstCode.Type);
        Assert.IsType<string>(firstCode.Name);
        Assert.IsType<string>(firstCode.AssetClass);
        Assert.IsType<SipMapping>(firstCode.SipMapping);
        Assert.IsType<UpdateRules>(firstCode.UpdateRules);
        Assert.IsType<List<string>>(firstCode.DataTypes);
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
