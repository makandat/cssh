/* cssh.Core.ShellState.cs v0.2.0 */
using cssh.Core.Commands;
namespace cssh.Core;

/// <summary>
/// Represents the ShellState class.
/// </summary>
public class ShellState
{
  /// <summary>
  /// The _currentDirectory field.
  /// </summary>
  private string _currentDirectory = Directory.GetCurrentDirectory();
  /// <summary>
  /// The _previousDirectory field.
  /// </summary>
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

  /// <summary>
  /// Gets or sets the Mode.
  /// </summary>
  public ShellMode Mode { get; set; } = ShellMode.Normal;

  // Command history (session-local)
  /// <summary>
  /// Gets or sets the History.
  /// </summary>
  public List<string> History { get; } = new List<string>();

  // Editing mode buffer and metadata
  /// <summary>
  /// Gets or sets the EditBuffer.
  /// </summary>
  public string EditBuffer { get; set; } = string.Empty;
  /// <summary>
  /// Gets or sets the EditFileName.
  /// </summary>
  public string EditFileName { get; set; } = string.Empty;
  /// <summary>
  /// Gets or sets the EditDirty.
  /// </summary>
  public bool EditDirty { get; set; } = false;

  /// <summary>
  /// Gets or sets the Registry.
  /// </summary>
  public CommandRegistry Registry { get; }

  public ShellState(CommandRegistry registry)
  {
    Registry = registry;
    CurrentDirectory = Directory.GetCurrentDirectory();
  }
}
