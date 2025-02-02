using System;
using System.Linq;
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
/// The WebSocket API Gateway wired up to supporting Lambda functions.
/// This includes a deployed stage for public access as long as the
/// user provides a valid API key.
/// </summary>
internal class TopicStreamApiGateway : Construct
{
  /// <summary>
  /// The CDK WebSocketApi construct appears to have a bug where it doesn't set the "API key required" property to true
  /// when ApiKeySelectionExpression is set. This is a workaround to ensure API key is required on the $connect route.
  /// </summary>
  private static void EnsureApiKeyRequiredOnConnectRoute(WebSocketApi api)
  {
    var connectRoute = api.Node.Children.OfType<WebSocketRoute>().FirstOrDefault(r => r.RouteKey == "$connect");
    if (connectRoute is null || connectRoute.Node.DefaultChild is null)
    {
      throw new InvalidOperationException("Could not find the $connect route in the WebSocket API");
    }
    var cfnConnectRoute = (CfnRoute)connectRoute.Node.DefaultChild;
    cfnConnectRoute.ApiKeyRequired = true;
  }


  public TopicStreamApiGateway(Construct scope, string id, ITopicStreamApiGatewayProps props) : base(scope, id)
  {
    // The CDK WebSocketApi appears to have a bug where it doesn't set the "API key required" property to true
    // when ApiKeySelectionExpression is set. This is a workaround to ensure API key is required on the $connect route.
    var api = new WebSocketApi(this, $"{id}-Api", new WebSocketApiProps
    {
      ApiName = props.ApiName,
      ApiKeySelectionExpression = new WebSocketApiKeySelectionExpression("$request.header.x-api-key"),
      RouteSelectionExpression = "$request.body.action",
      ConnectRouteOptions = new WebSocketRouteOptions
      {
        Integration = new WebSocketLambdaIntegration($"{id}-ConnectIntegration", props.ConnectFunction),
      },
      DisconnectRouteOptions = new WebSocketRouteOptions
      {
        Integration = new WebSocketLambdaIntegration($"{id}-DisconnectIntegration", props.DisconnectFunction),
      },
    });
    EnsureApiKeyRequiredOnConnectRoute(api);

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