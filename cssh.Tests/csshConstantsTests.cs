/* cssh.Tests.CsshConstantsTests.cs - A cross-platform C# shell ver.0.1.2 CsshConstantsTests class */
using cssh.Core.Constants;
using Xunit;

namespace cssh.Tests;

/// <summary>
/// Unit tests for <see cref="CsshConstants"/> ensuring that
/// versioning and constant values remain stable across releases.
/// </summary>
public class CsshConstantsTests
{
  /// <summary>
  /// Ensures that the shell version matches v0.1.5.
  /// </summary>
  [Fact]
  public void Version_ShouldBe_v015()
  {
    Assert.Equal("v0.1.5", CsshConstants.Version);
  }

  /// <summary>
  /// Ensures that the default prompt directory trim length is 2.
  /// </summary>
  [Fact]
  public void DefaultPromptDirTrim_ShouldBe_2()
  {
    Assert.Equal(2, CsshConstants.DefaultPromptDirTrim);
  }

  /// <summary>
  /// Ensures that the interactive mode identifier is correct.
  /// </summary>
  [Fact]
  public void ModeInteractive_ShouldBe_interactive()
  {
    Assert.Equal("interactive", CsshConstants.ModeInteractive);
  }

  /// <summary>
  /// Ensures that the edit mode identifier is correct.
  /// </summary>
  [Fact]
  public void ModeEdit_ShouldBe_edit()
  {
    Assert.Equal("edit", CsshConstants.ModeEdit);
  }
}