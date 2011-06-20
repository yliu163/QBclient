using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace QBClient
{
    partial class NoHttpResponseDialog : Form
    {
        volatile Auth m_auth;
        bool m_agreedToRetry;

        public NoHttpResponseDialog(Auth auth)
        {
            InitializeComponent();
            m_auth = auth;
            m_agreedToRetry = false;
        }

        private void buttonRetry_Click(object sender, EventArgs e)
        {
            m_agreedToRetry = true;
            this.Close();
        }

        private void buttonLogout_Click(object sender, EventArgs e)
        {
            m_agreedToRetry = false;
            //m_auth.Logout();
            this.Close();
        }

        private void buttonNoRetry_Click(object sender, EventArgs e)
        {
            m_agreedToRetry = false;
            this.Close();
        }
    }
}
