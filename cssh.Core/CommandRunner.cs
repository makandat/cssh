/* cssh.Core.CommandRunner.cs - Pipeline and Redirect support */
namespace cssh.Core;

using cssh.Core.Commands;
using Cssh.Core.Ast;
using System.Diagnostics;
using System.Linq;

/// <summary>
/// Executes commands by coordinating the parser, registry, and shell state.
/// Supports pipelines (|) and redirects (>, >>, <).
/// </summary>
public class CommandRunner
{
  /// <summary>
  /// The _parser field.
  /// </summary>
  private readonly CommandParser _parser;
  /// <summary>
  /// The _registry field.
  /// </summary>
  private readonly CommandRegistry _registry;

  /// <summary>
  /// Initializes a new instance of the <see cref="CommandRunner"/> class.
  /// </summary>
  /// <param name="parser">The command parser.</param>
  /// <param name="registry">The command registry.</param>
  public CommandRunner(CommandParser parser, CommandRegistry registry)
  {
    _parser = parser;
    _registry = registry;
  }

  /// <summary>
  /// Parses and executes a command string with support for pipelines and redirects.
  /// </summary>
  /// <param name="state">The current shell state.</param>
  /// <param name="input">The raw input string.</param>
  /// <returns>The command output, or an error message.</returns>
  public string Run(ShellState state, string input)
  {
    try
    {
      // If in edit mode, handle editor commands / buffer edits
      if (state.Mode == ShellMode.Edit)
      {
        return HandleEditModeInput(state, input);
      }

      var trimmed = input?.Trim() ?? string.Empty;
      if (string.IsNullOrEmpty(trimmed))
      return string.Empty;

      // History expansion: !<n> or !<prefix>
      if (trimmed.StartsWith("!") && trimmed.Length > 1)
      {
        var key = trimmed.Substring(1);

        // numeric index: !<n>
        if (int.TryParse(key, out var idx))
        {
          if (idx < 1 || idx > state.History.Count)
          return "history: event not found";

          // Replace input with historical entry (1-based)
          input = state.History[idx - 1];
        }
        else
        {
          // prefix search: find most recent history entry that starts with the given prefix
          string? found = null;
          for (int i = state.History.Count - 1; i >= 0; i--)
          {
            if (state.History[i].StartsWith(key, StringComparison.Ordinal))
            {
              found = state.History[i];
              break;
            }
          }

          if (found == null)
          return "history: event not found";

          input = found;
        }
      }

      // Ensure input is non-null and record it in history (session-local)
      input ??= string.Empty;
      state.History.Add(input);

      var sequence = _parser.ParseSequence(input);
      var results = new List<string>();

      foreach (var pipeline in sequence.Pipelines)
      {
        if (pipeline.Commands.Count == 0)
        continue;

        var output = ExecutePipeline(state, pipeline);
        if (!string.IsNullOrEmpty(output))
        results.Add(output);
      }

      return string.Join(Environment.NewLine, results);
    }
    catch (Exception ex)
    {
      return $"Error: {ex.Message}";
    }
  }

