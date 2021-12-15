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

        private List<byte[]> messages = new List<byte[]>();

        public void AddMessage(byte[] m)
        {
            lock (messages)
            {
                messages.Add(m);
            }
        }
        public List<byte[]> GetMessages()
        {
            List<byte[]> result = new List<byte[]>();
            lock (messages)
            {
                result.AddRange(messages);
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
