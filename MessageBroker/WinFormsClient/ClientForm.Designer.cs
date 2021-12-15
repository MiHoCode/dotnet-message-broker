
namespace WinFormsClient
{
    partial class ClientForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panel_connect = new System.Windows.Forms.Panel();
            this.button_connect = new System.Windows.Forms.Button();
            this.textBox_clientKey = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textBox_serverKey = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox_clientID = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox_hostname = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.panel_main = new System.Windows.Forms.Panel();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox_cmd = new System.Windows.Forms.TextBox();
            this.panel_connect.SuspendLayout();
            this.panel_main.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel_connect
            // 
            this.panel_connect.Controls.Add(this.button_connect);
            this.panel_connect.Controls.Add(this.textBox_clientKey);
            this.panel_connect.Controls.Add(this.label4);
            this.panel_connect.Controls.Add(this.textBox_serverKey);
            this.panel_connect.Controls.Add(this.label3);
            this.panel_connect.Controls.Add(this.textBox_clientID);
            this.panel_connect.Controls.Add(this.label2);
            this.panel_connect.Controls.Add(this.textBox_hostname);
            this.panel_connect.Controls.Add(this.label1);
            this.panel_connect.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel_connect.Location = new System.Drawing.Point(0, 0);
            this.panel_connect.Name = "panel_connect";
            this.panel_connect.Size = new System.Drawing.Size(836, 267);
            this.panel_connect.TabIndex = 0;
            // 
            // button_connect
            // 
            this.button_connect.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.button_connect.Location = new System.Drawing.Point(0, 223);
            this.button_connect.Name = "button_connect";
            this.button_connect.Size = new System.Drawing.Size(836, 44);
            this.button_connect.TabIndex = 8;
            this.button_connect.Text = "connect";
            this.button_connect.UseVisualStyleBackColor = true;
            this.button_connect.Click += new System.EventHandler(this.button_connect_Click);
            // 
            // textBox_clientKey
            // 
            this.textBox_clientKey.Dock = System.Windows.Forms.DockStyle.Top;
            this.textBox_clientKey.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.textBox_clientKey.Location = new System.Drawing.Point(0, 162);
            this.textBox_clientKey.Name = "textBox_clientKey";
            this.textBox_clientKey.Size = new System.Drawing.Size(836, 26);
            this.textBox_clientKey.TabIndex = 7;
            // 
            // label4
            // 
            this.label4.Dock = System.Windows.Forms.DockStyle.Top;
            this.label4.Location = new System.Drawing.Point(0, 141);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(836, 21);
            this.label4.TabIndex = 6;
            this.label4.Text = "Client Key";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // textBox_serverKey
            // 
            this.textBox_serverKey.Dock = System.Windows.Forms.DockStyle.Top;
            this.textBox_serverKey.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.textBox_serverKey.Location = new System.Drawing.Point(0, 115);
            this.textBox_serverKey.Name = "textBox_serverKey";
            this.textBox_serverKey.Size = new System.Drawing.Size(836, 26);
            this.textBox_serverKey.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.Dock = System.Windows.Forms.DockStyle.Top;
            this.label3.Location = new System.Drawing.Point(0, 94);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(836, 21);
            this.label3.TabIndex = 4;
            this.label3.Text = "Server Key";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // textBox_clientID
            // 
            this.textBox_clientID.Dock = System.Windows.Forms.DockStyle.Top;
            this.textBox_clientID.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.textBox_clientID.Location = new System.Drawing.Point(0, 68);
            this.textBox_clientID.Name = "textBox_clientID";
            this.textBox_clientID.Size = new System.Drawing.Size(836, 26);
            this.textBox_clientID.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.Dock = System.Windows.Forms.DockStyle.Top;
            this.label2.Location = new System.Drawing.Point(0, 47);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(836, 21);
            this.label2.TabIndex = 2;
            this.label2.Text = "Client ID";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // textBox_hostname
            // 
            this.textBox_hostname.Dock = System.Windows.Forms.DockStyle.Top;
            this.textBox_hostname.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.textBox_hostname.Location = new System.Drawing.Point(0, 21);
            this.textBox_hostname.Name = "textBox_hostname";
            this.textBox_hostname.Size = new System.Drawing.Size(836, 26);
            this.textBox_hostname.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(836, 21);
            this.label1.TabIndex = 0;
            this.label1.Text = "Hostname";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panel_main
            // 
            this.panel_main.Controls.Add(this.textBox1);
            this.panel_main.Controls.Add(this.textBox_cmd);
            this.panel_main.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel_main.Location = new System.Drawing.Point(0, 267);
            this.panel_main.Name = "panel_main";
            this.panel_main.Size = new System.Drawing.Size(836, 289);
            this.panel_main.TabIndex = 1;
            // 
            // textBox1
            // 
            this.textBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox1.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.textBox1.ForeColor = System.Drawing.Color.SteelBlue;
            this.textBox1.Location = new System.Drawing.Point(0, 0);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox1.Size = new System.Drawing.Size(836, 263);
            this.textBox1.TabIndex = 1;
            // 
            // textBox_cmd
            // 
            this.textBox_cmd.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.textBox_cmd.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.textBox_cmd.Location = new System.Drawing.Point(0, 263);
            this.textBox_cmd.Name = "textBox_cmd";
            this.textBox_cmd.Size = new System.Drawing.Size(836, 26);
            this.textBox_cmd.TabIndex = 0;
            this.textBox_cmd.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox_cmd_KeyDown);
            // 
            // ClientForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(836, 556);
            this.Controls.Add(this.panel_main);
            this.Controls.Add(this.panel_connect);
            this.Name = "ClientForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Message Client";
            this.panel_connect.ResumeLayout(false);
            this.panel_connect.PerformLayout();
            this.panel_main.ResumeLayout(false);
            this.panel_main.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel_connect;
        private System.Windows.Forms.Button button_connect;
        private System.Windows.Forms.TextBox textBox_clientKey;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBox_serverKey;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBox_clientID;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox_hostname;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel_main;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox textBox_cmd;
    }
}

