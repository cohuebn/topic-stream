using Amazon.CDK;
using Amazon.CDK.AWS.Lambda;
using Constructs;

namespace TopicStream.Infrastructure;

public class TopicStreamStack : Stack
{

  internal TopicStreamStack(Construct scope, string id, AssetCode bundledCode, IStackProps props = null) : base(scope, id, props)
  {
    _ = new Function(this, "Connect", new FunctionProps
    {
      Runtime = Runtime.DOTNET_8,
      Handler = "TopicStream.Functions::TopicStream.Functions.ConnectionManager::Connect",
      Code = bundledCode,
    });
  }
}