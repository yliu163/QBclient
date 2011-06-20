using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Threading;

using agsXMPP;
using agsXMPP.protocol;
using agsXMPP.protocol.client;


//When using System.Diagnostics;
//The debug works for a console (for a win form app, you can attach one, which is not necessary here)
//Debug.WriteLine("Debug Information-Product Starting ");

namespace QBClient
{
    public partial class QBClient : Form
    {
        XMPPCommandsHandler m_xmppCH;
        volatile Auth m_auth;
        //CompanyFileValidator m_companyFileValidator;
        //PresenceInformer m_presenceInformer;
        //SyncHandler m_syncRequestHandler;

        const int XMPP_CONNECT_TIMEOUT = 30 * 1000;


        // Here volatile is used as hint to the compiler that this data
        // member will be accessed by multiple threads.
        //volatile bool m_isCompanyMarkerValidated;

        public QBClient()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            listBoxEvent.Items.Clear();
        }

        private void buttonConnect_Click(object sender, EventArgs e)
        {
            //If button clicking will trigger a time-consuming procedure, a thread must be started to handle it.
            //And after openning the thread, no operation depending on the result of the thread running should be included in the function

            m_xmppCH = new XMPPCommandsHandler(this, this.labelSatusSign, this.listBoxEvent);
            Thread connectXMPPThread = new Thread(
                unused => m_xmppCH.Connnect(textBoxUsername.Text, textBoxPassword.Text, textBoxServer.Text, "")
            );
            connectXMPPThread.Start();
        }

        private void buttonDisconnect_Click(object sender, EventArgs e)
        {
            Thread disconnectXMPPThread = new Thread(m_xmppCH.Disconnect);
            disconnectXMPPThread.Start();
        }

        private void buttonLogout_Click(object sender, EventArgs e)
        {
            Thread logoutThread = new Thread(this.Logout);
            logoutThread.Start();
        }

        private void buttonLoginAuthorizeValidate_Click(object sender, EventArgs e)
        {
            //Thread loginThread = new Thread(this.Login);
            //loginThread.Start();
            this.Login();
        }

        void Login()
        {
            if (m_auth == null || m_auth != null && m_auth.IsAuthorized == false)
            {
                //m_xmppCH = new XMPPCommandsHandler(this, this.labelSatusSign, this.listBoxEvent);
                m_auth = new Auth(this, this.labelSatusSign, this.listBoxEvent);
                m_auth.LoginAndValidate();
            }
            else
            {
                LogHelper.Debug("Already online!");
                MessageBox.Show("Already online");
            }
        }
        void Logout()
        {
            if (m_auth != null)
            {
                m_auth.Logout();
                m_auth = null;
            }
            else
            {
                MessageBox.Show("Already offline!");
                LogHelper.Debug("Log out failure: Already offline");
            }
        }
    }
}
