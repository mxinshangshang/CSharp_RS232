using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.IO.Ports;


namespace rs232
{
    public partial class Form1 : Form
    {

        public int iTextbox2 = 0;
        public Form1()
        {
            InitializeComponent();
            // Nice methods to browse all available ports:
            string[] ports = SerialPort.GetPortNames();

            // Add all port names to the combo box:
            foreach (string port in ports)
            {
                comboBox1.Items.Add(port);
            }
            //comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 2;
            comboBox3.SelectedIndex = 0;
            comboBox4.SelectedIndex = 0;
            comboBox5.SelectedIndex = 0;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string[] str = SerialPort.GetPortNames();
            if (str == null)
            {
                MessageBox.Show("本机没有串口！", "Error");
                return;
            }
            //SerialPort1.PortName = comboBox1.SelectedItem.ToString();
            comboBox1.Items.AddRange(str);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            String str1 = comboBox1.Text;
            String str2 = comboBox2.Text;
            String str3 = comboBox3.Text;
            String str4 = comboBox4.Text;
            String str5 = comboBox5.Text;
            Int32 int2 = Convert.ToInt32(str2);
            Int32 int3 = Convert.ToInt32(str3);


            try
            {
                if (str1 == null)
                {
                    MessageBox.Show("请先选择串口！", "Error");
                    return;
                }

                serialPort1.PortName = str1;
                serialPort1.BaudRate = int2;
                serialPort1.DataBits = int3;
                switch (comboBox4.Text)
                {
                    case "1":
                        serialPort1.StopBits = StopBits.One;
                        break;
                    case "1.5":
                        serialPort1.StopBits = StopBits.OnePointFive;
                        break;
                    case "2":
                        serialPort1.StopBits = StopBits.Two;
                        break;
                    default:
                        MessageBox.Show("Error：停止位设置不正确", "Error");
                        break;
                }
                
                switch (comboBox5.Text)
                {
                    case "Even":
                        serialPort1.Parity = Parity.Even;
                        break;
                    case "Odd":
                        serialPort1.Parity = Parity.Odd;
                        break;
                    case "None":
                        serialPort1.Parity = Parity.None;
                        break;
                    default:
                        MessageBox.Show("Error：校验位设置不正确", "Error");
                        break;
                }

                if (serialPort1.IsOpen == true)
                {
                    serialPort1.Close();
                }

                serialPort1.Open();
                MessageBox.Show("串口打开成功！", str1);


                this.toolStripStatusLabel1.Text = "端口号：" + serialPort1.PortName + " ";
                this.toolStripStatusLabel2.Text = "波特率：" + serialPort1.BaudRate + " ";
                this.toolStripStatusLabel3.Text = "数据位：" + serialPort1.DataBits + " ";
                this.toolStripStatusLabel4.Text = "停止位：" + serialPort1.StopBits + " ";
                this.toolStripStatusLabel5.Text = "奇偶校验位：" + serialPort1.Parity + " ";

                button1.Enabled = false;
                comboBox1.Enabled = false;
                comboBox2.Enabled = false;
                comboBox3.Enabled = false;
                comboBox4.Enabled = false;
                comboBox5.Enabled = false;
            }
            catch (Exception er)
            {
                MessageBox.Show("Error:" + er.Message, "Error");
                return;
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            button1.Enabled = true;
            comboBox1.Enabled = true;
            comboBox2.Enabled = true;
            comboBox3.Enabled = true;
            comboBox4.Enabled = true;
            comboBox5.Enabled = true;
            serialPort1.Close();
            this.toolStripStatusLabel1.Text = "端口号：端口未打开";
            this.toolStripStatusLabel2.Text = "波特率：端口未打开";
            this.toolStripStatusLabel3.Text = "数据位：端口未打开";
            this.toolStripStatusLabel4.Text = "停止位：端口未打开";
            this.toolStripStatusLabel5.Text = "奇偶校验位：端口未打开";
        }


        //发送
        private void button3_Click(object sender, EventArgs e)
        {
            if (button1.Enabled == true)
            {
                MessageBox.Show("请先打开串口！", "Error");
                return;
            }

            String str1;
            str1 = textBox1.Text;
            byte[] data = Encoding.Default.GetBytes(str1);
            if (checkBox1.Checked == true)
            {
                //for (int i = 0; i < data.Length; i++)
                //{
                    //byte temp = data[i];
                  // string tempHex = temp.ToString("X2") + " ";
                  // serialPort1.Write(tempHex);

                    string a = textBox1.Text.Trim();
                    string[] aa = a.Split(' ');
                    byte[] message = new byte[aa.Length];
                    int s = aa.Length;
                    for (int i = 0; i < aa.Length; i++)
                    {
                        message[i] = Convert.ToByte(aa[i], 16);
                    }
                    serialPort1.Write(message, 0, s);
                    textBox1.Clear();
                //}
            }

            else
            {
                serialPort1.Write(data, 0, data.Length);
                textBox1.Clear();
            }
        }

        //使用Control.Invoke

        public delegate void DeleUpdateTextbox(string dataRe);

        private void UpdateTextbox(string dataRe)
        {

            if (iTextbox2 == 0)
            {
                this.textBox2.Text = dataRe;
                iTextbox2++;
            }
            else
            {
                textBox2.AppendText(dataRe);
            }
        }


        private void button4_Click(object sender, EventArgs e)
        {
            textBox2.Clear();
            iTextbox2 = 0; 
        }

        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string dataRe;
            string[] data = new string[1000];
            int j = 0;
            byte[] byteRead = new byte[serialPort1.BytesToRead];

            DeleUpdateTextbox deleupdatetextbox = new DeleUpdateTextbox(UpdateTextbox);

            serialPort1.Read(byteRead, 0, byteRead.Length);

            if (checkBox2.Checked == false)
            {
                dataRe = Encoding.Default.GetString(byteRead);
                textBox2.Invoke(deleupdatetextbox, dataRe);
            }
            else
            {
                for (int i = 0; i < byteRead.Length; i++)
                {
                    byte temp = byteRead[i];
                    dataRe = temp.ToString("X2") + " ";
                    textBox2.Invoke(deleupdatetextbox, dataRe);
                    //data[j++] = dataRe;
                    //if (dataRe == "0A ")
                    //{
                    //    textBox1.Invoke(deleupdatetextbox, data);
                    //    j = 0;
                    //}
                }
            }
           // this.Invoke(new DeleUpdateTextbox(serialPort1_DataReceived));
        }
    }
}