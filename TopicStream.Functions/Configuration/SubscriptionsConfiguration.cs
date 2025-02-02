namespace TopicStream.Functions.Configuration;

/// <summary>
/// Common configuration for updating subscriptions
/// </summary>
public static class SubscriptionsConfiguration
{
  public static string SubscriptionsTableName { get; } = EnvironmentVariables.GetRequired("SUBSCRIPTIONS_TABLE_NAME");
}