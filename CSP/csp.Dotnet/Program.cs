using System;
using System.Diagnostics;
using System.IO;
using csp.Csplus;

class Program
{
    static int Main(string[] args)
    {
        // dotnet-csp file.csp
        if (args.Length == 1)
        {
            var cspPath = args[0];

            if (!File.Exists(cspPath))
            {
                Console.Error.WriteLine($"File not found: {cspPath}");
                return 1;
            }

            var source = File.ReadAllText(cspPath);
            var csx = CSPlus.Translate(source);

            var temp = Path.GetTempFileName() + ".csx";
            File.WriteAllText(temp, csx);

            var p = Process.Start("dotnet-script", temp);
            p.WaitForExit();
            return p.ExitCode;
        }

        // dotnet-csp -s savefile file.csp
        if (args.Length == 3 && args[0] == "-s")
        {
            var savefile = args[1];
            var cspPath = args[2];

            if (!File.Exists(cspPath))
            {
                Console.Error.WriteLine($"File not found: {cspPath}");
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
}