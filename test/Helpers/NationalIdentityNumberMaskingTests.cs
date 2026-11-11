using Xunit;

namespace Altinn.Platform.Receipt.Helpers.Tests;

public class NationalIdentityNumberMaskingTests
{
    [Theory]
    [InlineData("12345678901", "123456*****")] // ordinary 11-digit number
    [InlineData("1234567", "123456*")] // one extra char masked
    [InlineData("123456", "******")] // exactly the visible length -> fully masked
    [InlineData("123", "***")] // shorter than visible length -> fully masked
    [InlineData("", "")] // empty returned as-is
    [InlineData(null, null)] // null returned as-is
    public void Mask_KeepsBirthDateVisible_MasksTheRest(string input, string expected)
    {
        Assert.Equal(expected, NationalIdentityNumberMasking.Mask(input));
    }

    [Fact]
    public void Mask_AlreadyMaskedNumber_IsUnchanged()
    {
        Assert.Equal("123456*****", NationalIdentityNumberMasking.Mask("123456*****"));
    }
}
