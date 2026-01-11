/* cssh.Core.Commands.TouchCommand.cs v0.1.5 */
namespace cssh.Core.Commands;

/// <summary>
/// Creates an empty file if it does not exist.
/// If the file exists, updates its last write time.
/// </summary>
public class TouchCommand : ICommand
{
  public string Name => "touch";

  public string Description => "Create a file or update its modification time.";

  public string Execute(ShellState state, string[] args)
  {
    if (args.Length == 0)
      return "touch: missing operand";

    var path = PathNormalizer.Normalize(args[0]);

    if (!Path.IsPathRooted(path))
      path = Path.Combine(state.CurrentDirectory, path);

    try
    {
      if (!File.Exists(path))
      {
        // Create empty file
        File.WriteAllText(path, string.Empty);
      }
      else
      {
        // Update modification time
        File.SetLastWriteTime(path, DateTime.Now);
      }
    }
    catch (Exception ex)
    {
      return $"touch: error: {ex.Message}";
    }

    return string.Empty;
  }
}