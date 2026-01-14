using System;
using System.IO;
using cssh.Core;
using cssh.Core.Commands;
using Xunit;

/// <summary>
/// Represents the AliasAndHistoryTests class.
/// </summary>
public class AliasAndHistoryTests : IDisposable
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

  public AliasAndHistoryTests()
  {
    _testDir = Path.Combine(Path.GetTempPath(), $"cssh_aliashist_test_{Guid.NewGuid()}");
    Directory.CreateDirectory(_testDir);

    _parser = new CommandParser();
    _registry = new CommandRegistry();
    _state = new ShellState(_registry);
    _runner = new CommandRunner(_parser, _registry);

    // Register commands needed
    _registry.Register(new EchoCommand(), "Print arguments");
    _registry.Register(new AliasBuiltinCommand(), "List or create aliases");
    _registry.Register(new HistoryCommand(), "Show history");
    _registry.Register(new AliasCommand("h", "history"), "Alias of history");
    _registry.Register(new LsCommand(), "List directory contents");
  }

  [Fact]
  /// <summary>
  /// Executes the Alias_CanCreateAndInvoke method.
  /// </summary>
  public void Alias_CanCreateAndInvoke()
  {
    var res = _runner.Run(_state, "alias greet echo hello");
    Assert.Equal(string.Empty, res);

    var outp = _runner.Run(_state, "greet");
    Assert.Equal("hello", outp);
  }

  [Fact]
  /// <summary>
  /// Executes the Alias_WithQuotedExpansion_PreservesSpaces method.
  /// </summary>
  public void Alias_WithQuotedExpansion_PreservesSpaces()
  {
    var res = _runner.Run(_state, "alias greet echo \"hello world\"");
    Assert.Equal(string.Empty, res);

    var outp = _runner.Run(_state, "greet");
    Assert.Equal("hello world", outp);
  }

  [Fact]
  /// <summary>
  /// Executes the Alias_List_ShowsDefined method.
  /// </summary>
  public void Alias_List_ShowsDefined()
  {
    _runner.Run(_state, "alias greet echo hello");
    var outp = _runner.Run(_state, "alias");
    Assert.Equal("greet=echo hello", outp.Trim());
  }

  [Fact]
  /// <summary>
  /// Executes the Unalias_IsNotRegistered method.
  /// </summary>
  public void Unalias_IsNotRegistered()
  {
    _runner.Run(_state, "alias greet echo hello");
    var outp1 = _runner.Run(_state, "greet");
    Assert.Equal("hello", outp1);

    var outp2 = _runner.Run(_state, "unalias greet");
    Assert.Equal("Unknown command: unalias", outp2);

    var outp3 = _runner.Run(_state, "greet");
    Assert.Equal("hello", outp3);
  }

  [Fact]
  /// <summary>
  /// Executes the History_RecordsAndLists method.
  /// </summary>
  public void History_RecordsAndLists()
  {
    _runner.Run(_state, "echo a");
    _runner.Run(_state, "echo b");
    _runner.Run(_state, "echo c");

    var outp = _runner.Run(_state, "history");
    var lines = outp.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
    Assert.Equal("1 echo a", lines[0]);
    Assert.Equal("2 echo b", lines[1]);
    Assert.Equal("3 echo c", lines[2]);
  }

  [Fact]
  /// <summary>
  /// Executes the Bang_ExecutesHistoryEntry method.
  /// </summary>
  public void Bang_ExecutesHistoryEntry()
  {
    _runner.Run(_state, "echo a");
    _runner.Run(_state, "echo b");

    var outp = _runner.Run(_state, "!1");
    Assert.Equal("a", outp);

    // confirm it was recorded as executed command at end of history
    var hist = _state.History;
    Assert.Equal("echo a", hist[^1]);
  }

  [Fact]
  /// <summary>
  /// Executes the BangPrefix_ExecutesMostRecentMatchingHistoryEntry method.
  /// </summary>
  public void BangPrefix_ExecutesMostRecentMatchingHistoryEntry()
  {
    _runner.Run(_state, "echo one");
    _runner.Run(_state, "ls -la");
    _runner.Run(_state, "echo two");

    var outp = _runner.Run(_state, "!echo");
    Assert.Equal("two", outp);

    var hist = _state.History;
    Assert.Equal("echo two", hist[^1]);

    var outp2 = _runner.Run(_state, "!ls");
    Assert.NotEqual($"Unknown command: ls", outp2);
    Assert.Equal("ls -la", _state.History[^1]);
  }

  [Fact]
  /// <summary>
  /// Executes the BangPrefix_NotFoundReturnsError method.
  /// </summary>
  public void BangPrefix_NotFoundReturnsError()
  {
    _runner.Run(_state, "echo a");
    var outp = _runner.Run(_state, "!nope");
    Assert.Equal("history: event not found", outp);
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
