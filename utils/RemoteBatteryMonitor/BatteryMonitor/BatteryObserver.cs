using System;
using System.Windows.Forms;

namespace BatteryMonitor
{
    public class BatteryObserver
    {
        private System.Threading.Timer updateExecutor;
        public event Action<int> onNewStatusAvailable;
        private int updateIntervall;

        public BatteryObserver(int updateIntervallInMilliseconds, Action<int> onNewStatusAvailable)
        {
            this.updateIntervall = updateIntervallInMilliseconds;

            this.onNewStatusAvailable = onNewStatusAvailable;
        }

        public void Initialize(object state = null)
        {
            if (updateExecutor != null)
                updateExecutor.Dispose();
            
            updateExecutor = new System.Threading.Timer(LookUpBatteryState, state, 0, updateIntervall);
        }

        private void LookUpBatteryState(object state)
        {
            PowerStatus p = SystemInformation.PowerStatus;

            int status = (int)(p.BatteryLifePercent * 100);

            onNewStatusAvailable(status);
        }

        public void Shutdown()
        {
            updateExecutor.Dispose();
        }
    }
}
