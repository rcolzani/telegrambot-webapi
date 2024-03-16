using Telegram.WebAPI.Shared.Extensions;

namespace Telegram.Shared.UnitTests;

public class StringExtensionTests
{
    [Theory]
    [InlineData("áãóç", "aaoc")]
    [InlineData("", "")]
    [InlineData(" ", " ")]
    [InlineData(null, null)]
    [InlineData("á", "b")]
    public async void ItRemovesAccents(string value, string expectedResult)
    {
        var result = value.RemoveAccents();
        Assert.Equal(expectedResult, result);
    }
}
