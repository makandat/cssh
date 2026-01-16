/* cssh.Core.ShellState.cs v0.2.0 */
using cssh.Core.Commands;
namespace cssh.Core;

/// <summary>
/// Represents the ShellState class.
/// </summary>
public class ShellState
{
  public string LastSearchPattern { get; set; } = string.Empty;
  public string SearchMessage { get; set; } = string.Empty;
  public bool IsInSearchMode { get; set; } = false;
  public int LastSearchIndex { get; set; } = -1;
  
// --- 編集モード用バッファ ---
  
  // メインの編集内容 (Main Buffer)
  public List<string> MainBuffer { get; set; } = new();

  // undo用、または外部エディタからの戻し用 (Backup Buffer)
  public List<string> BackupBuffer { get; set; } = new();

  // 現在編集中のファイルパス（rコマンドやwコマンドで使用）
  public string? CurrentEditingFile { get; set; }

// --- 状態管理メソッド ---

  /// <summary>
  /// MainBuffer と BackupBuffer の内容を交換します (undo/np用)
  /// </summary>
  public void SwapBuffers()
  {
    var temp = MainBuffer;
    MainBuffer = BackupBuffer;
    BackupBuffer = temp;
  }

  /// <summary>
  /// 編集バッファが変更されているかどうかを判定（保存確認用）
  /// ※ 簡易的には、ファイル読み込み時の内容を保持しておき比較する
  /// </summary>
  public bool IsDirty { get; set; } = false;

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
