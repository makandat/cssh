/* cssh.Core.CommandParser.cs - Quoted argument support (" and ') */
namespace cssh.Core;

using System.Text;

/// <summary>
/// Parses raw input into command name and arguments.
/// Supports quoted arguments using both "..." and '...'.
/// </summary>
public class CommandParser
{
  public ParsedCommand Parse(string input)
  {
    var args = new List<string>();
    var current = new StringBuilder();

    bool inDoubleQuotes = false;
    bool inSingleQuotes = false;

    foreach (var c in input.Trim())
    {
      if (c == '"' && !inSingleQuotes)
      {
        inDoubleQuotes = !inDoubleQuotes;
        continue;
      }

      if (c == '\'' && !inDoubleQuotes)
      {
        inSingleQuotes = !inSingleQuotes;
        continue;
      }

      if (char.IsWhiteSpace(c) && !inDoubleQuotes && !inSingleQuotes)
      {
        if (current.Length > 0)
        {
          args.Add(current.ToString());
          current.Clear();
        }
      }
      else
      {
        current.Append(c);
      }
    }

    if (current.Length > 0)
      args.Add(current.ToString());

    var command = args.Count > 0 ? args[0] : string.Empty;
    var commandArgs = args.Skip(1).ToArray();

    return new ParsedCommand(command, commandArgs);
  }
}