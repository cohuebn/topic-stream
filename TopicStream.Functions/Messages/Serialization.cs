using System.Text.Json;
using System.Text.Json.Serialization;

namespace TopicStream.Functions.Messages;

public static class MessageSerializerOptions
{
  /// <summary>
  /// Standard serialization options for messages used throughout the application
  /// </summary>
  public static JsonSerializerOptions Standard { get; } = new()
  {
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    PropertyNameCaseInsensitive = true,
    Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
  };
}