using System;
using System.Diagnostics;
using System.IO;

using csp.Csplus;

namespace csp.Dotnet;

public static class Program
{
  public static int Main(string[] args)
  {
    if (args.Length == 1)
    {
      var cspPath = ResolveCspPath(args[0]);
      if (cspPath is null)
      {
        Console.Error.WriteLine($"File not found: {args[0]}");
        return 1;
      }

      var source = File.ReadAllText(cspPath);
      var csx = CSPlus.Translate(source);

      var temp = Path.GetTempFileName() + ".csx";
      File.WriteAllText(temp, csx);

      var p = Process.Start("dotnet-script", temp);
      p?.WaitForExit();
      return p?.ExitCode ?? 1;
    }

    if (args.Length == 3 && args[0] == "-s")
    {
      var savefile = args[1];
      var cspPath = ResolveCspPath(args[2]);

      if (cspPath is null)
      {
        Console.Error.WriteLine($"File not found: {args[2]}");
        return 1;
      }

      var source = File.ReadAllText(cspPath);
      var csx = CSPlus.Translate(source);

      File.WriteAllText(savefile, csx);
      return 0;
    }

    Console.WriteLine("Usage:");
    Console.WriteLine("  dotnet-csp file.csp");
    Console.WriteLine("  dotnet-csp -s savefile file.csp");
    return 1;
  }

  // ---------------------------------------------------------
  // CSPPATH 対応
  // ---------------------------------------------------------
  private static string? ResolveCspPath(string filename)
  {
    // 1. カレントディレクトリ
    if (File.Exists(filename))
        return Path.GetFullPath(filename);

    // 2. CSPPATH（単一ディレクトリ）
    var dir = Environment.GetEnvironmentVariable("CSPPATH");
    if (!string.IsNullOrWhiteSpace(dir))
    {
        var path = Path.Combine(dir, filename);
        if (File.Exists(path))
        return Path.GetFullPath(path);
    }

    return null;
  }
}