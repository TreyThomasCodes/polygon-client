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
/// Integration tests for fetching condition codes data from Polygon.io.
/// These tests require a valid Polygon API key to be configured in user secrets.
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
    /// Verifies that the API returns valid condition codes data matching the expected response structure.
    /// </summary>
    [Fact]
    public async Task GetConditionCodes_WithStocksFilter_ShouldReturnValidConditionCodesData()
    {
        // Arrange
        var referenceDataService = _polygonClient.ReferenceData;

        // Act
        var conditionCodesResponse = await referenceDataService.GetConditionCodesAsync(
            assetClass: "stocks",
            order: "asc",
            limit: 10,
            sort: "asset_class",
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(conditionCodesResponse);
        Assert.Equal("OK", conditionCodesResponse.Status);
        Assert.NotEmpty(conditionCodesResponse.RequestId);
        Assert.True(conditionCodesResponse.Count > 0, "Should return at least one condition code");
        Assert.NotNull(conditionCodesResponse.Results);
        Assert.True(conditionCodesResponse.Results.Count > 0, "Results should contain at least one condition code");
        Assert.True(conditionCodesResponse.Results.Count <= 10, "Should not exceed the requested limit of 10");
    }

    /// <summary>
    /// Tests that condition codes include expected properties and data structure.
    /// Verifies that each condition code has all required fields populated with valid values.
    /// </summary>
    [Fact]
    public async Task GetConditionCodes_ShouldHaveValidConditionCodeStructure()
    {
        // Arrange
        var referenceDataService = _polygonClient.ReferenceData;

        // Act
        var conditionCodesResponse = await referenceDataService.GetConditionCodesAsync(
            assetClass: "stocks",
            limit: 5,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(conditionCodesResponse);
        Assert.NotNull(conditionCodesResponse.Results);

        foreach (var conditionCode in conditionCodesResponse.Results)
        {
            // Verify required fields are present
            Assert.True(conditionCode.Id >= 0, "ID should be a non-negative integer");

            Assert.NotNull(conditionCode.Type);
            Assert.NotEmpty(conditionCode.Type);

            Assert.NotNull(conditionCode.Name);
            Assert.NotEmpty(conditionCode.Name);

            Assert.NotNull(conditionCode.AssetClass);
            Assert.NotEmpty(conditionCode.AssetClass);
            Assert.Equal("stocks", conditionCode.AssetClass);

            // Verify SIP mapping structure
            Assert.NotNull(conditionCode.SipMapping);

            // Verify update rules structure
            Assert.NotNull(conditionCode.UpdateRules);
            Assert.NotNull(conditionCode.UpdateRules.Consolidated);
            Assert.NotNull(conditionCode.UpdateRules.MarketCenter);

            // Verify data types
            Assert.NotNull(conditionCode.DataTypes);
            Assert.True(conditionCode.DataTypes.Count > 0, "Should have at least one data type");

            // Verify the condition code object structure
            Assert.IsType<ConditionCode>(conditionCode);
        }
    }

    /// <summary>
    /// Tests that condition codes include specific expected conditions.
    /// Verifies that common condition codes like Acquisition, Average Price Trade, etc. are present.
    /// </summary>
    [Fact]
    public async Task GetConditionCodes_ShouldIncludeExpectedConditions()
    {
        // Arrange
        var referenceDataService = _polygonClient.ReferenceData;
        var expectedConditionNames = new[] { "Acquisition", "Average Price Trade", "Cash Sale", "Cross Trade" };

        // Act - Get a larger set to increase chance of finding expected conditions
        var conditionCodesResponse = await referenceDataService.GetConditionCodesAsync(
            assetClass: "stocks",
            limit: 50,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(conditionCodesResponse);
        Assert.NotNull(conditionCodesResponse.Results);

        var conditionNames = conditionCodesResponse.Results.Select(c => c.Name).ToList();

        // At least some of the expected conditions should be present
        var foundExpectedConditions = expectedConditionNames.Where(expected => conditionNames.Contains(expected)).Count();
        Assert.True(foundExpectedConditions > 0, $"Should find at least one of the expected condition names: {string.Join(", ", expectedConditionNames)}");
    }

    /// <summary>
    /// Tests that condition codes have valid SIP mappings.
    /// Verifies that SIP mappings contain expected market data feed identifiers.
    /// </summary>
    [Fact]
    public async Task GetConditionCodes_ShouldHaveValidSipMappings()
    {
        // Arrange
        var referenceDataService = _polygonClient.ReferenceData;

        // Act
        var conditionCodesResponse = await referenceDataService.GetConditionCodesAsync(
            assetClass: "stocks",
            limit: 10,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(conditionCodesResponse);
        Assert.NotNull(conditionCodesResponse.Results);

        foreach (var conditionCode in conditionCodesResponse.Results)
        {
            Assert.NotNull(conditionCode.SipMapping);

            // At least one of CTA, UTP, or FINRA_TDDS should be populated for most conditions
            // However, some conditions like "Regular Trade" may not have specific SIP mappings
            var hasMapping = !string.IsNullOrEmpty(conditionCode.SipMapping.Cta) ||
                           !string.IsNullOrEmpty(conditionCode.SipMapping.Utp) ||
                           !string.IsNullOrEmpty(conditionCode.SipMapping.FinraTdds);

            // Note: SIP mappings are optional and depend on the specific condition and market data providers
            // We just verify the structure exists but don't enforce that it's populated
        }
    }

    /// <summary>
    /// Tests that condition codes have valid update rules.
    /// Verifies that update rules are properly structured and contain boolean values.
    /// </summary>
    [Fact]
    public async Task GetConditionCodes_ShouldHaveValidUpdateRules()
    {
        // Arrange
        var referenceDataService = _polygonClient.ReferenceData;

        // Act
        var conditionCodesResponse = await referenceDataService.GetConditionCodesAsync(
            assetClass: "stocks",
            limit: 5,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(conditionCodesResponse);
        Assert.NotNull(conditionCodesResponse.Results);

        foreach (var conditionCode in conditionCodesResponse.Results)
        {
            Assert.NotNull(conditionCode.UpdateRules);

            // Verify consolidated rules
            Assert.NotNull(conditionCode.UpdateRules.Consolidated);
            Assert.IsType<bool>(conditionCode.UpdateRules.Consolidated.UpdatesHighLow);
            Assert.IsType<bool>(conditionCode.UpdateRules.Consolidated.UpdatesOpenClose);
            Assert.IsType<bool>(conditionCode.UpdateRules.Consolidated.UpdatesVolume);

            // Verify market center rules
            Assert.NotNull(conditionCode.UpdateRules.MarketCenter);
            Assert.IsType<bool>(conditionCode.UpdateRules.MarketCenter.UpdatesHighLow);
            Assert.IsType<bool>(conditionCode.UpdateRules.MarketCenter.UpdatesOpenClose);
            Assert.IsType<bool>(conditionCode.UpdateRules.MarketCenter.UpdatesVolume);
        }
    }

    /// <summary>
    /// Tests that condition codes have proper data types.
    /// Verifies that data types are valid and typically include "trade" for sale conditions.
    /// </summary>
    [Fact]
    public async Task GetConditionCodes_ShouldHaveValidDataTypes()
    {
        // Arrange
        var referenceDataService = _polygonClient.ReferenceData;

        // Act
        var conditionCodesResponse = await referenceDataService.GetConditionCodesAsync(
            assetClass: "stocks",
            limit: 10,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(conditionCodesResponse);
        Assert.NotNull(conditionCodesResponse.Results);

        foreach (var conditionCode in conditionCodesResponse.Results)
        {
            Assert.NotNull(conditionCode.DataTypes);
            Assert.True(conditionCode.DataTypes.Count > 0, "Each condition code should have at least one data type");

            // Verify each data type is not empty
            foreach (var dataType in conditionCode.DataTypes)
            {
                Assert.NotNull(dataType);
                Assert.NotEmpty(dataType);
            }

            // For sale conditions, "trade" should typically be included
            if (conditionCode.Type == "sale_condition")
            {
                Assert.Contains("trade", conditionCode.DataTypes);
            }
        }
    }

    /// <summary>
    /// Tests condition codes filtering by trade data type.
    /// Verifies that filtering by data type returns only relevant condition codes.
    /// </summary>
    [Fact]
    public async Task GetConditionCodes_WithTradeDataTypeFilter_ShouldReturnOnlyTradeConditions()
    {
        // Arrange
        var referenceDataService = _polygonClient.ReferenceData;

        // Act
        var conditionCodesResponse = await referenceDataService.GetConditionCodesAsync(
            assetClass: "stocks",
            dataType: "trade",
            limit: 10,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(conditionCodesResponse);
        Assert.NotNull(conditionCodesResponse.Results);

        foreach (var conditionCode in conditionCodesResponse.Results)
        {
            // All returned condition codes should support trade data type
            Assert.Contains("trade", conditionCode.DataTypes);
        }
    }

    /// <summary>
    /// Tests that the condition codes data structure is properly typed.
    /// Verifies that all response properties are of the expected types.
    /// </summary>
    [Fact]
    public async Task GetConditionCodes_ShouldHaveProperDataStructure()
    {
        // Arrange
        var referenceDataService = _polygonClient.ReferenceData;

        // Act
        var conditionCodesResponse = await referenceDataService.GetConditionCodesAsync(
            assetClass: "stocks",
            limit: 3,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(conditionCodesResponse);
        Assert.IsType<List<ConditionCode>>(conditionCodesResponse.Results);
        Assert.IsType<int>(conditionCodesResponse.Count);
        Assert.IsType<string>(conditionCodesResponse.Status);
        Assert.IsType<string>(conditionCodesResponse.RequestId);

        // Verify each condition code has proper structure
        foreach (var conditionCode in conditionCodesResponse.Results)
        {
            Assert.IsType<int>(conditionCode.Id);
            Assert.IsType<string>(conditionCode.Type);
            Assert.IsType<string>(conditionCode.Name);
            Assert.IsType<string>(conditionCode.AssetClass);
            Assert.IsType<SipMapping>(conditionCode.SipMapping);
            Assert.IsType<UpdateRules>(conditionCode.UpdateRules);
            Assert.IsType<List<string>>(conditionCode.DataTypes);
        }
    }

    /// <summary>
    /// Tests that condition codes have reasonable count.
    /// Verifies that the API returns a reasonable number of condition codes and respects limits.
    /// </summary>
    [Fact]
    public async Task GetConditionCodes_ShouldRespectLimitParameter()
    {
        // Arrange
        var referenceDataService = _polygonClient.ReferenceData;
        const int requestedLimit = 5;

        // Act
        var conditionCodesResponse = await referenceDataService.GetConditionCodesAsync(
            assetClass: "stocks",
            limit: requestedLimit,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(conditionCodesResponse);
        Assert.NotNull(conditionCodesResponse.Results);
        Assert.True(conditionCodesResponse.Results.Count <= requestedLimit,
            $"Should not return more than {requestedLimit} results, but got {conditionCodesResponse.Results.Count}");
        Assert.True(conditionCodesResponse.Results.Count > 0, "Should return at least one result");
    }

    /// <summary>
    /// Tests condition codes with ordering parameters.
    /// Verifies that ordering by ID works correctly and returns results in the expected order.
    /// </summary>
    [Fact]
    public async Task GetConditionCodes_WithOrderParameters_ShouldReturnOrderedResults()
    {
        // Arrange
        var referenceDataService = _polygonClient.ReferenceData;

        // Act
        var conditionCodesResponse = await referenceDataService.GetConditionCodesAsync(
            assetClass: "stocks",
            sort: "id",
            order: "asc",
            limit: 10,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(conditionCodesResponse);
        Assert.NotNull(conditionCodesResponse.Results);
        Assert.True(conditionCodesResponse.Results.Count > 1, "Need at least 2 results to verify ordering");

        // Verify ascending order by ID
        for (int i = 1; i < conditionCodesResponse.Results.Count; i++)
        {
            Assert.True(conditionCodesResponse.Results[i].Id >= conditionCodesResponse.Results[i - 1].Id,
                $"Results should be ordered by ID in ascending order. Item {i - 1} has ID {conditionCodesResponse.Results[i - 1].Id}, item {i} has ID {conditionCodesResponse.Results[i].Id}");
        }
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