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
bool wasInEditMode = false;
while (true)
{
  // 編集モードに入ったら画面をクリア
  if (state.Mode == ShellMode.Edit && !wasInEditMode)
  {
    Console.Clear();
    wasInEditMode = true;
  }
  else if (state.Mode == ShellMode.Normal)
  {
    wasInEditMode = false;
  }

  // プロンプトを表示
  string input;
  if (state.Mode == ShellMode.Edit)
  {
    // 編集モードでは画面最下行にプロンプトを表示
    try
    {
      Console.SetCursorPosition(0, Console.WindowHeight - 1);
      Console.Write("> ");
    }
    catch
    {
      // テスト環境などでカーソル位置の設定ができない場合は通常通り表示
      Console.Write("> ");
    }

    // 編集モードでは、ESCキーを検知するためにReadKeyを使用
    var keyInfo = Console.ReadKey(true);
    if (keyInfo.Key == ConsoleKey.Escape)
    {
      // ESCキーが押されたら、テキスト編集モードに入る
      // 現在はコマンド入力モードなので、ESCキーを押すとテキスト編集モードに入る
      // ここでは一旦ESCキーを無視して、次の入力に進む（将来の実装用）
      continue;
    }
    
    // ESCキー以外の場合は、通常のReadLineを使用
    // ただし、既に1文字読み込んでいるので、それを含めて読み込む
    input = keyInfo.KeyChar.ToString();
    if (!char.IsControl(keyInfo.KeyChar))
    {
      // 制御文字でない場合は、残りの入力を読み込む
      var remaining = Console.ReadLine();
      if (!string.IsNullOrEmpty(remaining))
      {
        input += remaining;
      }
    }
    else
    {
      // 制御文字の場合は、改行を追加
      input = Console.ReadLine() ?? string.Empty;
    }
  }
  else
  {
    Console.Write($"cssh: {state.CurrentDirectory}> ");
    input = Console.ReadLine() ?? string.Empty;
  }

  if (string.IsNullOrWhiteSpace(input))
  continue;

  // exit / quit は特別扱い（通常モードのみ）
  if (state.Mode == ShellMode.Normal)
  {
    var trimmed = input.Trim();
    if (trimmed == "exit" || trimmed == "quit")
    break;
  }

  var output = runner.Run(state, input);
  if (!string.IsNullOrEmpty(output))
  {
    Console.WriteLine(output);
  }
}
