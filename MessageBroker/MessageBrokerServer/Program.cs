using System;
using System.Linq;
using System.Text;

namespace MessageBrokerServer
{
    class Program
    {
        static void Main(string[] args)
        {
            // this block allows the admin to change configurations instead of running the application.
            if (args.Length >= 1)
            {
                if (args[0] == "addclient")
                {
                    string clientID = args[1];
                    if (!string.IsNullOrWhiteSpace(clientID))
                    {
                        KeyStore.Init();
                        KeyStore.GetEncryptionKey(clientID);
                        Console.WriteLine(string.Format("Encryption key generated for client '{0}'", clientID));
                    }
                }
                else if (args[0] == "addgroup")
                {
                    string groupID = args[1];
                    if (!string.IsNullOrWhiteSpace(groupID))
                    {
                        GroupsConfig.Init();
                        Group group = GroupsConfig.Instance.Groups.Where(Key => Key.Name == groupID).FirstOrDefault();
                        if (group == null)
                        {
                            group = new Group() { Name = groupID };
                            GroupsConfig.Instance.Groups.Add(group);
                            GroupsConfig.Instance.Save();
                            Console.WriteLine(string.Format("new group: {0}", groupID));
                        }
                    }
                }
                else if (args[0] == "removegroup")
                {
                    string groupID = args[1];
                    if (!string.IsNullOrWhiteSpace(groupID))
                    {
                        GroupsConfig.Init();
                        GroupsConfig.Instance.Groups.RemoveAll(Key => Key.Name == groupID);
                        GroupsConfig.Instance.Save();
                        Console.WriteLine(string.Format("group removed: {0}", groupID));
                    }
                }
                else if (args[0] == "addgroupmember")
                {
                    string groupID = args[1];
                    string memberID = args[2];
                    if (!string.IsNullOrWhiteSpace(groupID) && !string.IsNullOrWhiteSpace(memberID))
                    {
                        GroupsConfig.Init();
                        Group group = GroupsConfig.Instance.Groups.Where(Key => Key.Name == groupID).FirstOrDefault();
                        if (group != null)
                        {
                            if (!group.Members.Contains(memberID))
                                group.Members.Add(memberID);
                            KeyStore.GetEncryptionKey(memberID);
                            GroupsConfig.Instance.Save();
                            Console.WriteLine(string.Format("added '{0}' to group '{1}'", memberID, groupID));
                        }
                    }
                }
                else if (args[0] == "removegroupmember")
                {
                    string groupID = args[1];
                    string memberID = args[2];
                    if (!string.IsNullOrWhiteSpace(groupID) && !string.IsNullOrWhiteSpace(memberID))
                    {
                        GroupsConfig.Init();
                        Group group = GroupsConfig.Instance.Groups.Where(Key => Key.Name == groupID).FirstOrDefault();
                        if (group != null)
                        {
                            group.Members.RemoveAll(Key => Key == memberID);
                            GroupsConfig.Instance.Save();
                            Console.WriteLine(string.Format("removed '{0}' from group '{1}'", memberID, groupID));
                        }
                    }
                }
                else if (args[0] == "help")
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
                    sb.AppendLine();
                    Console.WriteLine(sb.ToString());
                }
                return;
            }

            // if no arguments used, start the application!

            try
            {
                using (MessageBroker server = new MessageBroker())
                {
                    server.Run();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Message Broker crashed with ERROR:");
                Console.WriteLine(ex.ToString());
            }

            Console.WriteLine("Message Broker stopped.");
        }
    }
}
