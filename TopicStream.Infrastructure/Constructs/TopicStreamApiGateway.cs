using Amazon.CDK.AWS.Apigatewayv2;
using Amazon.CDK.AWS.Lambda;
using Amazon.CDK.AwsApigatewayv2Authorizers;
using Amazon.CDK.AwsApigatewayv2Integrations;
using Constructs;

namespace TopicStream.Infrastructure.Constructs;

internal interface ITopicStreamApiGatewayProps
{
  string ApiName { get; }
  Function AuthorizeFunction { get; }
  Function ConnectFunction { get; }
  Function DisconnectFunction { get; }
}

internal class TopicStreamApiGatewayProps : ITopicStreamApiGatewayProps
{
  public required string ApiName { get; init; }
  public required Function AuthorizeFunction { get; init; }
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
  public TopicStreamApiGateway(Construct scope, string id, ITopicStreamApiGatewayProps props) : base(scope, id)
  {
    var api = new WebSocketApi(this, "Api", new WebSocketApiProps
    {
      ApiName = props.ApiName,
      RouteSelectionExpression = "$request.body.action",
    });

    var authorizer = new WebSocketLambdaAuthorizer("Authorizer", props.AuthorizeFunction, new WebSocketLambdaAuthorizerProps
    {
      AuthorizerName = "ApiKeyAuthorizer",
      IdentitySource = ["route.request.header.x-api-key"],
    });

    api.AddRoute("$connect", new WebSocketRouteOptions
    {
      Integration = new WebSocketLambdaIntegration("ConnectIntegration", props.ConnectFunction),
      Authorizer = authorizer,
    });
    api.AddRoute("$disconnect", new WebSocketRouteOptions
    {
      Integration = new WebSocketLambdaIntegration("DisconnectIntegration", props.DisconnectFunction)
    });

    // For this assessment, we only need a single stage; in a real production system, we might have multiple stages
    // based on requirements.
    _ = new WebSocketStage(this, "LiveStage", new WebSocketStageProps
    {
      StageName = "live",
      Description = "The live, publicly accessible stage of the API",
      AutoDeploy = true,
      WebSocketApi = api,
    });
  }
}