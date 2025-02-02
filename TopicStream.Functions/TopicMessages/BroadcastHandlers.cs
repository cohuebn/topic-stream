using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using TopicStream.Functions.Configuration;

namespace TopicStream.Functions.TopicMessages;


/// <summary>
/// Object responsible for broadcasting new messages
/// to all active subscribers
/// </summary>
class BroadcastHandlers
{
  /// <summary>
  /// Broadcast the provided message to all active subscribers
  /// </summary>
  /// <param name="event">The event from SQS triggering processing</param>
  /// <param name="context">Additional context for the Lambda environment</param>
  /// <returns>A task that completes after processing the publish request</returns>
  public static void Broadcast(SQSEvent sqsEvent, ILambdaContext context)
  {
    context.Logger.LogTrace("Using callback url {callbackUrl}", BroadcastConfiguration.WebSocketCallbackUrl);
    context.Logger.LogTrace("Received event {@event}", sqsEvent);
  }
}