# TopicStream

TopicStream is a WebSocket-based API designed for real-time message streaming. It enables users to establish persistent connections, subscribe to topics, and publish messages. When a user is both connected and subscribed to a topic, they receive all messages published to that topic in real time.

## Development

### Prerequisites

To develop, build, and deploy this application, you will need the following on your machine:

- [.NET](https://dotnet.microsoft.com/en-us/download): This project was created using .NET 8.0
- [AWS CDK](https://docs.aws.amazon.com/cdk/v2/guide/getting_started.html): The AWS CDK is
  used to deploy components to AWS
- [Docker](https://docs.docker.com/desktop/): The AWS CDK uses Docker under the hood when
  bundling source code for Lambda deployment

### Deploying to AWS

To deploy the stack to AWS, take the following steps using the AWS CDK:

1. Open a terminal session using the root directory of this solution as it's current working directory
1. Ensure your terminal is using the correct AWS credentials/account for deployment: `aws sts get-caller-identity`
1. If the AWS account you are deploying to has never [been bootstrapped for the CDK](https://docs.aws.amazon.com/cdk/v2/guide/bootstrapping.html), you'll need to bootstrap the AWS account: `cdk bootstrap`
1. Deploy the stack: `cdk deploy --app "dotnet run --project TopicStream.Infrastructure -- --functions-project TopicStream.Functions"`
