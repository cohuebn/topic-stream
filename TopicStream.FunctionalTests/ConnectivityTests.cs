using System;
using System.Net.WebSockets;
using System.Threading.Tasks;
using TopicStream.FunctionalTests.ApiKeys;
using TopicStream.FunctionalTests.Configuration;

namespace TopicStream.FunctionalTests;

[Collection(nameof(ApiKeyProvisioner))]
public class ConnectivityTests(ApiKeyProvisioner apiKeyProvisioner)
{
    private readonly TestApiKeys _testApiKeys = apiKeyProvisioner.GetRequiredApiKeys();

    [Fact]
    public async Task UnauthorizedUser_CannotConnect()
    {
        var testCancellation = TestContext.Current.CancellationToken;
        using ClientWebSocket user = new();

        var exception = await Assert.ThrowsAsync<WebSocketException>(
            () => user.ConnectAsync(TestConfiguration.GetUri(), testCancellation)
        );
        Assert.Contains("The server returned status code '401'", exception.Message);
    }

    [Fact]
    public async Task UserWithInvalidApiKey_CannotConnect()
    {
        var testCancellation = TestContext.Current.CancellationToken;
        using ClientWebSocket user = new();
        user.Options.SetRequestHeader("x-api-key", "if-i-succeed-this-test-is-broken");

        var exception = await Assert.ThrowsAsync<WebSocketException>(
            () => user.ConnectAsync(TestConfiguration.GetUri(), testCancellation)
        );
        Assert.Contains("The server returned status code '401'", exception.Message);
    }

    [Fact]
    public async Task AuthorizedUser_CanConnect()
    {
        var testCancellation = TestContext.Current.CancellationToken;
        using ClientWebSocket user = new();
        user.Options.SetRequestHeader("x-api-key", _testApiKeys.Subscriber1.ApiKey);
        await user.ConnectAsync(TestConfiguration.GetUri(), testCancellation);

        await user.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client closed", testCancellation);

        // This is a simple proof-of-life test to ensure the user can connect and disconnect
        // The assertion is silly, but this is a good sanity check that web socket API is allowing connections
        Assert.True(true, "User can connect and disconnect");
    }
}
