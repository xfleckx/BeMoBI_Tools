using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BatteryMonitor
{
    public partial class Form1 : Form
    {
        private const string STREAM_TYPE = "BatteryStatus";

        private const int DEFAULT_DATARATE = 30000; // 30 seconds

        private int dataRate = 0;

        LSL.liblsl.StreamOutlet outlet;

        private RemoteBatteryMonitor.BatteryObserver batteryObserver;

        public Form1()
        {
            InitializeComponent();

            button2.Enabled = false;
        }

        private void textBox2_Validating(object sender, CancelEventArgs e)
        {
            var tb = sender as TextBox;

            int updateIntervall;

            if (int.TryParse(tb.Text, out updateIntervall)) {
                e.Cancel = false;

                if(updateIntervall > 10) { //Update rates higher than 10 ms could cause issues
                    dataRate = updateIntervall;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Enabled = false;
            textBox2.Enabled = false;
            button2.Enabled = true;
            button1.Enabled = false;

            if (dataRate == 0)
                dataRate = DEFAULT_DATARATE;

            var streamInfo = new LSL.liblsl.StreamInfo(textBox1.Text, STREAM_TYPE, 1, dataRate, LSL.liblsl.channel_format_t.cf_int32);

            outlet = new LSL.liblsl.StreamOutlet(streamInfo);

            batteryObserver = new RemoteBatteryMonitor.BatteryObserver(dataRate, createSampleFromObserverUpdate);

            batteryObserver.onNewStatusAvailable += BatteryObserver_onNewStatusAvailable;

            batteryObserver.Initialize();
        }

        private void BatteryObserver_onNewStatusAvailable(int batteryStatus)
        {
            if (InvokeRequired)
                Invoke(new Action(() => {

                    progressBar1.Value = batteryStatus;

                }));
        }

        private void createSampleFromObserverUpdate(int percentageOfBatteryPower)
        {
            if(outlet.have_consumers())
                outlet.push_sample(new int[] { percentageOfBatteryPower });
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox1.Enabled = true;
            textBox2.Enabled = true;
            button1.Enabled = true;
            button2.Enabled = false;

            batteryObserver.Shutdown();
            outlet = null;
            GC.Collect();
        }
    }
}
