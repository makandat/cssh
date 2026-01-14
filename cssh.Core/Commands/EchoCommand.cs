/* cssh.Core.Commands.EchoCommand.cs v0.2.0 */
namespace cssh.Core.Commands;

/// <summary>
/// Outputs the given arguments as a single line of text.
/// No variable expansion or special processing is performed.
/// </summary>
public class EchoCommand : ICommand
{
  /// <summary>
  /// The Name field.
  /// </summary>
  public string Name => "echo";

  /// <summary>
  /// The Description field.
  /// </summary>
  public string Description => "Print the given arguments.";

  /// <param name="state"></param>
  /// <param name="args"></param>
  /// <returns></returns>
  public string Execute(ShellState state, string[] args)
  {
    // echo → empty line
    if (args.Length == 0)
    return string.Empty;

    return string.Join(" ", args);
  }
}
