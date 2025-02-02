using System.Collections.Generic;
using System.Linq;

namespace TopicStream.FunctionalTests.Users;

public class ReceivedMessages : Dictionary<string, List<string>>
{
  public int TotalMessageCount => Values.Sum(messages => messages.Count);

  public void AddMessage(string topic, string message)
  {
    if (!ContainsKey(topic))
    {
      Add(topic, []);
    }

    this[topic].Add(message);
  }

  public List<string> GetMessagesOnTopic(string topic)
  {
    return ContainsKey(topic) ? this[topic] : [];
  }
}