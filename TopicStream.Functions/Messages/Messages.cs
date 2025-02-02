using System;

namespace TopicStream.Functions.Messages;

/// <summary>
/// The base message type
/// </summary>
/// <param name="Action">The action to execute</param>
public record Message(string Action);

/// <summary>
/// A message to subscribe to a topic
/// </summary>
/// <param name="Topic">The name of the topic to subscribe to</param>
public record SubscribeMessage(string Topic) : Message("subscribe");

/// <summary>
/// A message to unsubscribe from a topic
/// </summary>
/// <param name="Topic">The name of the topic to unsubscribe from</param>
public record UnsubscribeMessage(string Topic) : Message("unsubscribe");