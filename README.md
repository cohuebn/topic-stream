# TopicStream

TopicStream is a WebSocket-based API designed for real-time message streaming. It enables users to establish persistent connections, subscribe to topics, and publish messages. When a user is both connected and subscribed to a topic, they receive all messages published to that topic in real time.

## Development

### Prerequisites

To develop, build, and deploy this application, you will need the following on your machine:

- [.NET](https://dotnet.microsoft.com/en-us/): This project was created using .NET 9.0
- [AWS CDK](https://docs.aws.amazon.com/cdk/v2/guide/getting_started.html) - The AWS CDK is
  used to deploy components to AWS

### Deploying to AWS

To deploy the stack to AWS, take the following steps using the AWS CDK:

1. Ensure your terminal is using the correct credentials/account for deployment: `aws sts get-caller-identity`
2. If the stack has never been deployed to the account, you'll need to bootstrap the AWS account: `cdk bootstrap`
3. Deploy the stack: `cdk deploy`
