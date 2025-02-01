# TopicStream

TopicStream is a WebSocket-based API designed for real-time message streaming. It enables users to establish persistent connections, subscribe to topics, and publish messages. When a user is both connected and subscribed to a topic, they receive all messages published to that topic in real time.

## Development

### Prerequisites

To develop, build, and deploy this application, you will need the following on your machine:

- [.NET](https://dotnet.microsoft.com/en-us/download): This project was created using .NET 8.0
- [AWS CDK](https://docs.aws.amazon.com/cdk/v2/guide/getting_started.html): The AWS CDK is
  used to deploy components to AWS
- [Docker](https://docs.docker.com/desktop/): The AWS CDK uses Docker under the hood when
  bundling source code for Lambda deployment. Additionally, the integration tests use
  Docker to spin up Localstack resources

### Deploying to AWS

This system is comprised of a couple of AWS CDK stacks:

1. `AccountStack` - These are account-wide resources. E.g. API Gateway requires a CloudWatch role for API Gateway access
   logging. This stack provisions these account-wide resource and there should only be one of these per account.
2. `TopicStream` - This is the primary system being deployed; it includes the API Gateway, Lambdas, Dynamo tables, etc. used
   to run the API that allows subscribing to topics and sending messages on those topics. There can multiple of these
   per account.

To deploy the system to AWS, take the following steps using the AWS CDK:

1. Open a terminal session using the root directory of this solution as it's current working directory
1. Ensure your terminal is using the correct AWS credentials/account for deployment: `aws sts get-caller-identity`. The IAM
   user/role used needs permission to create and manage all resources in the stack (API Gateways, Lambdas, Dynamo tables, IAM roles, etc.)
1. If the AWS account you are deploying to has never [been bootstrapped for the CDK](https://docs.aws.amazon.com/cdk/v2/guide/bootstrapping.html), you'll need to bootstrap the AWS account: `cdk bootstrap`
1. Deploy the stacks: `cdk deploy --all --app "dotnet run --project TopicStream.Infrastructure -- --functions-project TopicStream.Functions"`

In the last command above, you can deploy a specific stack by replacing `--all` with the stack name:

- Deploy the `AccountStack` stack: `cdk deploy AccountStack --app "dotnet run --project TopicStream.Infrastructure -- --functions-project TopicStream.Functions"`
- Deploy the `TopicStream` stack: `cdk deploy TopicStream --app "dotnet run --project TopicStream.Infrastructure -- --functions-project TopicStream.Functions"`
