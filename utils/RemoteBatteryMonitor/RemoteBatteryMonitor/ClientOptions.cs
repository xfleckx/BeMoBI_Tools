using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RemoteBatteryMonitor
{
    public class ClientOptions
    {

        [Option('p', "port", DefaultValue = 18980,
          HelpText = "Local listening Port")]
        public int LocalPort { get; set; }

    }
}
