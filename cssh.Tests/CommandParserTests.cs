/* cssh v0.2.0 - CommandParser tests based on specification */
using cssh.Core;
using Cssh.Core.Ast;
using Xunit;

/// <summary>
/// Tests for CommandParser covering:
/// - Basic command parsing
/// - Quoted arguments (single and double quotes)
/// - Pipeline operators (|)
/// - Redirection operators (>, >>, <)
/// - Multiple pipelines and redirects
/// Per specification section 5: "Shell operators"
/// </summary>
public class CommandParserTests
{
  /// <summary>
  /// The _parser field.
  /// </summary>
  private readonly CommandParser _parser = new();

  #region Basic Parsing Tests

  [Fact]
  /// <summary>
  /// Executes the Parse_ShouldReturnCommandName_WhenOnlyCommandGiven method.
  /// </summary>
  public void Parse_ShouldReturnCommandName_WhenOnlyCommandGiven()
  {
    var result = _parser.Parse("pwd");

    Assert.Equal("pwd", result.Command);
    Assert.Empty(result.Arguments);
  }

  [Fact]
  /// <summary>
  /// Executes the Parse_ShouldSplitCommandAndArguments method.
  /// </summary>
  public void Parse_ShouldSplitCommandAndArguments()
  {
    var result = _parser.Parse("echo hello world");

    Assert.Equal("echo", result.Command);
    Assert.Equal(new[] { "hello", "world" }, result.Arguments);
  }

  [Fact]
  /// <summary>
  /// Executes the Parse_ShouldHandleMultipleArguments method.
  /// </summary>
  public void Parse_ShouldHandleMultipleArguments()
  {
    var result = _parser.Parse("cp file1.txt file2.txt destination");

    Assert.Equal("cp", result.Command);
    Assert.Equal(new[] { "file1.txt", "file2.txt", "destination" }, result.Arguments);
  }

  #endregion

  #region Quoted Arguments Tests

  [Fact]
  /// <summary>
  /// Executes the Parse_ShouldHandleDoubleQuotedArguments_WithSpaces method.
  /// </summary>
  public void Parse_ShouldHandleDoubleQuotedArguments_WithSpaces()
  {
    var result = _parser.Parse("echo \"hello world\"");

    Assert.Equal("echo", result.Command);
    Assert.Single(result.Arguments);
    Assert.Equal("hello world", result.Arguments[0]);
  }

  [Fact]
  /// <summary>
  /// Executes the Parse_ShouldHandleSingleQuotedArguments_WithSpaces method.
  /// </summary>
  public void Parse_ShouldHandleSingleQuotedArguments_WithSpaces()
  {
    var result = _parser.Parse("echo 'hello world'");

    Assert.Equal("echo", result.Command);
    Assert.Single(result.Arguments);
    Assert.Equal("hello world", result.Arguments[0]);
  }

  [Fact]
  /// <summary>
  /// Executes the Parse_ShouldHandleMixedQuotes method.
  /// </summary>
  public void Parse_ShouldHandleMixedQuotes()
  {
    var result = _parser.Parse("echo \"first\" 'second' third");

    Assert.Equal("echo", result.Command);
    Assert.Equal(new[] { "first", "second", "third" }, result.Arguments);
  }

  [Fact]
  /// <summary>
  /// Executes the Parse_ShouldHandleQuotedPathsWithBackslash method.
  /// </summary>
  public void Parse_ShouldHandleQuotedPathsWithBackslash()
  {
    var result = _parser.Parse("cat \"C:\\Users\\test.txt\"");

    Assert.Equal("cat", result.Command);
    Assert.Single(result.Arguments);
    Assert.Equal("C:\\Users\\test.txt", result.Arguments[0]);
  }

  [Fact]
  /// <summary>
  /// Executes the Parse_ShouldHandleQuotedPathsWithForwardSlash method.
  /// </summary>
  public void Parse_ShouldHandleQuotedPathsWithForwardSlash()
  {
    var result = _parser.Parse("cat \"C:/Users/test.txt\"");

    Assert.Equal("cat", result.Command);
    Assert.Single(result.Arguments);
    Assert.Equal("C:/Users/test.txt", result.Arguments[0]);
  }

  #endregion

  #region Redirection Tests (v0.2.0)

  [Fact]
  /// <summary>
  /// Executes the ParseSequence_ShouldParseOutputRedirect_Truncate method.
  /// </summary>
  public void ParseSequence_ShouldParseOutputRedirect_Truncate()
  {
    var result = _parser.ParseSequence("echo hello > output.txt");

    Assert.Single(result.Pipelines);
    Assert.Single(result.Pipelines[0].Commands);
    var cmd = result.Pipelines[0].Commands[0];
    Assert.Equal("echo", cmd.Name);
    Assert.Equal(RedirectType.OutputTruncate, cmd.Redirect.Type);
    Assert.Equal("output.txt", cmd.Redirect.FilePath);
  }

