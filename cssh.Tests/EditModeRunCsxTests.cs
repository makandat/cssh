using System;
using System.IO;
using cssh.Core;
using cssh.Core.Commands;
using Xunit;

/// <summary>
/// Represents the EditModeRunCsxTests class.
/// </summary>
public class EditModeRunCsxTests : IDisposable
{
  /// <summary>
  /// The _parser field.
  /// </summary>
  private readonly CommandParser _parser;
  /// <summary>
  /// The _registry field.
  /// </summary>
  private readonly CommandRegistry _registry;
  /// <summary>
  /// The _runner field.
  /// </summary>
  private readonly CommandRunner _runner;
  /// <summary>
  /// The _state field.
  /// </summary>
  private readonly ShellState _state;
  /// <summary>
  /// The _testDir field.
  /// </summary>
  private readonly string _testDir;

  public EditModeRunCsxTests()
  {
    _testDir = Path.Combine(Path.GetTempPath(), $"cssh_csx_test_{Guid.NewGuid()}");
    Directory.CreateDirectory(_testDir);

    _parser = new CommandParser();
    _registry = new CommandRegistry();
    _state = new ShellState(_registry);
    _runner = new CommandRunner(_parser, _registry);

    _registry.Register(new EditCommand(), "Enter edit mode");
  }

  [Fact]
  /// <summary>
  /// Executes the Run_Csx_ReturnsConsoleWriteLineOutput method.
  /// </summary>
  public void Run_Csx_ReturnsConsoleWriteLineOutput()
  {
    var csx = "System.Console.WriteLine(\"hello csx\");";
    _runner.Run(_state, "edit");
    _state.EditBuffer = csx;

    var outp = _runner.Run(_state, "run");
    Assert.Equal(string.Empty, outp); // Console output goes to Console, script return value is empty
  }

  [Fact]
  /// <summary>
  /// Executes the Run_Csx_CompilationErrorReportsLine method.
  /// </summary>
  public void Run_Csx_CompilationErrorReportsLine()
  {
    var csx = "int x = ;\nSystem.Console.WriteLine(\"won't reach\");";
    _runner.Run(_state, "edit");
    _state.EditBuffer = csx;

    var outp = _runner.Run(_state, "run");
    Assert.Contains("run: error:", outp);
    Assert.Contains("Line", outp);
  }

  /// <summary>
  /// Executes the Dispose method.
  /// </summary>
  public void Dispose()
  {
    try
    {
      if (Directory.Exists(_testDir))
      Directory.Delete(_testDir, true);
    }
    catch { }
  }
}
