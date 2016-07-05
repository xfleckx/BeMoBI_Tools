using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

using CommandLine;



namespace RemoteBatteryMonitor
{
    class Program
    {

        static string invokedVerb;
        object invokedVerbInstance;

        private static bool canceled = false;

        static void Main(string[] args)
        {

            var options = new Options();
            if (!CommandLine.Parser.Default.ParseArguments(args, options,
              (verb, subOptions) =>
              {
      // if parsing succeeds the verb name and correct instance
      // will be passed to onVerbCommand delegate (string,object)
                    invokedVerb = verb;
                  invokedVerbInstance = subOptions;
              }))
            {
                Environment.Exit(CommandLine.Parser.DefaultExitCodeFail);
            }

            if (invokedVerb == "commit")
            {
                var commitSubOptions = (CommitSubOptions)invokedVerbInstance;
            }
            Thread.CurrentThread.Name = "ConsoleUI";

            // Create a task and supply a user delegate by using a lambda expression. 
            Task backgroundTask = null;

            var cancelToken = new CancellationToken();

            if (options.runAsClient)
            {
                var listener = new StatusListener(options.LocalPort);
                backgroundTask = new Task(new Action(listener.ListenForStatusMessages), cancelToken, TaskCreationOptions.LongRunning);
            }

            if(options.runAsObserver)
            {
                var observer = new BatteryObserver(options.TargetHostName, options.RemotePort, options.updateIntervall, status => Console.WriteLine(status));
            }


            if (backgroundTask != null)
                backgroundTask.Start();
            else
            {
                canceled = false;
                Console.WriteLine("No valid option provided... ending...");
            }
            
            while (!canceled)
            {
                Console.WriteLine("Await input");

                var input = Console.ReadKey();
                
                if(input.Key == ConsoleKey.Escape)
                {
                    canceled = false;
                }
            }

        }
        
    }
}
