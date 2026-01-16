/* cssh v0.2.0 - LsCommand tests */
using System;
using cssh.Core;
using cssh.Core.Commands;
using Xunit;

/// <summary>
/// Represents the LsCommandTests class.
/// </summary>
public class LsCommandTests
{
  [Fact]
  /// <summary>
  /// Executes the Ls_ShouldListFilesAndDirectories_InCurrentDirectory method.
  /// </summary>
  public void Ls_ShouldListFilesAndDirectories_InCurrentDirectory()
  {
    var state = new ShellState(null);
    var cmd = new LsCommand();

    var output = cmd.Execute(state, Array.Empty<string>());

    Assert.Contains("cssh.Tests", output); // プロジェクト内に存在するはずのディレクトリ
  }
}
