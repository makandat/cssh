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

///
/// 🔁 メイン REPL
///
while (true)
{
  // 1. 画面の描画
  RenderScreen(state);

  // 2. ユーザー入力の取得
  string input = await GetInputAsync(state);
  if (string.IsNullOrWhiteSpace(input)) continue;

  // 3. 終了判定 (通常モードのみ)
  if (state.Mode == ShellMode.Normal && (input == "exit" || input == "quit"))
    break;

  // 4. コマンドの実行
  await ExecuteCommandAsync(input, state, runner);
}

/// <summary>
/// 画面描画の集約
/// </summary>
static void RenderScreen(ShellState state)
{
  if (state.Mode == ShellMode.Edit)
  {
    Console.Clear();
    
    // 1. 編集バッファの内容を表示 (行番号付き)
    int lineNum = 1;
    foreach (var line in state.MainBuffer)
    {
      // 検索で見つかった行 (TargetLineIndex) があれば強調しても良いですが、
      // まずはシンプルに全行表示します。
      Console.WriteLine($"{lineNum,3}: {line}");
      lineNum++;
    }
    
    // 2. 最下行にプロンプトまたは検索メッセージを表示
    try
    {
      int lastRow = Console.WindowHeight - 1;
      Console.SetCursorPosition(0, lastRow);

      if (state.IsInSearchMode)
      {
        // 仕様 4.2.8: 検索メッセージを表示
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write(state.SearchMessage);
        Console.ResetColor();
      }
      else
      {
        Console.Write("> ");
      }
    }
    catch 
    { 
      // ウィンドウサイズ変更時などのエラー回避
      Console.Write("\n> "); 
    }
  }
  else
  {
    // 通常モードのプロンプト
    Console.Write($"cssh: {state.CurrentDirectory}> ");
  }
}

/// <summary>
/// 入力ロジックの集約
/// </summary>
static async Task<string> GetInputAsync(ShellState state)
{
  // 1. 通常モード: 標準の ReadLine を使用
  if (state.Mode == ShellMode.Normal)
  {
    return Console.ReadLine() ?? string.Empty;
  }

  // 2. 編集モード: 1文字ずつ入力を判定
  var keyInfo = Console.ReadKey(true);

  // --- ESC キーの処理 ---
  if (keyInfo.Key == ConsoleKey.Escape)
  {
    if (state.IsInSearchMode)
    {
      state.IsInSearchMode = false;
      state.SearchMessage = string.Empty;
    }
    // 空文字を返すことでメインループを回し、RenderScreen を実行させる
    return string.Empty;
  }

  // --- 検索モード中の / キーの処理 ---
  if (keyInfo.KeyChar == '/' && state.IsInSearchMode)
  {
    return "/"; // EditModeHandler 側で「次を検索」として処理する
  }

  // --- 通常の文字入力の開始 ---
  string input = "";
  if (!char.IsControl(keyInfo.KeyChar))
  {
    // 最初の1文字を表示し、残りを ReadLine で受け取る
    Console.Write(keyInfo.KeyChar);
    input = keyInfo.KeyChar + (Console.ReadLine() ?? "");
  }
  else if (keyInfo.Key == ConsoleKey.Enter)
  {
    // Enter 単体の場合
    return "";
  }
  else
  {
    // その他の制御文字（BackSpace等）は一旦 ReadLine に任せる
    input = Console.ReadLine() ?? string.Empty;
  }
  
  return input.Trim();
}

/// <summary>
/// 実行ロジックの分岐
/// </summary>
static async Task ExecuteCommandAsync(string input, ShellState state, CommandRunner runner)
{
  if (state.Mode == ShellMode.Edit)
  {
    // 今後 EditModeHandler クラスを Core に作り、そこで np/q/undo 等を処理
    await EditModeHandler.ExecuteAsync(input, state);
  }
  else
  {
    var output = runner.Run(state, input);
    if (!string.IsNullOrEmpty(output))
    {
      Console.WriteLine(output);
    }
  }
}