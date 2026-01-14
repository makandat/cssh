/* cssh.Core.Commands.TouchCommand.cs v0.2.0 */
namespace cssh.Core.Commands;

/// <summary>
/// Creates an empty file if it does not exist.
/// If the file exists, updates its last write time.
/// </summary>
public class TouchCommand : ICommand
{
  /// <summary>
  /// The Name field.
  /// </summary>
  public string Name => "touch";

  /// <summary>
  /// The Description field.
  /// </summary>
  public string Description => "Create a file or update its modification time.";

  /// <param name="state"></param>
  /// <param name="args"></param>
  /// <returns></returns>
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
