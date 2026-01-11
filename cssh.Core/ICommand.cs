/* cssh.Core.ICommand.cs - A cross-platform C# shell ver.0.1.0 ICommand interface */
namespace cssh.Core;

public interface ICommand
{
    string Name { get; }

    string Execute(ShellState state, string[] args);
}