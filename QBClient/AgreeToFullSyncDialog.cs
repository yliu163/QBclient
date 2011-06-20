using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace QBClient
{
    partial class AgreeToFullSyncDialog : Form
    {
        volatile Auth m_auth;

        public AgreeToFullSyncDialog(Auth auth)
        {
            InitializeComponent();
            m_auth = auth;
            m_auth.AgreedToFullSync = (int)FullSyncConfirmed.No;
        }

        private void Logout()
        {
            m_auth.Logout();
        }
        private void buttonYes_Click(object sender, EventArgs e)
        {
            m_auth.AgreedToFullSync = (int)FullSyncConfirmed.Yes;
            m_auth.ConfirmFullSync(true);
            this.Close();
        }

        private void buttonNo_Click(object sender, EventArgs e)
        {
            m_auth.AgreedToFullSync = (int)FullSyncConfirmed.No;
            m_auth.ConfirmFullSync(false);

            //Thread logoutThread = new Thread(this.Logout);
            //logoutThread.Start();

            this.Close();
        }
    }
}
