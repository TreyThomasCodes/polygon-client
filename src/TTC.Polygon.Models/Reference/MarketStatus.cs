// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using System.Text.Json.Serialization;
using NodaTime;
using NodaTime.Text;

namespace TTC.Polygon.Models.Reference;

/// <summary>
/// Represents the current trading status for various exchanges and overall financial markets.
/// Contains real-time information about market hours, exchange status, and trading sessions.
/// </summary>
public class MarketStatus
{
    /// <summary>
    /// Gets or sets whether the market is currently in post-market (after-hours) trading session.
    /// True indicates that regular market hours have ended and after-hours trading is active.
    /// </summary>
    [JsonPropertyName("afterHours")]
    public bool AfterHours { get; set; }

    /// <summary>
    /// Gets or sets the status of currency markets.
    /// Contains information about forex and currency trading status.
    /// </summary>
    [JsonPropertyName("currencies")]
    public CurrencyMarketsStatus? Currencies { get; set; }

    /// <summary>
    /// Gets or sets whether the market is currently in pre-market (early hours) trading session.
    /// True indicates that pre-market trading is active before regular market hours begin.
    /// </summary>
    [JsonPropertyName("earlyHours")]
    public bool EarlyHours { get; set; }

    /// <summary>
    /// Gets or sets the status of US stock exchanges.
    /// Contains detailed information about individual exchange trading status.
    /// </summary>
    [JsonPropertyName("exchanges")]
    public ExchangesStatus? Exchanges { get; set; }

    /// <summary>
    /// Gets or sets the status of various index groups.
    /// Contains information about the trading status of different market indices.
    /// </summary>
    [JsonPropertyName("indicesGroups")]
    public IndicesGroupsStatus? IndicesGroups { get; set; }

    /// <summary>
    /// Gets or sets the overall market status.
    /// Provides a high-level indication of the market state (e.g., "open", "closed", "extended-hours").
    /// </summary>
    [JsonPropertyName("market")]
    public string? Market { get; set; }

    /// <summary>
    /// Gets or sets the current server time in RFC3339 format.
    /// Represents the timestamp when this market status information was generated.
    /// </summary>
    [JsonPropertyName("serverTime")]
    public string? ServerTime { get; set; }

    /// <summary>
    /// Gets the server time converted to Eastern Time.
    /// Returns null if ServerTime is null or cannot be parsed.
    /// </summary>
    [JsonIgnore]
    public ZonedDateTime? MarketServerTime
    {
        get
        {
            if (string.IsNullOrEmpty(ServerTime)) return null;

            var pattern = OffsetDateTimePattern.CreateWithInvariantCulture("yyyy-MM-ddTHH:mm:ss.fffK");
            var parseResult = pattern.Parse(ServerTime);

            if (!parseResult.Success) return null;

            var offsetDateTime = parseResult.Value;
            var instant = offsetDateTime.ToInstant();
            var easternZone = DateTimeZoneProviders.Tzdb["America/New_York"];
            return instant.InZone(easternZone);
        }
    }
}

/// <summary>
/// Represents the trading status of currency (forex) markets.
/// Contains information about foreign exchange market hours and availability.
/// </summary>
public class CurrencyMarketsStatus
{
    /// <summary>
    /// Gets or sets the foreign exchange market status.
    /// Indicates whether forex trading is currently active.
    /// </summary>
    [JsonPropertyName("fx")]
    public string? Forex { get; set; }

    /// <summary>
    /// Gets or sets the cryptocurrency market status.
    /// Indicates the current status of cryptocurrency trading markets.
    /// </summary>
    [JsonPropertyName("crypto")]
    public string? Crypto { get; set; }
}

/// <summary>
/// Represents the trading status of US stock exchanges.
/// Contains detailed status information for major US stock exchanges.
/// </summary>
public class ExchangesStatus
{
    /// <summary>
    /// Gets or sets the New York Stock Exchange (NYSE) trading status.
    /// Indicates whether NYSE is currently open for trading.
    /// </summary>
    [JsonPropertyName("nyse")]
    public string? NYSE { get; set; }

    /// <summary>
    /// Gets or sets the NASDAQ exchange trading status.
    /// Indicates whether NASDAQ is currently open for trading.
    /// </summary>
    [JsonPropertyName("nasdaq")]
    public string? NASDAQ { get; set; }

    /// <summary>
    /// Gets or sets the OTC (Over-The-Counter) markets trading status.
    /// Indicates the status of over-the-counter trading venues.
    /// </summary>
    [JsonPropertyName("otc")]
    public string? OTC { get; set; }
}

/// <summary>
/// Represents the trading status of various market index groups.
/// Contains status information for different categories of market indices.
/// </summary>
public class IndicesGroupsStatus
{
    /// <summary>
    /// Gets or sets the S&amp;P index group trading status.
    /// Indicates the status of S&amp;P indices trading and calculation.
    /// </summary>
    [JsonPropertyName("s_and_p")]
    public string? SAndP { get; set; }

    /// <summary>
    /// Gets or sets the societe generale index group trading status.
    /// Indicates the status of Societe Generale indices.
    /// </summary>
    [JsonPropertyName("societe_generale")]
    public string? SocieteGenerale { get; set; }

    /// <summary>
    /// Gets or sets the MSCI index group trading status.
    /// Indicates the status of MSCI indices trading and calculation.
    /// </summary>
    [JsonPropertyName("msci")]
    public string? MSCI { get; set; }

    /// <summary>
    /// Gets or sets the FTSE Russell index group trading status.
    /// Indicates the status of FTSE Russell indices.
    /// </summary>
    [JsonPropertyName("ftse_russell")]
    public string? FTSERussell { get; set; }

    /// <summary>
    /// Gets or sets the MSCI index group trading status.
    /// Indicates the status of additional MSCI index products.
    /// </summary>
    [JsonPropertyName("mstar")]
    public string? MStar { get; set; }

    /// <summary>
    /// Gets or sets the ICE index group trading status.
    /// Indicates the status of Intercontinental Exchange indices.
    /// </summary>
    [JsonPropertyName("ice")]
    public string? ICE { get; set; }

    /// <summary>
    /// Gets or sets the Dow Jones index group trading status.
    /// Indicates the status of Dow Jones indices.
    /// </summary>
    [JsonPropertyName("dow_jones")]
    public string? DowJones { get; set; }
}