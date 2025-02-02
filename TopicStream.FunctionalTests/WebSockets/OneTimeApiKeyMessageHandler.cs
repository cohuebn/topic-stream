using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace TopicStream.FunctionalTests.WebSockets;

/// <summary>
/// A custom HTTP message handler that adds an API key to the first request.
/// This is useful to validate that once authorized, the user doesn't need
/// to keep sending their API key on subsequent requests.
/// </summary>
public class OneTimeApiKeyMessageHandler(string apiKey) : HttpMessageHandler
{
  private readonly string _apiKey = apiKey;
  private readonly HttpMessageInvoker _standardHttpInvoker = new HttpMessageInvoker(new HttpClientHandler());
  // Track if the key was sent in order to only send it once
  private bool _hasConnected = false;

  protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
  {
    // Only add the API key for the first request (WebSocket connection)
    if (!_hasConnected)
    {
      request.Headers.Add("x-api-key", _apiKey);
      _hasConnected = true;
    }

    // Send the request
    return _standardHttpInvoker.SendAsync(request, cancellationToken);
  }

  /// <summary>
  /// Ensure the HTTP invoker is disposed when this handler is disposed
  /// </summary>
  /// <param name="disposing"></param>
  protected override void Dispose(bool disposing)
  {
    if (disposing)
    {
      _standardHttpInvoker.Dispose();
    }
    base.Dispose(disposing);
  }
}