  [Fact]
  /// <summary>
  /// Executes the ParseSequence_ShouldParseOutputRedirect_Append method.
  /// </summary>
  public void ParseSequence_ShouldParseOutputRedirect_Append()
  {
    var result = _parser.ParseSequence("echo hello >> output.txt");

    Assert.Single(result.Pipelines);
    var cmd = result.Pipelines[0].Commands[0];
    Assert.Equal(RedirectType.OutputAppend, cmd.Redirect.Type);
    Assert.Equal("output.txt", cmd.Redirect.FilePath);
  }

  [Fact]
  /// <summary>
  /// Executes the ParseSequence_ShouldParseInputRedirect method.
  /// </summary>
  public void ParseSequence_ShouldParseInputRedirect()
  {
    var result = _parser.ParseSequence("cat < input.txt");

    Assert.Single(result.Pipelines);
    var cmd = result.Pipelines[0].Commands[0];
    Assert.Equal("cat", cmd.Name);
    Assert.Equal(RedirectType.Input, cmd.Redirect.Type);
    Assert.Equal("input.txt", cmd.Redirect.FilePath);
  }

  [Fact]
  /// <summary>
  /// Executes the ParseSequence_ShouldParseRedirectWithQuotedPath method.
  /// </summary>
  public void ParseSequence_ShouldParseRedirectWithQuotedPath()
  {
    var result = _parser.ParseSequence("echo test > \"output file.txt\"");

    Assert.Single(result.Pipelines);
    var cmd = result.Pipelines[0].Commands[0];
    Assert.Equal(RedirectType.OutputTruncate, cmd.Redirect.Type);
    Assert.Equal("output file.txt", cmd.Redirect.FilePath);
  }

  #endregion

  #region Pipeline Tests (v0.2.0)

  [Fact]
  /// <summary>
  /// Executes the ParseSequence_ShouldParseTwoCommandPipeline method.
  /// </summary>
  public void ParseSequence_ShouldParseTwoCommandPipeline()
  {
    var result = _parser.ParseSequence("echo hello | cat");

    Assert.Single(result.Pipelines);
    Assert.Equal(2, result.Pipelines[0].Commands.Count);
    Assert.Equal("echo", result.Pipelines[0].Commands[0].Name);
    Assert.Equal("cat", result.Pipelines[0].Commands[1].Name);
  }

  [Fact]
  /// <summary>
  /// Executes the ParseSequence_ShouldHaveEmptyArgsOnSecondCommandForEchoPipe method.
  /// </summary>
  public void ParseSequence_ShouldHaveEmptyArgsOnSecondCommandForEchoPipe()
  {
    var result = _parser.ParseSequence("echo hello | cat");
    var second = result.Pipelines[0].Commands[1];
    Assert.Empty(second.Args);
  }

  [Fact]
  /// <summary>
  /// Executes the ParseSequence_ShouldParseMultipleCommandPipeline method.
  /// </summary>
  public void ParseSequence_ShouldParseMultipleCommandPipeline()
  {
    var result = _parser.ParseSequence("echo hello | cat | grep e");

    Assert.Single(result.Pipelines);
    Assert.Equal(3, result.Pipelines[0].Commands.Count);
    Assert.Equal("echo", result.Pipelines[0].Commands[0].Name);
    Assert.Equal("cat", result.Pipelines[0].Commands[1].Name);
    Assert.Equal("grep", result.Pipelines[0].Commands[2].Name);
  }

  [Fact]
  /// <summary>
  /// Executes the ParseSequence_ShouldParsePipelineWithArguments method.
  /// </summary>
  public void ParseSequence_ShouldParsePipelineWithArguments()
  {
    var result = _parser.ParseSequence("echo hello world | cat -n");

    Assert.Equal(2, result.Pipelines[0].Commands.Count);
    Assert.Equal(new[] { "hello", "world" }, result.Pipelines[0].Commands[0].Args);
    Assert.Equal(new[] { "-n" }, result.Pipelines[0].Commands[1].Args);
  }

  [Fact]
  /// <summary>
  /// Executes the ParseSequence_ShouldParsePipelineWithFinalRedirect method.
  /// </summary>
  public void ParseSequence_ShouldParsePipelineWithFinalRedirect()
  {
    var result = _parser.ParseSequence("echo hello | cat > output.txt");

    Assert.Equal(2, result.Pipelines[0].Commands.Count);
    var lastCmd = result.Pipelines[0].Commands[1];
    Assert.Equal("cat", lastCmd.Name);
    Assert.Equal(RedirectType.OutputTruncate, lastCmd.Redirect.Type);
    Assert.Equal("output.txt", lastCmd.Redirect.FilePath);
  }

  #endregion

  #region Semicolon Tests

  [Fact]
  /// <summary>
  /// Executes the ParseSequence_ShouldSplitBySemicolon method.
  /// </summary>
  public void ParseSequence_ShouldSplitBySemicolon()
  {
    var result = _parser.ParseSequence("echo one; echo two");

    Assert.Equal(2, result.Pipelines.Count);
    Assert.Equal("echo", result.Pipelines[0].Commands[0].Name);
    Assert.Equal("one", result.Pipelines[0].Commands[0].Args[0]);
    Assert.Equal("echo", result.Pipelines[1].Commands[0].Name);
    Assert.Equal("two", result.Pipelines[1].Commands[0].Args[0]);
  }

