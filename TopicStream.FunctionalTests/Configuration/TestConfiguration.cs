using System;

namespace TopicStream.FunctionalTests.Configuration;

public static class TestConfiguration
{
  public static string GetRequired(string variableName)
  {
    var configValue = Environment.GetEnvironmentVariable(variableName);
    return configValue is null ? throw new InvalidOperationException($"{variableName} environment variable must be set to run these tests") : configValue;
  }

  public static Uri GetUri()
  {
    var configValue = GetRequired("TOPIC_STREAM_URL");
    return new Uri(configValue);
  }

  public static string GetSubscriber1ApiKey()
  {
    return GetRequired("TOPIC_STREAM_SUBSCRIBER_1_API_KEY");
  }

  public static string GetSubscriber2ApiKey()
  {
    return GetRequired("TOPIC_STREAM_SUBSCRIBER_2_API_KEY");
  }

  public static string GetPublisherApiKey()
  {
    return GetRequired("TOPIC_STREAM_PUBLISHER_API_KEY");
  }
}