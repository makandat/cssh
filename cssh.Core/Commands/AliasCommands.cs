/* cssh.Core.Commands.AliasCommand.cs v0.1.5 */
namespace cssh.Core.Commands;

public class AliasCommand : ICommand
{
  private readonly string _target;

  public AliasCommand(string name, string target)
  {
    Name = name;
    _target = target;
  }

  public string Name { get; }

  public string Description => $"Alias for '{_target}'.";

  public string Execute(ShellState state, string[] args)
  {
    // 実際のコマンドを呼び出す
    var registry = state.Registry; // ShellState に Registry を持たせる必要あり
    var cmd = registry.Resolve(_target);

    if (cmd == null)
      return $"alias: target command not found: {_target}";

    return cmd.Execute(state, args);
  }
}