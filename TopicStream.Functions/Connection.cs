using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;

namespace TopicStream.Functions;


/// <summary>
/// Object responsible for managing user connections to the application
/// </summary>
class Connection
{
  /// <summary>
  /// Register a user connecting to the application
  /// </summary>
  /// <param name="request">The API Gateway request</param>
  /// <param name="context">Additional context for the Lambda environment</param>
  /// <returns>A success response after processing the connection request</returns>
  public static APIGatewayProxyResponse Connect(APIGatewayProxyRequest request, ILambdaContext context)
  {
    context.Logger.LogTrace("Connection request received {@request}", request);
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
  public static APIGatewayProxyResponse Disconnect(APIGatewayProxyRequest request, ILambdaContext context)
  {
    context.Logger.LogTrace("Disconnection request received {@request}", request);
    return new APIGatewayProxyResponse
    {
      StatusCode = 200,
      Body = "Disconnected",
    };
  }
}