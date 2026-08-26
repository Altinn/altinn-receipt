using System;
using System.Globalization;

namespace Altinn.Platform.Receipt.Helpers;

/// <summary>
/// Formats the timestamps shown on the receipt.
/// </summary>
/// <remarks>
/// Timestamps are stored in UTC, but presented in Norwegian time. The container image must therefore include
/// time zone data, see the Dockerfile.
/// </remarks>
public static class ReceiptDateFormatter
{
    private const string DateTimeFormat = "dd.MM.yyyy / HH:mm";

    private static readonly TimeZoneInfo _norwegianTimeZone = ResolveNorwegianTimeZone();

    /// <summary>
    /// Formats a timestamp as Norwegian local time.
    /// </summary>
    /// <param name="dateTime">The timestamp to format.</param>
    /// <returns>The formatted timestamp, or null when no timestamp is given.</returns>
    public static string FormatDateTime(DateTime? dateTime)
    {
        if (dateTime == null)
        {
            return null;
        }

        DateTime utc =
            dateTime.Value.Kind == DateTimeKind.Unspecified
                ? DateTime.SpecifyKind(dateTime.Value, DateTimeKind.Utc)
                : dateTime.Value.ToUniversalTime();

        DateTime local = TimeZoneInfo.ConvertTimeFromUtc(utc, _norwegianTimeZone);

        return local.ToString(DateTimeFormat, CultureInfo.InvariantCulture);
    }

    private static TimeZoneInfo ResolveNorwegianTimeZone()
    {
        foreach (string id in new[] { "Europe/Oslo", "W. Europe Standard Time" })
        {
            try
            {
                return TimeZoneInfo.FindSystemTimeZoneById(id);
            }
            catch (TimeZoneNotFoundException)
            {
                // Try the next identifier.
            }
            catch (InvalidTimeZoneException)
            {
                // Try the next identifier.
            }
        }

        return TimeZoneInfo.Utc;
    }
}
