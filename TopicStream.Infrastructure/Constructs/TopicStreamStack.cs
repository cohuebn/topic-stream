using Amazon.CDK;
using Amazon.CDK.AWS.Lambda;
using Constructs;

namespace TopicStream.Infrastructure.Constructs;

internal interface ITopicStreamStackProps : IStackProps
{
  public string? ResourcePrefix { get; }
  public AssetCode BundledCode { get; }
}

internal class TopicStreamStackProps : StackProps, ITopicStreamStackProps
{
  public string? ResourcePrefix { get; init; }
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
    _ = new ConnectionStates(this, "ConnectionStates", new ConnectionStatesProps
    {
      ResourcePrefix = props.ResourcePrefix,
    });

    var authorizerFunction = new AuthorizerFunction(this, "Authorizer", new AuthorizerFunctionProps
    {
      Stack = this,
      FunctionName = ResourcePrefixer.Prefix(props.ResourcePrefix, "Authorizer"),
      Handler = "TopicStream.Functions::TopicStream.Functions.Connections.ApiKeyAuthorizer::Authorize",
      Code = props.BundledCode,
    });

    var connectionFunctions = new ConnectionFunctions(this, "ConnectionFunctions", new ConnectionFunctionsProps
    {
      ResourcePrefix = props.ResourcePrefix,
      BundledCode = props.BundledCode,
    });

    _ = new TopicStreamApiGateway(this, "Api", new TopicStreamApiGatewayProps
    {
      ApiName = $"{id}-Api",
      AuthorizeFunction = authorizerFunction,
      ConnectFunction = connectionFunctions.ConnectFunction,
      DisconnectFunction = connectionFunctions.DisconnectFunction,
    });
  }
}