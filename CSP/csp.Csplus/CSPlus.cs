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
  // 関数出力
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

    // 本文（最後以外）
    for (int i = 0; i < body.Count - 1; i++)
    {
      var line = body[i].Trim();

      // def 変数宣言
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

    // 最後の行
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
  // 式変換
  // ==========================================================
  private static string TransformExpr(string expr)
  {
    // special variables
    expr = expr.Replace("$stdout", "Console.Out")
              .Replace("$stderr", "Console.Error")
              .Replace("$stdin", "Console.In");

    // math functions
    expr = Regex.Replace(expr, @"\bsqrt\s*\(", "Math.Sqrt(");
    expr = Regex.Replace(expr, @"\bcos\s*\(", "Math.Cos(");
    expr = Regex.Replace(expr, @"\bPI\b", "Math.PI");

    // (x, y) => expr
    expr = Regex.Replace(expr, @"^\((.*?)\)\s*=>", m =>
    {
      var args = m.Groups[1].Value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
      for (int i = 0; i < args.Length; i++)
        args[i] = "dynamic " + args[i];
      return "(" + string.Join(", ", args) + ") =>";
    });

    // x => expr （ただし "(x)" ではない場合のみ）
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