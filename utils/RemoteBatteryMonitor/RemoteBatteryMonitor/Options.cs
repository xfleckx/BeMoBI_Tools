using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CommandLine;
using CommandLine.Text;

namespace RemoteBatteryMonitor
{
    
    public class Options
    {
        public Options()
        {
            // Since we create this instance the parser will not overwrite it
            ClientVerb = new ClientOptions();
        }

        [VerbOption("client", HelpText = "Record changes to the repository.")]
        public ClientOptions ClientVerb { get; set; }

        [VerbOption("server", HelpText = "Update remote refs along with associated objects.")]
        public ObserverOptions AddVerb { get; set; }

        [Option('r', "remotePort", DefaultValue = 18969,
          HelpText = "remote Port")]
        public int RemotePort { get; set; }

        [Option('u', "updateIntervall", DefaultValue = 2,
          HelpText = "Intervall to look up the battery state (min)")]
        public int updateIntervall { get; set; }

        [Option('c', "client",
          HelpText = "run as a client - awaits status informations",
            MutuallyExclusiveSet = "client")]
        public bool runAsClient { get; set; }

        [Option('o', "observer",
          HelpText = "run as a status observer - provides status informations to clients",
            MutuallyExclusiveSet = "observer")]
        public bool runAsObserver { get; set; }
        
        [Option('h', "Hostname", Required = true, MutuallyExclusiveSet = "observer",
          HelpText = "Target Hostname")]
        public string TargetHostName { get; set; }

        [ParserState]
        public IParserState LastParserState { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this,
              (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }
}
