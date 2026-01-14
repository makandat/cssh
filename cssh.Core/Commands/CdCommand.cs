/* cssh.Core.Commands.CdCommand.cs - v0.1.2 */
namespace cssh.Core.Commands;

/// <summary>
/// Implements the <c>cd</c> command, changing the current working directory.
/// Follows Bash behavior for basic directory navigation.
/// </summary>
public class CdCommand : ICommand
{
  /// <inheritdoc />
  public string Name => "cd";

  /// <summary>
  /// Changes the current working directory.
  /// Supports: cd, cd -, cd .., cd <path>.
  /// </summary>
  public string Execute(ShellState state, string[] args)
  {
    string target;

    // cd → home directory
    if (args.Length == 0)
    {
      target = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
    }
    // cd -
    else if (args[0] == "-")
    {
      target = state.PreviousDirectory;
    }
    else
    {
      target = args[0];
    }
    PathNormalizer.Normalize(target);

    // Resolve relative paths
    if (!Path.IsPathRooted(target))
    target = Path.GetFullPath(Path.Combine(state.CurrentDirectory, target));

    // Validate directory
    if (!Directory.Exists(target))
    return $"cd: no such file or directory: {args[0]}";

    // Update state
    state.PreviousDirectory = state.CurrentDirectory;
    state.CurrentDirectory = target;

    // Bash: cd は成功時に出力しない
    return string.Empty;
  }
}
