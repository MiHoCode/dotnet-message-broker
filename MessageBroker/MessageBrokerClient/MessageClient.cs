using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace MessageBrokerClient
{
    public class MessageClient
    {
        public event Action<Message> MessageReceived;

        public const int DEFAULT_PORT = 10242;
        public int Port { get; private set; }
        public string Hostname { get; private set; }
        public string ClientID { get; private set; }

        public byte[] ServerKey { get; private set; }
        public byte[] ClientKey { get; private set; }
        public byte[] ServerIV { get; private set; }
        public byte[] ClientIV { get; private set; }

        public int RequestDelay { get; set; }

        private TcpClient tcp;
        private BigEndianStream stream;
        private List<Message> messages = new List<Message>();
        private Dictionary<string, Action<Message>> callbacks = new Dictionary<string, Action<Message>>();

        public bool Running { get; private set; }

        public MessageClient() : this(DEFAULT_PORT) { }

        public MessageClient(int port)
        {
            this.Port = port;
            this.RequestDelay = 1000;
        }

        public bool Start(string hostname, string clientID, string serverKey, string clientKey)
        {
            try
            {
                this.Hostname = hostname;
                this.ClientID = clientID;
                this.ServerKey = Convert.FromBase64String(serverKey);
                this.ClientKey = Convert.FromBase64String(clientKey);

                tcp = new TcpClient(this.Hostname, this.Port);
                stream = new BigEndianStream(tcp.GetStream());

                this.ServerIV = stream.ReadMiniBuffer();
                byte[] authKey = Crypt.Decrypt(stream.ReadMiniBuffer(), this.ServerKey, this.ServerIV);

                stream.WriteMiniBuffer(authKey);
                stream.WriteMiniBuffer(Crypt.Encrypt(Encoding.UTF8.GetBytes(this.ClientID), this.ServerKey, this.ServerIV));

                this.ClientIV = Crypt.Decrypt(stream.ReadMiniBuffer(), this.ServerKey, this.ServerIV);

                authKey = Crypt.Decrypt(stream.ReadMiniBuffer(), this.ClientKey, this.ClientIV);
                stream.WriteMiniBuffer(authKey);


                this.Running = true;
                new Thread(new ThreadStart(run)).Start();
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void run()
        {
            while (this.Running)
            {
                Message outMessage = null;
                lock (messages)
                {
                    if (messages.Count > 0)
                    {
                        outMessage = messages[0];
                        messages.RemoveAt(0);
                    }
                }

                if(outMessage != null)
                {
                    stream.WriteMiniBuffer(Crypt.Encrypt(Encoding.UTF8.GetBytes("message"), this.ClientKey, this.ClientIV));
                    stream.WriteBuffer(Crypt.Encrypt(outMessage.ToByteArray(), this.ClientKey, this.ClientIV));
                }
                else
                {
                    stream.WriteMiniBuffer(Crypt.Encrypt(Encoding.UTF8.GetBytes("peek"), this.ClientKey, this.ClientIV));
                }

                int incomingMessages = stream.ReadInt16();
                for (int i = 0; i < incomingMessages; i++)
                {
                    byte[] messageBytes = Crypt.Decrypt(stream.ReadBuffer(), this.ClientKey, this.ClientIV);
                    Message message = Message.FromByteArray(messageBytes);

                    Action<Message> callback = null;
                    if (!string.IsNullOrEmpty(message.IsResponseOf))
                    {
                        lock (callbacks)
                        {
                            if (callbacks.ContainsKey(message.IsResponseOf))
                            {
                                callback = callbacks[message.IsResponseOf];
                                callbacks.Remove(message.IsResponseOf);
                            }
                        }
                    }

                    if(callback != null)
                    {
                        callback(message);
                    }
                    else
                    {
                        OnMessageReceived(message);
                    }
                }

                Thread.Sleep(this.RequestDelay);
            }
        }

        public void Close()
        {
            this.Running = false;
            tcp.Close();
            tcp.Dispose();
        }

        public void SendMessage(string receiver, string content, Action<Message> responseCallback = null)
        {
            SendMessage(receiver, Encoding.UTF8.GetBytes(content), responseCallback);
        }
        public void SendMessage(string receiver, byte[] content, Action<Message> responseCallback = null)
        {
            Message m = new Message();
            m.Sender = this.ClientID;
            m.Receiver = receiver;
            m.IsResponseOf = "";
            m.Content = content;
            SendMessage(m, responseCallback);
        }
        public void SendMessage(Message message, Action<Message> responseCallback = null)
        {
            message.Sender = this.ClientID;
            if (responseCallback != null)
            {
                lock (callbacks)
                {
                    callbacks.Add(message.ID, responseCallback);
                }
            }
            lock (messages)
            {
                messages.Add(message);
            }
        }

        protected virtual void OnMessageReceived(Message message)
        {
            if (this.MessageReceived != null)
                this.MessageReceived(message);
        }
    }
}
