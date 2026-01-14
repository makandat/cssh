/* cssh.Core.CommandParser.cs - Pipeline and Redirect support (>, >>, <, |) */
namespace cssh.Core;

using System.Text;
using Cssh.Core.Ast;

/// <summary>
/// Parses raw input into AST (Sequence of Pipelines with Redirects).
/// Supports quoted arguments using both "..." and '...' and operators (>, >>, <, |).
/// </summary>
public class CommandParser
{
  /// <summary>
  /// Parses input into a Sequence (for pipeline execution).
  /// Supports multiple command groups separated by ';' (outside quotes).
  /// </summary>
  public Sequence ParseSequence(string input)
  {
    var pipelines = new List<Pipeline>();

    var parts = SplitBySemicolons(input);

    foreach (var part in parts)
    {
      var commands = ParsePipeline(part);
      if (commands.Count > 0)
      {
        pipelines.Add(new Pipeline(commands));
      }
    }

    return new Sequence(pipelines.Count > 0 ? pipelines : new[] { new Pipeline(new List<CommandNode>()) });
  }

  /// <summary>
  /// Parses a single pipeline (one or more commands separated by |).
  /// </summary>
  private List<CommandNode> ParsePipeline(string input)
  {
    var commands = new List<CommandNode>();
    var tokens = TokenizeWithRedirects(input);

    int i = 0;
    while (i < tokens.Count)
    {
      var token = tokens[i];

      // Skip pipes
      if (token == "|")
      {
        i++;
        continue;
      }

      var args = new List<string>();
      var redirect = RedirectInfo.None();

      // Collect arguments until next pipe or redirect
      while (i < tokens.Count && tokens[i] != "|" && !IsRedirectOperator(tokens[i]))
      {
        args.Add(tokens[i]);
        i++;
      }

      // Check for redirect
      if (i < tokens.Count && IsRedirectOperator(tokens[i]))
      {
        var op = tokens[i];
        i++;

        if (i >= tokens.Count)
        {
          // Error: redirect operator without target
          break;
        }

        var filePath = tokens[i];
        var redirectType = op switch
        {
          ">" => RedirectType.OutputTruncate,
          ">>" => RedirectType.OutputAppend,
          "<" => RedirectType.Input,
          _ => RedirectType.None
        };

        redirect = new RedirectInfo(redirectType, filePath);
        i++;
      }

      if (args.Count > 0)
      {
        var cmdName = args[0];
        var cmdArgs = args.Skip(1).ToList();
        commands.Add(new CommandNode(cmdName, cmdArgs, redirect));
      }
    }

    return commands;
  }

  /// <summary>
  /// Tokenizes input while preserving redirect operators and quoted strings.
  /// </summary>
  private List<string> TokenizeWithRedirects(string input)
  {
    var tokens = new List<string>();
    var current = new StringBuilder();
    bool inDoubleQuotes = false;
    bool inSingleQuotes = false;

    for (int i = 0; i < input.Length; i++)
    {
      var c = input[i];

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

      // Detect redirect operators outside quotes
      if (!inDoubleQuotes && !inSingleQuotes && IsRedirectChar(c))
      {
        if (current.Length > 0)
        {
          tokens.Add(current.ToString());
          current.Clear();
        }

        // Handle >> and << as single tokens
        if (c == '>' && i + 1 < input.Length && input[i + 1] == '>')
        {
          tokens.Add(">>");
          i++;
        }
        else if (c == '<' && i + 1 < input.Length && input[i + 1] == '<')
        {
          tokens.Add("<<");
          i++;
        }
        else
        {
          tokens.Add(c.ToString());
        }
        continue;
      }

      // Handle pipes outside quotes
      if (!inDoubleQuotes && !inSingleQuotes && c == '|')
      {
        if (current.Length > 0)
        {
          tokens.Add(current.ToString());
          current.Clear();
        }
        tokens.Add("|");
        continue;
      }

      // Regular character or whitespace
      if (char.IsWhiteSpace(c) && !inDoubleQuotes && !inSingleQuotes)
      {
        if (current.Length > 0)
        {
          tokens.Add(current.ToString());
          current.Clear();
        }
      }
      else
      {
        // Environment variable expansion: expand $VAR when not inside single quotes
        if (c == '$' && !inSingleQuotes)
        {
          // Parse variable name (letters, digits, underscore)
          int j = i + 1;
          while (j < input.Length && (char.IsLetterOrDigit(input[j]) || input[j] == '_'))
          j++;

          if (j > i + 1)
          {
            var varName = input.Substring(i + 1, j - (i + 1));
            var value = Environment.GetEnvironmentVariable(varName) ?? string.Empty;
            current.Append(value);
            i = j - 1; // advance the main loop to the last var char
            continue;
          }
          else
          {
            // not a valid var name, treat '$' as literal
            current.Append(c);
            continue;
          }
        }

        current.Append(c);
      }
    }

    if (current.Length > 0)
    tokens.Add(current.ToString());

    return tokens;
  }

  /// <summary>
  /// Splits input by pipes (outside quotes).
  /// </summary>
  private List<string> SplitByPipes(string input)
  {
    var parts = new List<string>();
    var current = new StringBuilder();
    bool inDoubleQuotes = false;
    bool inSingleQuotes = false;

    foreach (var c in input)
    {
      if (c == '"' && !inSingleQuotes)
      {
        inDoubleQuotes = !inDoubleQuotes;
        current.Append(c);
        continue;
      }

      if (c == '\'' && !inDoubleQuotes)
      {
        inSingleQuotes = !inSingleQuotes;
        current.Append(c);
        continue;
      }

      if (c == '|' && !inDoubleQuotes && !inSingleQuotes)
      {
        if (current.Length > 0)
        parts.Add(current.ToString());
        current.Clear();
        continue;
      }

      current.Append(c);
    }

    if (current.Length > 0)
    parts.Add(current.ToString());

    return parts;
  }

  /// <summary>
  /// Splits input by semicolons (outside quotes).
  /// </summary>
  private List<string> SplitBySemicolons(string input)
  {
    var parts = new List<string>();
    var current = new StringBuilder();
    bool inDoubleQuotes = false;
    bool inSingleQuotes = false;

    for (int i = 0; i < input.Length; i++)
    {
      var c = input[i];

      if (c == '"' && !inSingleQuotes)
      {
        inDoubleQuotes = !inDoubleQuotes;
        current.Append(c);
        continue;
      }

      if (c == '\'' && !inDoubleQuotes)
      {
        inSingleQuotes = !inSingleQuotes;
        current.Append(c);
        continue;
      }

      if (c == ';' && !inDoubleQuotes && !inSingleQuotes)
      {
        if (current.Length > 0)
        parts.Add(current.ToString());
        current.Clear();
        continue;
      }

      current.Append(c);
    }

    if (current.Length > 0)
    parts.Add(current.ToString());

    return parts;
  }

  /// <param name="token"></param>
  /// <returns></returns>
  private bool IsRedirectOperator(string token)
  {
    return token == ">" || token == ">>" || token == "<" || token == "<<";
  }

  /// <param name="c"></param>
  /// <returns></returns>
  private bool IsRedirectChar(char c)
  {
    return c == '>' || c == '<';
  }

  /// <summary>
  /// Legacy Parse method for backward compatibility.
  /// </summary>
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
