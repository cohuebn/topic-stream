using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using TopicStream.FunctionalTests.ApiKeys;
using TopicStream.Functions.Messages;

namespace TopicStream.FunctionalTests.Users;

/// <summary>
/// A subscriber is an object that emulates a user connected to the websocket
/// that:
/// 1. Subscribes to topics
/// 2. Waits for messages
/// 3. Unsubscribes from topics
/// </summary>
public class Subscriber(TestApiKey apiKey, CancellationToken cancellationToken) : User(apiKey, cancellationToken)
{
  private readonly List<string> _subscribedTopics = [];
  public readonly ReceivedMessages ReceivedMessages = [];

  public async Task SubscribeAsync(string topic)
  {
    var subscribeMessage = new SubscribeMessage(topic);
    await SendAsync(subscribeMessage);
    _subscribedTopics.Add(topic);
  }

  public async Task UnsubscribeAsync(string topic)
  {
    var unsubscribeMessage = new UnsubscribeMessage(topic);
    await SendAsync(unsubscribeMessage);
    _subscribedTopics.Remove(topic);
  }

  public async Task UnsubscribeFromAllTopicsAsync()
  {
    var topicsToUnsubscribeFrom = _subscribedTopics.ToList();
    foreach (var topic in topicsToUnsubscribeFrom)
    {
      await UnsubscribeAsync(topic);
    }
  }

  /// <summary>
  /// Receive messages from the websocket until the expected number of messages is reached
  /// </summary>
  /// <param name="expectedMessageCount">The expected number of messages</param>
  private async Task ReceiveMessagesAsync(int expectedMessageCount)
  {
    // 4 KB buffer is good enough for these tests
    var buffer = new byte[1024 * 4];

    while (_wsClient.State == WebSocketState.Open && ReceivedMessages.TotalMessageCount < expectedMessageCount)
    {
      var result = await _wsClient.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

      if (result.MessageType == WebSocketMessageType.Close)
      {
        await DisconnectAsync();
        break;
      }

      var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
      if (message is not null)
      {
        var parsedMessage = JsonSerializer.Deserialize<PublishMessage>(message);
        if (parsedMessage is not null)
        {
          ReceivedMessages.AddMessage(parsedMessage.Topic, parsedMessage.Message);
        }
      }
    }
  }

  private static async Task TimeoutWaitingForMessagesAsync(TimeSpan timeout)
  {
    await Task.Delay(timeout);
    Assert.Fail("Timeout reached while waiting for messages");
  }

  /// <summary>
  /// Wait for the expected number of messages to be received or until the timeout is reached
  /// </summary>
  /// <param name="expectedMessageCount">How many messages are expected?</param>
  /// <param name="timeout">The timeout to wait for messages</param>
  /// <returns></returns>
  public async Task WaitForMessagesAsync(int expectedMessageCount, TimeSpan? timeout = null)
  {
    var defaultedTimeout = timeout ?? TimeSpan.FromSeconds(30);
    await Task.WhenAny(
      ReceiveMessagesAsync(expectedMessageCount),
      TimeoutWaitingForMessagesAsync(defaultedTimeout)
    );
  }

  /// <summary>
  /// Upon disposal, ensure any topics are unsubscribed from and the socket is closed
  /// </summary>
  /// <returns></returns>
  public override async ValueTask DisposeAsync()
  {
    if (_wsClient.State == WebSocketState.Open)
    {
      await UnsubscribeFromAllTopicsAsync();
    }
    await base.DisposeAsync();
  }
}