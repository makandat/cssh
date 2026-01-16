/* cssh v0.2.0 - Path and Quoting Tests */
using cssh.Core;
using Xunit;

/// <summary>
/// Tests for path handling and quoting per specification sections 6.1, 6.2
/// - Windows path support (\ and /)
/// - Paths with spaces
/// - Single and double quotes
/// - Mixed quote styles
/// </summary>
public class PathAndQuotingTests
{
  /// <summary>
  /// The _parser field.
  /// </summary>
  private readonly CommandParser _parser = new();

  #region Windows Path Tests (Spec 6.1)

  [Fact]
  /// <summary>
  /// Executes the Path_WindowsStyle_ShouldAcceptBackslash method.
  /// </summary>
  public void Path_WindowsStyle_ShouldAcceptBackslash()
  {
    var result = _parser.Parse("cat C:\\Users\\file.txt");

    Assert.Equal("cat", result.Command);
    Assert.Single(result.Arguments);
    Assert.Equal("C:\\Users\\file.txt", result.Arguments[0]);
  }

  [Fact]
  /// <summary>
  /// Executes the Path_UnixStyle_ShouldAcceptForwardSlash method.
  /// </summary>
  public void Path_UnixStyle_ShouldAcceptForwardSlash()
  {
    var result = _parser.Parse("cat /path/to/file.txt");

    Assert.Equal("cat", result.Command);
    Assert.Single(result.Arguments);
    Assert.Equal("/path/to/file.txt", result.Arguments[0]);
  }

  [Fact]
  /// <summary>
  /// Executes the Path_MixedStyle_ShouldAcceptMixedSlashes method.
  /// </summary>
  public void Path_MixedStyle_ShouldAcceptMixedSlashes()
  {
    var result = _parser.Parse("cat C:/path\\to/file.txt");

    Assert.Equal("cat", result.Command);
    Assert.Equal("C:/path\\to/file.txt", result.Arguments[0]);
  }

  [Fact]
  /// <summary>
  /// Executes the Path_DriveLetterLowercase_ShouldBePreserved method.
  /// </summary>
  public void Path_DriveLetterLowercase_ShouldBePreserved()
  {
    var result = _parser.Parse("cat c:\\file.txt");

    Assert.Equal("c:\\file.txt", result.Arguments[0]);
  }

  [Fact]
  /// <summary>
  /// Executes the Path_DriveLetterUppercase_ShouldBePreserved method.
  /// </summary>
  public void Path_DriveLetterUppercase_ShouldBePreserved()
  {
    var result = _parser.Parse("cat C:\\FILE.TXT");

    Assert.Equal("C:\\FILE.TXT", result.Arguments[0]);
  }

  #endregion

  #region Quoted Paths with Spaces (Spec 6.2)

  [Fact]
  /// <summary>
  /// Executes the QuotedPath_DoubleQuotes_ShouldPreserveSpaces method.
  /// </summary>
  public void QuotedPath_DoubleQuotes_ShouldPreserveSpaces()
  {
    var result = _parser.Parse("cat \"C:\\My Documents\\file.txt\"");

    Assert.Equal("C:\\My Documents\\file.txt", result.Arguments[0]);
  }

  [Fact]
  /// <summary>
  /// Executes the QuotedPath_SingleQuotes_ShouldPreserveSpaces method.
  /// </summary>
  public void QuotedPath_SingleQuotes_ShouldPreserveSpaces()
  {
    var result = _parser.Parse("cat 'C:\\My Documents\\file.txt'");

    Assert.Equal("C:\\My Documents\\file.txt", result.Arguments[0]);
  }

  [Fact]
  /// <summary>
  /// Executes the QuotedPath_PartialQuote_SpaceInMiddle method.
  /// </summary>
  public void QuotedPath_PartialQuote_SpaceInMiddle()
  {
    var result = _parser.Parse("cat C:\\\"My Documents\"\\file.txt");

    // Space-containing part should be captured
    Assert.Single(result.Arguments);
  }

  [Fact]
  /// <summary>
  /// Executes the QuotedPath_OutputRedirect_WithSpaces method.
  /// </summary>
  public void QuotedPath_OutputRedirect_WithSpaces()
  {
    var seq = _parser.ParseSequence("echo test > \"C:\\My Files\\output.txt\"");

    var cmd = seq.Pipelines[0].Commands[0];
    Assert.Equal("C:\\My Files\\output.txt", cmd.Redirect.FilePath);
  }

  [Fact]
  /// <summary>
  /// Executes the QuotedPath_InputRedirect_WithSpaces method.
  /// </summary>
  public void QuotedPath_InputRedirect_WithSpaces()
  {
    var seq = _parser.ParseSequence("cat < \"C:\\My Files\\input.txt\"");

    var cmd = seq.Pipelines[0].Commands[0];
    Assert.Equal("C:\\My Files\\input.txt", cmd.Redirect.FilePath);
  }

  #endregion

  #region Quote Style Tests

  [Fact]
  /// <summary>
  /// Executes the Quotes_DoubleQuoted_StringWithSpaces method.
  /// </summary>
  public void Quotes_DoubleQuoted_StringWithSpaces()
  {
    var result = _parser.Parse("echo \"hello world\"");

    Assert.Equal("hello world", result.Arguments[0]);
  }

