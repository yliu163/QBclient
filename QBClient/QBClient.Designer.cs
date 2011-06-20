namespace QBClient
{
    partial class QBClient
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tabControlConnectRestRequest = new System.Windows.Forms.TabControl();
            this.tabPageLogin = new System.Windows.Forms.TabPage();
            this.buttonDisconnect = new System.Windows.Forms.Button();
            this.buttonConnect = new System.Windows.Forms.Button();
            this.labelAtSign = new System.Windows.Forms.Label();
            this.textBoxPassword = new System.Windows.Forms.TextBox();
            this.textBoxServer = new System.Windows.Forms.TextBox();
            this.textBoxUsername = new System.Windows.Forms.TextBox();
            this.labelSatusSign = new System.Windows.Forms.Label();
            this.labelStatus = new System.Windows.Forms.Label();
            this.labelPassword = new System.Windows.Forms.Label();
            this.labelServer = new System.Windows.Forms.Label();
            this.labelUsername = new System.Windows.Forms.Label();
            this.tabPageConnect = new System.Windows.Forms.TabPage();
            this.buttonLoginAuthorizeValidate = new System.Windows.Forms.Button();
            this.buttonLogout = new System.Windows.Forms.Button();
            this.listBoxEvent = new System.Windows.Forms.ListBox();
            this.tabControlConnectRestRequest.SuspendLayout();
            this.tabPageLogin.SuspendLayout();
            this.tabPageConnect.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControlConnectRestRequest
            // 
            this.tabControlConnectRestRequest.Controls.Add(this.tabPageLogin);
            this.tabControlConnectRestRequest.Controls.Add(this.tabPageConnect);
            this.tabControlConnectRestRequest.Location = new System.Drawing.Point(12, 12);
            this.tabControlConnectRestRequest.Name = "tabControlConnectRestRequest";
            this.tabControlConnectRestRequest.SelectedIndex = 0;
            this.tabControlConnectRestRequest.Size = new System.Drawing.Size(694, 254);
            this.tabControlConnectRestRequest.TabIndex = 0;
            // 
            // tabPageLogin
            // 
            this.tabPageLogin.Controls.Add(this.buttonDisconnect);
            this.tabPageLogin.Controls.Add(this.buttonConnect);
            this.tabPageLogin.Controls.Add(this.labelAtSign);
            this.tabPageLogin.Controls.Add(this.textBoxPassword);
            this.tabPageLogin.Controls.Add(this.textBoxServer);
            this.tabPageLogin.Controls.Add(this.textBoxUsername);
            this.tabPageLogin.Controls.Add(this.labelSatusSign);
            this.tabPageLogin.Controls.Add(this.labelStatus);
            this.tabPageLogin.Controls.Add(this.labelPassword);
            this.tabPageLogin.Controls.Add(this.labelServer);
            this.tabPageLogin.Controls.Add(this.labelUsername);
            this.tabPageLogin.Location = new System.Drawing.Point(4, 22);
            this.tabPageLogin.Name = "tabPageLogin";
            this.tabPageLogin.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageLogin.Size = new System.Drawing.Size(686, 228);
            this.tabPageLogin.TabIndex = 0;
            this.tabPageLogin.Text = "Login";
            this.tabPageLogin.UseVisualStyleBackColor = true;
            // 
            // buttonDisconnect
            // 
            this.buttonDisconnect.Location = new System.Drawing.Point(560, 186);
            this.buttonDisconnect.Name = "buttonDisconnect";
            this.buttonDisconnect.Size = new System.Drawing.Size(100, 27);
            this.buttonDisconnect.TabIndex = 10;
            this.buttonDisconnect.Text = "Disconnect";
            this.buttonDisconnect.UseVisualStyleBackColor = true;
            this.buttonDisconnect.Click += new System.EventHandler(this.buttonDisconnect_Click);
            // 
            // buttonConnect
            // 
            this.buttonConnect.Location = new System.Drawing.Point(23, 186);
            this.buttonConnect.Name = "buttonConnect";
            this.buttonConnect.Size = new System.Drawing.Size(89, 28);
            this.buttonConnect.TabIndex = 9;
            this.buttonConnect.Text = "Connect";
            this.buttonConnect.UseVisualStyleBackColor = true;
            this.buttonConnect.Click += new System.EventHandler(this.buttonConnect_Click);
            // 
            // labelAtSign
            // 
            this.labelAtSign.AutoSize = true;
            this.labelAtSign.Location = new System.Drawing.Point(324, 54);
            this.labelAtSign.Name = "labelAtSign";
            this.labelAtSign.Size = new System.Drawing.Size(18, 13);
            this.labelAtSign.TabIndex = 8;
            this.labelAtSign.Text = "@";
            // 
            // textBoxPassword
            // 
            this.textBoxPassword.Location = new System.Drawing.Point(23, 139);
            this.textBoxPassword.Name = "textBoxPassword";
            this.textBoxPassword.Size = new System.Drawing.Size(292, 20);
            this.textBoxPassword.TabIndex = 7;
            this.textBoxPassword.Text = "secret";
            // 
            // textBoxServer
            // 
            this.textBoxServer.Location = new System.Drawing.Point(352, 50);
            this.textBoxServer.Name = "textBoxServer";
            this.textBoxServer.Size = new System.Drawing.Size(308, 20);
            this.textBoxServer.TabIndex = 6;
            this.textBoxServer.Text = "yangzi-lius-macbook-pro.local";
            // 
            // textBoxUsername
            // 
            this.textBoxUsername.Location = new System.Drawing.Point(24, 50);
            this.textBoxUsername.Name = "textBoxUsername";
            this.textBoxUsername.Size = new System.Drawing.Size(293, 20);
            this.textBoxUsername.TabIndex = 5;
            this.textBoxUsername.Text = "clientdemo";
            // 
            // labelSatusSign
            // 
            this.labelSatusSign.AutoSize = true;
            this.labelSatusSign.Location = new System.Drawing.Point(623, 142);
            this.labelSatusSign.Name = "labelSatusSign";
            this.labelSatusSign.Size = new System.Drawing.Size(37, 13);
            this.labelSatusSign.TabIndex = 4;
            this.labelSatusSign.Text = "Offline";
            // 
            // labelStatus
            // 
            this.labelStatus.AutoSize = true;
            this.labelStatus.Location = new System.Drawing.Point(517, 142);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(100, 13);
            this.labelStatus.TabIndex = 3;
            this.labelStatus.Text = "Connection Status: ";
            // 
            // labelPassword
            // 
            this.labelPassword.AutoSize = true;
            this.labelPassword.Location = new System.Drawing.Point(20, 112);
            this.labelPassword.Name = "labelPassword";
            this.labelPassword.Size = new System.Drawing.Size(59, 13);
            this.labelPassword.TabIndex = 2;
            this.labelPassword.Text = "Password: ";
            // 
            // labelServer
            // 
            this.labelServer.AutoSize = true;
            this.labelServer.Location = new System.Drawing.Point(349, 23);
            this.labelServer.Name = "labelServer";
            this.labelServer.Size = new System.Drawing.Size(44, 13);
            this.labelServer.TabIndex = 1;
            this.labelServer.Text = "Server: ";
            // 
            // labelUsername
            // 
            this.labelUsername.AutoSize = true;
            this.labelUsername.Location = new System.Drawing.Point(20, 23);
            this.labelUsername.Name = "labelUsername";
            this.labelUsername.Size = new System.Drawing.Size(61, 13);
            this.labelUsername.TabIndex = 0;
            this.labelUsername.Text = "Username: ";
            // 
            // tabPageConnect
            // 
            this.tabPageConnect.Controls.Add(this.buttonLoginAuthorizeValidate);
            this.tabPageConnect.Controls.Add(this.buttonLogout);
            this.tabPageConnect.Location = new System.Drawing.Point(4, 22);
            this.tabPageConnect.Name = "tabPageConnect";
            this.tabPageConnect.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageConnect.Size = new System.Drawing.Size(686, 228);
            this.tabPageConnect.TabIndex = 1;
            this.tabPageConnect.Text = "RestRequest";
            this.tabPageConnect.UseVisualStyleBackColor = true;
            // 
            // buttonLoginAuthorizeValidate
            // 
            this.buttonLoginAuthorizeValidate.Location = new System.Drawing.Point(396, 115);
            this.buttonLoginAuthorizeValidate.Name = "buttonLoginAuthorizeValidate";
            this.buttonLoginAuthorizeValidate.Size = new System.Drawing.Size(129, 34);
            this.buttonLoginAuthorizeValidate.TabIndex = 2;
            this.buttonLoginAuthorizeValidate.Text = "Login";
            this.buttonLoginAuthorizeValidate.UseVisualStyleBackColor = true;
            this.buttonLoginAuthorizeValidate.Click += new System.EventHandler(this.buttonLoginAuthorizeValidate_Click);
            // 
            // buttonLogout
            // 
            this.buttonLogout.Location = new System.Drawing.Point(550, 162);
            this.buttonLogout.Name = "buttonLogout";
            this.buttonLogout.Size = new System.Drawing.Size(85, 24);
            this.buttonLogout.TabIndex = 1;
            this.buttonLogout.Text = "Log out";
            this.buttonLogout.UseVisualStyleBackColor = true;
            this.buttonLogout.Click += new System.EventHandler(this.buttonLogout_Click);
            // 
            // listBoxEvent
            // 
            this.listBoxEvent.FormattingEnabled = true;
            this.listBoxEvent.HorizontalScrollbar = true;
            this.listBoxEvent.IntegralHeight = false;
            this.listBoxEvent.Location = new System.Drawing.Point(11, 294);
            this.listBoxEvent.Name = "listBoxEvent";
            this.listBoxEvent.ScrollAlwaysVisible = true;
            this.listBoxEvent.Size = new System.Drawing.Size(690, 277);
            this.listBoxEvent.TabIndex = 1;
            // 
            // QBClient
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(718, 594);
            this.Controls.Add(this.listBoxEvent);
            this.Controls.Add(this.tabControlConnectRestRequest);
            this.Name = "QBClient";
            this.Text = "Quick Book Client";
            this.tabControlConnectRestRequest.ResumeLayout(false);
            this.tabPageLogin.ResumeLayout(false);
            this.tabPageLogin.PerformLayout();
            this.tabPageConnect.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControlConnectRestRequest;
        private System.Windows.Forms.TabPage tabPageLogin;
        private System.Windows.Forms.TabPage tabPageConnect;
        private System.Windows.Forms.TextBox textBoxPassword;
        private System.Windows.Forms.TextBox textBoxServer;
        private System.Windows.Forms.TextBox textBoxUsername;
        private System.Windows.Forms.Label labelSatusSign;
        private System.Windows.Forms.Label labelStatus;
        private System.Windows.Forms.Label labelPassword;
        private System.Windows.Forms.Label labelServer;
        private System.Windows.Forms.Label labelUsername;
        private System.Windows.Forms.Button buttonDisconnect;
        private System.Windows.Forms.Button buttonConnect;
        private System.Windows.Forms.Label labelAtSign;
        private System.Windows.Forms.ListBox listBoxEvent;
        private System.Windows.Forms.Button buttonLogout;
        private System.Windows.Forms.Button buttonLoginAuthorizeValidate;
    }
}

