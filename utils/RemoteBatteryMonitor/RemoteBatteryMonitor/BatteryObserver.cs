using System; 
using System.Net.Sockets; 
using System.Windows.Forms;

namespace RemoteBatteryMonitor
{
    class BatteryObserver
    {
        private int remotePort;
        private string targetHostName;
       
        private System.Threading.Timer updateExecutor;
        private Action<int> onNewStatusAvailable;
        private int updateIntervall;

        TcpClient client;

        public BatteryObserver(string targetHostName, int remotePort, int updateIntervall, Action<int> onNewStatusAvailable)
        {
            this.targetHostName = targetHostName;

            this.remotePort = remotePort;

            this.updateIntervall = updateIntervall;

            this.onNewStatusAvailable = onNewStatusAvailable;
        }

        public void Initialize(object state = null)
        {
            try
            {
                client = new TcpClient(targetHostName, remotePort);
            }
            catch (Exception)
            {
                if(updateExecutor == null) {
                    Console.Error.WriteLine("Connection attemp failed - retry...");
                    updateExecutor = new System.Threading.Timer(Initialize, state, 0, updateIntervall * 1000 * 60);
                }
            }

            if (updateExecutor != null)
                updateExecutor.Dispose();

            Console.Error.WriteLine("Connection attemp succeed - Lookup battery state...");
            updateExecutor = new System.Threading.Timer(LookUpBatteryState, state, 0, updateIntervall * 1000 * 60);
        }

        private void LookUpBatteryState(object state)
        {
            PowerStatus p = SystemInformation.PowerStatus;

            int status = (int)(p.BatteryLifePercent * 100);

            onNewStatusAvailable(status);

            publishStatus(status);
        }

        public void publishStatus(int percentage)
        {
            var message = string.Format("Battery: {0}%", percentage);

            Byte[] data = System.Text.Encoding.ASCII.GetBytes(message);
            
            NetworkStream stream = client.GetStream();
            
            stream.Write(data, 0, data.Length);

            Console.WriteLine("Sent: {0}", message);
        }

    }
}
