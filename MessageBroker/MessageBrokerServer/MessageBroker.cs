using System;
using System.Collections.Generic;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Linq;
using System.IO;


namespace MessageBrokerServer
{
    public class MessageBroker : IDisposable
    {
        public const int DEFAULT_PORT = 10242;

        public int Port { get; private set; }
        public TcpListener TcpListener { get; private set; }

        private byte[] brokerKey;
        private bool running;

        /// <summary>
        /// A cache for all client objects used since application startup.
        /// </summary>
        private Dictionary<string, BrokerClient> clients = new Dictionary<string, BrokerClient>();

        /// <summary>
        /// Gets a client object from cache or creates and adds it if not present. Threadsafe!
        /// </summary>
        private BrokerClient getClient(string clientID)
        {
            lock (clients)
            {
                if (!clients.ContainsKey(clientID))
                { 
                    BrokerClient client = new BrokerClient(clientID);
                    clients.Add(clientID, client);
                }

                return clients[clientID];
            }
        }

        public MessageBroker() : this(DEFAULT_PORT) { }

        public MessageBroker(int port)
        {
            this.Port = port;
            this.TcpListener = new TcpListener(IPAddress.Any, this.Port);
            KeyStore.Init();
            brokerKey = KeyStore.GetEncryptionKey("broker");
            GroupsConfig.Init();
        }

        public void Run()
        {
            running = true;
            this.TcpListener.Start();
            Console.WriteLine("Message Broker running.");
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
                BigEndianStream stream = new BigEndianStream(tcp.GetStream());

                byte[] brokerIV = KeyStore.GenerateIV(); // generate a iv for this session
                string auth = Guid.NewGuid().ToString(); // generate a guid for the auth process

                stream.WriteMiniBuffer(brokerIV); // send broker iv
                stream.WriteEncrypt(Encoding.UTF8.GetBytes(auth), brokerKey, brokerIV); // send encrypted auth guid

                string authAnswer = Encoding.UTF8.GetString(stream.ReadMiniBuffer()); // receive decrypted auth guid
                if (authAnswer != auth) // check if valid!
                {
                    Console.WriteLine(string.Format("invalid auth: [{0}] / [{1}]", auth, authAnswer));
                    tcp.Close();
                    tcp.Dispose();
                }

                string clientID = Encoding.UTF8.GetString(stream.ReadDecrypt(brokerKey, brokerIV)); // receive client ID

                BrokerClient client = getClient(clientID); // get/create/cache client object
                stream.WriteEncrypt(client.ClientIV, brokerKey, brokerIV); // send encrypted client iv for this session

                // all following encryptions/decryptions use the client iv and the client key!

                auth = Guid.NewGuid().ToString(); // generate a guid for the second auth process
                stream.WriteEncrypt(Encoding.UTF8.GetBytes(auth), client.ClientKey, client.ClientIV); // send encrypted auth guid
                authAnswer = Encoding.UTF8.GetString(stream.ReadMiniBuffer()); // receive decrypted auth guid
                if (authAnswer != auth) // check if valid!
                {
                    Console.WriteLine(string.Format("invalid auth (2): [{0}] / [{1}]", auth, authAnswer));
                    tcp.Close();
                    tcp.Dispose();
                }

                bool connected = true;

                while (connected) // loop
                {
                    // receive a command
                    string command = Encoding.UTF8.GetString(stream.ReadDecrypt(client.ClientKey, client.ClientIV));

                    if(command == "message") // incoming message
                    {
                        // read and decrypt the message
                        byte[] message = stream.ReadDecrypt(client.ClientKey, client.ClientIV);
                        if (clientID == "admin")
                        {
                            processAdminMessage(Message.FromByteArray(message));
                        }
                        else
                        {
                            string receiver = Message.ExtractRecipient(message);
                            forwardMessage(receiver, message);
                        }
                    }
                    else if(command == "disconnect") // close session
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

                    List<byte[]> messages = client.GetMessages();
                    stream.WriteInt16((short)messages.Count);
                    foreach(byte[] m in messages)
                    {
                        stream.WriteEncrypt(m, client.ClientKey, client.ClientIV);
                    }
                }

                tcp.Close();
                tcp.Dispose();
            }
            catch { }
        }

