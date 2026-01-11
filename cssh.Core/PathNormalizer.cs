/* cssh.Core.PathNormalizer.cs Added on v0.1.3 */
namespace cssh.Core;

/// <summary>
/// Provides utilities for normalizing paths so that both '/' and '\'
/// can be used as directory separators.
/// </summary>
public static class PathNormalizer
{
  /// <summary>
  /// Normalizes a path by converting '/' to '\' on Windows.
  /// </summary>
  public static string Normalize(string path)
  {
    if (string.IsNullOrWhiteSpace(path))
      return path;

    return path.Replace('/', '\\');
  }
}