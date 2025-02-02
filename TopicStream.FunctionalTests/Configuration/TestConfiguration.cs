using System;

namespace TopicStream.FunctionalTests.Configuration;

public static class TestConfiguration
{
  private static string GetRequiredEnvironmentVariable(string variableName)
  {
    var configValue = Environment.GetEnvironmentVariable(variableName);
    return configValue is null ? throw new InvalidOperationException($"{variableName} environment variable must be set to run these tests") : configValue;
  }

  public static string GetUrl()
  {
    return GetRequiredEnvironmentVariable("TOPIC_STREAM_URL");
  }

  public static string GetSubscriber1ApiKey()
  {
    return GetRequiredEnvironmentVariable("TOPIC_STREAM_SUBSCRIBER_1_API_KEY");
  }

  public static string GetSubscriber2ApiKey()
  {
    return GetRequiredEnvironmentVariable("TOPIC_STREAM_SUBSCRIBER_2_API_KEY");
  }

  public static string GetPublisherApiKey()
  {
    return GetRequiredEnvironmentVariable("TOPIC_STREAM_PUBLISHER_API_KEY");
  }
}