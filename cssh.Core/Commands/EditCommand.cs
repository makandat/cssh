/* cssh.Core.Commands.EditCommand - A cross-platform C# shell ver.0.1.2 EditCommand.cs */
using cssh.Core;

namespace cssh.Core.Commands;

public class EditCommand : ICommand
{
    public string Name => "edit";

    public string Execute(ShellState state, string[] args)
    {
        // 編集モードへ遷移
        state.Mode = ShellMode.Edit;

        // 画面クリア + カーソルを左下へ移動
        int bottom = Console.WindowHeight;
        return $"\u001b[2J\u001b[{bottom};1H";
    }
}