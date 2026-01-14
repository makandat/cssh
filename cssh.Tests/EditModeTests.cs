using System;
using System.IO;
using cssh.Core;
using cssh.Core.Commands;
using Xunit;

/// <summary>
/// Represents the EditModeTests class.
/// </summary>
public class EditModeTests : IDisposable
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

  public EditModeTests()
  {
    _testDir = Path.Combine(Path.GetTempPath(), $"cssh_edit_test_{Guid.NewGuid()}");
    Directory.CreateDirectory(_testDir);

    _parser = new CommandParser();
    _registry = new CommandRegistry();
    _state = new ShellState(_registry);
    _runner = new CommandRunner(_parser, _registry);

    // Register only the Edit command; editor commands are handled by runner in edit mode
    _registry.Register(new EditCommand(), "Enter edit mode");
  }

  [Fact]
  /// <summary>
  /// Executes the EditCommand_EntersEditMode method.
  /// </summary>
  public void EditCommand_EntersEditMode()
  {
    var outp = _runner.Run(_state, "edit");
    Assert.Equal("> ", outp);
    Assert.Equal(ShellMode.Edit, _state.Mode);
  }

  [Fact]
  /// <summary>
  /// Executes the ReadCommand_LoadsFileIntoBuffer method.
  /// </summary>
  public void ReadCommand_LoadsFileIntoBuffer()
  {
    var file = Path.Combine(_testDir, "sample.txt");
    File.WriteAllText(file, "line1\nline2");

    _runner.Run(_state, "edit");
    var outp = _runner.Run(_state, $"read \"{file}\"");

    Assert.Equal(string.Empty, outp);
    Assert.Equal("line1\nline2", _state.EditBuffer);
    Assert.Equal(file, _state.EditFileName);
    Assert.False(_state.EditDirty);
  }

  [Fact]
  /// <summary>
  /// Executes the AppendLines_AppendsToBufferAndMarksDirty method.
  /// </summary>
  public void AppendLines_AppendsToBufferAndMarksDirty()
  {
    _runner.Run(_state, "edit");
    _runner.Run(_state, "first line");
    _runner.Run(_state, "second line");

    Assert.Equal("first line\nsecond line\n", _state.EditBuffer);
    Assert.True(_state.EditDirty);
  }

  [Fact]
  /// <summary>
  /// Executes the ClearCommand_ClearsBufferAndResetsDirty method.
  /// </summary>
  public void ClearCommand_ClearsBufferAndResetsDirty()
  {
    _runner.Run(_state, "edit");
    _runner.Run(_state, "line one");
    _runner.Run(_state, "line two");

    var outp = _runner.Run(_state, "clear");
    Assert.Equal(string.Empty, outp);
    Assert.Equal(string.Empty, _state.EditBuffer);
    Assert.False(_state.EditDirty);
    // still in edit mode
    Assert.Equal(ShellMode.Edit, _state.Mode);
  }

  [Fact]
  /// <summary>
  /// Executes the WriteCommand_SavesBufferToFile method.
  /// </summary>
  public void WriteCommand_SavesBufferToFile()
  {
    var file = Path.Combine(_testDir, "out.txt");

    _runner.Run(_state, "edit");
    _runner.Run(_state, "hello world");
    var outp = _runner.Run(_state, $"write \"{file}\"");

    Assert.Equal(string.Empty, outp);
    Assert.True(File.Exists(file));
    Assert.Equal("hello world\n", File.ReadAllText(file));
    Assert.Equal(file, _state.EditFileName);
    Assert.False(_state.EditDirty);
  }

  [Fact]
  /// <summary>
  /// Executes the WriteCommand_UsesExistingFileName_WhenNoArgProvided method.
  /// </summary>
  public void WriteCommand_UsesExistingFileName_WhenNoArgProvided()
  {
    var file = Path.Combine(_testDir, "out2.txt");
    _state.EditFileName = file;

    _runner.Run(_state, "edit");
    _runner.Run(_state, "data");
    var outp = _runner.Run(_state, "write");

    Assert.Equal(string.Empty, outp);
    Assert.True(File.Exists(file));
    Assert.Equal("data\n", File.ReadAllText(file));
    Assert.False(_state.EditDirty);
  }

  [Fact]
  /// <summary>
  /// Executes the RunCommand_ExecutesEchoLines method.
  /// </summary>
  public void RunCommand_ExecutesEchoLines()
  {
    _runner.Run(_state, "edit");
    _runner.Run(_state, "echo hello");
    _runner.Run(_state, "echo world");

    var outp = _runner.Run(_state, "run");
    Assert.Equal($"hello{Environment.NewLine}world", outp);
  }

  [Fact]
  /// <summary>
  /// Executes the Quit_ExitsEditMode method.
  /// </summary>
  public void Quit_ExitsEditMode()
  {
    _runner.Run(_state, "edit");
    _runner.Run(_state, "quit");
    Assert.Equal(ShellMode.Normal, _state.Mode);
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
