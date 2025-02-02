using Constructs;
using Amazon.CDK;
using Amazon.CDK.AWS.Lambda;
using Amazon.CDK.AWS.IAM;

namespace TopicStream.Infrastructure.Constructs;

internal interface IAuthorizerFunctionProps : IFunctionProps
{
  public Stack Stack { get; }
}

internal class AuthorizerFunctionProps() : TopicStreamFunctionProps, IAuthorizerFunctionProps
{
  public required Stack Stack { get; init; }
}

internal class AuthorizerFunction : Function
{
  public AuthorizerFunction(Construct scope, string id, IAuthorizerFunctionProps props) : base(scope, id, props)
  {
    var policyStatementProps = new PolicyStatementProps
    {
      Effect = Effect.ALLOW,
      Resources = [
        props.Stack.FormatArn(new ArnComponents
        {
          // This is a wildcard because the API Gateway resource doesn't have an account on the key resources
          // when you request a list of API keys
          Account = "*",
          Service = "apigateway",
          Resource = "/apikeys"
        })
      ],
      Actions = ["apigateway:GET"],
    };
    AddToRolePolicy(new PolicyStatement(policyStatementProps));
  }
}