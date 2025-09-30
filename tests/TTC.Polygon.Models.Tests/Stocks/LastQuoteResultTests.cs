// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using System.Text.Json;
using TTC.Polygon.Models.Stocks;

namespace TTC.Polygon.Models.Tests.Stocks;

/// <summary>
/// Unit tests for the LastQuoteResult model.
/// </summary>
public class LastQuoteResultTests
{
    [Fact]
    public void LastQuoteResult_DefaultConstructor_InitializesWithDefaults()
    {
        var lastQuote = new LastQuoteResult();

        Assert.Null(lastQuote.Ticker);
        Assert.Null(lastQuote.AskPrice);
        Assert.Null(lastQuote.BidPrice);
        Assert.Null(lastQuote.AskSize);
        Assert.Null(lastQuote.BidSize);
        Assert.Null(lastQuote.AskExchange);
        Assert.Null(lastQuote.BidExchange);
        Assert.Null(lastQuote.Tape);
        Assert.Null(lastQuote.Timestamp);
        Assert.Null(lastQuote.ParticipantTimestamp);
        Assert.Null(lastQuote.Sequence);
        Assert.Null(lastQuote.Indicators);
    }

    [Fact]
    public void LastQuoteResult_Deserialization_WithCompleteData_DeserializesCorrectly()
    {
        var json = """
        {
            "P": 254.05,
            "S": 2,
            "T": "AAPL",
            "X": 12,
            "i": [604],
            "p": 254,
            "q": 69066304,
            "s": 6,
            "t": 1759266523134155000,
            "x": 8,
            "y": 1759266523134142700,
            "z": 3
        }
        """;

        var lastQuote = JsonSerializer.Deserialize<LastQuoteResult>(json);

        Assert.NotNull(lastQuote);
        Assert.Equal("AAPL", lastQuote.Ticker);
        Assert.Equal(254.05m, lastQuote.BidPrice);
        Assert.Equal(2, lastQuote.BidSize);
        Assert.Equal(254m, lastQuote.AskPrice);
        Assert.Equal(6, lastQuote.AskSize);
        Assert.Equal(12, lastQuote.BidExchange);
        Assert.Equal(8, lastQuote.AskExchange);
        Assert.Equal(3, lastQuote.Tape);
        Assert.Equal(1759266523134155000L, lastQuote.Timestamp);
        Assert.Equal(1759266523134142700L, lastQuote.ParticipantTimestamp);
        Assert.Equal(69066304L, lastQuote.Sequence);
        Assert.NotNull(lastQuote.Indicators);
        Assert.Single(lastQuote.Indicators);
        Assert.Equal(604, lastQuote.Indicators[0]);
    }

    [Fact]
    public void LastQuoteResult_Deserialization_WithMinimalData_DeserializesCorrectly()
    {
        var json = """
        {
            "T": "MSFT",
            "P": 100.50,
            "p": 100.55
        }
        """;

        var lastQuote = JsonSerializer.Deserialize<LastQuoteResult>(json);

        Assert.NotNull(lastQuote);
        Assert.Equal("MSFT", lastQuote.Ticker);
        Assert.Equal(100.50m, lastQuote.BidPrice);
        Assert.Equal(100.55m, lastQuote.AskPrice);
        Assert.Null(lastQuote.BidSize);
        Assert.Null(lastQuote.AskSize);
        Assert.Null(lastQuote.BidExchange);
        Assert.Null(lastQuote.AskExchange);
        Assert.Null(lastQuote.Tape);
        Assert.Null(lastQuote.Timestamp);
        Assert.Null(lastQuote.ParticipantTimestamp);
        Assert.Null(lastQuote.Sequence);
        Assert.Null(lastQuote.Indicators);
    }

    [Fact]
    public void LastQuoteResult_Serialization_WithCompleteData_SerializesCorrectly()
    {
        var lastQuote = new LastQuoteResult
        {
            Ticker = "AAPL",
            BidPrice = 254.05m,
            BidSize = 2,
            AskPrice = 254m,
            AskSize = 6,
            BidExchange = 12,
            AskExchange = 8,
            Tape = 3,
            Timestamp = 1759266523134155000L,
            ParticipantTimestamp = 1759266523134142700L,
            Sequence = 69066304L,
            Indicators = [604]
        };

        var json = JsonSerializer.Serialize(lastQuote);
        var deserialized = JsonSerializer.Deserialize<LastQuoteResult>(json);

        Assert.NotNull(deserialized);
        Assert.Equal(lastQuote.Ticker, deserialized.Ticker);
        Assert.Equal(lastQuote.BidPrice, deserialized.BidPrice);
        Assert.Equal(lastQuote.BidSize, deserialized.BidSize);
        Assert.Equal(lastQuote.AskPrice, deserialized.AskPrice);
        Assert.Equal(lastQuote.AskSize, deserialized.AskSize);
        Assert.Equal(lastQuote.BidExchange, deserialized.BidExchange);
        Assert.Equal(lastQuote.AskExchange, deserialized.AskExchange);
        Assert.Equal(lastQuote.Tape, deserialized.Tape);
        Assert.Equal(lastQuote.Timestamp, deserialized.Timestamp);
        Assert.Equal(lastQuote.ParticipantTimestamp, deserialized.ParticipantTimestamp);
        Assert.Equal(lastQuote.Sequence, deserialized.Sequence);
        Assert.NotNull(deserialized.Indicators);
        Assert.Equal(lastQuote.Indicators.Count, deserialized.Indicators.Count);
    }

    [Fact]
    public void LastQuoteResult_Deserialization_WithMultipleIndicators_DeserializesCorrectly()
    {
        var json = """
        {
            "T": "GOOGL",
            "P": 150.25,
            "p": 150.30,
            "i": [604, 605, 606]
        }
        """;

        var lastQuote = JsonSerializer.Deserialize<LastQuoteResult>(json);

        Assert.NotNull(lastQuote);
        Assert.NotNull(lastQuote.Indicators);
        Assert.Equal(3, lastQuote.Indicators.Count);
        Assert.Equal(604, lastQuote.Indicators[0]);
        Assert.Equal(605, lastQuote.Indicators[1]);
        Assert.Equal(606, lastQuote.Indicators[2]);
    }

    [Fact]
    public void LastQuoteResult_Deserialization_WithEmptyIndicators_DeserializesCorrectly()
    {
        var json = """
        {
            "T": "TSLA",
            "P": 200.00,
            "p": 200.10,
            "i": []
        }
        """;

        var lastQuote = JsonSerializer.Deserialize<LastQuoteResult>(json);

        Assert.NotNull(lastQuote);
        Assert.NotNull(lastQuote.Indicators);
        Assert.Empty(lastQuote.Indicators);
    }

    [Theory]
    [InlineData(1, 2, 3)]
    [InlineData(4, 8, 12)]
    [InlineData(11, 12, 15)]
    public void LastQuoteResult_Tape_AcceptsValidValues(int tape, int bidExchange, int askExchange)
    {
        var json = $$"""
        {
            "T": "TEST",
            "P": 100.00,
            "p": 100.10,
            "z": {{tape}},
            "X": {{bidExchange}},
            "x": {{askExchange}}
        }
        """;

        var lastQuote = JsonSerializer.Deserialize<LastQuoteResult>(json);

        Assert.NotNull(lastQuote);
        Assert.Equal(tape, lastQuote.Tape);
        Assert.Equal(bidExchange, lastQuote.BidExchange);
        Assert.Equal(askExchange, lastQuote.AskExchange);
    }
}
