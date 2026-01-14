/* cssh.Core.Execution.CommandExecutor.cs - v0.2.0 string-based executor */
using Cssh.Core.Ast;
namespace cssh.Core.Execution;

using System.Text;
using cssh.Core;
using cssh.Core.Commands; // CommandRegistry がここにある前提。実際の namespace に合わせて調整してください。

/// <summary>
/// Executes parsed AST (Sequence / Pipeline / CommandNode) using the existing
/// ICommand (string-based) command model.
/// </summary>
public sealed class CommandExecutor
{
  /// <summary>
  /// The _registry field.
  /// </summary>
  private readonly CommandRegistry _registry;

  public CommandExecutor(CommandRegistry registry)
  {
    _registry = registry ?? throw new ArgumentNullException(nameof(registry));
  }

  /// <summary>
  /// Executes a sequence of pipelines separated by ';'.
  /// Returns the output of the last pipeline (for REPL display).
  /// </summary>
  public string ExecuteSequence(ShellState state, Sequence sequence)
  {
    if (sequence == null) throw new ArgumentNullException(nameof(sequence));
    if (state == null) throw new ArgumentNullException(nameof(state));

    string lastOutput = string.Empty;

    foreach (var pipeline in sequence.Pipelines)
    {
      lastOutput = ExecutePipeline(state, pipeline);
    }

    return lastOutput;
  }

  /// <summary>
  /// Executes a single pipeline, e.g. cmd1 | cmd2 | cmd3.
  /// The output of each command becomes the "piped input" for the next one.
  /// </summary>
  private string ExecutePipeline(ShellState state, Pipeline pipeline)
  {
    if (pipeline == null) throw new ArgumentNullException(nameof(pipeline));

    string? pipedInput = null;
    string lastOutput = string.Empty;

    foreach (var node in pipeline.Commands)
    {
      lastOutput = ExecuteCommandNode(state, node, pipedInput);
      pipedInput = lastOutput;
    }

    return lastOutput;
  }

  /// <summary>
  /// Executes a single command node with optional redirects and piped input.
  /// </summary>
  private string ExecuteCommandNode(
  ShellState state,
  CommandNode node,
  string? pipedInput)
  {
    if (node == null) throw new ArgumentNullException(nameof(node));

    // 1. Resolve command
    var command = _registry.Resolve(node.Name);
    if (command == null)
    {
      return $"cssh: command not found: {node.Name}";
    }

    // 2. Build argument list, optionally including "piped input" and redirected input
    var args = BuildArgumentsWithInput(node, pipedInput);

    // 3. Execute command (ICommand is string-based, unchanged)
    var rawOutput = command.Execute(state, args);

    // 4. Apply redirects (>, >>). Input redirect (<) is already handled in args.
    var finalOutput = ApplyOutputRedirects(rawOutput, node.Redirect);

    return finalOutput;
  }

  /// <summary>
  /// Builds the argument array for a command, combining:
  /// - original parsed arguments
  /// - piped input (if present)
  /// - input redirection content (&lt; file) if configured
  ///
  /// NOTE: この挙動は「最小構成」の暫定方針です。
  ///       - 元の args をそのまま維持
  ///       - pipedInput があれば末尾に追加
  ///       - &lt; file があればさらに末尾に追加
  /// 実運用で不自然に感じたら、ここを差し替えれば済みます（他の構造には影響しません）。
  /// </summary>
  private static string[] BuildArgumentsWithInput(
  CommandNode node,
  string? pipedInput)
  {
    var list = new List<string>(node.Args);

    // パイプからの入力を最後の引数として追加（暫定仕様）
    if (!string.IsNullOrEmpty(pipedInput))
    {
      list.Add(pipedInput);
    }

    // 入力リダイレクト (< file) がある場合、ファイル内容をさらに追加
    if (node.Redirect?.FilePath is string inputPath && !string.IsNullOrWhiteSpace(inputPath))
    {
      try
      {
        var text = File.ReadAllText(inputPath, Encoding.UTF8);
        list.Add(text);
      }
      catch (Exception ex)
      {
        // ここでは例外をそのまま引数として渡すのではなく、
        // エラー内容を特殊な形で埋め込む簡易実装とする。
        // 実運用では、エラーを即時返却するなどの方針に変えても構いません。
        list.Add($"[cssh: failed to read '{inputPath}': {ex.Message}]");
      }
    }

    return list.ToArray();
  }

  /// <summary>
  /// Applies output redirects (&gt;, &gt;&gt;) if configured.
  /// When redirected, the text is written to file and an empty string is returned
  /// so that REPL does not duplicate the output.
  /// </summary>
  private static string ApplyOutputRedirects(string output, RedirectInfo? redirect)
  {
    if (redirect == null)
    return output;

    if (string.IsNullOrWhiteSpace(redirect.FilePath))
    return output;

    try
    {
      var path = redirect.FilePath;

      if (redirect.Type == RedirectType.OutputAppend)
      {
        File.AppendAllText(path, output, Encoding.UTF8);
      }
      else
      {
        File.WriteAllText(path, output, Encoding.UTF8);
      }

      // リダイレクトされた場合は画面には何も表示しない
      return string.Empty;
    }
    catch (Exception ex)
    {
      // エラーは画面に出す（リダイレクト失敗はユーザーに見せたい）
      return $"cssh: redirect failed: {ex.Message}";
    }
  }
}
