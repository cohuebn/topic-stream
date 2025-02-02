using System;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using TopicStream.Functions.Configuration;
using TopicStream.Functions.Connections;
using TopicStream.Functions.Messages;

namespace TopicStream.Functions.Subscriptions;


/// <summary>
/// Object responsible for managing subscriptions to topics
/// </summary>
class SubscriptionHandlers
{
  private readonly AmazonDynamoDBClient _dynamoClient;
  private readonly Table _table;

  public SubscriptionHandlers()
  {
    _dynamoClient = new AmazonDynamoDBClient();
    _table = Table.LoadTable(_dynamoClient, SubscriptionsConfiguration.SubscriptionsTableName);
  }

  /// <summary>
  /// Subscribe the calling user to the requested topic
  /// </summary>
  /// <param name="request">The API Gateway request</param>
  /// <param name="context">Additional context for the Lambda environment</param>
  /// <returns>A success response after processing the subscription request</returns>
  public async Task<APIGatewayProxyResponse> Subscribe(APIGatewayProxyRequest request, ILambdaContext context)
  {
    var connection = ApiGatewayRequestParser.GetAuthorizedWebSocketConnection(request);
    if (
      !RequestMessageParser.TryGetMessage<SubscribeMessage>(request, context, out var subscriptionMessage) ||
      subscriptionMessage is null
    )
    {
      return new APIGatewayProxyResponse { StatusCode = 400, Body = "Invalid subscription message" };
    }
    context.Logger.LogDebug(
      "Subscribe request received: Topic {topic}, Principal {principal}",
      subscriptionMessage.Topic,
      connection.PrincipalId
    );

    var subscription = new Subscription(
      subscriptionMessage.Topic,
      connection.PrincipalId,
      DateTime.UtcNow
    );
    var dynamoDocument = DynamoSubscriptionConverter.ToDynamoDocument(subscription);
    await _table.PutItemAsync(dynamoDocument);
    return new APIGatewayProxyResponse
    {
      StatusCode = 200,
      Body = "Created subscription",
    };
  }

  /// <summary>
  /// Unsubscribe the calling user from the requested topic
  /// </summary>
  /// <param name="request">The API Gateway request</param>
  /// <param name="context">Additional context for the Lambda environment</param>
  /// <returns>A success response after processing the unsubscribe request</returns>
  public async Task<APIGatewayProxyResponse> Unsubscribe(APIGatewayProxyRequest request, ILambdaContext context)
  {
    var connection = ApiGatewayRequestParser.GetAuthorizedWebSocketConnection(request);
    if (!RequestMessageParser.TryGetMessage<UnsubscribeMessage>(request, context, out var subscriptionMessage) || subscriptionMessage is null)
    {
      return new APIGatewayProxyResponse { StatusCode = 400, Body = "Invalid subscription message" };
    }
    context.Logger.LogDebug(
      "Unsubscribe request received: Topic {topic}, Principal {principal}",
      subscriptionMessage.Topic,
      connection.PrincipalId
    );

    await _table.DeleteItemAsync(subscriptionMessage.Topic, connection.PrincipalId);
    return new APIGatewayProxyResponse
    {
      StatusCode = 200,
      Body = "Removed subscription",
    };
  }
}