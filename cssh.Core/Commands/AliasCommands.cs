/* cssh.Core.Commands.AliasCommand.cs v0.2.0 */
namespace cssh.Core.Commands;

/// <summary>
/// Represents the AliasCommand class.
/// </summary>
public class AliasCommand : ICommand
{
  /// <summary>
  /// The _target field.
  /// </summary>
  private readonly string _target;

  public AliasCommand(string name, string target)
  {
    Name = name;
    _target = target;
  }

  /// <summary>
  /// Gets or sets the Name.
  /// </summary>
  public string Name { get; }

  /// <summary>
  /// The Description field.
  /// </summary>
  public string Description => $"Alias for '{_target}'.";

  /// <param name="state"></param>
  /// <param name="args"></param>
  /// <returns></returns>
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
