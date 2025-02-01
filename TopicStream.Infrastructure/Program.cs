using Amazon.CDK;
using Amazon.CDK.AWS.Lambda;
using TopicStream.Infrastructure;

// Bundle up app code for Lambdas
var buildOptions = new BundlingOptions
{
  Image = Runtime.DOTNET_8.BundlingImage,
  User = "root",
  OutputType = BundlingOutput.ARCHIVED,
  Command = [
        "/bin/sh",
          "-c",
          " dotnet tool install -g Amazon.Lambda.Tools" +
          " && dotnet build" +
          " && dotnet lambda package --output-package /asset-output/function.zip"
      ]
};
var bundledCode = Code.FromAsset("../TopicStream.Functions", new Amazon.CDK.AWS.S3.Assets.AssetOptions
{
  Bundling = buildOptions
});

// Create the AWS resources for the stack
var app = new App();
_ = new TopicStreamStack(app, "TopicStreamStack", bundledCode, new StackProps());
app.Synth();