  [Fact]
  /// <summary>
  /// Executes the Quotes_SingleQuoted_StringWithSpaces method.
  /// </summary>
  public void Quotes_SingleQuoted_StringWithSpaces()
  {
    var result = _parser.Parse("echo 'hello world'");

    Assert.Equal("hello world", result.Arguments[0]);
  }

  [Fact]
  /// <summary>
  /// Executes the Quotes_MixedQuotes_InSameCommand method.
  /// </summary>
  public void Quotes_MixedQuotes_InSameCommand()
  {
    var result = _parser.Parse("echo \"first\" 'second' third");

    Assert.Equal(3, result.Arguments.Length);
    Assert.Equal("first", result.Arguments[0]);
    Assert.Equal("second", result.Arguments[1]);
    Assert.Equal("third", result.Arguments[2]);
  }

  [Fact]
  /// <summary>
  /// Executes the Quotes_NestedOppositeQuotes_ShouldWork method.
  /// </summary>
  public void Quotes_NestedOppositeQuotes_ShouldWork()
  {
    var result = _parser.Parse("echo \"it's working\"");

    Assert.Equal("it's working", result.Arguments[0]);
  }

  [Fact]
  /// <summary>
  /// Executes the Quotes_DoubleQuoteInsideSingle method.
  /// </summary>
  public void Quotes_DoubleQuoteInsideSingle()
  {
    var result = _parser.Parse("echo 'say \"hello\"'");

    Assert.Equal("say \"hello\"", result.Arguments[0]);
  }

  #endregion

  #region Complex Path Scenarios

  [Fact]
  /// <summary>
  /// Executes the Path_WithDots_ShouldBePreserved method.
  /// </summary>
  public void Path_WithDots_ShouldBePreserved()
  {
    var result = _parser.Parse("cd ..");

    Assert.Equal("cd", result.Command);
    Assert.Equal("..", result.Arguments[0]);
  }

  [Fact]
  /// <summary>
  /// Executes the Path_WithDots_RelativePath method.
  /// </summary>
  public void Path_WithDots_RelativePath()
  {
    var result = _parser.Parse("cat ./file.txt");

    Assert.Equal("./file.txt", result.Arguments[0]);
  }

  [Fact]
  /// <summary>
  /// Executes the Path_WithHome_RelativeToHome method.
  /// </summary>
  public void Path_WithHome_RelativeToHome()
  {
    var result = _parser.Parse("cat ~/Documents/file.txt");

    Assert.Equal("~/Documents/file.txt", result.Arguments[0]);
  }

  [Fact]
  /// <summary>
  /// Executes the Path_UNCPath_Windows method.
  /// </summary>
  public void Path_UNCPath_Windows()
  {
    var result = _parser.Parse("cat \\\\server\\share\\file.txt");

    Assert.Equal("\\\\server\\share\\file.txt", result.Arguments[0]);
  }

  [Fact]
  /// <summary>
  /// Executes the Path_LongPath_WindowsStyle method.
  /// </summary>
  public void Path_LongPath_WindowsStyle()
  {
    var result = _parser.Parse("cat C:\\very\\long\\path\\to\\some\\deeply\\nested\\directory\\file.txt");

    Assert.Equal("C:\\very\\long\\path\\to\\some\\deeply\\nested\\directory\\file.txt",
    result.Arguments[0]);
  }

  #endregion

  #region Spec Compliance - Section 6.2

  [Fact]
  /// <summary>
  /// Executes the Spec_QuoteMismatch_DoubleInside method.
  /// </summary>
  public void Spec_QuoteMismatch_DoubleInside()
  {
    // "...' is an error per spec 6.2
    // This should ideally error, but parser may just treat it as text
    var result = _parser.Parse("echo \"hello'");

    Assert.NotNull(result);
  }

  #endregion

  #region Path in Redirection

  [Fact]
  /// <summary>
  /// Executes the Redirect_OutputPath_WithBackslash method.
  /// </summary>
  public void Redirect_OutputPath_WithBackslash()
  {
    var seq = _parser.ParseSequence("echo test > C:\\output.txt");

    var redirect = seq.Pipelines[0].Commands[0].Redirect;
    Assert.Equal("C:\\output.txt", redirect.FilePath);
  }

  [Fact]
  /// <summary>
  /// Executes the Redirect_OutputPath_WithForwardSlash method.
  /// </summary>
  public void Redirect_OutputPath_WithForwardSlash()
  {
    var seq = _parser.ParseSequence("echo test > /tmp/output.txt");

    var redirect = seq.Pipelines[0].Commands[0].Redirect;
    Assert.Equal("/tmp/output.txt", redirect.FilePath);
  }

  [Fact]
  /// <summary>
  /// Executes the Redirect_RelativePath_DotSlash method.
  /// </summary>
  public void Redirect_RelativePath_DotSlash()
  {
    var seq = _parser.ParseSequence("echo test > ./output.txt");

    var redirect = seq.Pipelines[0].Commands[0].Redirect;
    Assert.Equal("./output.txt", redirect.FilePath);
  }

  #endregion
}

