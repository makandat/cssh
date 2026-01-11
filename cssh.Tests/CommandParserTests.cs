/* cssh - A cross-platform C# shell ver.0.1.0 CommandParser tests */
using cssh.Core;
using Xunit;

public class CommandParserTests
{
    [Fact]
    public void Parse_ShouldReturnCommandName_WhenOnlyCommandGiven()
    {
        var parser = new CommandParser();
        var result = parser.Parse("pwd");

        Assert.Equal("pwd", result.Command);
        Assert.Empty(result.Arguments);
    }

    [Fact]
    public void Parse_ShouldSplitCommandAndArguments()
    {
        var parser = new CommandParser();
        var result = parser.Parse("echo hello world");

        Assert.Equal("echo", result.Command);
        Assert.Equal(new[] { "hello", "world" }, result.Arguments);
    }
}