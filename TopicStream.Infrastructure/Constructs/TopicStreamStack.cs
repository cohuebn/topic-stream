using Amazon.CDK;
using Amazon.CDK.AWS.IAM;
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
    var authorizerFunction = new AuthorizerFunction(this, "Authorizer", new AuthorizerFunctionProps
    {
      Stack = this,
      FunctionName = $"{id}-Authorizer",
      Handler = "TopicStream.Functions::TopicStream.Functions.ApiKeyAuthorizer::Authorize",
      Code = props.BundledCode,
    });

    var connectFunction = new Function(this, "Connect", new TopicStreamFunctionProps
    {
      FunctionName = $"{id}-Connect",
      Handler = "TopicStream.Functions::TopicStream.Functions.Connection::Connect",
      Code = props.BundledCode,
    });

    var disconnectFunction = new Function(this, "Disconnect", new TopicStreamFunctionProps
    {
      FunctionName = $"{id}-Disconnect",
      Handler = "TopicStream.Functions::TopicStream.Functions.Connection::Connect",
      Code = props.BundledCode,
    });

    _ = new TopicStreamApiGateway(this, "Api", new TopicStreamApiGatewayProps
    {
      ApiName = $"{id}-Api",
      AuthorizeFunction = authorizerFunction,
      ConnectFunction = connectFunction,
      DisconnectFunction = disconnectFunction,
    });
  }
}