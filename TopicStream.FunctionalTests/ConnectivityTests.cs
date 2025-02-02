using System;
using System.Net.WebSockets;
using System.Threading.Tasks;
using TopicStream.FunctionalTests.ApiKeys;
using TopicStream.FunctionalTests.Configuration;
using TopicStream.FunctionalTests.WebSockets;

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
    public async Task AuthorizedUser_CanConnect_AndDisconnect()
    {
        // This is a simple proof-of-life test to ensure the user can connect and disconnect
        // The assertion is silly, but this is a good sanity check that web socket API is allowing connections.
        // It validates:
        // 1. Auth is working as expected for a valid user
        // 2. The user only needs an API key to connect; they can disconnect without the key

        var testCancellation = TestContext.Current.CancellationToken;
        using ClientWebSocket user = new();
        await user.ConnectAsync(
            TestConfiguration.GetUri(),
            OneTimeApiKeyMessageInvokerFactory.Create(_testApiKeys.Subscriber1.ApiKey),
            testCancellation
        );

        await user.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client closed", testCancellation);

        Assert.True(true, "User can connect and disconnect");
    }
}
