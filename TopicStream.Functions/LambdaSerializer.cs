using Amazon.Lambda.Core;

// Set up the serializer for all Lambda functions
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]