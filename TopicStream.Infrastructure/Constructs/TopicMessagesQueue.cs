using Amazon.CDK;
using Amazon.CDK.AWS.SQS;
using Constructs;

namespace TopicStream.Infrastructure.Constructs;

internal interface IMessagesQueueProps
{
  public string? ResourcePrefix { get; }
}

internal class MessagesQueueProps : IMessagesQueueProps
{
  public string? ResourcePrefix { get; init; }
}

/// <summary>
/// The SQS queue that stores messages to be broadcast to subscribing WebSocket clients.
/// </summary>
internal class TopicMessagesQueue : Construct
{
  public Queue Queue { get; init; }

  public TopicMessagesQueue(Construct scope, string id, IMessagesQueueProps props) : base(scope, id)
  {
    // Primary index supports fast lookup by topic
    Queue = new Queue(this, "MessagesQueue", new QueueProps
    {
      QueueName = ResourcePrefixer.Prefix(props.ResourcePrefix, "Messages"),
      RemovalPolicy = RemovalPolicy.DESTROY,
    });
  }
}