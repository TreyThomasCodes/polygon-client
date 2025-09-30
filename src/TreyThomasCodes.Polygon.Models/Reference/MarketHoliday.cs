// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using System.Text.Json.Serialization;
using NodaTime;
using NodaTime.Text;

namespace TreyThomasCodes.Polygon.Models.Reference;

/// <summary>
/// Represents an upcoming market holiday for a specific exchange.
/// Contains information about holiday dates, affected exchanges, and market operating hours during holidays.
/// </summary>
public class MarketHoliday
{
    /// <summary>
    /// Gets or sets the date of the market holiday in YYYY-MM-DD format.
    /// Represents the calendar date when the holiday occurs.
    /// </summary>
    [JsonPropertyName("date")]
    public string? Date { get; set; }

    /// <summary>
    /// Gets or sets the exchange affected by this holiday.
    /// Examples include "NYSE", "NASDAQ", etc.
    /// </summary>
    [JsonPropertyName("exchange")]
    public string? Exchange { get; set; }

    /// <summary>
    /// Gets or sets the name of the holiday.
    /// Examples include "Thanksgiving", "Christmas", "New Years Day", etc.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the market status for this holiday.
    /// Possible values include "closed" for full closures and "early-close" for shortened trading days.
    /// </summary>
    [JsonPropertyName("status")]
    public string? Status { get; set; }

    /// <summary>
    /// Gets or sets the market opening time on this holiday in ISO 8601 format.
    /// Only present when Status is "early-close", indicating modified trading hours.
    /// </summary>
    [JsonPropertyName("open")]
    public string? Open { get; set; }

    /// <summary>
    /// Gets or sets the market closing time on this holiday in ISO 8601 format.
    /// Only present when Status is "early-close", indicating modified trading hours.
    /// </summary>
    [JsonPropertyName("close")]
    public string? Close { get; set; }

    /// <summary>
    /// Gets the holiday date converted to a LocalDate.
    /// Returns null if Date is null or cannot be parsed.
    /// </summary>
    [JsonIgnore]
    public LocalDate? HolidayDate
    {
        get
        {
            if (string.IsNullOrEmpty(Date)) return null;

            var pattern = LocalDatePattern.CreateWithInvariantCulture("yyyy-MM-dd");
            var parseResult = pattern.Parse(Date);

            return parseResult.Success ? parseResult.Value : null;
        }
    }

    /// <summary>
    /// Gets the market opening time converted to a ZonedDateTime in Eastern Time.
    /// Returns null if Open is null or cannot be parsed.
    /// </summary>
    [JsonIgnore]
    public ZonedDateTime? OpenTime
    {
        get
        {
            if (string.IsNullOrEmpty(Open)) return null;

            var pattern = OffsetDateTimePattern.CreateWithInvariantCulture("yyyy-MM-ddTHH:mm:ss.fffK");
            var parseResult = pattern.Parse(Open);

            if (!parseResult.Success) return null;

            var offsetDateTime = parseResult.Value;
            var instant = offsetDateTime.ToInstant();
            var easternZone = DateTimeZoneProviders.Tzdb["America/New_York"];
            return instant.InZone(easternZone);
        }
    }

    /// <summary>
    /// Gets the market closing time converted to a ZonedDateTime in Eastern Time.
    /// Returns null if Close is null or cannot be parsed.
    /// </summary>
    [JsonIgnore]
    public ZonedDateTime? CloseTime
    {
        get
        {
            if (string.IsNullOrEmpty(Close)) return null;

            var pattern = OffsetDateTimePattern.CreateWithInvariantCulture("yyyy-MM-ddTHH:mm:ss.fffK");
            var parseResult = pattern.Parse(Close);

            if (!parseResult.Success) return null;

            var offsetDateTime = parseResult.Value;
            var instant = offsetDateTime.ToInstant();
            var easternZone = DateTimeZoneProviders.Tzdb["America/New_York"];
            return instant.InZone(easternZone);
        }
    }
}