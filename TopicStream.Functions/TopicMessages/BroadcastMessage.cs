namespace TopicStream.Functions.Messages;

/// <summary>
/// A message to be broadcast to all subscribers
/// </summary>
/// <param name="Topic">The topic for the message</param>
/// <param name="Message">The message itself</param>
public record class BroadcastMessage(string Topic, string Message);