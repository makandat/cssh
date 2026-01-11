/* cssh.Core.Commands.EchoCommand.cs v0.1.4 */
namespace cssh.Core.Commands;

/// <summary>
/// Outputs the given arguments as a single line of text.
/// No variable expansion or special processing is performed.
/// </summary>
public class EchoCommand : ICommand
{
  public string Name => "echo";

  public string Description => "Print the given arguments.";

  public string Execute(ShellState state, string[] args)
  {
    // echo → empty line
    if (args.Length == 0)
      return string.Empty;

    return string.Join(" ", args);
  }
}