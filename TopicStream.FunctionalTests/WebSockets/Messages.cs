using System;
using System.Text;
using System.Text.Json;
using TopicStream.Functions.Messages;

namespace TopicStream.FunctionalTests.WebSockets;

public static class MessagePreparer
{
  /// <summary>
  /// Prepare a message for sending via websockets
  /// </summary>
  /// <param name="message">The message to send</param>
  /// <returns>The message in byte form</returns>
  public static ArraySegment<byte> GetBytes<T>(T message) where T : Message
  {
    var serializedMessage = JsonSerializer.Serialize(message, MessageSerializerOptions.Standard);
    var bytes = Encoding.UTF8.GetBytes(serializedMessage);
    return new ArraySegment<byte>(bytes);
  }
}
