using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using Amazon.ApiGatewayManagementApi;
using TopicStream.Functions.Configuration;
using TopicStream.Functions.Connections;
using TopicStream.Functions.Messages;
using TopicStream.Functions.Subscriptions;
using Amazon.ApiGatewayManagementApi.Model;
using System.IO;
using System.Text;
using System.Net;

namespace TopicStream.Functions.TopicMessages;


/// <summary>
/// Object responsible for broadcasting new messages
/// to all active subscribers
/// </summary>
class BroadcastHandlers
{
  private readonly AmazonDynamoDBClient _dynamoClient;
  private readonly Table _subscriptionsTable;
  private readonly Table _connectionsTable;
  private readonly AmazonApiGatewayManagementApiClient _apiGatewayManagementClient;

  public BroadcastHandlers()
  {
    _dynamoClient = new AmazonDynamoDBClient();
    _subscriptionsTable = Table.LoadTable(_dynamoClient, SubscriptionsConfiguration.SubscriptionsTableName);
    _connectionsTable = Table.LoadTable(_dynamoClient, ConnectionStatesConfiguration.ConnectionsTableName);
    _apiGatewayManagementClient = new AmazonApiGatewayManagementApiClient(new AmazonApiGatewayManagementApiConfig
    {
      ServiceURL = BroadcastConfiguration.WebSocketCallbackUrl
    });
  }

  private static List<Connection> FindMatchingConnectionsForPrincipal(
    string principalId, Dictionary<string,
    List<Connection>> activeConnections
  )
  {
    return activeConnections.GetValueOrDefault(principalId, []);
  }

  private async Task NotifyConnection(Connection connection, PublishMessage message, ILambdaContext context)
  {
    context.Logger.LogTrace("Notifying connection {connection} with message {@message}", connection.ConnectionId, message);
    byte[] byteArray = Encoding.UTF8.GetBytes(message.Message);
    using var memoryStream = new MemoryStream(byteArray);
    var postToConnectionRequest = new PostToConnectionRequest
    {
      ConnectionId = connection.ConnectionId,
      Data = memoryStream
    };
    var response = await _apiGatewayManagementClient.PostToConnectionAsync(postToConnectionRequest);
    if (response.HttpStatusCode == HttpStatusCode.OK)
    {
      context.Logger.LogInformation("Notified connection {connection} on topic {topic}", connection.ConnectionId, message.Topic);
    }
    else
    {
      context.Logger.LogError(
        "Failed to notify connection {connection} on topic {topic}. Response status {responseStatus}",
        connection.ConnectionId,
        message.Topic,
        response.HttpStatusCode
      );
    }
  }

  private async Task ProcessMessage(
    SQSEvent.SQSMessage message,
    ILambdaContext context,
    Dictionary<string, List<Connection>> activeConnections
  )
  {
    var publishedMessage = JsonSerializer.Deserialize<PublishMessage>(message.Body, MessageSerializerOptions.Standard);
    context.Logger.LogDebug("Processing {@message}", publishedMessage);

    if (publishedMessage is null)
    {
      context.Logger.LogError("Invalid message received: {message}", message.Body);
      return;
    }

    // Get all subscriptions for the topic that are tied to an active connection
    var subscriptionsDocuments = await _subscriptionsTable.Query(publishedMessage.Topic, new QueryFilter()).GetRemainingAsync();
    var subscriptions = subscriptionsDocuments.Select(DynamoSubscriptionConverter.FromDynamoDocument);
    var connectionsToNotify = subscriptions
      .Select(subscription => FindMatchingConnectionsForPrincipal(subscription.PrincipalId, activeConnections))
      .SelectMany(connections => connections);

    await Task.WhenAll(connectionsToNotify.Select(connection => NotifyConnection(connection, publishedMessage, context)));
  }

  /// <summary>
  /// Broadcast the provided message to all active subscribers
  /// </summary>
  /// <param name="event">The event from SQS triggering processing</param>
  /// <param name="context">Additional context for the Lambda environment</param>
  /// <returns>A task that completes after processing the publish request</returns>
  public async Task Broadcast(SQSEvent sqsEvent, ILambdaContext context)
  {
    context.Logger.LogTrace("Using callback url {callbackUrl}", BroadcastConfiguration.WebSocketCallbackUrl);
    context.Logger.LogTrace("Received event {@event}", sqsEvent);

    var activeConnectionDocuments = await _connectionsTable.Scan(new ScanFilter()).GetRemainingAsync();
    // Get active connections by principal to avoid iterating over all connections for each message
    var activeConnectionsByPrincipal = activeConnectionDocuments
      .Select(DynamoConnectionConverter.FromDynamoDocument)
      .GroupBy(connection => connection.PrincipalId)
      .ToDictionary(group => group.Key, group => group.ToList());
    await Task.WhenAll(sqsEvent.Records.Select(record => ProcessMessage(record, context, activeConnectionsByPrincipal)));
  }
}