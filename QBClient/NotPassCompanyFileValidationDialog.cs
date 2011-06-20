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
    partial class NotPassCompanyFileValidationDialog : Form
    {
        volatile Auth m_auth;
        
        public NotPassCompanyFileValidationDialog(Auth auth)
        {
            InitializeComponent();
            m_auth = auth;
            m_auth.CompanyFileValidated = false;
        }

        private void buttonRevalidate_Click(object sender, EventArgs e)
        {
            m_auth.CompanyFileValidated = ValidateCompanyMarker();
            this.Close();
        }

        private void buttonLogout_Click(object sender, EventArgs e)
        {
            //m_auth.Logout();
            m_auth.CompanyFileValidated = false;   //verbose but robust

            //Thread logoutThread = new Thread(this.Logout);
            //logoutThread.Start();
            this.Close();
        }
        /*
        private void Logout()
        {
            m_auth.Logout();
        }
         */
        //after revalidation, if not validated log out, else
        private bool ValidateCompanyMarker()
        {

            if (m_auth.IsXMPPConnected())
            {
                CompanyFileValidator companyFileValidator = new CompanyFileValidator(m_auth);
                try
                {
                    m_auth.CompanyFileValidated = companyFileValidator.CheckCompanyMarker();
                }
                catch (System.Net.WebException ex)
                {
                    //Http connection lost
                    LogHelper.Error(ex);
                    MessageBox.Show(ex.Message);
                    //Thread logoutThread = new Thread(this.Logout);
                    //logoutThread.Start();
                    return false;
                }

                if (m_auth.CompanyFileValidated)
                {
                    return true;
                }
                else
                {
                    MessageBox.Show("Validation failed again. Will log out");
                    LogHelper.Debug("Validation failed again. Will log out");
                    //m_auth.Logout();
                    return false;
                }
            }
            else
            {
                MessageBox.Show("XMPP offline. Can't validate. Will log out");
                LogHelper.Debug("XMPP offline. Can't validate. Will log out");
                //m_auth.Logout();
                return false;
            }
        }

    }
}