        private void processAdminMessage(Message message)
        {
            try
            {
                if (message.Recipient == "server" || message.Recipient == "system" || message.Recipient == "broker")
                {
                    string[] args = Encoding.UTF8.GetString(message.Content).Split(' ');
                    string cmd = args[0];

                    Message response = new Message();
                    response.Sender = "admin";
                    response.Recipient = "admin";
                    response.ID = Guid.NewGuid().ToString();
                    response.IsResponseOf = "";
                    response.Content = Encoding.UTF8.GetBytes("done.");

                    if (cmd == "addclient")
                    {
                        string clientID = args[1];
                        if (!string.IsNullOrWhiteSpace(clientID))
                        {
                            string key = Convert.ToBase64String(KeyStore.GetEncryptionKey(clientID));
                            response.Content = Encoding.UTF8.GetBytes(string.Format("{0}.key: {1}", clientID, key));
                        }
                    }
                    else if (cmd == "addgroup")
                    {
                        string groupID = args[1];
                        if (!string.IsNullOrWhiteSpace(groupID))
                        {
                            Group group = GroupsConfig.Instance.Groups.Where(Key => Key.Name == groupID).FirstOrDefault();
                            if(group == null)
                            {
                                group = new Group() { Name = groupID };
                                GroupsConfig.Instance.Groups.Add(group);
                                GroupsConfig.Instance.Save();
                                response.Content = Encoding.UTF8.GetBytes(string.Format("new group: {0}", groupID));
                            }
                        }
                    }
                    else if (cmd == "removegroup")
                    {
                        string groupID = args[1];
                        if (!string.IsNullOrWhiteSpace(groupID))
                        {
                            GroupsConfig.Instance.Groups.RemoveAll(Key => Key.Name == groupID);
                            GroupsConfig.Instance.Save();
                            response.Content = Encoding.UTF8.GetBytes(string.Format("group removed: {0}", groupID));
                        }
                    }
                    else if (cmd == "addgroupmember")
                    {
                        string groupID = args[1];
                        string memberID = args[2];
                        if (!string.IsNullOrWhiteSpace(groupID) && !string.IsNullOrWhiteSpace(memberID))
                        {
                            Group group = GroupsConfig.Instance.Groups.Where(Key => Key.Name == groupID).FirstOrDefault();
                            if (group != null)
                            {
                                if (!group.Members.Contains(memberID))
                                    group.Members.Add(memberID);
                                KeyStore.GetEncryptionKey(memberID);
                                GroupsConfig.Instance.Save();
                                response.Content = Encoding.UTF8.GetBytes(string.Format("added '{0}' to group '{1}'", memberID, groupID));
                            }
                        }
                    }
                    else if (cmd == "removegroupmember")
                    {
                        string groupID = args[1];
                        string memberID = args[2];
                        if (!string.IsNullOrWhiteSpace(groupID) && !string.IsNullOrWhiteSpace(memberID))
                        {
                            Group group = GroupsConfig.Instance.Groups.Where(Key => Key.Name == groupID).FirstOrDefault();
                            if (group != null)
                            {
                                group.Members.RemoveAll(Key => Key == memberID);
                                GroupsConfig.Instance.Save();
                                response.Content = Encoding.UTF8.GetBytes(string.Format("removed '{0}' from group '{1}'", memberID, groupID));
                            }
                        }
                    }
                    else if (cmd == "help")
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine("AVAILABLE ADMIN COMMANDS:");
                        sb.AppendLine();
                        sb.AppendLine("addclient [clientID]");
                        sb.AppendLine("addgroup [groupID]");
                        sb.AppendLine("removegroup [groupID]");
                        sb.AppendLine("addgroupmember [groupID] [clientID]");
                        sb.AppendLine("removegroupmember [groupID] [clientID]");
                        sb.AppendLine("help");
                        response.Content = Encoding.UTF8.GetBytes(sb.ToString());
                    }
                    else
                    {
                        response.Content = Encoding.UTF8.GetBytes(string.Format("unknown admin command: {0}", cmd));
                    }

                    forwardMessage("admin", response.ToByteArray());
                }
                else
                {
                    forwardMessage(message.Recipient, message.ToByteArray());
                }
            }
            catch (Exception ex)
            {
                Message response = new Message();
                response.Sender = "admin";
                response.Recipient = "admin";
                response.ID = Guid.NewGuid().ToString();
                response.IsResponseOf = "";
                response.Content = Encoding.UTF8.GetBytes(string.Format("ERROR: {0}", ex.Message));
                forwardMessage("admin", response.ToByteArray());
            }
        }

        private void forwardMessage(string receiver, byte[] message)
        {
            if (receiver.StartsWith("group:"))
            {
                string groupID = receiver.Substring("group:".Length).Trim();
                Group group = GroupsConfig.Instance.Groups.Where(Key => Key.Name == groupID).FirstOrDefault();
                if(group != null)
                {
                    foreach(string member in group.Members)
                    {
                        getClient(member).AddMessage(message);
                    }
                }
            }
            else
            {
                getClient(receiver.Split('/')[0]).AddMessage(message);
            }
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