  /// <summary>
  /// Handles inputs while in edit mode.
  /// Supported editor commands:
  ///  - read|r <file>  : load file into buffer
  ///  - write|w [file] : write buffer to file (optional file to save as)
  ///  - run [file]     : run buffer as simple script (supports only lines starting with 'echo ')
  ///  - quit|q         : exit edit mode
  /// Any other input is appended as a line to the edit buffer.
  /// </summary>
  private string HandleEditModeInput(ShellState state, string input)
  {
    var trimmed = input?.Trim() ?? string.Empty;
    if (string.IsNullOrEmpty(trimmed))
    return string.Empty;

    var parts = trimmed.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
    var cmd = parts[0].ToLowerInvariant();
    var arg = parts.Length > 1 ? parts[1].Trim() : string.Empty;

    if (cmd == "read" || cmd == "r")
    {
      if (string.IsNullOrEmpty(arg))
      return "read: error: missing filename";

      var path = arg.Trim('"');
      if (!File.Exists(path))
      return $"read: error: {path}: No such file or directory";

      try
      {
        state.EditBuffer = File.ReadAllText(path);
        state.EditFileName = path;
        state.EditDirty = false;
        return string.Empty;
      }
      catch (Exception ex)
      {
        return $"read: error: {ex.Message}";
      }
    }

    if (cmd == "write" || cmd == "w")
    {
      string path = state.EditFileName;
      if (!string.IsNullOrEmpty(arg))
      path = arg.Trim('"');

      if (string.IsNullOrEmpty(path))
      return "write: error: missing filename";

      try
      {
        File.WriteAllText(path, state.EditBuffer ?? string.Empty);
        state.EditFileName = path;
        state.EditDirty = false;
        return string.Empty;
      }
      catch (Exception ex)
      {
        return $"write: error: {ex.Message}";
      }
    }

    if (cmd == "run")
    {
      // Optional filename argument: load it first
      if (!string.IsNullOrEmpty(arg))
      {
        var path = arg.Trim('"');
        if (!File.Exists(path))
        return $"run: error: {path}: No such file or directory";

        try
        {
          state.EditBuffer = File.ReadAllText(path);
          state.EditFileName = path;
          state.EditDirty = false;
        }
        catch (Exception ex)
        {
          return $"run: error: {ex.Message}";
        }
      }

      // Script runner: support simple 'echo' lines, and C# (.csx) execution via Roslyn scripting for files with .csx or when buffer looks like C# script
      var buffer = state.EditBuffer ?? string.Empty;
      var lines = buffer.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

      // If buffer looks like a C# script (shebang // or contains 'using ' or lines with C# code), try Roslyn scripting
      bool looksLikeCSharp = buffer.Contains("using ") || buffer.Contains("System") || buffer.Contains("class ") || buffer.Contains("Console.") || buffer.TrimStart().StartsWith("#") || buffer.TrimStart().StartsWith("//");

      if (looksLikeCSharp)
      {
        try
        {
          // Execute C# script using Roslyn scripting.
          // To avoid races with test frameworks that may close Console.Out,
          // temporarily redirect Console.Out/Err to a local StringWriter so
          // the script can always write safely. After execution, restore
          // the originals and write captured output back to the original
          // Console if possible.
          var scriptOptions = Microsoft.CodeAnalysis.Scripting.ScriptOptions.Default
          .AddReferences(AppDomain.CurrentDomain.GetAssemblies().Where(a => !a.IsDynamic && !string.IsNullOrEmpty(a.Location)))
          .AddImports("System", "System.IO", "System.Linq", "System.Text");

          var script = Microsoft.CodeAnalysis.CSharp.Scripting.CSharpScript.Create(buffer, scriptOptions);

          var originalOut = Console.Out;
          var originalErr = Console.Error;
          var sw = new System.IO.StringWriter();
          try
          {
            Console.SetOut(System.IO.TextWriter.Synchronized(sw));
            Console.SetError(System.IO.TextWriter.Synchronized(sw));

            var scriptState = script.RunAsync().GetAwaiter().GetResult();

            if (scriptState?.ReturnValue != null)
            return scriptState.ReturnValue.ToString() ?? string.Empty;

            return string.Empty;
          }
          finally
          {
            // restore original writers first
            try { Console.SetOut(originalOut); } catch { }
            try { Console.SetError(originalErr); } catch { }

            // attempt to write captured output back to the original console
            try
            {
              var captured = sw.ToString();
              if (!string.IsNullOrEmpty(captured))
              {
                try { originalOut.Write(captured); }
                catch { /* swallow if original is closed */ }
              }
            }
            catch { }
          }
        }
        catch (Microsoft.CodeAnalysis.Scripting.CompilationErrorException cex)
        {
          // Return first diagnostic with line number info
          var diag = cex.Diagnostics.FirstOrDefault();
          if (diag != null)
          return $"run: error: {diag.GetMessage()} (Line {diag.Location.GetLineSpan().StartLinePosition.Line + 1})";

          return $"run: error: {cex.Message}";
        }
        catch (Exception ex)
        {
          return $"run: error: {ex.Message}";
        }
      }

      // Fallback to simple echo-only script: each 'echo ' line is printed
      var outputs = new List<string>();
      foreach (var line in lines)
      {
        var s = line.Trim();
        if (s.StartsWith("echo ", StringComparison.OrdinalIgnoreCase))
        {
          outputs.Add(s.Substring(5));
        }
        else if (string.IsNullOrEmpty(s))
        {
          // skip
        }
        else
        {
          return "run: error: unsupported script content";
        }
      }

      return string.Join(Environment.NewLine, outputs);
    }

    if (cmd == "clear" || cmd == "cls")
    {
      // Clear the edit buffer and reset dirty flag; also clear the screen to mimic UI behavior
      state.EditBuffer = string.Empty;
      state.EditDirty = false;
      try {
        Console.Clear();
      } catch {
        // ignore if running in non-interactive test environment
      }
      return string.Empty;
    }

    if (cmd == "quit" || cmd == "q")
    {
      state.Mode = ShellMode.Normal;
      return string.Empty;
    }

    // Any other input => append to buffer as a new line
    if (!string.IsNullOrEmpty(state.EditBuffer) && !state.EditBuffer.EndsWith("\n"))
    state.EditBuffer += "\n";
    state.EditBuffer += input + "\n";
    state.EditDirty = true;
    return string.Empty;
  }

