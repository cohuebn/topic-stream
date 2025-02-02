namespace TopicStream.Functions.Configuration;

/// <summary>
/// Common configuration for components that broadcast messages
/// </summary>
public static class BroadcastConfiguration
{
  public static string WebSocketCallbackUrl { get; } = EnvironmentVariables.GetRequired("WEBSOCKET_CALLBACK_URL");
}