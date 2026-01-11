/* cssh.Core.Commands.RmdirCommand.cs v0.1.5 */
namespace cssh.Core.Commands;

using cssh.Core;
public class RmdirCommand : ICommand
{
  public string Name => "rmdir";
  public string Description => "Remove an empty directory.";

  public string Execute(ShellState state, string[] args)
  {
    if (args.Length == 0)
      return "rmdir: missing operand";

    var path = PathNormalizer.Normalize(args[0]);

    if (!Path.IsPathRooted(path))
      path = Path.Combine(state.CurrentDirectory, path);

    if (!Directory.Exists(path))
      return $"rmdir: no such directory: {args[0]}";

    try
    {
      Directory.Delete(path, false); // 空でないと例外
    }
    catch (Exception ex)
    {
      return $"rmdir: error: {ex.Message}";
    }

    return string.Empty;
  }
}