// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using System.Text.Json;
using TreyThomasCodes.Polygon.Models.Reference;

namespace TreyThomasCodes.Polygon.Models.Tests.Reference;

/// <summary>
/// Unit tests for the Exchange model.
/// </summary>
public class ExchangeTests
{
    [Fact]
    public void Exchange_DefaultConstructor_InitializesWithDefaults()
    {
        var exchange = new Exchange();

        Assert.Equal(0, exchange.Id);
        Assert.Equal(string.Empty, exchange.Type);
        Assert.Equal(string.Empty, exchange.AssetClass);
        Assert.Equal(string.Empty, exchange.Locale);
        Assert.Equal(string.Empty, exchange.Name);
        Assert.Equal(string.Empty, exchange.OperatingMic);
        Assert.Null(exchange.Acronym);
        Assert.Null(exchange.Mic);
        Assert.Null(exchange.ParticipantId);
        Assert.Null(exchange.Url);
    }

    [Fact]
    public void Exchange_Deserialization_WithCompleteData_DeserializesCorrectly()
    {
        var json = """
        {
            "id": 1,
            "type": "exchange",
            "asset_class": "stocks",
            "locale": "us",
            "name": "New York Stock Exchange",
            "acronym": "NYSE",
            "mic": "XNYS",
            "operating_mic": "XNYS",
            "participant_id": "N",
            "url": "https://www.nyse.com"
        }
        """;

        var exchange = JsonSerializer.Deserialize<Exchange>(json);

        Assert.NotNull(exchange);
        Assert.Equal(1, exchange.Id);
        Assert.Equal("exchange", exchange.Type);
        Assert.Equal("stocks", exchange.AssetClass);
        Assert.Equal("us", exchange.Locale);
        Assert.Equal("New York Stock Exchange", exchange.Name);
        Assert.Equal("NYSE", exchange.Acronym);
        Assert.Equal("XNYS", exchange.Mic);
        Assert.Equal("XNYS", exchange.OperatingMic);
        Assert.Equal("N", exchange.ParticipantId);
        Assert.Equal("https://www.nyse.com", exchange.Url);
    }

    [Fact]
    public void Exchange_Deserialization_WithMinimalData_DeserializesCorrectly()
    {
        var json = """
        {
            "id": 5,
            "type": "SIP",
            "asset_class": "stocks",
            "locale": "us",
            "name": "Unlisted Trading Privileges",
            "operating_mic": "XNAS",
            "participant_id": "E",
            "url": "https://www.utpplan.com"
        }
        """;

        var exchange = JsonSerializer.Deserialize<Exchange>(json);

        Assert.NotNull(exchange);
        Assert.Equal(5, exchange.Id);
        Assert.Equal("SIP", exchange.Type);
        Assert.Equal("stocks", exchange.AssetClass);
        Assert.Equal("us", exchange.Locale);
        Assert.Equal("Unlisted Trading Privileges", exchange.Name);
        Assert.Null(exchange.Acronym);
        Assert.Null(exchange.Mic);
        Assert.Equal("XNAS", exchange.OperatingMic);
        Assert.Equal("E", exchange.ParticipantId);
        Assert.Equal("https://www.utpplan.com", exchange.Url);
    }

    [Fact]
    public void Exchange_Deserialization_WithNullOptionalFields_DeserializesCorrectly()
    {
        var json = """
        {
            "id": 62,
            "type": "ORF",
            "asset_class": "stocks",
            "locale": "us",
            "name": "OTC Equity Security",
            "mic": "OOTC",
            "operating_mic": "FINR",
            "url": "https://www.finra.org/filing-reporting/over-the-counter-reporting-facility-orf",
            "acronym": null,
            "participant_id": null
        }
        """;

        var exchange = JsonSerializer.Deserialize<Exchange>(json);

        Assert.NotNull(exchange);
        Assert.Equal(62, exchange.Id);
        Assert.Equal("ORF", exchange.Type);
        Assert.Equal("stocks", exchange.AssetClass);
        Assert.Equal("us", exchange.Locale);
        Assert.Equal("OTC Equity Security", exchange.Name);
        Assert.Null(exchange.Acronym);
        Assert.Equal("OOTC", exchange.Mic);
        Assert.Equal("FINR", exchange.OperatingMic);
        Assert.Null(exchange.ParticipantId);
        Assert.Equal("https://www.finra.org/filing-reporting/over-the-counter-reporting-facility-orf", exchange.Url);
    }

    [Fact]
    public void Exchange_Serialization_WithCompleteData_SerializesCorrectly()
    {
        var exchange = new Exchange
        {
            Id = 10,
            Type = "exchange",
            AssetClass = "stocks",
            Locale = "us",
            Name = "New York Stock Exchange",
            Acronym = "NYSE",
            Mic = "XNYS",
            OperatingMic = "XNYS",
            ParticipantId = "N",
            Url = "https://www.nyse.com"
        };

        var json = JsonSerializer.Serialize(exchange);
        var deserialized = JsonSerializer.Deserialize<Exchange>(json);

        Assert.NotNull(deserialized);
        Assert.Equal(exchange.Id, deserialized.Id);
        Assert.Equal(exchange.Type, deserialized.Type);
        Assert.Equal(exchange.AssetClass, deserialized.AssetClass);
        Assert.Equal(exchange.Locale, deserialized.Locale);
        Assert.Equal(exchange.Name, deserialized.Name);
        Assert.Equal(exchange.Acronym, deserialized.Acronym);
        Assert.Equal(exchange.Mic, deserialized.Mic);
        Assert.Equal(exchange.OperatingMic, deserialized.OperatingMic);
        Assert.Equal(exchange.ParticipantId, deserialized.ParticipantId);
        Assert.Equal(exchange.Url, deserialized.Url);
    }

    [Theory]
    [InlineData("stocks")]
    [InlineData("options")]
    [InlineData("crypto")]
    [InlineData("fx")]
    public void Exchange_AssetClass_AcceptsValidValues(string assetClass)
    {
        var json = $$"""
        {
            "id": 1,
            "type": "exchange",
            "asset_class": "{{assetClass}}",
            "locale": "us",
            "name": "Test Exchange",
            "operating_mic": "TEST"
        }
        """;

        var exchange = JsonSerializer.Deserialize<Exchange>(json);

        Assert.NotNull(exchange);
        Assert.Equal(assetClass, exchange.AssetClass);
    }

    [Theory]
    [InlineData("exchange")]
    [InlineData("TRF")]
    [InlineData("SIP")]
    [InlineData("ORF")]
    public void Exchange_Type_AcceptsValidValues(string type)
    {
        var json = $$"""
        {
            "id": 1,
            "type": "{{type}}",
            "asset_class": "stocks",
            "locale": "us",
            "name": "Test Exchange",
            "operating_mic": "TEST"
        }
        """;

        var exchange = JsonSerializer.Deserialize<Exchange>(json);

        Assert.NotNull(exchange);
        Assert.Equal(type, exchange.Type);
    }

    [Theory]
    [InlineData("us")]
    [InlineData("global")]
    public void Exchange_Locale_AcceptsValidValues(string locale)
    {
        var json = $$"""
        {
            "id": 1,
            "type": "exchange",
            "asset_class": "stocks",
            "locale": "{{locale}}",
            "name": "Test Exchange",
            "operating_mic": "TEST"
        }
        """;

        var exchange = JsonSerializer.Deserialize<Exchange>(json);

        Assert.NotNull(exchange);
        Assert.Equal(locale, exchange.Locale);
    }
}