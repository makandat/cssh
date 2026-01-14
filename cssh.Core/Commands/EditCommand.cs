/* cssh.Core.Commands.EditCommand - A cross-platform C# shell ver.0.2.0 EditCommand.cs */
using cssh.Core;

namespace cssh.Core.Commands;

/// <summary>
/// Represents the EditCommand class.
/// </summary>
public class EditCommand : ICommand
{
  /// <summary>
  /// The Name field.
  /// </summary>
  public string Name => "edit";

  /// <param name="state"></param>
  /// <param name="args"></param>
  /// <returns></returns>
  public string Execute(ShellState state, string[] args)
  {
    // 編集モードへ遷移
    state.Mode = ShellMode.Edit;

    // Enter edit mode silently (no console manipulation to keep tests simple)
    return "> ";
  }
}
