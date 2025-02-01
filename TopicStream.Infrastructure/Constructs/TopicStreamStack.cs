using Amazon.CDK;
using Amazon.CDK.AWS.Lambda;
using Constructs;

namespace TopicStream.Infrastructure.Constructs;

internal interface ITopicStreamStackProps : IStackProps
{
  public AssetCode BundledCode { get; }
}

internal class TopicStreamStackProps : StackProps, ITopicStreamStackProps
{
  public required AssetCode BundledCode { get; init; }
}

/// <summary>
/// The entire system stack; this is all AWS resources used to build
/// and integrate system components (API Gateways, Lambdas, Dynamo tables, etc.)
/// </summary>
internal class TopicStreamStack : Stack
{

  public TopicStreamStack(Construct scope, string id, ITopicStreamStackProps props) : base(scope, id, props)
  {
    _ = new Function(this, "Connect", new TopicStreamFunctionProps
    {
      Handler = "TopicStream.Functions::TopicStream.Functions.ConnectionManager::Connect",
      Code = props.BundledCode,
      FunctionName = $"{id}-Connect",
    });

    _ = new Function(this, "Disconnect", new TopicStreamFunctionProps
    {
      Handler = "TopicStream.Functions::TopicStream.Functions.ConnectionManager::Connect",
      Code = props.BundledCode,
      FunctionName = $"{id}-Disconnect",
    });
  }
}