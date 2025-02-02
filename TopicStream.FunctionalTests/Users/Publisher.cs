using System.Threading;
using System.Threading.Tasks;
using TopicStream.FunctionalTests.ApiKeys;
using TopicStream.Functions.Messages;

namespace TopicStream.FunctionalTests.Users;

/// <summary>
/// A publisher is an object that emulates a user connected to the websocket
/// that publishes new messages
/// </summary>
public class Publisher(TestApiKey apiKey, CancellationToken cancellationToken) : User(apiKey, cancellationToken)
{
  public async Task Publish(string topic, string message)
  {
    var publishMessage = new PublishMessage(topic, message);
    await SendAsync(publishMessage);
  }
}