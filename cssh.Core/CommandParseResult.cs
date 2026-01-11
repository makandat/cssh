/* cssh.Core.CommandParseResult.cs - A cross-platform C# shell ver.0.1.0 CommandParseResult record */
namespace cssh.Core;

public record CommandParseResult(string Command, string[] Arguments);