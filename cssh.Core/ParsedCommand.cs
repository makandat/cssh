/* cssh.Core.ParsedCommand.cs - Parsed command representation */
namespace cssh.Core;

/// <summary>
/// Represents a parsed command with its arguments.
/// Provides helper methods for option detection.
/// </summary>
public record ParsedCommand(string Command, string[] Arguments)
{
  /// <summary>
  /// Returns true if the argument list contains the given short option.
  /// Example: HasOption("a") matches "-a", "-la", "-al".
  /// </summary>
  public bool HasOption(string opt)
  {
    foreach (var arg in Arguments)
    {
      if (!arg.StartsWith("-"))
      continue;

      if (arg.Contains(opt))
      return true;
    }
    return false;
  }
}
