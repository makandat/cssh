// Rev.4 cssh.Std.dll
namespace cssh.Std;

using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.Text.Json;
using System.Linq;

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

    string translated = format;
    for (int i = 0; i < args.Length; i++)
    {
      var regex = new Regex("%[dsf]");
      translated = regex.Replace(translated, "{" + i + "}", 1);
    }
    Console.Write(string.Format(translated, args));
  }

  public static void debug(object value) => Console.Error.WriteLine(value);
  public static void error(object value) => Console.Error.WriteLine(value);

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
  public static string datetime(string format) => DateTime.Now.ToString(format);

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

  // --- 11. JSON 関数 (Rev.3) ---
  public static object? from_json(string json) => string.IsNullOrWhiteSpace(json) ? null : JsonSerializer.Deserialize<object>(json);
  public static string to_json(object obj) => JsonSerializer.Serialize(obj);

  // --- 12. Python 組み込み関数 (Rev.3) ---
  public static dynamic abs(dynamic x) => Math.Abs(x);
  public static dynamic ascii(dynamic x) => x?.ToString() ?? "None";
  public static dynamic boolean(dynamic x) => x is bool b ? b : (x != null); // bool は予約後のため、boolean に改名
  public static dynamic chr(dynamic x) => (char)x;
  public static dynamic hex(dynamic x) => "0x" + Convert.ToString((long)x, 16);
  public static dynamic len(dynamic x)
  {
    if (x == null) return 0;
    if (x is string s) return s.Length;
    if (x is System.Collections.ICollection c) return c.Count;
    return x.Length;
  }
  public static dynamic max(dynamic a, dynamic b) => Math.Max(a, b);
  public static dynamic min(dynamic a, dynamic b) => Math.Min(a, b);
  public static dynamic oct(dynamic x) => "0o" + Convert.ToString((long)x, 8);
  public static dynamic ord(dynamic x) => (int)(char)x;
  public static dynamic pow(dynamic x, dynamic y) => Math.Pow(x, y);

  public static dynamic range(dynamic start, dynamic stop, dynamic step)
  {
    double s = Convert.ToDouble(start);
    double e = Convert.ToDouble(stop);
    double st = Convert.ToDouble(step);
    var list = new List<double>();
    if (st > 0) { for (double i = s; i < e; i += st) list.Add(i); }
    else if (st < 0) { for (double i = s; i > e; i += st) list.Add(i); }
    return list;
  }
  public static dynamic range(dynamic start, dynamic stop) => range(start, stop, 1.0);
  public static dynamic range(dynamic stop) => range(0.0, stop, 1.0);

  public static dynamic round(dynamic x) => Math.Round((double)x);
  public static IEnumerable<T> sorted<T>(IEnumerable<T> xs)
  {
    return xs.OrderBy(x => x);
  }
  public static dynamic sum(dynamic x)
  {
    double s = 0;
    foreach (var item in (System.Collections.IEnumerable)x) { s += Convert.ToDouble(item); }
    return s;
  }
  public static dynamic type(dynamic x) => x?.GetType() ?? typeof(object); // nullの場合はobject型を返すか、お好みで調整

  // --- 13. Python 数学関数 (Rev.3) ---
  public static readonly double PI = Math.PI;
  public static readonly double E = Math.E;

  public static dynamic ceil(dynamic x) => Math.Ceiling((double)x);
  public static dynamic floor(dynamic x) => Math.Floor((double)x);
  public static dynamic fmod(dynamic x, dynamic y) => Math.IEEERemainder((double)x, (double)y);
  public static dynamic isnan(dynamic x) => double.IsNaN((double)x);
  public static dynamic sqrt(dynamic x) => Math.Sqrt((double)x);
  public static dynamic exp(dynamic x) => Math.Exp((double)x);
  public static dynamic log(dynamic x) => Math.Log((double)x);
  public static dynamic log10(dynamic x) => Math.Log10((double)x);
  public static dynamic degrees(dynamic x) => (double)x * (180.0 / Math.PI);
  public static dynamic radians(dynamic x) => (double)x * (Math.PI / 180.0);
  public static dynamic acos(dynamic x) => Math.Acos((double)x);
  public static dynamic asin(dynamic x) => Math.Asin((double)x);
  public static dynamic atan(dynamic x) => Math.Atan((double)x);
  public static dynamic cos(dynamic x) => Math.Cos((double)x);
  public static dynamic sin(dynamic x) => Math.Sin((double)x);
  public static dynamic tan(dynamic x) => Math.Tan((double)x);

  // --- 14. Assert 関数 (Rev.4)  ---
  public static void assert(bool condition, string message = "assertion failed")
  {
    if (!condition)
      throw new Exception(message);
  }
  public static void assertArray<T>(T[] actual, T[] expected, string message = "array assertion failed")
  {
    if (!actual.SequenceEqual(expected))
      throw new Exception(message + $": {string.Join(",", actual)} != {string.Join(",", expected)}");
  }
  public static void assertList<T>(List<T> actual, List<T> expected, string message = "list assertion failed")
  {
    if (!actual.SequenceEqual(expected))
      throw new Exception(message + $": [{string.Join(",", actual)}] != [{string.Join(",", expected)}]");
  }
  public static void assertObject(
    Dictionary<string, object> actual,
    Dictionary<string, object> expected,
    string message = "object assertion failed")
  {
    if (actual.Count != expected.Count)
      throw new Exception(message + ": key count mismatch");

      foreach (var kv in expected)
      {
        if (!actual.ContainsKey(kv.Key))
          throw new Exception(message + $": missing key '{kv.Key}'");

        var a = actual[kv.Key];
        var e = kv.Value;

        if (!Equals(a, e))
          throw new Exception(message + $": key '{kv.Key}' mismatch: {a} != {e}");
      }
  }
}