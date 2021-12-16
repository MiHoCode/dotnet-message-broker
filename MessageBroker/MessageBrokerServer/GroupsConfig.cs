using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace MessageBrokerServer
{
    public class GroupsConfig
    {
        public static GroupsConfig Instance { get; private set; }
        public static string Filename { get; private set; }

        public static void Init()
        {
            Filename = Path.Combine(Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName), "groups.cfg");
            if (!File.Exists(Filename))
            {
                Instance = new GroupsConfig();

                Group admins = new Group() { Name = "admins" };
                admins.Members.Add("admin");

                Instance.Groups.Add(admins);
                Instance.Save();

            }
            else
                Instance = GroupsConfig.FromJson(File.ReadAllText(Filename));
        }

        public List<Group> Groups { get; set; } = new List<Group>();
        public string ToJson()
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
            };
            return JsonSerializer.Serialize(this, options);
        }
        public override string ToString()
        {
            return this.ToJson();
        }

        public static GroupsConfig FromJson(string json)
        {
            return JsonSerializer.Deserialize<GroupsConfig>(json);
        }

        public void Save()
        {
            File.WriteAllText(Filename, this.ToJson());
        }
    }

}
