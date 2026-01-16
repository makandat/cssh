/* cssh v0.2.0 - ShellState tests */
using System;
using cssh.Core;
using Xunit;

/// <summary>
/// Represents the ShellStateTests class.
/// </summary>
public class ShellStateTests
{
  [Fact]
  /// <summary>
  /// Executes the Initial_CurrentDirectory_ShouldBe_EnvironmentCurrentDirectory method.
  /// </summary>
  public void Initial_CurrentDirectory_ShouldBe_EnvironmentCurrentDirectory()
  {
    var state = new ShellState(null);
    Assert.Equal(Environment.CurrentDirectory, state.CurrentDirectory);
  }

  [Fact]
  /// <summary>
  /// Executes the Initial_PreviousDirectory_ShouldBe_EnvironmentCurrentDirectory method.
  /// </summary>
  public void Initial_PreviousDirectory_ShouldBe_EnvironmentCurrentDirectory()
  {
    var state = new ShellState(null);
    Assert.Equal(Environment.CurrentDirectory, state.PreviousDirectory);
  }

  [Fact]
  /// <summary>
  /// Executes the Updating_CurrentDirectory_ShouldNotChange_PreviousDirectory method.
  /// </summary>
  public void Updating_CurrentDirectory_ShouldNotChange_PreviousDirectory()
  {
    var state = new ShellState(null);
    var original = state.CurrentDirectory;

    state.CurrentDirectory = @"C:\Test";

    Assert.Equal(original, state.PreviousDirectory);
    Assert.Equal(@"C:\Test", state.CurrentDirectory);
  }

  [Fact]
  /// <summary>
  /// Executes the Updating_PreviousDirectory_ShouldNotChange_CurrentDirectory method.
  /// </summary>
  public void Updating_PreviousDirectory_ShouldNotChange_CurrentDirectory()
  {
    var state = new ShellState(null);
    var original = state.CurrentDirectory;

    state.PreviousDirectory = @"C:\Old";

    Assert.Equal(original, state.CurrentDirectory);
    Assert.Equal(@"C:\Old", state.PreviousDirectory);
  }
}
