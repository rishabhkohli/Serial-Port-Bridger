using System;
using System.IO;
using System.IO.Ports;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{

    public partial class Form1 : Form
    {
        public delegate void RecieveData(string update);
        public RecieveData recieve, recieve2;
        int savelog1 = 0, savelog2 = 0;
        int bridge1 = 0, bridge2 = 0;
        StreamWriter log1, log2;
        string[] defaults = new string[50];
        string[] defaults2 = new string[50];
        public Form1()
        {
            InitializeComponent();
            getAvailablePorts();
            comboBox2.SelectedIndex = 12;
            comboBox3.SelectedIndex = 12;
            serialPort1.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
            serialPort2.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler2);
            recieve = new RecieveData(DataRecieve);
            recieve2 = new RecieveData(DataRecieve2);
            string appPath = Path.GetDirectoryName(Application.ExecutablePath);
            if (File.Exists(appPath + "\\conf.ini"))
            {
                PopulateCommands(appPath + "\\conf.ini");
                PopulateCommands2(appPath + "\\conf.ini");
            }
        }

        //Recieve Data Function
        public void DataRecieve(string update)
        {
            textBox1.AppendText(update);
        }
        public void DataRecieve2(string update)
        {
            textBox6.AppendText(update);
        }

        //Print data recieved on port 1
        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            string input = serialPort1.ReadExisting();
            if (checkBox1.Checked == true || checkBox3.Checked == true) if (serialPort2.IsOpen)
                {
                    bridge2 = 1;
                    senddata2(input);
                }
            if (checkBox8.Checked == true) input = input.Replace("\r", "\r\n");
            if (savelog1 == 1)
            {
                log1.Write(input);
            }
            textBox1.Invoke(this.recieve, new Object[] { input});
        }
        private void DataReceivedHandler2(object sender, SerialDataReceivedEventArgs e)
        {
            string input = serialPort2.ReadExisting();
            if (checkBox1.Checked == true || checkBox2.Checked == true) if (serialPort1.IsOpen)
                {
                    bridge1 = 1;
                    senddata(input);
                }
            if (checkBox9.Checked == true) input = input.Replace("\r", "\r\n");
            if (savelog2 == 1)
            {
                log2.Write(input);
            }
            textBox6.Invoke(this.recieve2, new Object[] { input});
        }


        //Open Port 
        private void button3_Click(object sender, EventArgs e)
        {
            if (comboBox1.Text != "") serialPort1.PortName = comboBox1.Text;
            serialPort1.BaudRate = Convert.ToInt32(comboBox2.Text);
            serialPort1.DataBits = 8;
            try
            {
                serialPort1.Open();
            }
            catch (System.IO.IOException)
            {
                label1.Text = "Can't find port!";
            }
            catch (System.UnauthorizedAccessException)
            {
                label1.Text = "Port in use";
            }
            catch
            {
                label1.Text = "Error Occurred";
            }
            if (serialPort1.IsOpen)
            {
                textBox7.Enabled = true;
                button3.Enabled = false;
                button13.Enabled = true;
                label7.Enabled = true;
                textBox5.Enabled = true;
                button6.Enabled = true;
                listBox1.Enabled = true;
                textBox2.Enabled = true;
                button15.Enabled = true;
                button1.Enabled = true;
                checkBox4.Enabled = true;
                checkBox5.Enabled = true;
                label1.Text = "Port Open: " + comboBox1.Text;
            }                
        }

        //Clear
        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
        }
        
        //Function
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem.ToString() == "TIME")
            {
                label7.Visible = true;
                textBox5.Visible = true;
                label7.Text = listBox1.SelectedItem.ToString();
            }
            else if (defaults[listBox1.SelectedIndex] != null && defaults[listBox1.SelectedIndex] != "")
            {
                label7.Visible = true;
                textBox5.Visible = true;
                label7.Text = listBox1.SelectedItem.ToString();
                textBox5.Text = defaults[listBox1.SelectedIndex];
            }
            else if (defaults[listBox1.SelectedIndex] == null || defaults[listBox1.SelectedIndex] == "")
            {
                label7.Visible = false;
                textBox5.Visible = false;
            }
        }

        //Ennumerate ports
        private void getAvailablePorts()
        {
            string[] Ports = SerialPort.GetPortNames();            
            if (Array.Exists(Ports, element => element.StartsWith("C")))
            {
                comboBox1.Items.AddRange(Ports);
                comboBox4.Items.AddRange(Ports);
                comboBox1.SelectedIndex = 0;
                comboBox4.SelectedIndex = 0;
                if (!serialPort1.IsOpen)
                {
                    button3.Enabled = true;
                    button3.Text = "Open Port";
                }
                else comboBox1.SelectedIndex = comboBox1.FindStringExact(label1.Text.Substring(label1.Text.IndexOf("COM")));
                if (!serialPort2.IsOpen)
                {
                    button11.Enabled = true;
                    button11.Text = "Open Port";
                }
                else comboBox4.SelectedIndex = comboBox4.FindStringExact(label12.Text.Substring(label12.Text.IndexOf("COM")));
            }
            else
            {
                button3.Enabled = false;
                button3.Text = "No Ports Exist";

                button11.Enabled = false;
                button11.Text = "No Ports Exist";
            }
        }
       
        private void senddata(string data)
        {
            if (serialPort1.IsOpen)
            {
                if (bridge1 == 1)
                {
                    bridge1 = 0;
                }
                else
                {
                    if (checkBox4.Checked == true)
                        data = data + "\r";
                    if (checkBox5.Checked == true)
                        data = data + "\n";
                }
                try
                {
                    serialPort1.Write(data);
                }
                catch
                {
                    label1.Text = "Error Occurred";
                }
                data = data.Replace("\r", "<CR>");
                data = data.Replace("\n", "<LF>");
                textBox2.AppendText(data + "\r\n");
                if (savelog1 == 1)
                {
                    log1.Flush();
                    log1.Write(data);                    
                }
            }
            else
            {
                Form2 subForm = new Form2();
                subForm.Show();
            }
        }

        private void senddata2(string data)
        {
            if (serialPort2.IsOpen)
            {
                if (bridge2 == 1)
                {
                    bridge2 = 0;
                }
                else
                {
                    if (checkBox6.Checked == true)
                        data = data + "\r";
                    if (checkBox7.Checked == true)
                        data = data + "\n";
                }
                try
                {
                    serialPort2.Write(data);
                }
                catch
                {
                    label12.Text = "Error Occurred";
                }
                data = data.Replace("\r", "<CR>");
                data = data.Replace("\n", "<LF>");
                textBox4.AppendText(data + "\r\n");
                if (savelog2 == 1)
                {
                    log2.Flush();
                    log2.Write(data);
                }
            }
            else
            {
                Form2 subForm = new Form2();
                subForm.Show();
            }
        }

        //Clear Textbox
        private void button5_Click(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();
            comboBox4.Items.Clear();
            getAvailablePorts();
        }

        //Open Config File
        private void button6_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "Configuration Files|*.ini";
            openFileDialog1.FilterIndex = 1;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                PopulateCommands(openFileDialog1.FileName);
            }
        }

        //Populate Commands
        private void PopulateCommands(string filepath)
        {
            listBox1.Items.Clear();
            string conf = filepath;
            string[] filedata = File.ReadAllLines(conf);
            for (int i = 0; i < filedata.Length; i++)
            {
                int startindex = filedata[i].IndexOf("=");
                if (startindex > 0)
                {
                    listBox1.Items.Add(filedata[i].Substring(0, startindex + 1));
                    defaults[i] = filedata[i].Substring(startindex + 1, filedata[i].Length - (startindex + 1));
                }
                else listBox1.Items.Add(filedata[i]);
            }
            listBox1.SelectedIndex = 0;
        }

        private void PopulateCommands2(string filepath)
        {
            listBox2.Items.Clear();
            string conf = filepath;
            string[] filedata = File.ReadAllLines(conf);
            for (int i = 0; i < filedata.Length; i++)
            {
                int startindex = filedata[i].IndexOf("=");
                if (startindex > 0)
                {
                    listBox2.Items.Add(filedata[i].Substring(0, startindex + 1));
                    defaults2[i] = filedata[i].Substring(startindex + 1, filedata[i].Length - (startindex + 1));
                }
                else listBox2.Items.Add(filedata[i]);
            }
            listBox2.SelectedIndex = 0;
        }

        //Double Click
        private void listBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (defaults[listBox1.SelectedIndex] == "") senddata(listBox1.SelectedItem.ToString());
            else if (listBox1.SelectedItem.ToString() == "TIME")
            {
                senddata(listBox1.SelectedItem.ToString() + DateTime.Now.ToString("HH:mm:ss dd-MM-yy"));
            }
            else senddata(listBox1.SelectedItem.ToString() + defaults[listBox1.SelectedIndex]);
        }

        //Close Port
        private void button4_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                serialPort1.Close();
                textBox7.Enabled = false;
                textBox5.Enabled = false;
                label7.Enabled = false;
                button3.Enabled = true;
                button13.Enabled = false;
                button6.Enabled = false;
                listBox1.Enabled = false;
                textBox2.Enabled = false;
                button15.Enabled = false;
                button1.Enabled = false;
                checkBox4.Enabled = false;
                checkBox5.Enabled = false;
                label1.Text = "Port closed";
                if (savelog1==1)
                {
                    log1.Close();
                    savelog1 = 0;
                }
            }
            else label1.Text = "Port not open";
        }

        //Hover
        private void comboBox1_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("Select port to open", comboBox1);
        }

        private void button15_Click(object sender, EventArgs e)
        {
            textBox2.Text = "";
        }

        private void listBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                if (defaults[listBox1.SelectedIndex] == "") senddata(listBox1.SelectedItem.ToString());
                else senddata(listBox1.SelectedItem.ToString() + defaults[listBox1.SelectedIndex]);
            }
        }

        private void textBox5_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                senddata(listBox1.SelectedItem.ToString() + "=" + textBox5.Text);
                e.Handled = true;
            }
        }

        private void button13_Click(object sender, EventArgs e)
        {
            if (savelog1 == 0)
            {
                OpenFileDialog savefiledialog1 = new OpenFileDialog();
                savefiledialog1.CheckFileExists = false;
                savefiledialog1.Filter = "Log Files|*.log";
                savefiledialog1.FilterIndex = 1;                
                savefiledialog1.ShowDialog();
                if (savefiledialog1.FileName != "")
                {
                    savelog1 = 1;
                    log1 = new StreamWriter(savefiledialog1.FileName, true);
                    button13.Text = "Stop Logging";
                }
            }
            else if (savelog1 == 1)
            {
                log1.Close();
                savelog1 = 0;
                button13.Text = "Start Logging";
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();
            comboBox4.Items.Clear();
            getAvailablePorts();
        }

        private void listBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                if (defaults2[listBox2.SelectedIndex] == "") senddata2(listBox2.SelectedItem.ToString());
                else senddata2(listBox2.SelectedItem.ToString() + defaults2[listBox2.SelectedIndex]);
            }
        }

        private void listBox2_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (defaults2[listBox2.SelectedIndex] == "") senddata2(listBox2.SelectedItem.ToString());
            else if (listBox2.SelectedItem.ToString() == "TIME")
            {
                senddata2(listBox1.SelectedItem.ToString() + DateTime.Now.ToString("HH:mm:ss dd-MM-yy"));
            }
            else senddata2(listBox1.SelectedItem.ToString() + defaults2[listBox2.SelectedIndex]);
        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox2.SelectedItem.ToString() == "TIME")
            {
                label12.Visible = true;
                textBox3.Visible = true;
                label12.Text = listBox2.SelectedItem.ToString();
                textBox3.Text = DateTime.Now.ToString("HH:mm:ss dd-MM-yy");
            }
            else if (defaults2[listBox2.SelectedIndex] != null && defaults2[listBox2.SelectedIndex] != "")
            {
                label12.Visible = true;
                textBox3.Visible = true;
                label12.Text = listBox2.SelectedItem.ToString();
                textBox3.Text = defaults2[listBox2.SelectedIndex];
            }
            else if (defaults2[listBox2.SelectedIndex] == null || defaults2[listBox2.SelectedIndex] == "")
            {
                label12.Visible = false;
                textBox3.Visible = false;
            }
        }

        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                senddata2(listBox2.SelectedItem.ToString() + "=" + textBox3.Text);
                e.Handled = true;
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            textBox4.Text = "";
        }

        private void button10_Click(object sender, EventArgs e)
        {

            textBox6.Text = "";
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            if (savelog2 == 0)
            {
                OpenFileDialog savefiledialog2 = new OpenFileDialog();
                savefiledialog2.CheckFileExists = false;
                savefiledialog2.Filter = "Log Files|*.log";
                savefiledialog2.FilterIndex = 1;
                savefiledialog2.ShowDialog();
                if (savefiledialog2.FileName != "")
                {
                    savelog2 = 1;
                    log2 = new StreamWriter(savefiledialog2.FileName, true);
                    button2.Text = "Stop Logging";
                }
            }
            else if (savelog2 == 1)
            {
                log2.Close();
                savelog2 = 0;
                button2.Text = "Start Logging";
            }
        }

        private void textBox8_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                senddata2(textBox8.Text);
                textBox8.Text = "";
                e.Handled = true;
            }
        }

        private void textBox7_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                senddata(textBox7.Text);
                textBox7.Text = "";
                e.Handled = true;
            }
        }

        private void listBox2_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            if (listBox2.SelectedItem.ToString() == "TIME")
            {
                label11.Visible = true;
                textBox3.Visible = true;
                label11.Text = listBox2.SelectedItem.ToString();
                textBox3.Text = DateTime.Now.ToString("HH:mm:ss dd-MM-yy");
            }
            else if (defaults2[listBox2.SelectedIndex] != null && defaults2[listBox2.SelectedIndex] != "")
            {
                label11.Visible = true;
                textBox3.Visible = true;
                label11.Text = listBox2.SelectedItem.ToString();
                textBox3.Text = defaults2[listBox2.SelectedIndex];
            }
            else if (defaults2[listBox2.SelectedIndex] == null || defaults2[listBox2.SelectedIndex] == "")
            {
                label11.Visible = false;
                textBox3.Visible = false;
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog2 = new OpenFileDialog();
            openFileDialog2.Filter = "Configuration Files|*.ini";
            openFileDialog2.FilterIndex = 1;
            if (openFileDialog2.ShowDialog() == DialogResult.OK)
            {
                PopulateCommands(openFileDialog2.FileName);
            }
        }

        private void button11_Click(object sender, EventArgs e)
        {

            if (comboBox4.Text != "") serialPort2.PortName = comboBox4.Text;
            serialPort2.BaudRate = Convert.ToInt32(comboBox3.Text);
            serialPort2.DataBits = 8;
            try
            {
                serialPort2.Open();
            }
            catch (System.IO.IOException)
            {
                label12.Text = "Can't find port!";
            }
            catch (System.UnauthorizedAccessException)
            {
                label12.Text = "Port in use";
            }
            catch
            {
                label12.Text = "Error Occurred";
            }
            if (serialPort2.IsOpen)
            {
                textBox8.Enabled = true;
                button11.Enabled = false;
                button2.Enabled = true;
                label11.Enabled = true;
                textBox3.Enabled = true;
                button8.Enabled = true;
                listBox2.Enabled = true;
                textBox4.Enabled = true;
                button7.Enabled = true;
                button10.Enabled = true;
                checkBox6.Enabled = true;
                checkBox7.Enabled = true;
                label12.Text = "Port Open: " + comboBox4.Text;
            }
        }

        private void button12_Click(object sender, EventArgs e)
        {
            if (serialPort2.IsOpen)
            {
                serialPort2.Close();
                textBox8.Enabled = false;
                textBox3.Enabled = false;
                label11.Enabled = false;
                button11.Enabled = true;
                button2.Enabled = false;
                button8.Enabled = false;
                listBox2.Enabled = false;
                textBox4.Enabled = false;
                button7.Enabled = false;
                button10.Enabled = false;
                checkBox6.Enabled = false;
                checkBox7.Enabled = false;
                label12.Text = "Port closed";
                if (savelog2 == 1)
                {
                    log2.Close();
                    savelog2 = 0;
                }
            }
            else label12.Text = "Port not open";
        }
    }
}
