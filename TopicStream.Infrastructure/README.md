# TopicStream.Infrastructure

This project is responsible for managing the infrastructure used to run the TopicStream system.
It uses the AWS CDK to deploy resources into AWS. For simplicity's sake, this project creates a bundle for [the TopicStream.Functions code](../TopicStream.Functions/) and pushes that to the corresponding Lambda infrastructure as
part of the build/deployment process
