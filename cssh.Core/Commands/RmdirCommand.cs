/* cssh.Core.Commands.RmdirCommand.cs v0.2.0 */
namespace cssh.Core.Commands;

using cssh.Core;
/// <summary>
/// Represents the RmdirCommand class.
/// </summary>
public class RmdirCommand : ICommand
{
  /// <summary>
  /// The Name field.
  /// </summary>
  public string Name => "rmdir";
  /// <summary>
  /// The Description field.
  /// </summary>
  public string Description => "Remove an empty directory.";

  /// <param name="state"></param>
  /// <param name="args"></param>
  /// <returns></returns>
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
