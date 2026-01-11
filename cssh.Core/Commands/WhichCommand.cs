/* cssh.Core.Commands.WhichCommand.cs v0.1.5 */
namespace cssh.Core.Commands;
using cssh.Core.Commands;
using cssh.Core;
public class WhichCommand : ICommand
{
  private readonly CommandRegistry _registry;

  public WhichCommand(CommandRegistry registry)
  {
    _registry = registry;
  }

  public string Name => "which";
  public string Description => "Show the implementation of a command.";

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