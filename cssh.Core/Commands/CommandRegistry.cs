/* cssh.Core.Commands.CommandRegistry - A cross-platform C# shell ver.0.1.1 CommandRegistry class */
using System.Collections.Generic;

namespace cssh.Core;

public class CommandRegistry
{
  private readonly Dictionary<string, ICommand> _commands = new();
  private readonly Dictionary<string, string> _descriptions = new();

  /// <summary>
  /// コマンドを登録する（説明文付き）
  /// </summary>
  public void Register(ICommand command, string description)
  {
    _commands[command.Name] = command;
    _descriptions[command.Name] = description;
  }

  /// <summary>
  /// コマンド名から ICommand を取得する
  /// </summary>
  public ICommand? Resolve(string name)
  {
    return _commands.TryGetValue(name, out var cmd) ? cmd : null;
  }

  /// <summary>
  /// コマンドの説明文を取得する
  /// </summary>
  public string? GetDescription(string name)
  {
    return _descriptions.TryGetValue(name, out var desc) ? desc : null;
  }

  /// <summary>
  /// 登録されているすべてのコマンド名を返す
  /// </summary>
  public IEnumerable<string> GetAllCommandNames()
  {
    return _commands.Keys;
  }
}