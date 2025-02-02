using System.Net.Http;

namespace TopicStream.FunctionalTests.WebSockets;

/// <summary>
/// An provider that can be used to create a message invoker that adds an API key to only the first request.
/// </summary>
public static class OneTimeApiKeyMessageInvokerFactory
{
  /// <summary>
  /// Get a new message invoker that adds the API key to only the first request.
  /// Once connected, the user doesn't need to keep sending their API key on subsequent requests.
  /// </summary>
  /// <param name="apiKey">The API key to include in the first request</param>
  /// <returns></returns>
  public static HttpMessageInvoker Create(string apiKey)
  {
    return new HttpMessageInvoker(new OneTimeApiKeyMessageHandler(apiKey));
  }
}