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
  Function SubscribeFunction { get; }
  Function UnsubscribeFunction { get; }
  Function PublishFunction { get; }
  Function UnknownActionFunction { get; }
}

internal class TopicStreamApiGatewayProps : ITopicStreamApiGatewayProps
{
  public required string ApiName { get; init; }
  public required Function AuthorizeFunction { get; init; }
  public required Function ConnectFunction { get; init; }
  public required Function DisconnectFunction { get; init; }
  public required Function SubscribeFunction { get; init; }
  public required Function UnsubscribeFunction { get; init; }
  public required Function PublishFunction { get; init; }
  public required Function UnknownActionFunction { get; init; }
}

/// <summary>
/// The WebSocket API Gateway wired up to supporting Lambda functions.
/// This includes a deployed stage for public access as long as the
/// user provides a valid API key.
/// </summary>
internal class TopicStreamApiGateway : Construct
{
  public WebSocketApi Api { get; }
  public WebSocketStage LiveStage { get; }

  public TopicStreamApiGateway(Construct scope, string id, ITopicStreamApiGatewayProps props) : base(scope, id)
  {
    Api = new WebSocketApi(this, "Api", new WebSocketApiProps
    {
      ApiName = props.ApiName,
      RouteSelectionExpression = "$request.body.action",
    });

    var authorizer = new WebSocketLambdaAuthorizer("Authorizer", props.AuthorizeFunction, new WebSocketLambdaAuthorizerProps
    {
      AuthorizerName = "ApiKeyAuthorizer",
      IdentitySource = ["route.request.header.x-api-key"],
    });

    // Connection routes
    Api.AddRoute("$connect", new WebSocketRouteOptions
    {
      Integration = new WebSocketLambdaIntegration("ConnectIntegration", props.ConnectFunction),
      Authorizer = authorizer,
    });
    Api.AddRoute("$disconnect", new WebSocketRouteOptions
    {
      Integration = new WebSocketLambdaIntegration("DisconnectIntegration", props.DisconnectFunction)
    });

    //Unknown actions route
    Api.AddRoute("$default", new WebSocketRouteOptions
    {
      Integration = new WebSocketLambdaIntegration("UnknownActionIntegration", props.UnknownActionFunction)
    });

    // Subscription routes
    Api.AddRoute("subscribe", new WebSocketRouteOptions
    {
      Integration = new WebSocketLambdaIntegration("SubscribeIntegration", props.SubscribeFunction),
    });
    Api.AddRoute("unsubscribe", new WebSocketRouteOptions
    {
      Integration = new WebSocketLambdaIntegration("UnsubscribeIntegration", props.UnsubscribeFunction),
    });

    // Topic messages
    Api.AddRoute("publish", new WebSocketRouteOptions
    {
      Integration = new WebSocketLambdaIntegration("PublishIntegration", props.PublishFunction),
    });

    // For this assessment, we only need a single stage; in a real production system, we might have multiple stages
    // based on requirements.
    LiveStage = new WebSocketStage(this, "LiveStage", new WebSocketStageProps
    {
      StageName = "live",
      Description = "The live, publicly accessible stage of the API",
      AutoDeploy = true,
      WebSocketApi = Api,
    });
  }
}