  [Fact]
  /// <summary>
  /// Executes the ParseSequence_ShouldNotSplitSemicolonInsideQuotes method.
  /// </summary>
  public void ParseSequence_ShouldNotSplitSemicolonInsideQuotes()
  {
    var result = _parser.ParseSequence("echo \"a;b\"; echo after");

    Assert.Equal(2, result.Pipelines.Count);
    var first = result.Pipelines[0].Commands[0];
    Assert.Equal(new[] { "a;b" }, first.Args);
  }

  #endregion

  #region Complex Scenarios

  [Fact]
  /// <summary>
  /// Executes the ParseSequence_ShouldHandleCommandWithMultipleArguments_AndRedirect method.
  /// </summary>
  public void ParseSequence_ShouldHandleCommandWithMultipleArguments_AndRedirect()
  {
    var result = _parser.ParseSequence("cp file1 file2 destination > result.log");

    Assert.Single(result.Pipelines);
    var cmd = result.Pipelines[0].Commands[0];
    Assert.Equal("cp", cmd.Name);
    Assert.Equal(new[] { "file1", "file2", "destination" }, cmd.Args);
    Assert.Equal(RedirectType.OutputTruncate, cmd.Redirect.Type);
    Assert.Equal("result.log", cmd.Redirect.FilePath);
  }

  [Fact]
  /// <summary>
  /// Executes the ParseSequence_ShouldPreservePathsWithSpaces method.
  /// </summary>
  public void ParseSequence_ShouldPreservePathsWithSpaces()
  {
    var result = _parser.ParseSequence("echo \"hello world\" > \"my output.txt\"");

    var cmd = result.Pipelines[0].Commands[0];
    Assert.Equal(new[] { "hello world" }, cmd.Args);
    Assert.Equal("my output.txt", cmd.Redirect.FilePath);
  }

  [Fact]
  /// <summary>
  /// Executes the ParseSequence_ShouldHandleWindowsPaths method.
  /// </summary>
  public void ParseSequence_ShouldHandleWindowsPaths()
  {
    var result = _parser.ParseSequence("cat C:\\path\\to\\file.txt");

    var cmd = result.Pipelines[0].Commands[0];
    Assert.Equal(new[] { "C:\\path\\to\\file.txt" }, cmd.Args);
  }

  [Fact]
  /// <summary>
  /// Executes the ParseSequence_ShouldHandleUnixStylePaths method.
  /// </summary>
  public void ParseSequence_ShouldHandleUnixStylePaths()
  {
    var result = _parser.ParseSequence("cat /path/to/file.txt");

    var cmd = result.Pipelines[0].Commands[0];
    Assert.Equal(new[] { "/path/to/file.txt" }, cmd.Args);
  }

  [Fact]
  /// <summary>
  /// Executes the ParseSequence_ShouldHandleOptionsWithDash method.
  /// </summary>
  public void ParseSequence_ShouldHandleOptionsWithDash()
  {
    var result = _parser.ParseSequence("ls -la");

    var cmd = result.Pipelines[0].Commands[0];
    Assert.Equal("ls", cmd.Name);
    Assert.Equal(new[] { "-la" }, cmd.Args);
  }

  [Fact]
  /// <summary>
  /// Executes the ParseSequence_ShouldHandleEmptyInput method.
  /// </summary>
  public void ParseSequence_ShouldHandleEmptyInput()
  {
    var result = _parser.ParseSequence("");

    Assert.Single(result.Pipelines);
    Assert.Empty(result.Pipelines[0].Commands);
  }

  [Fact]
  /// <summary>
  /// Executes the ParseSequence_ShouldHandleWhitespaceOnlyInput method.
  /// </summary>
  public void ParseSequence_ShouldHandleWhitespaceOnlyInput()
  {
    var result = _parser.ParseSequence("   ");

    Assert.Single(result.Pipelines);
    Assert.Empty(result.Pipelines[0].Commands);
  }

  #endregion

  #region Error Cases

  [Fact]
  /// <summary>
  /// Executes the Parse_ShouldReturnEmptyCommand_WhenInputIsEmpty method.
  /// </summary>
  public void Parse_ShouldReturnEmptyCommand_WhenInputIsEmpty()
  {
    var result = _parser.Parse("");

    Assert.Empty(result.Command);
    Assert.Empty(result.Arguments);
  }

  [Fact]
  /// <summary>
  /// Executes the ParseSequence_ShouldHandleConsecutiveSpaces method.
  /// </summary>
  public void ParseSequence_ShouldHandleConsecutiveSpaces()
  {
    var result = _parser.ParseSequence("echo    hello    world");

    var cmd = result.Pipelines[0].Commands[0];
    Assert.Equal(new[] { "hello", "world" }, cmd.Args);
  }

  #endregion
}

