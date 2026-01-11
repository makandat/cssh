/* cssh.Core.Commands.CatCommand.cs v0.1.5 */
namespace cssh.Core.Commands;

/// <summary>
/// Outputs the contents of a file.
/// </summary>
public class CatCommand : ICommand
{
  public string Name => "cat";

  public string Description => "Print the contents of a file.";


  public string Execute(ShellState state, string[] args)
  {
    if (args.Length == 0)
      return "cat: missing operand";

    var path = PathNormalizer.Normalize(args[0]);

    if (!Path.IsPathRooted(path))
      path = Path.Combine(state.CurrentDirectory, path);

    if (!File.Exists(path))
      return $"cat: no such file: {args[0]}";

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