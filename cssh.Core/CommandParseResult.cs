/* cssh.Core.CommandParseResult.cs - A cross-platform C# shell ver.0.2.0 CommandParseResult record */
namespace cssh.Core;

/// <param name="Command"></param>
/// <param name="Arguments"></param>
/// <returns></returns>
public record CommandParseResult(string Command, string[] Arguments);