  /// <summary>
  /// Executes a pipeline of one or more commands.
  /// </summary>
  private string ExecutePipeline(ShellState state, Pipeline pipeline)
  {
    var commands = pipeline.Commands;

    if (commands.Count == 0)
    return string.Empty;

    // Single command with potential redirect
    if (commands.Count == 1)
    {
      return ExecuteCommandWithRedirect(state, commands[0]);
    }

    // Multiple commands (pipe chain)
    string intermediateOutput = string.Empty;

    for (int i = 0; i < commands.Count; i++)
    {
      var cmd = commands[i];
      string output;

      if (i == 0)
      {
        // First command - execute normally with potential input redirect
        output = ExecuteCommandWithInputRedirect(state, cmd);
      }
      else
      {
        // Subsequent commands receive previous output as stdin
        output = ExecutePipedCommand(state, cmd, intermediateOutput);
      }

      intermediateOutput = output;
    }

    // Handle final redirect if present
    var lastCmd = commands[commands.Count - 1];
    if (lastCmd.Redirect.Type != RedirectType.None && lastCmd.Redirect.Type != RedirectType.Input)
    {
      HandleRedirect(lastCmd.Redirect, intermediateOutput);
      return string.Empty;
    }

    return intermediateOutput;
  }

  /// <summary>
  /// Executes a command (first in pipeline or standalone) with input redirect support.
  /// </summary>
  private string ExecuteCommandWithInputRedirect(ShellState state, CommandNode cmd)
  {
    var command = _registry.Resolve(cmd.Name);

    var args = cmd.Args.ToArray();

    // Handle input redirect (<) for builtins: pass file contents as an extra arg
    if (cmd.Redirect.Type == RedirectType.Input)
    {
      if (!File.Exists(cmd.Redirect.FilePath))
      return $"{cmd.Name}: error: {cmd.Redirect.FilePath}: No such file or directory";

      try
      {
        var fileContent = File.ReadAllText(cmd.Redirect.FilePath);
        var newArgs = new List<string>(args) { StdinPrefix + fileContent };

        if (command != null)
        return command.Execute(state, newArgs.ToArray());

        // Try external command with file content as stdin
        return ExecuteExternalCommand(cmd.Name, args, fileContent) ?? $"Unknown command: {cmd.Name}";
      }
      catch (Exception ex)
      {
        return $"{cmd.Name}: error reading file: {ex.Message}";
      }
    }

    if (command != null)
    return command.Execute(state, args);

    // Fallback to external command execution
    return ExecuteExternalCommand(cmd.Name, args, null) ?? $"Unknown command: {cmd.Name}";
  }

  /// <summary>
  /// Attempts to execute an external command using PATH lookup and Process.
  /// Returns the command output, or null if command not found/run failed.
  /// </summary>
  private string? ExecuteExternalCommand(string name, string[] args, string? stdin)
  {
    // Resolve executable path
    string? resolved = null;

    // If name is rooted or contains directory separators, try it directly
    if (Path.IsPathRooted(name) || name.Contains("\\") || name.Contains("/"))
    {
      if (File.Exists(name))
      resolved = name;
    }
    else
    {
      var pathEnv = Environment.GetEnvironmentVariable("PATH") ?? string.Empty;
      var dirs = pathEnv.Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries);
      var exts = new[] { string.Empty, ".exe", ".bat", ".cmd", ".ps1" };

      foreach (var dir in dirs)
      {
        foreach (var ext in exts)
        {
          var candidate = Path.Combine(dir, name + ext);
          if (File.Exists(candidate))
          {
            resolved = candidate;
            break;
          }
        }
        if (resolved != null)
        break;
      }
    }

    if (resolved == null)
    return null;

