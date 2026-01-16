using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

public static class ScriptStd
{
  // --- 2. 出力系関数 ---
  public static void print(object value) => Console.Write(value);
  public static void println(object value) => Console.WriteLine(value);

  public static void printf(string format, params object[] args)
  {
    if (args == null || args.Length == 0)
    {
      Console.Write(format);
      return;
    }

    // %d, %s, %f を {0}, {1}... に順番に置換する
    string translated = format;
    for (int i = 0; i < args.Length; i++)
    {
      // 最初にマッチした %d, %s, %f のいずれか1つを置換
      var regex = new Regex("%[dsf]");
      translated = regex.Replace(translated, "{" + i + "}", 1);
    }

    Console.Write(string.Format(translated, args));
  }
  
  public static void debug(object value) => Console.Error.WriteLine(value);

  // --- 3. 文字列系関数 ---
  public static string format(string format, params object[] args) => string.Format(format, args);
  public static string merge(string[] arr, string separator = "") => string.Join(separator, arr);
  public static string[] split(string str, string separator = ",") => str.Split(new[] { separator }, StringSplitOptions.None);
  public static int index(string input, string str) => input.IndexOf(str);
  public static string substr(string input, int start, int length) => input.Substring(start, length);
  public static bool startsWith(string input, string str) => input.StartsWith(str);
  public static bool endsWith(string input, string str) => input.EndsWith(str);
  public static string trim(string input) => input.Trim();

  // --- 4. 入力系関数 ---
  public static string? input(string prompt)
  {
    Console.Write(prompt);
    return Console.ReadLine();
  }
  public static string? gets() => Console.ReadLine();

  // --- 5. 日付・時間系関数 ---
  public static string today() => DateTime.Now.ToString("yyyy-MM-dd");
  public static string now() => DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
  public static string datetime(DateTime? dt = null, string format = "yyyy-MM-ddTHH:mm:ss") 
    => (dt ?? DateTime.Now).ToString(format);

  // --- 6. ファイル操作系関数 ---
  public static string read(string path) => File.ReadAllText(path);
  public static void write(string path, string content) => File.WriteAllText(path, content);
  public static void append(string path, string content) => File.AppendAllText(path, content);
  public static bool exists(string path) => File.Exists(path);

  // --- 7. プロセス実行系関数 ---
  public static string system(string command)
  {
    var startInfo = new ProcessStartInfo("cmd", $"/c {command}") { RedirectStandardOutput = true, UseShellExecute = false };
    using var process = Process.Start(startInfo);
    return process?.StandardOutput.ReadToEnd() ?? "";
  }

  public static void run(string command)
  {
    var startInfo = new ProcessStartInfo("cmd", $"/c {command}") { UseShellExecute = false };
    using var process = Process.Start(startInfo);
    process?.WaitForExit();
  }

  // --- 8. 制御系関数 ---
  public static void exit(int code = 0) => Environment.Exit(code);
  public static void abort(string message = "")
  {
    if (!string.IsNullOrEmpty(message)) Console.Error.WriteLine(message);
    Environment.Exit(1);
  }

  // --- 9. 正規表現系関数 ---
  public static bool match(string text, string pattern) => Regex.IsMatch(text, pattern);
  public static string? search(string text, string pattern)
  {
    var m = Regex.Match(text, pattern);
    return m.Success ? m.Value : null;
  }
  public static string replace(string text, string pattern, string replacement) => Regex.Replace(text, pattern, replacement);

  // --- 10. コマンドライン引数系関数 ---
  // スクリプト実行時にこれらをセットする仕組みが cssh.App 側に必要
  private static string[] _args = Array.Empty<string>();
  public static void SetArgs(string[] args) => _args = args;
  public static int argc() => _args.Length;
  public static string? args(int index) => (index >= 0 && index < _args.Length) ? _args[index] : null;
}