/* cssh.Core.Commands.RmCommand.cs v0.2.0 */
namespace cssh.Core.Commands;

using cssh.Core;
/// <summary>
/// Represents the RmCommand class.
/// </summary>
public class RmCommand : ICommand
{
  /// <summary>
  /// The Name field.
  /// </summary>
  public string Name => "rm";
  /// <summary>
  /// The Description field.
  /// </summary>
  public string Description => "Remove a file.";

  /// <param name="state"></param>
  /// <param name="args"></param>
  /// <returns></returns>
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
