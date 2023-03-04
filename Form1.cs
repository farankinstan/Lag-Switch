using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;



namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        [DllImport("User32.dll")]
        private static extern bool GetAsyncKeyState(ushort vKey);


        public Form1()
        {
            InitializeComponent();
            //L_hotkey.Text = VK.VirtualKeys.CapsLock.ToString();
            comboBox2.DataSource = Enum.GetValues(typeof(VK.VirtualKeys));
            timer1.Enabled= false;
        }


        bool active = false;
        string gamepath = "";
        ushort hotkey;
        int duration = -1;

        ProcessStartInfo AddRuleIn;
        ProcessStartInfo AddRuleOut;
        ProcessStartInfo DeleteRule;

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (checkBox1.Checked) { 
                duration = int.Parse(comboBox1.Text);
                }
            }
            catch { 
                checkBox1.Checked = false;
                comboBox1.Enabled = false;
                comboBox1.Text = "5";
                duration = -1;

                MessageBox.Show("ON\\OFF mode turned on", "No duration", MessageBoxButtons.OK);
            }

            hotkey = (ushort)Enum.Parse(typeof(VK.VirtualKeys), comboBox2.SelectedItem.ToString());

            if(gamepath.Length == 0)
            {
                MessageBox.Show("Choose the path of you'r game", "No path", MessageBoxButtons.OK);
                return;
            }

            AddRuleIn = new ProcessStartInfo("cmd.exe", "/c netsh advfirewall firewall add rule name=\"UCLagSwitch\" dir=in action=block program=\"" + gamepath + "\" enable=yes");
            AddRuleOut = new ProcessStartInfo("cmd.exe", "/c netsh advfirewall firewall add rule name=\"UCLagSwitch\" dir=out action=block program=\"" + gamepath + "\" enable=yes");
            DeleteRule = new ProcessStartInfo("cmd.exe", "/c netsh advfirewall firewall delete rule name=\"UCLagSwitch\" program=\"" + gamepath + "\"");

            AddRuleIn.WindowStyle = ProcessWindowStyle.Hidden;
            AddRuleOut.WindowStyle = ProcessWindowStyle.Hidden;
            DeleteRule.WindowStyle = ProcessWindowStyle.Hidden;

            timer1.Enabled = true;
            MessageBox.Show("Enjoy you'r lag, sir", "Saved!", MessageBoxButtons.OK);
        }

        void OnOff()
        {
            if (active) {
                L_state.Text = "ON";
                L_state.ForeColor = Color.Green;
            }

            else
            {
                L_state.Text = "OFF";
                L_state.ForeColor = Color.Red;
            }
        }

        private async void timer1_Tick(object sender, EventArgs e)
        {
            if (GetAsyncKeyState(hotkey) && duration > 0)
            {
                Process.Start(AddRuleIn);
                Process.Start(AddRuleOut);
                Console.Beep();
                active = true;
                OnOff();

                await Task.Delay(duration * 1000);

                Console.Beep();
                active = false;
                OnOff();
                Process.Start(DeleteRule);
            }

            else if(GetAsyncKeyState(hotkey))
            {
                active = !active;

                if (active)
                {
                    OnOff();
                    Process.Start(AddRuleIn);
                    Process.Start(AddRuleOut);
                    Console.Beep();
                }

                else
                {
                    OnOff();
                    Console.Beep();
                    Process.Start(DeleteRule);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog selectExecutable = new OpenFileDialog())
            {
                //selectExecutable.Filter = ""; 

                if (selectExecutable.ShowDialog() == DialogResult.OK)
                {
                    gamepath = L_path.Text = selectExecutable.FileName;
                }

            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (!checkBox1.Checked)
            {
                duration = -1;
            }

            comboBox1.Enabled = checkBox1.Checked;
        }
    }
}
