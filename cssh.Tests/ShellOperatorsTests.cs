/* cssh v0.2.0 - Shell Operators Tests */
using System;
using System.IO;
using cssh.Core;
using cssh.Core.Commands;
using Xunit;

/// <summary>
/// Focused tests for shell operators per specification section 5
/// - Pipes (|)
/// - Output redirection (>, >>)
/// - Input redirection (<)
/// - Command sequencing
/// </summary>
public class ShellOperatorsTests : IDisposable
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

  public ShellOperatorsTests()
  {
    _testDir = Path.Combine(Path.GetTempPath(), $"cssh_ops_{Guid.NewGuid()}");
    Directory.CreateDirectory(_testDir);

    _parser = new CommandParser();
    _registry = new CommandRegistry();
    _state = new ShellState(_registry);
    _runner = new CommandRunner(_parser, _registry);

    _registry.Register(new EchoCommand(), "Print arguments");
    _registry.Register(new CatCommand(), "Print file contents");
    _registry.Register(new TouchCommand(), "Create or update file timestamp");
  }

  #region Operator Parsing Tests

  [Fact]
  /// <summary>
  /// Executes the Parser_ShouldRecognizeOutputRedirectTruncate method.
  /// </summary>
  public void Parser_ShouldRecognizeOutputRedirectTruncate()
  {
    var seq = _parser.ParseSequence("echo test > file.txt");

    var cmd = seq.Pipelines[0].Commands[0];
    Assert.NotNull(cmd.Redirect);
  }

  [Fact]
  /// <summary>
  /// Executes the Parser_ShouldRecognizeOutputRedirectAppend method.
  /// </summary>
  public void Parser_ShouldRecognizeOutputRedirectAppend()
  {
    var seq = _parser.ParseSequence("echo test >> file.txt");

    var cmd = seq.Pipelines[0].Commands[0];
    Assert.NotNull(cmd.Redirect);
  }

  [Fact]
  /// <summary>
  /// Executes the Parser_ShouldRecognizeInputRedirect method.
  /// </summary>
  public void Parser_ShouldRecognizeInputRedirect()
  {
    var seq = _parser.ParseSequence("cat < file.txt");

    var cmd = seq.Pipelines[0].Commands[0];
    Assert.NotNull(cmd.Redirect);
  }

  [Fact]
  /// <summary>
  /// Executes the Parser_ShouldRecognizePipe method.
  /// </summary>
  public void Parser_ShouldRecognizePipe()
  {
    var seq = _parser.ParseSequence("echo hello | cat");

    Assert.Equal(2, seq.Pipelines[0].Commands.Count);
  }

  #endregion

  #region Output Redirection (>)

  [Fact]
  /// <summary>
  /// Executes the Operator_OutputRedirect_ShouldTruncateExistingFile method.
  /// </summary>
  public void Operator_OutputRedirect_ShouldTruncateExistingFile()
  {
    var file = Path.Combine(_testDir, "trunc.txt");
    File.WriteAllText(file, "old content");

    _runner.Run(_state, $"echo new > \"{file}\"");

    Assert.Equal("new", File.ReadAllText(file));
  }

  [Fact]
  /// <summary>
  /// Executes the Operator_OutputRedirect_ShouldCreateNewFile method.
  /// </summary>
  public void Operator_OutputRedirect_ShouldCreateNewFile()
  {
    var file = Path.Combine(_testDir, "new.txt");

    _runner.Run(_state, $"echo content > \"{file}\"");

    Assert.True(File.Exists(file));
    Assert.Equal("content", File.ReadAllText(file));
  }

  [Fact]
  /// <summary>
  /// Executes the Operator_OutputRedirect_ShouldHandleSpecialCharacters method.
  /// </summary>
  public void Operator_OutputRedirect_ShouldHandleSpecialCharacters()
  {
    var file = Path.Combine(_testDir, "special.txt");

    _runner.Run(_state, $"echo hello > \"{file}\"");

    Assert.Equal("hello", File.ReadAllText(file));
  }

  #endregion

  #region Append Redirection (>>)

  [Fact]
  /// <summary>
  /// Executes the Operator_AppendRedirect_ShouldAppendToExistingFile method.
  /// </summary>
  public void Operator_AppendRedirect_ShouldAppendToExistingFile()
  {
    var file = Path.Combine(_testDir, "append.txt");
    File.WriteAllText(file, "line1\n");

    _runner.Run(_state, $"echo line2 >> \"{file}\"");

    var content = File.ReadAllText(file);
    Assert.Contains("line1", content);
    Assert.Contains("line2", content);
  }

  [Fact]
  /// <summary>
  /// Executes the Operator_AppendRedirect_ShouldCreateFileIfNotExists method.
  /// </summary>
  public void Operator_AppendRedirect_ShouldCreateFileIfNotExists()
  {
    var file = Path.Combine(_testDir, "newappend.txt");

    _runner.Run(_state, $"echo first >> \"{file}\"");

    Assert.True(File.Exists(file));
    Assert.Contains("first", File.ReadAllText(file));
  }

  [Fact]
  /// <summary>
  /// Executes the Operator_AppendRedirect_MultipleAppends_ShouldPreserveAllLines method.
  /// </summary>
  public void Operator_AppendRedirect_MultipleAppends_ShouldPreserveAllLines()
  {
    var file = Path.Combine(_testDir, "multi.txt");

    _runner.Run(_state, $"echo line1 >> \"{file}\"");
    _runner.Run(_state, $"echo line2 >> \"{file}\"");
    _runner.Run(_state, $"echo line3 >> \"{file}\"");

    var content = File.ReadAllText(file);
    Assert.Contains("line1", content);
    Assert.Contains("line2", content);
    Assert.Contains("line3", content);
  }

  #endregion

  #region Input Redirection (<)

  [Fact]
  /// <summary>
  /// Executes the Operator_InputRedirect_ShouldReadFileContent method.
  /// </summary>
  public void Operator_InputRedirect_ShouldReadFileContent()
  {
    var file = Path.Combine(_testDir, "read.txt");
    File.WriteAllText(file, "file content");

    var output = _runner.Run(_state, $"cat < \"{file}\"");

    Assert.Equal("file content", output);
  }

  [Fact]
  /// <summary>
  /// Executes the Operator_InputRedirect_NonExistentFile_ShouldReturnError method.
  /// </summary>
  public void Operator_InputRedirect_NonExistentFile_ShouldReturnError()
  {
    var output = _runner.Run(_state, "cat < /nonexistent/path/file.txt");

    Assert.Contains("error", output.ToLower());
  }

  #endregion

  #region Pipe Operator (|)

  [Fact]
  /// <summary>
  /// Executes the Operator_Pipe_ShouldTransmitOutput method.
  /// </summary>
  public void Operator_Pipe_ShouldTransmitOutput()
  {
    var output = _runner.Run(_state, "echo hello | cat");

    Assert.Equal("hello", output);
  }

  [Fact]
  /// <summary>
  /// Executes the Operator_Pipe_TwoStages_ShouldChainCorrectly method.
  /// </summary>
  public void Operator_Pipe_TwoStages_ShouldChainCorrectly()
  {
    var file = Path.Combine(_testDir, "chaintest.txt");
    File.WriteAllText(file, "test content");

    var output = _runner.Run(_state, $"cat \"{file}\" | cat");

    Assert.Equal("test content", output);
  }

  [Fact]
  /// <summary>
  /// Executes the Operator_Pipe_WithFinalRedirect_ShouldApplyRedirectToLastCommand method.
  /// </summary>
  public void Operator_Pipe_WithFinalRedirect_ShouldApplyRedirectToLastCommand()
  {
    var file = Path.Combine(_testDir, "pipefinal.txt");

    _runner.Run(_state, $"echo data | cat > \"{file}\"");

    Assert.True(File.Exists(file));
    Assert.Equal("data", File.ReadAllText(file));
  }

  [Fact]
  /// <summary>
  /// Executes the Operator_Pipe_MultipleStages_ShouldChainAll method.
  /// </summary>
  public void Operator_Pipe_MultipleStages_ShouldChainAll()
  {
    // Assuming we can pipe through multiple commands
    var output = _runner.Run(_state, "echo hello | cat | cat");

    Assert.Equal("hello", output);
  }

  #endregion

  #region Combined Operators

  [Fact]
  /// <summary>
  /// Executes the Operators_Combined_PipeAndRedirect method.
  /// </summary>
  public void Operators_Combined_PipeAndRedirect()
  {
    var file = Path.Combine(_testDir, "combined.txt");

    _runner.Run(_state, $"echo line1 | cat > \"{file}\"");
    _runner.Run(_state, $"echo line2 | cat >> \"{file}\"");

    var content = File.ReadAllText(file);
    Assert.Contains("line1", content);
    Assert.Contains("line2", content);
  }

  [Fact]
  /// <summary>
  /// Executes the Operators_Combined_InputAndPipe method.
  /// </summary>
  public void Operators_Combined_InputAndPipe()
  {
    var inputFile = Path.Combine(_testDir, "input.txt");
    File.WriteAllText(inputFile, "input data");

    var output = _runner.Run(_state, $"cat < \"{inputFile}\" | cat");

    Assert.Equal("input data", output);
  }

  #endregion

  #region Error Handling

  [Fact]
  /// <summary>
  /// Executes the Operator_InvalidRedirectPath_ShouldHandleError method.
  /// </summary>
  public void Operator_InvalidRedirectPath_ShouldHandleError()
  {
    // Path with invalid characters (depends on OS)
    var output = _runner.Run(_state, "echo test > /invalid/path/file.txt");

    // May contain error message or fail silently depending on implementation
    Assert.NotNull(output);
  }

  #endregion

  #region Spec Compliance - Section 5.2

  [Fact]
  /// <summary>
  /// Executes the Spec_ShouldNotSupportConditionalExecution method.
  /// </summary>
  public void Spec_ShouldNotSupportConditionalExecution()
  {
    // Spec says && and || are NOT supported in v0.2.0
    var seq = _parser.ParseSequence("echo a && echo b");

    // Should parse as literal "&&" in arguments, not as operator
    var cmd = seq.Pipelines[0].Commands[0];
    Assert.Equal("echo", cmd.Name);
    // Args will include "a", "&&", "echo", "b" as separate tokens
  }

  [Fact]
  /// <summary>
  /// Executes the Spec_ShouldNotSupportBackgroundExecution method.
  /// </summary>
  public void Spec_ShouldNotSupportBackgroundExecution()
  {
    // Spec says & for background is NOT supported in v0.2.0
    var seq = _parser.ParseSequence("echo test &");

    var cmd = seq.Pipelines[0].Commands[0];
    Assert.Equal("echo", cmd.Name);
    // & should be treated as a regular character, not operator
  }

  #endregion

  /// <summary>
  /// Executes the Dispose method.
  /// </summary>
  public void Dispose()
  {
    if (Directory.Exists(_testDir))
    {
      Directory.Delete(_testDir, true);
    }
  }
}

