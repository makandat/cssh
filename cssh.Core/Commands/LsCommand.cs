/* cssh.Core.Commands.LsCommand.cs - v0.1.3 */
namespace cssh.Core.Commands;

using System.Globalization;

/// <summary>
/// Implements the <c>ls</c> command with Bash-like behavior,
/// supporting <c>-a</c> and <c>-l</c> options.
/// </summary>
public class LsCommand : ICommand
{
  /// <summary>
  /// The Name field.
  /// </summary>
  public string Name => "ls";

  /// <param name="state"></param>
  /// <param name="args"></param>
  /// <returns></returns>
  public string Execute(ShellState state, string[] args)
  {
    var parsed = new ParsedCommand("ls", args);

    bool showAll = parsed.HasOption("a");
    bool longFormat = parsed.HasOption("l");

    var dir = PathNormalizer.Normalize(state.CurrentDirectory);

    var entries = Directory.GetFileSystemEntries(dir)
    .OrderBy(Path.GetFileName)
    .Where(path =>
    {
      var name = Path.GetFileName(path)!;

      if (showAll)
      return true;

      // hide dotfiles
      return !name.StartsWith(".");
    });

    if (longFormat)
    return FormatLong(entries);

    return FormatShort(entries);
  }

  /// <param name="entries"></param>
  /// <returns></returns>
  private static string FormatShort(IEnumerable<string> entries)
  {
    return string.Join(Environment.NewLine, entries.Select(FormatEntry));
  }

  /// <param name="entries"></param>
  /// <returns></returns>
  private static string FormatLong(IEnumerable<string> entries)
  {
    var lines = new List<string>();

    foreach (var path in entries)
    {
      var info = new FileInfo(path);
      var name = FormatEntry(path);

      string perms = GetPseudoPermissions(path);
      string size = Directory.Exists(path) ? "-" : info.Length.ToString();
      string date = info.LastWriteTime.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);

      lines.Add($"{perms} {size,8} {date} {name}");
    }

    return string.Join(Environment.NewLine, lines);
  }

  /// <param name="path"></param>
  /// <returns></returns>
  private static string FormatEntry(string path)
  {
    var name = Path.GetFileName(path)!;

    if (Directory.Exists(path))
    return name + "/";

    if (IsSymlink(path))
    return name + "@";

    if (IsExecutable(path))
    return name + "*";

    return name;
  }

  /// <param name="path"></param>
  /// <returns></returns>
  private static string GetPseudoPermissions(string path)
  {
    if (Directory.Exists(path))
    return "drwxr-xr-x";

    return "-rw-r--r--";
  }

  /// <param name="path"></param>
  /// <returns></returns>
  private static bool IsExecutable(string path)
  {
    var ext = Path.GetExtension(path).ToLowerInvariant();
    return ext is ".exe" or ".bat" or ".cmd";
  }

  /// <param name="path"></param>
  /// <returns></returns>
  private static bool IsSymlink(string path)
  {
    var attr = File.GetAttributes(path);
    return (attr & FileAttributes.ReparsePoint) != 0;
  }
}
