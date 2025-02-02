using System;

namespace TopicStream.Functions.Connections;

public record class Connection(string PrincipalId, string ConnectionId, DateTime ConnectedAt) { }