using Xunit;
using PsEdScript.Parser;

namespace PsEdScript.Tests;

public class ShebangParser_Tests
{
    [Theory]
    [InlineData("#! /usr/bin/pwsh")]
    [InlineData("#!    /usr/bin/pwsh   ")]
    [InlineData("#! \t /usr/bin/pwsh \t ")]
    public void ParseExampleShebang_ReturnCorrectValues(string firstLine)
    {
        var parsedInfo = ShebangParser.Parse(firstLine);
        Assert.True(parsedInfo.IsShebang);
        Assert.Equal("/usr/bin/pwsh", parsedInfo.Command);
        Assert.Equal("pwsh", parsedInfo.CommandExact);
    }

    [Theory]
    [InlineData("#! /usr/bin/env pwsh")]
    [InlineData("#!    /usr/bin/env   pwsh   ")]
    [InlineData("#! \t /usr/bin/env\tpwsh \t ")]
    public void ParseExampleShebangWithCommandDefinedInEnv_ReturnCorrectValues(string firstLine)
    {
        var parsedInfo = ShebangParser.Parse(firstLine);
        Assert.True(parsedInfo.IsShebang);
        Assert.Equal("/usr/bin/env pwsh", parsedInfo.Command);
        Assert.Equal("pwsh", parsedInfo.CommandExact);
    }

    [Theory]
    [InlineData("#! /usr/bin/pwsh\r\ntext\r\ntext", "/usr/bin/pwsh")]
    [InlineData("#! /usr/bin/pwsh\ntext\ntext", "/usr/bin/pwsh")]
    [InlineData("#! /usr/bin/env pwsh\r\ntext\r\ntext", "/usr/bin/env pwsh")]
    [InlineData("#! /usr/bin/env pwsh\ntext\ntext", "/usr/bin/env pwsh")]
    public void ParseExampleShebangWithMultipleLines_ReturnCorrectValues(string firstLine, string actualCommand)
    {
        var parsedInfo = ShebangParser.Parse(firstLine);
        Assert.True(parsedInfo.IsShebang);
        Assert.Equal(actualCommand, parsedInfo.Command);
        Assert.Equal("pwsh", parsedInfo.CommandExact);
    }
}
