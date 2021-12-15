using System;

namespace MessageBrokerServer
{
    class Program
    {
        static void Main(string[] args)
        {
            // this block allows the admin to create client keys on the server instead of running the application.
            if (args.Length >= 2 && args[0] == "addclient")
            {
                string clientID = args[1];
                if (!string.IsNullOrWhiteSpace(clientID))
                {
                    KeyStore.Init();
                    KeyStore.GetEncryptionKey(clientID);
                    Console.WriteLine(string.Format("Encryption key generated for client '{0}'", clientID));
                }
                return;
            }

            // if no commands used, start the application!

            using (MessageBroker server = new MessageBroker())
            {
                server.Run();
            }

            Console.WriteLine("Message Broker stopped.");
        }
    }
}
