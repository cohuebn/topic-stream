using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using TopicStream.FunctionalTests.ApiKeys;
using TopicStream.FunctionalTests.Configuration;
using TopicStream.FunctionalTests.WebSockets;
using TopicStream.Functions.Messages;

namespace TopicStream.FunctionalTests.Users;

/// <summary>
/// A user is a base class emulating a user of the application.
/// It includes methods for connecting and disconnecting from the websocket
/// API
/// </summary>
public abstract class User(TestApiKey apiKey, CancellationToken cancellationToken) : IAsyncDisposable, IDisposable
{
  protected readonly TestApiKey _apiKey = apiKey;
  protected readonly ClientWebSocket _wsClient = new();

  /// <summary>
  /// Connect to the websocket API
  /// </summary>
  public async Task ConnectAsync()
  {
    await _wsClient.ConnectAsync(
        TestConfiguration.GetUri(),
        OneTimeApiKeyMessageInvokerFactory.Create(_apiKey.ApiKey),
        cancellationToken
    );
  }

  /// <summary>
  /// Send a message to the API
  /// </summary>
  public async Task SendAsync<T>(T message) where T : Message
  {
    await _wsClient.SendAsync(
        MessagePreparer.GetBytes(message),
        WebSocketMessageType.Text,
        true,
        cancellationToken
    );
  }

  /// <summary>
  /// Disconnect from the websocket API
  /// </summary>
  public async Task DisconnectAsync()
  {
    await _wsClient.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client closed", cancellationToken);
  }

  /// <summary>
  /// Upon disposal, ensure any topics are unsubscribed from and the socket is closed
  /// </summary>
  /// <returns></returns>
  public virtual async ValueTask DisposeAsync()
  {
    if (_wsClient.State == WebSocketState.Open)
    {
      await DisconnectAsync();
    }
    GC.SuppressFinalize(this);
  }

  public virtual void Dispose()
  {
    DisposeAsync().AsTask().Wait();
  }
}