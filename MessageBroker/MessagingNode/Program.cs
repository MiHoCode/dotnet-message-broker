using System;
using System.IO;
using System.Xml;

namespace MessagingNode
{
    class Program
    {
        static void Main(string[] args)
        {
            string filename = Path.Combine(Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName), "nodecfg.xml");

            if (!File.Exists(filename))
            {
                Console.WriteLine("Config file missing: " + filename);
                return;
            }

            XmlDocument cfg = new XmlDocument();
            cfg.Load(filename);

            string host = cfg.GetElementsByTagName("host")[0].InnerText;
            string clientId = cfg.GetElementsByTagName("clientId")[0].InnerText;
            string serverKey = cfg.GetElementsByTagName("serverKey")[0].InnerText;
            string clientKey = cfg.GetElementsByTagName("clientKey")[0].InnerText;

            try
            {
                using (MessagingNodeService node = new MessagingNodeService(host, clientId, serverKey, clientKey))
                {
                    node.Run();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("MessagingNode crashed with ERROR:");
                Console.WriteLine(ex.ToString());
            }

            Console.WriteLine("MessagingNode stopped.");
        }
    }
}
