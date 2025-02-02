using System.Net.WebSockets;
using System.Threading.Tasks;
using TopicStream.FunctionalTests.Api;
using TopicStream.FunctionalTests.ApiKeys;

namespace TopicStream.FunctionalTests;

[Collection(nameof(ApiKeyProvisioner))]
public class ConnectivityTests(ApiKeyProvisioner apiKeyProvisioner)
{
    private readonly TestApiKeys _testApiKeys = apiKeyProvisioner.GetRequiredApiKeys();

    [Fact]
    public async Task UserCanConnectWithAnApiKey()
    {
        var testCancellation = TestContext.Current.CancellationToken;
        using ClientWebSocket user = await TestWebSocketClient.CreateConnectedClient(_testApiKeys.Subscriber1.ApiKey, testCancellation);

        await user.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client closed", testCancellation);

        // This is a simple proof-of-life test to ensure the user can connect and disconnect
        // The assertion is silly, but this is a good sanity check that web socket API is allowing connections
        Assert.True(true, "User can connect and disconnect");
    }
}
