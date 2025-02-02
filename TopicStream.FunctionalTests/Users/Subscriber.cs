using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using TopicStream.FunctionalTests.ApiKeys;
using TopicStream.FunctionalTests.Configuration;
using TopicStream.FunctionalTests.WebSockets;
using TopicStream.Functions.Messages;

namespace TopicStream.FunctionalTests.Users;

/// <summary>
/// A subscriber is an object that emulates a user connected to the websocket
/// that:
/// 1. Subscribes to topics
/// 2. Waits for messages
/// 3. Unsubscribes from topics
/// </summary>
public class Subscriber : IAsyncDisposable, IDisposable
{
  private readonly TestApiKey _apiKey;
  private readonly ClientWebSocket _wsClient = new();
  private readonly List<string> _subscribedTopics = [];

  public Subscriber(TestApiKey apiKey)
  {
    _apiKey = apiKey;
  }

  public async Task ConnectAsync(CancellationToken cancellationToken)
  {
    await _wsClient.ConnectAsync(
        TestConfiguration.GetUri(),
        OneTimeApiKeyMessageInvokerFactory.Create(_apiKey.ApiKey),
        cancellationToken
    );
  }

  public async Task SendAsync<T>(T message, CancellationToken cancellationToken) where T : Message
  {
    await _wsClient.SendAsync(
        MessagePreparer.GetBytes(message),
        WebSocketMessageType.Text,
        true,
        cancellationToken
    );
  }

  public async Task SubscribeAsync(string topic, CancellationToken cancellationToken)
  {
    var subscribeMessage = new SubscribeMessage(topic);
    await SendAsync(subscribeMessage, cancellationToken);
    _subscribedTopics.Add(topic);
  }

  public async Task UnsubscribeAsync(string topic, CancellationToken cancellationToken)
  {
    var unsubscribeMessage = new UnsubscribeMessage(topic);
    await SendAsync(unsubscribeMessage, cancellationToken);
    _subscribedTopics.Remove(topic);
  }

  public async Task UnsubscribeFromAllTopicsAsync(CancellationToken cancellationToken)
  {
    await Task.WhenAll(_subscribedTopics.Select(topic => UnsubscribeAsync(topic, cancellationToken)));
  }

  public async Task DisconnectAsync(CancellationToken cancellationToken)
  {
    await _wsClient.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client closed", cancellationToken);
  }

  /// <summary>
  /// Upon disposal, ensure any topics are unsubscribed from and the socket is closed
  /// </summary>
  /// <returns></returns>
  /// <exception cref="NotImplementedException"></exception>
  public async ValueTask DisposeAsync()
  {
    if (_wsClient.State == WebSocketState.Open)
    {
      await UnsubscribeFromAllTopicsAsync(CancellationToken.None);
      await DisconnectAsync(CancellationToken.None);
    }
    GC.SuppressFinalize(this);
  }

  public void Dispose()
  {
    DisposeAsync().AsTask().Wait();
  }
}