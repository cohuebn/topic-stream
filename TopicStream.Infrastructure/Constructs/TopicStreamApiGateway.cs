using Amazon.CDK.AWS.Apigatewayv2;
using Amazon.CDK.AWS.Lambda;
using Amazon.CDK.AwsApigatewayv2Integrations;
using Constructs;

namespace TopicStream.Infrastructure.Constructs;

internal interface ITopicStreamApiGatewayProps
{
  string ApiName { get; }
  Function ConnectFunction { get; }
  Function DisconnectFunction { get; }
}

internal class TopicStreamApiGatewayProps : ITopicStreamApiGatewayProps
{
  public required string ApiName { get; init; }
  public required Function ConnectFunction { get; init; }
  public required Function DisconnectFunction { get; init; }
}

/// <summary>
/// The entire system stack; this is all AWS resources used to build
/// and integrate system components (API Gateways, Lambdas, Dynamo tables, etc.)
/// </summary>
internal class TopicStreamApiGateway : Construct
{

  public TopicStreamApiGateway(Construct scope, string id, ITopicStreamApiGatewayProps props) : base(scope, id)
  {
    var api = new WebSocketApi(this, $"{id}-Api", new WebSocketApiProps()
    {
      ApiName = props.ApiName,
      ConnectRouteOptions = new WebSocketRouteOptions
      {
        Integration = new WebSocketLambdaIntegration($"{id}-ConnectIntegration", props.ConnectFunction),
        Authorizer = null
      },
      DisconnectRouteOptions = new WebSocketRouteOptions
      {
        Integration = new WebSocketLambdaIntegration($"{id}-DisconnectIntegration", props.DisconnectFunction),
      },
    });

    // For this assessment, we only need a single stage; in a real production system, we might have multiple stages
    // based on requirements.
    _ = new WebSocketStage(this, $"{id}-DefaultStage", new WebSocketStageProps
    {
      StageName = "$default",
      Description = "The live, publicly accessible stage of the API",
      AutoDeploy = true,
      WebSocketApi = api,
    });
  }
}