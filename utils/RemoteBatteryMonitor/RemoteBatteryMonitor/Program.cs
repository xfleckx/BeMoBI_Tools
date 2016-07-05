using System;
using System.Threading;

namespace RemoteBatteryMonitor
{
    class Program
    {
        private static bool canceled = false;

        static void Main(string[] args)
        {
            var observer = new BatteryObserver(3000, status => Console.WriteLine(status));

            while (!canceled)
            {
                Console.WriteLine("Await input");

                var input = Console.ReadKey();
                
                if(input.Key == ConsoleKey.Escape)
                {
                    observer.Shutdown();

                    canceled = false;
                }
            }

        }
        
    }
}
