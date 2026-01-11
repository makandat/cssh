/* cssh - A cross-platform C# shell ver.0.1.0 LsCommand tests */
using System;
using cssh.Core;
using cssh.Core.Commands;
using Xunit;

public class LsCommandTests
{
    [Fact]
    public void Ls_ShouldListFilesAndDirectories_InCurrentDirectory()
    {
        var state = new ShellState(null);
        var cmd = new LsCommand();

        var output = cmd.Execute(state, Array.Empty<string>());

        Assert.Contains("cssh.Tests", output); // プロジェクト内に存在するはずのディレクトリ
    }
}