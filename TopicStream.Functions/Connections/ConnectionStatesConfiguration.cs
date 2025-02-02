using TopicStream.Configuration;

namespace TopicStream.Functions.Connections;

/// <summary>
/// Common configuration for updating connection states
/// </summary>
public static class ConnectionStatesConfiguration
{
  public static string ConnectionsTableName { get; } = EnvironmentVariables.GetRequired("CONNECTION_STATES_TABLE_NAME");
}