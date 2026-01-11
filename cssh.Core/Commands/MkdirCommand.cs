/* cssh.Core.Commands.MkdirCommand.cs v0.1.5 */
namespace cssh.Core.Commands;

using cssh.Core;
public class MkdirCommand : ICommand
{
  public string Name => "mkdir";
  public string Description => "Create a directory.";

  public string Execute(ShellState state, string[] args)
  {
    if (args.Length == 0)
      return "mkdir: missing operand";

    var path = PathNormalizer.Normalize(args[0]);

    if (!Path.IsPathRooted(path))
      path = Path.Combine(state.CurrentDirectory, path);

    try
    {
      Directory.CreateDirectory(path);
    }
    catch (Exception ex)
    {
      return $"mkdir: error: {ex.Message}";
    }

    return string.Empty;
  }
}