// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using System.Globalization;
using System.Text.RegularExpressions;

namespace TreyThomasCodes.Polygon.Models.Options;

/// <summary>
/// Represents an OCC (Options Clearing Corporation) format options ticker and provides utilities for constructing and parsing options ticker symbols.
/// The OCC ticker format is: O:[UNDERLYING][YYMMDD][C/P][STRIKE_PRICE_PADDED]
/// Example: "O:UBER220121C00050000" represents a Call option on UBER expiring January 21, 2022 with a $50 strike price.
/// </summary>
public class OptionsTicker
{
    private const string OccPrefix = "O:";
    private static readonly Regex TickerPattern = new(
        @"^O:([A-Z]+)(\d{6})([CP])(\d{8})$",
        RegexOptions.Compiled);

    /// <summary>
    /// Gets the underlying asset ticker symbol (e.g., "UBER", "SPY", "F").
    /// </summary>
    public string Underlying { get; }

    /// <summary>
    /// Gets the expiration date of the options contract.
    /// </summary>
    public DateTime ExpirationDate { get; }

    /// <summary>
    /// Gets the type of the options contract (Call or Put).
    /// </summary>
    public OptionType Type { get; }

    /// <summary>
    /// Gets the strike price of the options contract.
    /// </summary>
    public decimal Strike { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="OptionsTicker"/> class.
    /// </summary>
    /// <param name="underlying">The underlying asset ticker symbol.</param>
    /// <param name="expirationDate">The expiration date of the options contract.</param>
    /// <param name="type">The type of the options contract (Call or Put).</param>
    /// <param name="strike">The strike price of the options contract.</param>
    /// <exception cref="ArgumentNullException">Thrown when underlying is null or whitespace.</exception>
    /// <exception cref="ArgumentException">Thrown when underlying contains invalid characters or strike price is negative.</exception>
    public OptionsTicker(string underlying, DateTime expirationDate, OptionType type, decimal strike)
    {
        if (string.IsNullOrWhiteSpace(underlying))
            throw new ArgumentNullException(nameof(underlying), "Underlying ticker symbol cannot be null or empty.");

        if (!Regex.IsMatch(underlying, "^[A-Za-z]+$"))
            throw new ArgumentException("Underlying ticker symbol must contain only letters.", nameof(underlying));

        if (strike < 0)
            throw new ArgumentException("Strike price cannot be negative.", nameof(strike));

        Underlying = underlying.ToUpperInvariant();
        ExpirationDate = expirationDate.Date;
        Type = type;
        Strike = strike;
    }

    /// <summary>
    /// Creates an OCC format options ticker string from the specified components.
    /// </summary>
    /// <param name="underlying">The underlying asset ticker symbol (e.g., "UBER", "SPY").</param>
    /// <param name="expirationDate">The expiration date of the options contract.</param>
    /// <param name="type">The type of the options contract (Call or Put).</param>
    /// <param name="strike">The strike price of the options contract (e.g., 50 for $50).</param>
    /// <returns>A formatted OCC options ticker string (e.g., "O:UBER220121C00050000").</returns>
    /// <exception cref="ArgumentNullException">Thrown when underlying is null or whitespace.</exception>
    /// <exception cref="ArgumentException">Thrown when underlying contains invalid characters or strike price is negative.</exception>
    /// <example>
    /// <code>
    /// // Create a Call option ticker for UBER expiring January 21, 2022 with $50 strike
    /// string ticker = OptionsTicker.Create("UBER", new DateTime(2022, 1, 21), OptionType.Call, 50m);
    /// // Returns: "O:UBER220121C00050000"
    ///
    /// // Create a Put option ticker for Ford (F) expiring November 19, 2021 with $14 strike
    /// string ticker = OptionsTicker.Create("F", new DateTime(2021, 11, 19), OptionType.Put, 14m);
    /// // Returns: "O:F211119P00014000"
    /// </code>
    /// </example>
    public static string Create(string underlying, DateTime expirationDate, OptionType type, decimal strike)
    {
        var ticker = new OptionsTicker(underlying, expirationDate, type, strike);
        return ticker.ToString();
    }

    /// <summary>
    /// Parses an OCC format options ticker string into its component parts.
    /// </summary>
    /// <param name="ticker">The OCC format options ticker string (e.g., "O:UBER220121C00050000").</param>
    /// <returns>An <see cref="OptionsTicker"/> instance containing the parsed components.</returns>
    /// <exception cref="ArgumentNullException">Thrown when ticker is null or whitespace.</exception>
    /// <exception cref="FormatException">Thrown when ticker is not in valid OCC format.</exception>
    /// <example>
    /// <code>
    /// var parsed = OptionsTicker.Parse("O:UBER220121C00050000");
    /// Console.WriteLine(parsed.Underlying);      // "UBER"
    /// Console.WriteLine(parsed.ExpirationDate);  // 2022-01-21
    /// Console.WriteLine(parsed.Type);            // OptionType.Call
    /// Console.WriteLine(parsed.Strike);          // 50
    /// </code>
    /// </example>
    public static OptionsTicker Parse(string ticker)
    {
        if (TryParse(ticker, out var result))
            return result!;

        throw new FormatException(
            $"The ticker '{ticker}' is not in valid OCC format. " +
            "Expected format: O:[UNDERLYING][YYMMDD][C/P][STRIKE_PRICE_PADDED] " +
            "Example: O:UBER220121C00050000");
    }

    /// <summary>
    /// Attempts to parse an OCC format options ticker string into its component parts.
    /// </summary>
    /// <param name="ticker">The OCC format options ticker string (e.g., "O:UBER220121C00050000").</param>
    /// <param name="result">When this method returns, contains the parsed <see cref="OptionsTicker"/> if parsing succeeded, or null if parsing failed.</param>
    /// <returns>true if the ticker was successfully parsed; otherwise, false.</returns>
    /// <example>
    /// <code>
    /// if (OptionsTicker.TryParse("O:UBER220121C00050000", out var ticker))
    /// {
    ///     Console.WriteLine($"Strike: {ticker.Strike}");
    /// }
    /// else
    /// {
    ///     Console.WriteLine("Invalid ticker format");
    /// }
    /// </code>
    /// </example>
    public static bool TryParse(string? ticker, out OptionsTicker? result)
    {
        result = null;

        if (string.IsNullOrWhiteSpace(ticker))
            return false;

        var match = TickerPattern.Match(ticker);
        if (!match.Success)
            return false;

        try
        {
            string underlying = match.Groups[1].Value;
            string dateStr = match.Groups[2].Value;
            char typeChar = match.Groups[3].Value[0];
            string strikeStr = match.Groups[4].Value;

            // Parse date (YYMMDD format)
            int year = int.Parse(dateStr.Substring(0, 2), CultureInfo.InvariantCulture) + 2000;
            int month = int.Parse(dateStr.Substring(2, 2), CultureInfo.InvariantCulture);
            int day = int.Parse(dateStr.Substring(4, 2), CultureInfo.InvariantCulture);
            var expirationDate = new DateTime(year, month, day);

            // Parse type
            var type = typeChar == 'C' ? OptionType.Call : OptionType.Put;

            // Parse strike price (divide by 1000 to get actual price)
            decimal strike = decimal.Parse(strikeStr, CultureInfo.InvariantCulture) / 1000m;

            result = new OptionsTicker(underlying, expirationDate, type, strike);
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Returns the OCC format options ticker string representation.
    /// </summary>
    /// <returns>A formatted OCC options ticker string (e.g., "O:UBER220121C00050000").</returns>
    public override string ToString()
    {
        // Format: O:UNDERLYING + YYMMDD + C/P + STRIKE_PADDED
        string dateStr = ExpirationDate.ToString("yyMMdd", CultureInfo.InvariantCulture);
        char typeChar = Type == OptionType.Call ? 'C' : 'P';

        // Strike price is multiplied by 1000 and padded to 8 digits
        int strikePadded = (int)(Strike * 1000);
        string strikeStr = strikePadded.ToString("D8", CultureInfo.InvariantCulture);

        return $"{OccPrefix}{Underlying}{dateStr}{typeChar}{strikeStr}";
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current <see cref="OptionsTicker"/>.
    /// </summary>
    /// <param name="obj">The object to compare with the current instance.</param>
    /// <returns>true if the specified object is equal to the current instance; otherwise, false.</returns>
    public override bool Equals(object? obj)
    {
        if (obj is OptionsTicker other)
        {
            return Underlying == other.Underlying &&
                   ExpirationDate == other.ExpirationDate &&
                   Type == other.Type &&
                   Strike == other.Strike;
        }
        return false;
    }

    /// <summary>
    /// Serves as the default hash function.
    /// </summary>
    /// <returns>A hash code for the current object.</returns>
    public override int GetHashCode()
    {
        return HashCode.Combine(Underlying, ExpirationDate, Type, Strike);
    }
}
