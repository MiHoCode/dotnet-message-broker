using MessageBrokerClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace MessagingNode
{
    public class NodeClient
    {
        private List<KeyValuePair<DateTime, Message>> messages = new List<KeyValuePair<DateTime, Message>>();

        public void AddMessage(Message m)
        {
            lock (messages)
            {
                DateTime time = DateTime.Now.AddMinutes(-5);
                messages.RemoveAll(Item => Item.Key < time);
                messages.Add(new KeyValuePair<DateTime, Message>(DateTime.Now, m));
            }
        }
        public List<Message> GetMessages()
        {
            List<Message> result = new List<Message>();
            lock (messages)
            {
                foreach (var m in messages)
                    result.Add(m.Value);
                messages.Clear();
            }
            return result;
        }
    }
}
