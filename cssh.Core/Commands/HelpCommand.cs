/* cssh.Core.Commands.HelpCommand - A cross-platform C# shell ver.0.2.0 HelpCommand.cs */
using cssh.Core;
using System.Globalization;

namespace cssh.Core.Commands;

/// <summary>
/// Represents the HelpCommand class.
/// </summary>
public class HelpCommand : ICommand
{
  /// <summary>
  /// The Name field.
  /// </summary>
  public string Name => "help";

  /// <returns></returns>
  private bool IsJapanese()
  {
    return CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "ja";
  }

  /// <param name="state"></param>
  /// <param name="args"></param>
  /// <returns></returns>
  public string Execute(ShellState state, string[] args)
  {
    // help → 全コマンド一覧
    if (args.Length == 0)
    {
      var names = state.Registry.GetAllCommandNames()
      .OrderBy(n => n);

      if (IsJapanese())
      {
        return "利用可能なコマンド:\n" +
        string.Join(Environment.NewLine, names);
      }
      else
      {
        return "Available commands:\n" +
        string.Join(Environment.NewLine, names);
      }
    }

    // help <command>
    var target = args[0];
    var cmd = state.Registry.Resolve(target);

    if (cmd == null)
    {
      if (IsJapanese())
      return $"help: コマンドが見つかりません: {target}";
      else
      return $"help: no such command: {target}";
    }

    var desc = state.Registry.GetDescription(target)
    ?? (IsJapanese() ? "(説明なし)" : "(no description available)");

    if (IsJapanese())
    return $"{target}: {desc}";
    else
    return $"{target}: {desc}";
  }
}
