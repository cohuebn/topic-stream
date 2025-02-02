using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using System.Threading.Tasks;
using TopicStream.Functions.Configuration;
using TopicStream.Functions.Dynamo;

namespace TopicStream.Functions.Connections;


/// <summary>
/// Object responsible for managing user connections to the application
/// </summary>
class ConnectionHandlers
{
  private readonly AmazonDynamoDBClient _dynamoClient;
  private readonly Table _table;

  public ConnectionHandlers()
  {
    _dynamoClient = new AmazonDynamoDBClient();
    _table = Table.LoadTable(_dynamoClient, ConnectionStatesConfiguration.ConnectionsTableName);
  }

  /// <summary>
  /// Register a user connecting to the application
  /// </summary>
  /// <param name="request">The API Gateway request</param>
  /// <param name="context">Additional context for the Lambda environment</param>
  /// <returns>A success response after processing the connection request</returns>
  public async Task<APIGatewayProxyResponse> Connect(APIGatewayProxyRequest request, ILambdaContext context)
  {
    var connection = ApiGatewayRequestParser.GetAuthorizedWebSocketConnection(request);
    context.Logger.LogDebug("Connection request received: {@connection}", connection);
    var dynamoDocument = DynamoConnectionConverter.ToDynamoDocument(connection);
    await _table.PutItemAsync(dynamoDocument);
    return new APIGatewayProxyResponse
    {
      StatusCode = 200,
      Body = "Connected",
    };
  }

  /// <summary>
  /// Register a user disconnecting from the application
  /// </summary>
  /// <param name="request">The API Gateway request</param>
  /// <param name="context">Additional context for the Lambda environment</param>
  /// <returns>A success response after processing the disconnection request</returns>
  public async Task<APIGatewayProxyResponse> Disconnect(APIGatewayProxyRequest request, ILambdaContext context)
  {
    var connection = ApiGatewayRequestParser.GetAuthorizedWebSocketConnection(request);
    context.Logger.LogDebug("Disconnect request received: {@connection}", connection);
    await _table.DeleteItemAsync(connection.ConnectionId, DynamoConverters.ToDynamoDate(connection.ConnectedAt));
    return new APIGatewayProxyResponse
    {
      StatusCode = 200,
      Body = "Disconnected",
    };
  }
}