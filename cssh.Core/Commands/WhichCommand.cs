/* cssh.Core.Commands.WhichCommand.cs v0.2.0 */
namespace cssh.Core.Commands;
using cssh.Core.Commands;
using cssh.Core;
/// <summary>
/// Represents the WhichCommand class.
/// </summary>
public class WhichCommand : ICommand
{
  /// <summary>
  /// The _registry field.
  /// </summary>
  private readonly CommandRegistry _registry;

  public WhichCommand(CommandRegistry registry)
  {
    _registry = registry;
  }

  /// <summary>
  /// The Name field.
  /// </summary>
  public string Name => "which";
  /// <summary>
  /// The Description field.
  /// </summary>
  public string Description => "Show the implementation of a command.";

  /// <param name="state"></param>
  /// <param name="args"></param>
  /// <returns></returns>
  public string Execute(ShellState state, string[] args)
  {
    if (args.Length == 0)
    return "which: missing operand";

    var cmd = _registry.Resolve(args[0]);
    if (cmd == null)
    return $"which: no such command: {args[0]}";

    return cmd.GetType().FullName ?? cmd.GetType().Name;
  }
}
