using System;
using System.IO;
using System.Text;

namespace MessageBrokerClient
{
    public class Message
    {
        public string ID { get; set; } = Guid.NewGuid().ToString();
        public string IsResponseOf { get; set; }
        public string Sender { get; set; }
        public string Receiver { get; set; }
        public byte[] Content { get; set; }

        public byte[] ToByteArray()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BigEndianStream stream = new BigEndianStream(ms);

                stream.WriteShortString(this.Receiver);
                stream.WriteShortString(this.ID);
                stream.WriteShortString(this.IsResponseOf);
                stream.WriteShortString(this.Sender);

                stream.WriteBuffer(this.Content);

                return ms.ToArray();
            }
        }

        public static Message FromByteArray(byte[] data)
        {
            using (MemoryStream ms = new MemoryStream(data))
            {
                ms.Position = 0;
                BigEndianStream stream = new BigEndianStream(ms);

                Message message = new Message();
                message.Receiver = stream.ReadShortString();
                message.ID = stream.ReadShortString();
                message.IsResponseOf = stream.ReadShortString();
                message.Sender = stream.ReadShortString();

                message.Content = stream.ReadBuffer();

                return message;
            }
        }

        public static string ExtractReceiver(byte[] messageAsBytes)
        {
            int l = messageAsBytes[0];
            byte[] data = new byte[l];
            for (int i = 0; i < l; i++)
            {
                data[i] = messageAsBytes[1 + i];
            }
            return Encoding.UTF8.GetString(data);
        }

        public Message CreateResponse()
        {
            Message response = new Message();
            response.IsResponseOf = this.ID;
            response.Sender = this.Receiver;

            return response;
        }
    }
}
