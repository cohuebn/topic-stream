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

### IDEs

This solution was developed in VS Code, but an attemp was made to also make it Visual Studio compatible.
Launch configurations for debugging have only been setup for VS Code.

### Project Structure

This solution is organized into the following projects:

- [TopicStream.FunctionalTests](./TopicStream.FunctionalTests/): These are functional tests to test a deployed instance of
  the application end-to-end. They connect to an actual deployed instance in AWS
- [TopicStream.Functions](./TopicStream.Functions/): This is the source code run by Lambdas within the TopicStream system
- [TopicStream.Functions.UnitTests](./TopicStream.Functions.UnitTests/): These are unit tests of Lambda function code
- [TopicStream.Infrastructure](./TopicStream.Infrastructure/): This project is responsible for managing/deploying the infrastructure used to run the TopicStream system into AWS

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

## Destroying a Deployed Stack

See the above section for context about the stacks. To destroy a stack, follow the same
steps as the above section, except the last one. Instead, run:

- Destroy all stacks: `cdk destroy --all --app "dotnet run --project TopicStream.Infrastructure -- --functions-project TopicStream.Functions"`
- Destroy the TopicStream stack: `cdk destroy TopicStream --app "dotnet run --project TopicStream.Infrastructure -- --functions-project TopicStream.Functions"`

## Running Unit Tests

While priority was placed on developing functional tests, there are unit tests for some of the Lambda source code.
To run these unit tests, take the following steps:

1. Open a terminal session using the root directory of this solution as it's current working directory
2. Run the tests: `dotnet test TopicStream.Functions.UnitTests`

## Running Functional Tests

This section describes how to run the [functional tests](./TopicStream.FunctionalTests/) against
a deployed version of the API.

### Required Environment Variables

To run these tests, you'll need to provide environment variables to:

1. Connect to a deployed system
2. Provide API key values; the tests will then create, use, and dispose of these keys

In order to document the required environment variables, an [example.env](./example.env) file
has been created. These aren't actually used by the tests, but just there for documenting
needed environment variables. The most important variables are as follows:

| Variable name                     | Purpose                                                    |
| --------------------------------- | ---------------------------------------------------------- |
| TOPIC_STREAM_URL                  | The web socket url to use to connect to the running system |
| TOPIC_STREAM_SUBSCRIBER_1_API_KEY | The API key value for subscriber 1 used by tests           |
| TOPIC_STREAM_SUBSCRIBER_2_API_KEY | The API key value for subscriber 2 used by tests           |
| TOPIC_STREAM_PUBLISHER_API_KEY    | The API key value for the publisher used by tests          |

### Running the Tests

One way to run the functional tests is to provide the environment variables directly to the
tests. This section documents how to do that:

1. Open a terminal session using the root directory of this solution as it's current working directory
1. Ensure your terminal is using the correct AWS credentials/account for deployment: `aws sts get-caller-identity`. The IAM
   user/role used needs permission to create and delete API keys used by the test
1. Run the tests, providing all relevant environment variables: `

Additionally, a launch configuration named "Debug Functional Tests" has been created for debugging
the tests. In order to use it, you'll want to update the environment variables in that configuration
to match your deployed target environment.
