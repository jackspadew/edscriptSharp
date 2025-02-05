using Xunit;
using PsEdScript.Parser;

namespace PsEdScript.Tests;

public class ShebangParser_Tests
{
    [Fact]
    public void ParseExampleShebang_ReturnCorrectValues()
    {
        string firstLine = "#! /usr/bin/pwsh";
        var parsedInfo = ShebangParser.Parse(firstLine);
        Assert.True(parsedInfo.IsShebang);
        Assert.Equal("/usr/bin/pwsh", parsedInfo.Command);
        Assert.Equal("pwsh", parsedInfo.CommandExact);
    }
}
