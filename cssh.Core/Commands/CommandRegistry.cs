/* cssh.Core.Commands.CommandRegistry - A cross-platform C# shell ver.0.2.0 CommandRegistry class */
using System.Collections.Generic;
using cssh.Core.Commands;

namespace cssh.Core;

/// <summary>
/// Represents the CommandRegistry class.
/// </summary>
public class CommandRegistry
{
  /// <summary>
  /// The _commands field.
  /// </summary>
  private readonly Dictionary<string, ICommand> _commands = new();
  /// <summary>
  /// The _descriptions field.
  /// </summary>
  private readonly Dictionary<string, string> _descriptions = new();

  // 動的な alias (name -> expansion)
  /// <summary>
  /// The _dynamicAliases field.
  /// </summary>
  private readonly Dictionary<string, string> _dynamicAliases = new();

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
    if (_commands.TryGetValue(name, out var cmd))
    return cmd;

    if (_dynamicAliases.TryGetValue(name, out var expansion))
    return new DynamicAliasInvoker(name, expansion, this);

    return null;
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

  /// <summary>
  /// 動的 alias を追加する
  /// </summary>
  public void AddAlias(string name, string expansion)
  {
    _dynamicAliases[name] = expansion;
  }

  /// <summary>
  /// 動的 alias を削除する
  /// </summary>
  public bool RemoveAlias(string name)
  {
    return _dynamicAliases.Remove(name);
  }

  /// <summary>
  /// 現在の alias を取得する
  /// </summary>
  public IReadOnlyDictionary<string,string> GetAliases()
  {
    return _dynamicAliases;
  }
}
