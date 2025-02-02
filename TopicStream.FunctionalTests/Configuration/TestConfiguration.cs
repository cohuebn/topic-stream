using System;
using TopicStream.Configuration;

namespace TopicStream.FunctionalTests.Configuration;

public static class TestConfiguration
{
  public static Uri GetUri()
  {
    var configValue = EnvironmentVariables.GetRequired("TOPIC_STREAM_URL");
    return new Uri(configValue);
  }

  public static string GetSubscriber1ApiKey()
  {
    return EnvironmentVariables.GetRequired("TOPIC_STREAM_SUBSCRIBER_1_API_KEY");
  }

  public static string GetSubscriber2ApiKey()
  {
    return EnvironmentVariables.GetRequired("TOPIC_STREAM_SUBSCRIBER_2_API_KEY");
  }

  public static string GetPublisherApiKey()
  {
    return EnvironmentVariables.GetRequired("TOPIC_STREAM_PUBLISHER_API_KEY");
  }
}