    try
    {
      var ext = Path.GetExtension(resolved).ToLowerInvariant();
      ProcessStartInfo psi;

      if (ext == ".bat" || ext == ".cmd")
      {
        // Use cmd.exe /C to run batch files
        psi = new ProcessStartInfo("cmd.exe", $"/C \"{resolved}\" {EscapeArgs(args)}");
      }
      else if (ext == ".ps1")
      {
        psi = new ProcessStartInfo("powershell", $"-NoProfile -NonInteractive -File \"{resolved}\" {EscapeArgs(args)}");
      }
      else
      {
        psi = new ProcessStartInfo(resolved, EscapeArgs(args));
      }

      psi.UseShellExecute = false;
      psi.RedirectStandardOutput = true;
      psi.RedirectStandardError = true;
      psi.RedirectStandardInput = true;
      psi.CreateNoWindow = true;

      using var proc = Process.Start(psi);
      if (proc == null)
      return null;

      if (!string.IsNullOrEmpty(stdin))
      {
        proc.StandardInput.Write(stdin);
        proc.StandardInput.Close();
      }
      else
      {
        proc.StandardInput.Close();
      }

      var stdout = proc.StandardOutput.ReadToEnd();
      var stderr = proc.StandardError.ReadToEnd();
      proc.WaitForExit();

      var outText = stdout.TrimEnd('\r', '\n');
      if (!string.IsNullOrEmpty(outText))
      return outText;

      var errText = stderr.TrimEnd('\r', '\n');
      if (!string.IsNullOrEmpty(errText))
      return errText;

      return string.Empty;
    }
    catch
    {
      return null;
    }
  }

  /// <param name="args"></param>
  /// <returns></returns>
  private string EscapeArgs(string[] args)
  {
    return string.Join(" ", args.Select(a => a.Contains(' ') ? $"\"{a}\"" : a));
  }

  /// <summary>
  /// Executes a piped command (receives input from previous command).
  /// </summary>
  private const string StdinPrefix = "\u0001";

  /// <param name="state"></param>
  /// <param name="cmd"></param>
  /// <param name="pipeInput"></param>
  /// <returns></returns>
  private string ExecutePipedCommand(ShellState state, CommandNode cmd, string pipeInput)
  {
    var command = _registry.Resolve(cmd.Name);

    if (command == null)
    return $"Unknown command: {cmd.Name}";

    // For piped input: if no args, pass piped input as the only content
    // If args exist, pass them as-is (some commands may not support piping)
    var args = cmd.Args.ToList();

    // If no arguments and we have piped input, this is stdin
    if (args.Count == 0 && !string.IsNullOrEmpty(pipeInput))
    {
      // Mark the arg as stdin using a special prefix so command implementations can detect it
      args.Add(StdinPrefix + pipeInput);
    }

    return command.Execute(state, args.ToArray());
  }

  /// <summary>
  /// Executes a command with output redirect (>, >>).
  /// </summary>
  private string ExecuteCommandWithRedirect(ShellState state, CommandNode cmd)
  {
    // First execute the command normally
    var output = ExecuteCommandWithInputRedirect(state, cmd);

    // Then handle output redirect if present
    if (cmd.Redirect.Type != RedirectType.None && cmd.Redirect.Type != RedirectType.Input)
    {
      HandleRedirect(cmd.Redirect, output);
      return string.Empty; // Output was redirected, so return empty
    }

    return output;
  }

  /// <summary>
  /// Handles output redirection (>, >>).
  /// </summary>
  private void HandleRedirect(RedirectInfo redirect, string output)
  {
    if (string.IsNullOrEmpty(redirect.FilePath))
    return;

    try
    {
      if (redirect.Type == RedirectType.OutputTruncate)
      {
        // Truncate writes the output as-is (no added newline)
        File.WriteAllText(redirect.FilePath, output);
      }
      else if (redirect.Type == RedirectType.OutputAppend)
      {
        // Append: ensure a single '\n' separates existing content and the new content
        var existing = File.Exists(redirect.FilePath) ? File.ReadAllText(redirect.FilePath) : string.Empty;
        if (!string.IsNullOrEmpty(existing) && !existing.EndsWith("\n"))
        {
          File.AppendAllText(redirect.FilePath, "\n");
        }
        File.AppendAllText(redirect.FilePath, output + "\n");
      }
    }
    catch (Exception ex)
    {
      Console.WriteLine($"Redirect error: {ex.Message}");
    }
  }
}
