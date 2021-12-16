using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace MessageBrokerServer
{
    public class Group
    {
        public string Name { get; set; }
        public List<string> Members { get; set; } = new List<string>();
    }
}
