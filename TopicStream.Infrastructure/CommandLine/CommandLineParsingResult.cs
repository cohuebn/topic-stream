using TopicStream.Infrastructure.CommandLine;

internal enum ParsingStatus
{
  InformationOnlyRequest,
  Proceed,
  Halt,
}

internal record class CommandLineParsingResult(ParsingStatus ParsingStatus, CommandLineOptions? Options);