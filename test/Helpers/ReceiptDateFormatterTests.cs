using System;
using Altinn.Platform.Receipt.Helpers;
using Xunit;

namespace Altinn.Platform.Receipt.Tests.Helpers;

public class ReceiptDateFormatterTests
{
    [Fact]
    public void FormatDateTime_WinterTime_IsOneHourAheadOfUtc()
    {
        DateTime utc = new(2024, 1, 15, 9, 30, 0, DateTimeKind.Utc);

        Assert.Equal("15.01.2024 / 10:30", ReceiptDateFormatter.FormatDateTime(utc));
    }

    [Fact]
    public void FormatDateTime_SummerTime_IsTwoHoursAheadOfUtc()
    {
        DateTime utc = new(2024, 7, 15, 9, 30, 0, DateTimeKind.Utc);

        Assert.Equal("15.07.2024 / 11:30", ReceiptDateFormatter.FormatDateTime(utc));
    }

    [Fact]
    public void FormatDateTime_NoValue_ReturnsNull()
    {
        Assert.Null(ReceiptDateFormatter.FormatDateTime(null));
    }
}
