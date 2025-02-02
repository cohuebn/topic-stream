namespace TopicStream.Functions.UnitTests.Connections;

using System.Collections.Generic;
using System.Linq;
using Bogus;
using TopicStream.Functions.Connections;

public class UserIdProviderTests
{
    [Fact]
    public void GetUserId_WhenCalledWithTheSameApiKeyMultipleTimes_ReturnsTheSameUserId()
    {
        var apiKey = "this-here-my-secret-dont-tell-nobody";

        var providedId1 = UserIdProvider.GetUserId(apiKey);
        var providedId2 = UserIdProvider.GetUserId(apiKey);

        Assert.Equal(providedId1, providedId2);
    }

    [Fact]
    public void GetUserId_WhenCalledWithDifferentApiKeys_ReturnsDifferentUserIds()
    {
        // Generate 100 unique pseudorandom API keys and verify none of the derived user ids are the same
        // API Gateway allows API keys to be between 20 and 128 characters long, so match that
        var apiKeyGenerator = new Faker<string>().CustomInstantiator(faker =>
        {
            var apiKeyLength = faker.Random.Number(20, 128);
            return faker.Random.AlphaNumeric(apiKeyLength);
        });
        var apiKeys = apiKeyGenerator.Generate(100);

        var userIdProvider = new UserIdProvider();
        var userIds = apiKeys.Select(UserIdProvider.GetUserId);
        var uniqueUserIds = new HashSet<string>(userIds);

        Assert.Equal(apiKeys.Count, uniqueUserIds.Count);
    }
}
