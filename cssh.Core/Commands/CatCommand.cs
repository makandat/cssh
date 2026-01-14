/* cssh.Core.Commands.CatCommand.cs v0.2.0 */
namespace cssh.Core.Commands;

/// <summary>
/// Outputs the contents of a file.
/// </summary>
public class CatCommand : ICommand
{
  /// <summary>
  /// The Name field.
  /// </summary>
  public string Name => "cat";

  /// <summary>
  /// The Description field.
  /// </summary>
  public string Description => "Print the contents of a file.";


  /// <param name="state"></param>
  /// <param name="args"></param>
  /// <returns></returns>
  public string Execute(ShellState state, string[] args)
  {
    if (args.Length == 0)
    return "cat: missing operand";

    var first = args[0];

    // If input came from stdin (prefixed), return it directly
    if (first.Length > 0 && first[0] == '\u0001')
    {
      return first.Substring(1);
    }

    var path = PathNormalizer.Normalize(first);

    if (!Path.IsPathRooted(path))
    path = Path.Combine(state.CurrentDirectory, path);

    if (!File.Exists(path))
    return $"cat: no such file: {first}";

    try
    {
      return File.ReadAllText(path);
    }
    catch (Exception ex)
    {
      return $"cat: error reading file: {ex.Message}";
    }
  }
}
