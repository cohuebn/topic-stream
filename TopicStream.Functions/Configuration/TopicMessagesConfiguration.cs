namespace TopicStream.Functions.Configuration;

/// <summary>
/// Common configuration for interacting with messages on application topics
/// </summary>
public static class TopicMessagesConfiguration
{
  public static string QueueUrl { get; } = EnvironmentVariables.GetRequired("TOPIC_MESSAGES_QUEUE_URL");
}