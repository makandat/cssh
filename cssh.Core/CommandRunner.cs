/* cssh.Core.CommandRunner.cs - A cross-platform C# shell v0.1.1 CommandRunner class */
namespace cssh.Core;

using cssh.Core.Commands;

/// <summary>
/// Executes commands by coordinating the parser, registry, and shell state.
/// </summary>
public class CommandRunner
{
  private readonly CommandParser _parser;
  private readonly CommandRegistry _registry;

  /// <summary>
  /// Initializes a new instance of the <see cref="CommandRunner"/> class.
  /// </summary>
  /// <param name="parser">The command parser.</param>
  /// <param name="registry">The command registry.</param>
  public CommandRunner(CommandParser parser, CommandRegistry registry)
  {
    _parser = parser;
    _registry = registry;
  }

  /// <summary>
  /// Parses and executes a command string.
  /// </summary>
  /// <param name="state">The current shell state.</param>
  /// <param name="input">The raw input string.</param>
  /// <returns>The command output, or an error message.</returns>
  public string Run(ShellState state, string input)
  {
    var parsed = _parser.Parse(input);
    var cmd = _registry.Resolve(parsed.Command);

    if (cmd == null)
      return $"Unknown command: {parsed.Command}";

    return cmd.Execute(state, parsed.Arguments);
  }
}