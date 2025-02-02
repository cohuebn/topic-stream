using System.Text.Json;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using TopicStream.Functions.Connections;

namespace TopicStream.Functions.Messages;

public static class RequestMessageParser
{
  /// <summary>
  /// A helper that tries parsing the request body into a message
  /// </summary>
  /// <typeparam name="TMessage">The type of the expected message</typeparam>
  /// <param name="request">The API Gateway request</param>
  /// <param name="context">The surrounding Lambda context</param>
  /// <param name="message">The out parameter that will contain the parsed message upon success</param>
  /// <returns>true if the message was parsed successfully, false otherwise</returns>
  public static bool TryGetMessage<TMessage>(APIGatewayProxyRequest request, ILambdaContext context, out TMessage? message) where TMessage : Message
  {
    message = JsonSerializer.Deserialize<TMessage>(request.Body, MessageSerializerOptions.Standard);
    if (message is null)
    {
      context.Logger.LogWarning(
        "Invalid message, could not parse: Connection {@connection}, Message: {message}",
        ApiGatewayRequestParser.GetAuthorizedWebSocketConnection(request),
        request.Body
      );
      message = null;
      return false;
    }
    return true;
  }
}