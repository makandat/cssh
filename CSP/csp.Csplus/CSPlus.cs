using System;
using System.Text;
using System.Text.RegularExpressions;

namespace csp.Csplus
{
    public static class CSPlus
    {
        public static string Translate(string source)
        {
            var sb = new StringBuilder();
            var lines = source.Split('\n');

            foreach (var raw in lines)
            {
                var line = raw.Trim();

                if (line == "" || line.StartsWith("#"))
                    continue;

                // def x = expr
                var defMatch = Regex.Match(line, @"^def\s+([A-Za-z_]\w*)\s*=\s*(.+)$");
                if (defMatch.Success)
                {
                    var name = defMatch.Groups[1].Value.Trim();
                    var expr = TransformExpr(defMatch.Groups[2].Value.Trim());
                    sb.AppendLine($"dynamic {name} = {expr};");
                    continue;
                }

                // println(expr)
                var printlnMatch = Regex.Match(line, @"^println\s*\((.*)\)\s*$");
                if (printlnMatch.Success)
                {
                    var expr = TransformExpr(printlnMatch.Groups[1].Value.Trim());
                    sb.AppendLine($"Console.Out.WriteLine({expr});");
                    continue;
                }

                // expr => $stdout
                var routeMatch = Regex.Match(line, @"^(.*)=>\s*(\$\w+)\s*$");
                if (routeMatch.Success)
                {
                    var expr = TransformExpr(routeMatch.Groups[1].Value.Trim());
                    var target = TransformTarget(routeMatch.Groups[2].Value.Trim());
                    sb.AppendLine($"{target}.Write({expr});");
                    continue;
                }

                // fallback: treat as expression
                sb.AppendLine(TransformExpr(line) + ";");
            }

            return sb.ToString();
        }

        private static string TransformExpr(string expr)
        {
            // special variables
            expr = expr.Replace("$stdout", "Console.Out")
                       .Replace("$stderr", "Console.Error")
                       .Replace("$stdin", "Console.In");

            // math functions
            expr = Regex.Replace(expr, @"\bcos\s*\(", "Math.Cos(");
            expr = Regex.Replace(expr, @"\bPI\b", "Math.PI");

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
}