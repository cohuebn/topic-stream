using Amazon.CDK;
using Amazon.CDK.AWS.IAM;
using Constructs;

namespace TopicStream.Infrastructure.Constructs;

/// <summary>
/// Account-wide resources that are not part of the main system stack.
/// </summary>
internal class AccountStack : Stack
{

  public AccountStack(Construct scope, string id) : base(scope, id)
  {
    // The AWS CDK doesn't currently support:
    // 1. Directly setting the IAM role used across the whole account for API Gateway logging
    // 2. Wiring up API Gateway logging on web socket API stages.
    // For now, just create an IAM role that can be wired up via the console; in a real system, a workaround
    // could be created with custom CloudFormation resources to integrate directly into API Gateway.
    // Considering that out-of-scope for this assessment
    _ = new Role(this, "ApiGatewayLoggingRole", new RoleProps
    {
      RoleName = $"{id}-ApiGatewayLoggingRole",
      AssumedBy = new ServicePrincipal("apigateway.amazonaws.com"),
      ManagedPolicies =
      [
        ManagedPolicy.FromAwsManagedPolicyName("service-role/AmazonAPIGatewayPushToCloudWatchLogs")
      ]
    });
  }
}