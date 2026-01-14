/* cssh - A cross-platform C# shell ver.0.2.0 Program.cs */
using cssh.Core;
using cssh.Core.Commands;
using System.Globalization;
using static ScriptStd;

/// <summary>
/// Entry point of the cssh shell application (v0.2.0).
/// Initializes shell state, command registry, and the main REPL loop.
/// </summary>
var parser = new CommandParser();
var registry = new CommandRegistry();
var state = new ShellState(registry);
var runner = new CommandRunner(parser, registry);

ScriptStd.SetArgs(args);

/// <summary>
/// Registers built-in commands for v0.2.0
/// </summary>
if (CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "ja")
{
  registry.Register(new HelpCommand(), "ヘルプを表示する");
  registry.Register(new ClearCommand(), "画面をクリアする");
  registry.Register(new EditCommand(), "編集モードへ遷移する");
  registry.Register(new CdCommand(), "現在のディレクトリを変更する");
  registry.Register(new LsCommand(), "ディレクトリの内容を表示する");
  registry.Register(new PwdCommand(), "現在のディレクトリを表示する");
  registry.Register(new EchoCommand(), "文字列を表示する");
  registry.Register(new CatCommand(), "ファイルの内容を表示する");
  registry.Register(new TouchCommand(), "ファイルのタイムスタンプを作成または更新する");
  registry.Register(new RmCommand(), "ファイルを削除する");
  registry.Register(new MkdirCommand(), "ディレクトリを作成する");
  registry.Register(new RmdirCommand(), "ディレクトリを削除する");
  registry.Register(new WhichCommand(registry), "コマンドの場所を表示する");
  registry.Register(new AliasCommand("dir", "ls"), "ls コマンドの別名");
  registry.Register(new AliasCommand("type", "cat"), "cat コマンドの別名");
  registry.Register(new AliasCommand("del", "rm"), "rm コマンドの別名");
  registry.Register(new AliasCommand("cls", "clear"), "clear コマンドの別名");
  registry.Register(new AliasCommand("h", "history"), "history コマンドの別名");
  registry.Register(new AliasCommand("where", "which"), "which コマンドの別名");

  // 動的 alias / 履歴サポート (v0.2.1)
  registry.Register(new AliasBuiltinCommand(), "エイリアスの一覧または作成");
  registry.Register(new HistoryCommand(), "コマンド履歴を表示する");
  // 履歴の短縮名
  registry.Register(new AliasCommand("h", "history"), "history コマンドの短縮名");
}
else
{
  registry.Register(new HelpCommand(), "Show help for commands.");
  registry.Register(new ClearCommand(), "Clear the screen.");
  registry.Register(new EditCommand(), "Enter script edit mode.");
  registry.Register(new CdCommand(), "Change directory.");
  registry.Register(new LsCommand(), "List directory contents.");
  registry.Register(new PwdCommand(), "Print working directory.");
  registry.Register(new EchoCommand(), "Print arguments.");
  registry.Register(new CatCommand(), "Print file contents.");
  registry.Register(new TouchCommand(), "Create or update file timestamp.");
  registry.Register(new RmCommand(), "Remove a file.");
  registry.Register(new MkdirCommand(), "Create a directory.");
  registry.Register(new RmdirCommand(), "Remove a directory.");
  registry.Register(new WhichCommand(registry), "Show command location.");

  // Static aliases
  registry.Register(new AliasCommand("dir", "ls"), "Alias of ls command.");
  registry.Register(new AliasCommand("type", "cat"), "Alias of cat command.");
  registry.Register(new AliasCommand("del", "rm"), "Alias of rm command.");
  registry.Register(new AliasCommand("cls", "clear"), "Alias of clear command.");

  // Dynamic alias/history support (v0.2.1)
  registry.Register(new AliasBuiltinCommand(), "List or create aliases (alias name expansion)");
  registry.Register(new HistoryCommand(), "Show command history");
  // convenience short-name for history
  registry.Register(new AliasCommand("h", "history"), "Alias of history");
  registry.Register(new AliasCommand("where", "which"), "Alias of which");
}

//
// 🔥 起動時の画面クリア + タイトル表示
//
Console.Clear();
Console.WriteLine($"cssh {cssh.Core.Constants.CsshConstants.Version}");
Console.WriteLine();

//
// 🔁 メイン REPL
//
while (true)
{
  Console.Write($"cssh: {state.CurrentDirectory}> ");
  var input = Console.ReadLine();
  if (string.IsNullOrWhiteSpace(input))
  continue;

  // exit / quit は特別扱い
  var trimmed = input.Trim();
  if (trimmed == "exit" || trimmed == "quit")
  break;

  var output = runner.Run(state, input);
  if (!string.IsNullOrEmpty(output))
  {
    Console.WriteLine(output);
  }
}
