/* cssh.Core.Commands.ClearCommand.cs v0.2.0 */
using cssh.Core;

/// <summary>
/// Represents the ClearCommand class.
/// </summary>
public class ClearCommand : ICommand
{
  /// <summary>
  /// The Name field.
  /// </summary>
  public string Name => "clear";
  /// <summary>
  /// The Description field.
  /// </summary>
  public string Description => "Clear the screen.";

  /// <param name="state"></param>
  /// <param name="args"></param>
  /// <returns></returns>
  public string Execute(ShellState state, string[] args)
  {
    // Bash と同じ動作：画面クリア + カーソルを左上へ
    // Console.Write("\u001b[2J\u001b[H");
    // return string.Empty;
    return "\u001b[2J\u001b[H";
  }
}
