/* cssh.Core.Commands.RmCommand.cs v0.1.5 */
namespace cssh.Core.Commands;

using cssh.Core;
public class RmCommand : ICommand
{
  public string Name => "rm";
  public string Description => "Remove a file.";

  public string Execute(ShellState state, string[] args)
  {
    if (args.Length == 0)
      return "rm: missing operand";

    var path = PathNormalizer.Normalize(args[0]);

    if (!Path.IsPathRooted(path))
      path = Path.Combine(state.CurrentDirectory, path);

    if (!File.Exists(path))
      return $"rm: no such file: {args[0]}";

    try
    {
      File.Delete(path);
    }
    catch (Exception ex)
    {
      return $"rm: error: {ex.Message}";
    }

    return string.Empty;
  }
}