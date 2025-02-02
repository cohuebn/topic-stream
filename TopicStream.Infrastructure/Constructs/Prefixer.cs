namespace TopicStream.Infrastructure.Constructs;

internal static class ResourcePrefixer
{

  /// <summary>
  /// Prefix a resource by joining the prefix and resource name with a hyphen.
  /// If the prefix is null or whitespace, the resource name is returned as-is.
  /// </summary>
  /// <param name="prefix">The prefix to include in the resource name</param>
  /// <param name="resourceName">The base resource name</param>
  /// <returns>The prefixed resource name</returns>
  public static string Prefix(string? prefix, string resourceName)
  {
    return string.IsNullOrWhiteSpace(prefix) ? resourceName : $"{prefix}-{resourceName}";
  }
}