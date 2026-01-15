/* cssh.Core.Constants.CsshConstants - A cross-platform C# shell ver.0.2.2 Constants */
namespace cssh.Core.Constants;

/// <summary>
/// Provides constant values used throughout the cssh shell.
/// </summary>
public static class CsshConstants
{
  /// <summary>
  /// Shell version string.
  /// </summary>
  public const string Version = "v0.2.2";
  /// <summary>
  /// The Revision field.
  /// </summary>
  public const int Revision = 1;


  /// <summary>
  /// Default directory trim length for prompt display.
  /// </summary>
  public const int DefaultPromptDirTrim = 2;

  /// <summary>
  /// Interactive mode identifier.
  /// </summary>
  public const string ModeInteractive = "interactive";

  /// <summary>
  /// Edit mode identifier.
  /// </summary>
  public const string ModeEdit = "edit";

  /// <summary>
  /// Prefix used for shell title display.
  /// </summary>
  public const string TitlePrefix = "cssh ";

  //
  // v0.2.0 追加: コマンド名の定数（将来の help 実装で利用予定）
  //

  /// <summary>Command name for <c>ls</c>.</summary>
  public const string CmdLs = "ls";

  /// <summary>Command name for <c>cd</c>.</summary>
  public const string CmdCd = "cd";

  /// <summary>Command name for <c>pwd</c>.</summary>
  public const string CmdPwd = "pwd";
}
