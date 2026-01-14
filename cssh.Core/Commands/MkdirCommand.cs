/* cssh.Core.Commands.MkdirCommand.cs v0.2.0 */
namespace cssh.Core.Commands;

using cssh.Core;
/// <summary>
/// Represents the MkdirCommand class.
/// </summary>
public class MkdirCommand : ICommand
{
  /// <summary>
  /// The Name field.
  /// </summary>
  public string Name => "mkdir";
  /// <summary>
  /// The Description field.
  /// </summary>
  public string Description => "Create a directory.";

  /// <param name="state"></param>
  /// <param name="args"></param>
  /// <returns></returns>
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
