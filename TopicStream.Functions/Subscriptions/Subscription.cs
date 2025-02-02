using System;

namespace TopicStream.Functions.Subscriptions;

public record class Subscription(string Topic, string PrincipalId, DateTime SubscribedAt);