namespace QBClient
{
    partial class NoHttpResponseDialog
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
            this.buttonRetry = new System.Windows.Forms.Button();
            this.buttonLogout = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonNoRetry = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // buttonRetry
            // 
            this.buttonRetry.Location = new System.Drawing.Point(34, 181);
            this.buttonRetry.Name = "buttonRetry";
            this.buttonRetry.Size = new System.Drawing.Size(63, 30);
            this.buttonRetry.TabIndex = 0;
            this.buttonRetry.Text = "Retry";
            this.buttonRetry.UseVisualStyleBackColor = true;
            this.buttonRetry.Click += new System.EventHandler(this.buttonRetry_Click);
            // 
            // buttonLogout
            // 
            this.buttonLogout.Location = new System.Drawing.Point(204, 181);
            this.buttonLogout.Name = "buttonLogout";
            this.buttonLogout.Size = new System.Drawing.Size(76, 29);
            this.buttonLogout.TabIndex = 1;
            this.buttonLogout.Text = "Log out";
            this.buttonLogout.UseVisualStyleBackColor = true;
            this.buttonLogout.Click += new System.EventHandler(this.buttonLogout_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(53, 68);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(185, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Timeout and no response from server.";
            // 
            // buttonNoRetry
            // 
            this.buttonNoRetry.Location = new System.Drawing.Point(113, 181);
            this.buttonNoRetry.Name = "buttonNoRetry";
            this.buttonNoRetry.Size = new System.Drawing.Size(85, 28);
            this.buttonNoRetry.TabIndex = 3;
            this.buttonNoRetry.Text = "Don\'t retry";
            this.buttonNoRetry.UseVisualStyleBackColor = true;
            this.buttonNoRetry.Click += new System.EventHandler(this.buttonNoRetry_Click);
            // 
            // NoHttpResponseDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 273);
            this.Controls.Add(this.buttonNoRetry);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonLogout);
            this.Controls.Add(this.buttonRetry);
            this.Name = "NoHttpResponseDialog";
            this.Text = "NoHttpResponseDialog";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonRetry;
        private System.Windows.Forms.Button buttonLogout;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonNoRetry;
    }
}