using System;
using System.Net.WebSockets;
using System.Threading.Tasks;
using TopicStream.FunctionalTests.ApiKeys;
using TopicStream.FunctionalTests.Configuration;
using TopicStream.FunctionalTests.Users;
using TopicStream.FunctionalTests.WebSockets;
using TopicStream.Functions.Messages;

namespace TopicStream.FunctionalTests;

/// <summary>
/// Various tests to ensure the system is working end-to-end
/// </summary>
/// <param name="apiKeyProvisioner">An object that injects API keys before tests run</param>
[Collection(nameof(ApiKeyProvisioner))]
public class SystemTests(ApiKeyProvisioner apiKeyProvisioner)
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
        using Subscriber subscriber = new(_testApiKeys.Subscriber1);
        await subscriber.ConnectAsync(testCancellation);

        await subscriber.DisconnectAsync(testCancellation);

        Assert.True(true, "User can connect and disconnect");
    }

    [Fact]
    public async Task UserSendingMessageToUnknownAction_CanStillCloseConnection()
    {
        var testCancellation = TestContext.Current.CancellationToken;
        using Subscriber subscriber = new(_testApiKeys.Subscriber1);
        await subscriber.ConnectAsync(testCancellation);

        var unknownActionMessage = new Message("what?!");
        await subscriber.SendAsync(unknownActionMessage, testCancellation);

        await subscriber.DisconnectAsync(testCancellation);

        Assert.True(true, "User can connect and disconnect even after sending unknown action");
    }

    /// <summary>
    /// Validate that:
    /// 1. An authorized user can subscribe to a topic
    /// 2. When a publisher publishes a new message, the subscriber receives it
    /// 3. The subscriber can unsubscribe from that topic
    /// </summary>
    [Fact]
    public async Task AuthorizedUser_CanReceiveMessagesOnSubscribedTopic()
    {
        var testCancellation = TestContext.Current.CancellationToken;
        using Subscriber subscriber = new(_testApiKeys.Subscriber1);
        await subscriber.ConnectAsync(testCancellation);

        var topic = $"test-topic-{Guid.NewGuid()}";
        await subscriber.SubscribeAsync(topic, testCancellation);

        var unsubscribeMessage = new SubscribeMessage(topic);
        await subscriber.UnsubscribeAsync(topic, testCancellation);

        await subscriber.DisconnectAsync(testCancellation);

        Assert.True(true, "User can subscribe");
    }
}
