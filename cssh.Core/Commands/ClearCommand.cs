/* cssh.Core.Commands.ClearCommand.cs v0.1.5 */
using cssh.Core;

public class ClearCommand : ICommand
{
  public string Name => "clear";
  public string Description => "Clear the screen.";

  public string Execute(ShellState state, string[] args)
  {
    // Bash と同じ動作：画面クリア + カーソルを左上へ
    // Console.Write("\u001b[2J\u001b[H");
    // return string.Empty;
    return "\u001b[2J\u001b[H";
  }
}