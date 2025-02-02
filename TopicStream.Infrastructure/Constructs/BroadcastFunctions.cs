using System.Collections.Generic;
using Amazon.CDK.AWS.Lambda;
using Amazon.CDK.AWS.Lambda.EventSources;
using Amazon.CDK.AWS.SQS;
using Constructs;

namespace TopicStream.Infrastructure.Constructs;

internal interface IBroadcastFunctionsProps : ILambdaInitializerProps
{
  public Queue TopicMessageQueue { get; }
  public string BroadcastCallbackUrl { get; }
}

internal class BroadcastFunctionsProps : IBroadcastFunctionsProps
{
  public string? ResourcePrefix { get; init; }
  public required AssetCode BundledCode { get; init; }
  public required Queue TopicMessageQueue { get; init; }
  public required string BroadcastCallbackUrl { get; init; }
}

/// <summary>
/// The Lambda functions that handle WebSocket connections and disconnections.
/// </summary>
internal class BroadcastFunctions : Construct
{
  public BroadcastFunctions(Construct scope, string id, IBroadcastFunctionsProps props) : base(scope, id)
  {
    var broadcastFunction = new Function(this, "Broadcast", new TopicStreamFunctionProps
    {
      FunctionName = ResourcePrefixer.Prefix(props.ResourcePrefix, "Broadcast"),
      Handler = "TopicStream.Functions::TopicStream.Functions.TopicMessages.BroadcastHandlers::Broadcast",
      Code = props.BundledCode,
      Environment = new Dictionary<string, string>
      {
        ["WEBSOCKET_CALLBACK_URL"] = props.BroadcastCallbackUrl,
      },
    });
    props.TopicMessageQueue.GrantConsumeMessages(broadcastFunction);
    broadcastFunction.AddEventSource(new SqsEventSource(props.TopicMessageQueue, new SqsEventSourceProps
    {
      // In real-life this would be tuned, but just start with 10 messages at a time
      BatchSize = 10,
    }));
  }
}