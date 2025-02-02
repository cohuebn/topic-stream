using System;
using System.Net.WebSockets;
using System.Threading.Tasks;
using TopicStream.FunctionalTests.ApiKeys;
using TopicStream.FunctionalTests.Configuration;

namespace TopicStream.FunctionalTests.Api;

public static class TestWebSocketClient
{
  /// <summary>
  /// Get a new web socket client connected to the test environment
  /// </summary>
  /// <returns>The connected web socket client</returns>
  public static async Task<ClientWebSocket> CreateConnectedClient(string apiKeyValue, System.Threading.CancellationToken cancellationToken)
  {
    ClientWebSocket client = new();
    var uri = new Uri(TestConfiguration.GetUrl());
    client.Options.SetRequestHeader("x-api-key", apiKeyValue);
    await client.ConnectAsync(uri, cancellationToken);
    return client;
  }
}