/* cssh.Core.ShellState.cs v0.1.3 */
using cssh.Core.Commands;
namespace cssh.Core;

public class ShellState
{
  private string _currentDirectory = Directory.GetCurrentDirectory();
  private string _previousDirectory = Directory.GetCurrentDirectory();

  public string CurrentDirectory
  {
    get => _currentDirectory;
    set => _currentDirectory = PathNormalizer.Normalize(value);
  }

  public string PreviousDirectory
  {
    get => _previousDirectory;
    set => _previousDirectory = PathNormalizer.Normalize(value);
  }

  public ShellMode Mode { get; set; } = ShellMode.Normal;

  public CommandRegistry Registry { get; }

  public ShellState(CommandRegistry registry)
  {
    Registry = registry;
    CurrentDirectory = Directory.GetCurrentDirectory();
  }
}