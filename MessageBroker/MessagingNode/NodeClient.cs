using MessageBrokerClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace MessagingNode
{
    public class NodeClient
    {

        private List<Message> messages = new List<Message>();

        public void AddMessage(Message m)
        {
            lock (messages)
            {
                messages.Add(m);
            }
        }
        public List<Message> GetMessages()
        {
            List<Message> result = new List<Message>();
            lock (messages)
            {
                result.AddRange(messages);
                messages.Clear();
            }
            return result;
        }
    }
}
