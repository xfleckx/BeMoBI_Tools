using System;
using System.ComponentModel; 
using System.Windows.Forms;

namespace BatteryMonitor
{
    public partial class Main : Form
    {
        private const string STREAM_TYPE = "BatteryStatusInPercentage";

        private const string DEFAULT_STREAM_NAME = "BatteryStatus";

        private const int DEFAULT_DATARATE = 30000; // 30 seconds

        private int dataRate = 0;

        private int minUpdateIntervall = 10;

        LSL.liblsl.StreamOutlet outlet;

        private BatteryObserver batteryObserver;

        public Main()
        {
            InitializeComponent();

            button2.Enabled = false;

            textBoxStreamName.Text = DEFAULT_STREAM_NAME;
            textBox2.Text = DEFAULT_DATARATE.ToString();
        }

        private void updateIntervalltextBox_Validating(object sender, CancelEventArgs e)
        {
            var tb = sender as TextBox;

            int updateIntervall;

            if (int.TryParse(tb.Text, out updateIntervall) && updateIntervall > minUpdateIntervall) {
                
                    e.Cancel = false;
                    dataRate = updateIntervall;
            }
            else
            {
                e.Cancel = true;
                this.button1.Enabled = false;
                errorProvider1.SetError(tb, "No number or to small - value must be > 10");
            }
        }

        private void updateIntervalltextBox_Validated(object sender, EventArgs e)
        {
            errorProvider1.Clear();
            this.button1.Enabled = true;
        }

        private void streamNameTextBox_Validating(object sender, CancelEventArgs e)
        {
            var tb = sender as TextBox;

            if (tb.Text == "")
            {
                e.Cancel = true;
                errorProvider1.SetError(tb, "Stream Name must not be Empty");

                return;
            }

            e.Cancel = false;
            errorProvider1.Clear();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBoxStreamName.Enabled = false;
            textBox2.Enabled = false;
            button2.Enabled = true;
            button1.Enabled = false;

            if (dataRate == 0)
                dataRate = DEFAULT_DATARATE;

            var streamInfo = new LSL.liblsl.StreamInfo(textBoxStreamName.Text, STREAM_TYPE, 1, dataRate, LSL.liblsl.channel_format_t.cf_int32);

            outlet = new LSL.liblsl.StreamOutlet(streamInfo);

            batteryObserver = new BatteryMonitor.BatteryObserver(dataRate, createSampleFromObserverUpdate);

            batteryObserver.onNewStatusAvailable += BatteryObserver_onNewStatusAvailable;

            batteryObserver.Initialize();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBoxStreamName.Enabled = true;
            textBox2.Enabled = true;
            button1.Enabled = true;
            button2.Enabled = false;

            batteryObserver.Shutdown();
            outlet = null;
            GC.Collect();
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

    }
}
