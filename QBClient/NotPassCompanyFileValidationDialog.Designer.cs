namespace QBClient
{
    partial class NotPassCompanyFileValidationDialog
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
            this.buttonRevalidate = new System.Windows.Forms.Button();
            this.buttonLogout = new System.Windows.Forms.Button();
            this.labelCompanyFileValidationFailed = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // buttonRevalidate
            // 
            this.buttonRevalidate.Location = new System.Drawing.Point(39, 150);
            this.buttonRevalidate.Name = "buttonRevalidate";
            this.buttonRevalidate.Size = new System.Drawing.Size(85, 34);
            this.buttonRevalidate.TabIndex = 0;
            this.buttonRevalidate.Text = "Revalidate";
            this.buttonRevalidate.UseVisualStyleBackColor = true;
            this.buttonRevalidate.Click += new System.EventHandler(this.buttonRevalidate_Click);
            // 
            // buttonLogout
            // 
            this.buttonLogout.Location = new System.Drawing.Point(164, 150);
            this.buttonLogout.Name = "buttonLogout";
            this.buttonLogout.Size = new System.Drawing.Size(97, 33);
            this.buttonLogout.TabIndex = 1;
            this.buttonLogout.Text = "Log out";
            this.buttonLogout.UseVisualStyleBackColor = true;
            this.buttonLogout.Click += new System.EventHandler(this.buttonLogout_Click);
            // 
            // labelCompanyFileValidationFailed
            // 
            this.labelCompanyFileValidationFailed.AutoSize = true;
            this.labelCompanyFileValidationFailed.Location = new System.Drawing.Point(50, 59);
            this.labelCompanyFileValidationFailed.Name = "labelCompanyFileValidationFailed";
            this.labelCompanyFileValidationFailed.Size = new System.Drawing.Size(153, 13);
            this.labelCompanyFileValidationFailed.TabIndex = 2;
            this.labelCompanyFileValidationFailed.Text = "Company File Validation Failed!";
            // 
            // NotPassCompanyFileValidationDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 273);
            this.Controls.Add(this.labelCompanyFileValidationFailed);
            this.Controls.Add(this.buttonLogout);
            this.Controls.Add(this.buttonRevalidate);
            this.Name = "NotPassCompanyFileValidationDialog";
            this.Text = "NotPassCompanyFileValidation";

            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonRevalidate;
        private System.Windows.Forms.Button buttonLogout;
        private System.Windows.Forms.Label labelCompanyFileValidationFailed;
    }
}