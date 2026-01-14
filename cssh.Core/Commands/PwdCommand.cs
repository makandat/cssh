/* cssh.Core.Commands.PwdCommand.cs - v0.1.2 */
namespace cssh.Core.Commands;

/// <summary>
/// Implements the <c>pwd</c> command, printing the current working directory.
/// </summary>
public class PwdCommand : ICommand
{
  /// <inheritdoc />
  public string Name => "pwd";

  /// <summary>
  /// Returns the current working directory, following Bash behavior.
  /// </summary>
  public string Execute(ShellState state, string[] args)
  {
    return state.CurrentDirectory;
  }
}
