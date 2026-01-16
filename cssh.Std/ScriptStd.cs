/* Cssh/cssh.Std/ScriptStd.cs ver.1.0 */
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

public static class ScriptStd
{
  // ----------------------------------------
  // コマンドライン引数（Program.cs から設定）
  // ----------------------------------------
  private static string[] _args = Array.Empty<string>();

  public static void SetArgs(string[] args)
  {
    _args = args ?? Array.Empty<string>();
  }

  public static int argc()
  {
    return _args.Length;
  }

  public static string? args(int index)
  {
    if (index < 0 || index >= _args.Length)
      return null;
    return _args[index];
  }

  // ----------------------------------------
  // 出力系
  // ----------------------------------------
  public static void print(object value)
  {
    Console.Write(value?.ToString());
  }

  public static void println(object value)
  {
    Console.WriteLine(value?.ToString());
  }

  public static void printf(string format, params object[] args)
  {
    // C スタイル %d %s %f を .NET 形式に変換
    string f = format
      .Replace("%d", "{0}")
      .Replace("%s", "{1}")
      .Replace("%f", "{2}");

    // args の順番はそのまま
    Console.Write(string.Format(f, args));
  }

  public static void debug(object value)
  {
    Console.Error.WriteLine(value?.ToString());
  }

  // ----------------------------------------
  // 文字列生成
  // ----------------------------------------
  public static string format(string format, params object[] args)
  {
    return string.Format(format, args);
  }

  // ----------------------------------------
  // 入力系
  // ----------------------------------------
  public static string? input(string prompt)
  {
    Console.Write(prompt);
    return Console.ReadLine();
  }

  public static string? gets()
  {
    return Console.ReadLine();
  }

  // ----------------------------------------
  // 日付・時間
  // ----------------------------------------
  public static string today()
  {
    return DateTime.Now.ToString("yyyy-MM-dd");
  }

  public static string now()
  {
    return DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
  }

  public static string datetime(DateTime? dt = null, string format = "yyyy-MM-ddTHH:mm:ss")
  {
    return (dt ?? DateTime.Now).ToString(format);
  }

  // ----------------------------------------
  // ファイル操作
  // ----------------------------------------
  public static string read(string path)
  {
    return File.ReadAllText(path);
  }

  public static void write(string path, string content)
  {
    File.WriteAllText(path, content);
  }

  public static void append(string path, string content)
  {
    File.AppendAllText(path, content);
  }

  public static bool exists(string path)
  {
    return File.Exists(path);
  }

  // ----------------------------------------
  // プロセス実行
  // ----------------------------------------
  public static string system(string command)
  {
    var psi = new ProcessStartInfo("sh", $"-c \"{command}\"")
    {
      RedirectStandardOutput = true,
      RedirectStandardError = false,
      UseShellExecute = false,
      CreateNoWindow = true
    };

    using var p = Process.Start(psi);
    return p!.StandardOutput.ReadToEnd();
  }

  public static void run(string command)
  {
    var psi = new ProcessStartInfo("sh", $"-c \"{command}\"")
    {
      RedirectStandardOutput = false,
      RedirectStandardError = false,
      UseShellExecute = false,
      CreateNoWindow = true
    };

    using var p = Process.Start(psi);
    p!.WaitForExit();
  }

  // ----------------------------------------
  // 制御系
  // ----------------------------------------
  public static void exit(int code = 0)
  {
    Environment.Exit(code);
  }

  public static void abort(string message = "")
  {
    if (!string.IsNullOrEmpty(message))
      Console.Error.WriteLine(message);

    Environment.Exit(1);
  }

  // ----------------------------------------
  // 正規表現
  // ----------------------------------------
  public static bool match(string text, string pattern)
  {
    return Regex.IsMatch(text, pattern);
  }

  public static string? search(string text, string pattern)
  {
    var m = Regex.Match(text, pattern);
    return m.Success ? m.Value : null;
  }

  public static string replace(string text, string pattern, string replacement)
  {
    return Regex.Replace(text, pattern, replacement);
  }
}