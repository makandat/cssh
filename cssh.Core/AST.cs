/* cssh.Core.AST.cs - A cross-platform C# shell v0.2.0 Abstract Syntax Tree definitions */
namespace Cssh.Core.Ast;

/// <summary>
/// Represents the RedirectType enum.
/// </summary>
public enum RedirectType
{
  None,
  OutputTruncate, // >
  OutputAppend,   // >>
  Input           // <
}

/// <summary>
/// Represents the RedirectInfo class.
/// </summary>
public sealed class RedirectInfo
{
  /// <summary>
  /// Gets or sets the Type.
  /// </summary>
  public RedirectType Type { get; }
  public string? FilePath { get; }

  public RedirectInfo(RedirectType type, string? filePath)
  {
    Type = type;
    FilePath = filePath;
  }

  /// <param name="new(RedirectType.None"></param>
  /// <param name="null"></param>
  /// <returns></returns>
  public static RedirectInfo None() => new(RedirectType.None, null);
}

/// <summary>
/// Represents the CommandNode class.
/// </summary>
public sealed class CommandNode
{
  /// <summary>
  /// Gets or sets the Name.
  /// </summary>
  public string Name { get; }
  /// <summary>
  /// Gets or sets the Args.
  /// </summary>
  public IReadOnlyList<string> Args { get; }
  /// <summary>
  /// Gets or sets the Redirect.
  /// </summary>
  public RedirectInfo Redirect { get; }

  public CommandNode(string name, IReadOnlyList<string> args, RedirectInfo redirect)
  {
    Name = name;
    Args = args;
    Redirect = redirect;
  }
}

/// <summary>
/// Represents the Pipeline class.
/// </summary>
public sealed class Pipeline
{
  /// <summary>
  /// Gets or sets the Commands.
  /// </summary>
  public IReadOnlyList<CommandNode> Commands { get; }

  public Pipeline(IReadOnlyList<CommandNode> commands)
  {
    Commands = commands;
  }
}

/// <summary>
/// Represents the Sequence class.
/// </summary>
public sealed class Sequence
{
  /// <summary>
  /// Gets or sets the Pipelines.
  /// </summary>
  public IReadOnlyList<Pipeline> Pipelines { get; }

  public Sequence(IReadOnlyList<Pipeline> pipelines)
  {
    Pipelines = pipelines;
  }
}
