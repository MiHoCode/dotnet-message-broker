using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MessageBrokerClient;

namespace WinFormsClient
{
    public partial class ClientForm : Form
    {
        MessageClient client;
        private string receiver = "server: ";

        public ClientForm()
        {
            InitializeComponent();
        }

        private void button_connect_Click(object sender, EventArgs e)
        {
            client = new MessageClient();
            client.MessageReceived += OnMessageReceived;
            if(client.Start(textBox_hostname.Text, textBox_clientID.Text, textBox_serverKey.Text, textBox_clientKey.Text))
            {
                panel_connect.Visible = false;
                textBox_cmd.Text = receiver;
                textBox_cmd.Focus();
            }
        }

        private void OnMessageReceived(MessageBrokerClient.Message obj)
        {
            if (!client.Running)
                return;
            addOutput(obj.Sender + ":");
            addOutput(Encoding.UTF8.GetString(obj.Content).Replace("\n", "\r\n"));
        }

        private void executeCommand(string cmd)
        {
            if (cmd.Contains(':'))
            {
                int idx = cmd.IndexOf(':');
                string rec = cmd.Substring(0, idx);
                receiver = rec + ": ";
                string msg = cmd.Substring(idx + 1).Trim();

                client.SendMessage(rec, msg);

                addOutput("> " + cmd);
            }
            else
            {
                addOutput("no receiver given...");
                receiver = "server: ";
            }
        }

        private void textBox_cmd_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                executeCommand(textBox_cmd.Text);
                textBox_cmd.Text = receiver;
                textBox_cmd.SelectionStart = textBox_cmd.Text.Length;
            }
        }

        private void addOutput(string line)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<string>(addOutput), line);
                return;
            }

            textBox1.AppendText("\r\n" + line);
            textBox1.SelectionStart = textBox1.Text.Length;
            textBox1.ScrollToCaret();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (client.Running)
                client.Close();
            base.OnClosing(e);
        }
    }
}
