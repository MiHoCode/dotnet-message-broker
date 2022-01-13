using MessageBrokerClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace MessagingNode
{
    public class MessagingNodeService : IDisposable
    {
        public const int PORT = 10243;

        public string Host { get; private set; }
        public string ClientID { get; private set; }
        public string ServerKey { get; private set; }
        public string ClientKey { get; private set; }

        public TcpListener TcpListener { get; private set; }
        private bool running;

        public MessageClient BrokerClient { get; private set; }


        private Dictionary<string, NodeClient> clients = new Dictionary<string, NodeClient>();

        private NodeClient getClient(string clientID)
        {
            lock (clients)
            {
                if (!clients.ContainsKey(clientID))
                {
                    NodeClient client = new NodeClient();
                    clients.Add(clientID, client);
                }

                return clients[clientID];
            }
        }

        public MessagingNodeService(string host, string clientID, string serverKey, string clientKey)
        {
            this.Host = host;
            this.ClientID = clientID;
            this.ServerKey = serverKey;
            this.ClientKey = clientKey;

            this.TcpListener = new TcpListener(IPAddress.Any, PORT);
            this.BrokerClient = new MessageClient();
            this.BrokerClient.MessageReceived += OnMessageReceived;
        }

        private void OnMessageReceived(Message message)
        {
            if (!message.Recipient.Contains('/'))
            {
                return;
            }

            string device = message.Recipient.Split('/')[1];
            getClient(device).AddMessage(message);
        }

        public void Run()
        {
            if(!this.BrokerClient.Start(this.Host, this.ClientID, this.ServerKey, this.ClientKey))
            {
                Console.WriteLine("BrokerClient failed.");
                return;
            }
            Console.WriteLine("BrokerClient running.");

            running = true;
            this.TcpListener.Start();
            Console.WriteLine("MessagingNodeService running.");
            while (running)
            {
                try
                {
                    TcpClient client = this.TcpListener.AcceptTcpClient();
                    new Thread(new ParameterizedThreadStart(clientThread)).Start(client);
                }
                catch
                {
                    Thread.Sleep(100);
                }
            }
        }

        private void clientThread(object clientObj)
        {
            try
            {
                TcpClient tcp = (TcpClient)clientObj;
                NetworkStream stream = tcp.GetStream();
                StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                StreamWriter writer = new StreamWriter(stream, Encoding.UTF8);
                writer.NewLine = "\n";
                

                string deviceID = reader.ReadLine().Trim();
                NodeClient client = getClient(deviceID);
                bool connected = true;
                Console.WriteLine(string.Format("Device connected: '{0}'", deviceID));

                while (connected) // loop
                {
                    // receive a command
                    string command = reader.ReadLine().Trim();

                    if (command == "message")
                    {
                        Message m = new Message();
                        m.Sender = this.ClientID + "/" + deviceID;
                        m.Recipient = reader.ReadLine().Trim();
                        m.Content = Encoding.UTF8.GetBytes(reader.ReadLine().Trim());

                        this.BrokerClient.SendMessage(m);
                    }
                    else if (command == "disconnect") // close session
                    {
                        tcp.Close();
                        tcp.Dispose();
                        return;
                    }
                    else // i.e. "peek"
                    {
                        // no action required
                        // all other commands will result in sending pending messages to the client.
                    }

                    List<Message> messages = client.GetMessages();
                    writer.WriteLine(messages.Count.ToString());
                    foreach (Message m in messages)
                    {
                        writer.WriteLine(m.Sender);
                        writer.WriteLine(Encoding.UTF8.GetString(m.Content));
                    }
                    writer.Flush();
                }

                tcp.Close();
                tcp.Dispose();
            }
            catch { }
        }

        public void Stop()
        {
            if (!running)
                return;
            running = false;
            this.TcpListener.Stop();
        }


        public void Dispose()
        {
            if (running)
                this.Stop();
        }
    }
}
