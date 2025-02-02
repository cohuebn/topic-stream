using Amazon.Lambda.APIGatewayEvents;

namespace TopicStream.Functions.Connections;

public static class IamPolicyGenerator
{
  private static APIGatewayCustomAuthorizerPolicy GeneratePolicy(string methodArn, string effect)
  {
    return new APIGatewayCustomAuthorizerPolicy
    {
      Version = "2012-10-17",
      Statement =
      [
        new() {
          Action = ["execute-api:Invoke"],
          Effect = effect,
          Resource = [methodArn]
        }
      ]
    };
  }

  public static APIGatewayCustomAuthorizerPolicy GenerateAllowPolicy(string methodArn)
  {
    return GeneratePolicy(methodArn, "Allow");
  }

  public static APIGatewayCustomAuthorizerPolicy GenerateDenyPolicy(string methodArn)
  {
    return GeneratePolicy(methodArn, "Deny");
  }
}