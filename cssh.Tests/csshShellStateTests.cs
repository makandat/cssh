/* cssh - A cross-platform C# shell ver.0.1.0 ShellState tests */
using System;
using cssh.Core;
using Xunit;

public class ShellStateTests
{
    [Fact]
    public void Initial_CurrentDirectory_ShouldBe_EnvironmentCurrentDirectory()
    {
        var state = new ShellState(null);
        Assert.Equal(Environment.CurrentDirectory, state.CurrentDirectory);
    }

    [Fact]
    public void Initial_PreviousDirectory_ShouldBe_EnvironmentCurrentDirectory()
    {
        var state = new ShellState(null);
        Assert.Equal(Environment.CurrentDirectory, state.PreviousDirectory);
    }

    [Fact]
    public void Updating_CurrentDirectory_ShouldNotChange_PreviousDirectory()
    {
        var state = new ShellState(null);
        var original = state.CurrentDirectory;

        state.CurrentDirectory = @"C:\Test";

        Assert.Equal(original, state.PreviousDirectory);
        Assert.Equal(@"C:\Test", state.CurrentDirectory);
    }

    [Fact]
    public void Updating_PreviousDirectory_ShouldNotChange_CurrentDirectory()
    {
        var state = new ShellState(null);
        var original = state.CurrentDirectory;

        state.PreviousDirectory = @"C:\Old";

        Assert.Equal(original, state.CurrentDirectory);
        Assert.Equal(@"C:\Old", state.PreviousDirectory);
    }
}