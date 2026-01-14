/* cssh.Core.ICommand.cs - A cross-platform C# shell ver.0.2.0 ICommand interface */
namespace cssh.Core;

/// <summary>
/// Represents the ICommand interface.
/// </summary>
public interface ICommand
{
  string Name { get; }

  string Execute(ShellState state, string[] args);
}
