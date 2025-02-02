using System.Collections.Generic;
using Amazon.CDK.AWS.Lambda;
using Amazon.CDK.AWS.SQS;
using Constructs;

namespace TopicStream.Infrastructure.Constructs;

internal interface ITopicMessageFunctionsProps : ILambdaInitializerProps
{
  public Queue TopicMessageQueue { get; }
}

internal class TopicMessageFunctionsProps : ITopicMessageFunctionsProps
{
  public string? ResourcePrefix { get; init; }
  public required AssetCode BundledCode { get; init; }
  public required Queue TopicMessageQueue { get; init; }
}

/// <summary>
/// The Lambda functions that handle WebSocket connections and disconnections.
/// </summary>
internal class TopicMessageFunctions : Construct
{
  public Function PublishFunction { get; }

  public TopicMessageFunctions(Construct scope, string id, ITopicMessageFunctionsProps props) : base(scope, id)
  {
    PublishFunction = new Function(this, "Publish", new TopicStreamFunctionProps
    {
      FunctionName = ResourcePrefixer.Prefix(props.ResourcePrefix, "Publish"),
      Handler = "TopicStream.Functions::TopicStream.Functions.TopicMessages.TopicMessageHandlers::Publish",
      Code = props.BundledCode,
      Environment = new Dictionary<string, string>
      {
        ["TOPIC_MESSAGES_QUEUE_URL"] = props.TopicMessageQueue.QueueUrl,
      },
    });
    props.TopicMessageQueue.GrantSendMessages(PublishFunction);
  }
}