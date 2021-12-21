using System;
using System.Collections.Generic;
using System.Text;

namespace MessageBrokerServer
{
    public class BrokerClient
    {
        public string ClientID { get; private set; }
        public byte[] ClientKey { get; private set; }
        public byte[] ClientIV { get; private set; }

        private List<KeyValuePair<DateTime, byte[]>> messages = new List<KeyValuePair<DateTime, byte[]>>();

        public void AddMessage(byte[] m)
        {
            lock (messages)
            {
                DateTime time = DateTime.Now.AddMinutes(-5);
                messages.RemoveAll(Item => Item.Key < time);
                messages.Add(new KeyValuePair<DateTime, byte[]>(DateTime.Now, m));
            }
        }
        public List<byte[]> GetMessages()
        {
            List<byte[]> result = new List<byte[]>();
            lock (messages)
            {
                foreach (KeyValuePair<DateTime, byte[]> m in messages)
                    result.Add(m.Value);
                messages.Clear();
            }
            return result;
        }

        public BrokerClient(string clientID)
        {
            this.ClientID = clientID;
            this.ClientKey = KeyStore.GetEncryptionKey(clientID);
            this.ClientIV = KeyStore.GenerateIV();
        }

    }
}
