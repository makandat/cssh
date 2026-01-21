using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.IO;

namespace csp.Csplus;

public static class CSPlus
{
  public static string Translate(string source)
  {
    var sb = new StringBuilder();

    // ---------------------------------------------------------
    // ScriptStd.dll の場所を CSPPATH から決定
    // ---------------------------------------------------------
    var stdDir = Environment.GetEnvironmentVariable("CSPPATH");
    var stdDll = stdDir is not null
      ? Path.Combine(stdDir, "cssh.Std.dll")
      : "cssh.Std.dll";

    sb.AppendLine($@"#r ""{stdDll}""");
    sb.AppendLine("using static cssh.Std.ScriptStd;");
    sb.AppendLine();

    var lines = source.Split('\n');

    bool inFunction = false;
    string funcName = "";
    string[] funcArgs = Array.Empty<string>();
    var funcBody = new List<string>();

    foreach (var raw in lines)
    {
      var line = raw.Trim();
      if (line == "" || line.StartsWith("#"))
        continue;

      // -------------------------------------------------------
      // ★ C# の構文はそのまま通す
      // -------------------------------------------------------
      if (IsCSharpStatement(line))
      {
        sb.AppendLine(raw);
        continue;
      }

      // -------------------------------------------------------
      // def 関数定義: def name(x, y) {
      // -------------------------------------------------------
      var funcMatch = Regex.Match(line, @"^def\s+([A-Za-z_]\w*)\s*\((.*?)\)\s*\{$");
      if (funcMatch.Success)
      {
        inFunction = true;
        funcName = funcMatch.Groups[1].Value.Trim();
        funcArgs = funcMatch.Groups[2].Value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        funcBody.Clear();
        continue;
      }

      // -------------------------------------------------------
      // 関数ブロック内部
      // -------------------------------------------------------
      if (inFunction)
      {
        if (line == "}")
        {
          EmitFunction(sb, funcName, funcArgs, funcBody);
          inFunction = false;
          continue;
        }

        funcBody.Add(line);
        continue;
      }

      // -------------------------------------------------------
      // def 変数宣言
      // -------------------------------------------------------
      var defMatch = Regex.Match(line, @"^def\s+([A-Za-z_]\w*)\s*=\s*(.+?);?$");
      if (defMatch.Success)
      {
        var name = defMatch.Groups[1].Value.Trim();
        var expr = TransformExpr(defMatch.Groups[2].Value.Trim());
        sb.AppendLine($"dynamic {name} = {expr};");
        continue;
      }

      // -------------------------------------------------------
      // => ルーティング
      // -------------------------------------------------------
      var routeMatch = Regex.Match(line, @"^(.*)=>\s*(\$\w+)\s*$");
      if (routeMatch.Success)
      {
        var expr = TransformExpr(routeMatch.Groups[1].Value.Trim());
        var target = TransformTarget(routeMatch.Groups[2].Value.Trim());
        sb.AppendLine($"{target}.Write({expr});");
        continue;
      }

      // -------------------------------------------------------
      // fallback: 通常の式
      // -------------------------------------------------------
      sb.AppendLine(TransformExpr(line) + ";");
    }

    return sb.ToString();
  }

  // ==========================================================
  // ★ C# 文判定（v0.3 追加）
  // ==========================================================
  private static bool IsCSharpStatement(string line)
  {
    if (line == "" || line.StartsWith("//"))
      return true;

    if (line == "{" || line == "}")
      return true;

    if (Regex.IsMatch(line, @"^(for|foreach|if|else|while|do|switch|case|default)\b"))
      return true;

    if (Regex.IsMatch(line, @"^(try|catch|finally|throw)\b"))
      return true;

    if (Regex.IsMatch(line, @"^(using|lock|checked|unchecked|fixed)\b"))
      return true;

    if (Regex.IsMatch(line, @"^(const|var|int|double|float|long|string|bool|decimal|char|byte|short|uint|ulong|ushort)\b"))
      return true;

    if (Regex.IsMatch(line, @"^(return|break|continue|goto|yield)\b"))
      return true;

    // 関数呼び出し（ScriptStd.dll の関数も含む）
    if (Regex.IsMatch(line, @"^[A-Za-z_]\w*\s*\("))
      return true;

    // C# のラムダ式 (x) => ...
    if (Regex.IsMatch(line, @"^\(.*\)\s*=>"))
      return true;

    // ; で終わる C# の式文
    if (line.EndsWith(";"))
      return true;

    return false;
  }

  // ==========================================================
  // 関数出力（v0.2 のまま）
  // ==========================================================
  private static void EmitFunction(StringBuilder sb, string name, string[] args, List<string> body)
  {
    var argList = new List<string>();
    foreach (var a in args)
    {
      var arg = a.Trim();
      if (arg != "")
        argList.Add($"dynamic {arg}");
    }

    sb.AppendLine($"dynamic {name}({string.Join(", ", argList)})");
    sb.AppendLine("{");

    for (int i = 0; i < body.Count - 1; i++)
    {
      var line = body[i].Trim();

      var defMatch = Regex.Match(line, @"^def\s+([A-Za-z_]\w*)\s*=\s*(.+)$");
      if (defMatch.Success)
      {
        var name2 = defMatch.Groups[1].Value.Trim();
        var expr2 = TransformExpr(defMatch.Groups[2].Value.Trim());
        sb.AppendLine($"  dynamic {name2} = {expr2};");
        continue;
      }

      sb.AppendLine("  " + TransformExpr(line) + ";");
    }

    var last = body[^1].Trim();

    var defLast = Regex.Match(last, @"^def\s+([A-Za-z_]\w*)\s*=\s*(.+)$");
    if (defLast.Success)
    {
      var name3 = defLast.Groups[1].Value.Trim();
      var expr3 = TransformExpr(defLast.Groups[2].Value.Trim());
      sb.AppendLine($"  dynamic {name3} = {expr3};");
    }
    else if (last.StartsWith("return "))
    {
      sb.AppendLine("  " + TransformExpr(last) + ";");
    }
    else
    {
      sb.AppendLine("  return " + TransformExpr(last) + ";");
    }

    sb.AppendLine("}");
  }

  // ==========================================================
  // 式変換（v0.3: Math.* 変換削除）
  // ==========================================================
  private static string TransformExpr(string expr)
  {
    expr = expr.Replace("$stdout", "Console.Out")
               .Replace("$stderr", "Console.Error")
               .Replace("$stdin", "Console.In");

    // ★ Math.* 変換は削除（ScriptStd.dll の関数として扱う）

    // (x, y) => expr
    expr = Regex.Replace(expr, @"^\((.*?)\)\s*=>", m =>
    {
      var args = m.Groups[1].Value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
      for (int i = 0; i < args.Length; i++)
        args[i] = "dynamic " + args[i];
      return "(" + string.Join(", ", args) + ") =>";
    });

    // x => expr
    expr = Regex.Replace(expr, @"^(?!\()([A-Za-z_]\w*)\s*=>", "(dynamic $1) =>");

    return expr;
  }

  private static string TransformTarget(string target)
  {
    return target switch
    {
      "$stdout" => "Console.Out",
      "$stderr" => "Console.Error",
      "$stdin"  => "Console.In",
      _ => target
    };
  }
}