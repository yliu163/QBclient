using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace QBClient
{
    //this class timely send message to the server to let it know the client is still on-line
    class PresenceInformer
    {
        const int INTERVAL = 20 * 1000;
        Thread m_informerThread;
        Account m_account;
        volatile Auth m_auth;    //in case connection lost, log out
        /*
         * * * The volatile keyword indicates that a field can be modified in the program 
         * * by something such as the operating system, the hardware, or a concurrently executing thread.
         */
        private volatile bool m_shouldStop;
        private volatile bool m_shouldVanish;

        public PresenceInformer(Auth auth)
        {
            m_auth = auth;
            m_account = auth.Account;
            m_account.Id = auth.Account.Id.ToString();
            m_informerThread = new Thread(Ping);
            m_shouldStop = true;
            m_shouldVanish = false;
            m_informerThread.Start();
        }

        public bool IsRunning()
        {
            if (!m_shouldVanish && !m_shouldStop)
                return true;
            return false;
        }

        public void Ping()
        {
            try
            {
                while (!m_shouldVanish)
                {
                    if (!m_shouldStop)
                    {
                        string[] paraName = new string[] { "account_id", "client_version" };
                        string[] paraValue = new string[] { m_account.Id, ClientInfo.VERSION };
                        LogHelper.Debug("Send presence");
                        string xmlResponse = RestComm.SendRestRequest(RestComm.URL_ACCOUNT_PING, paraName, paraValue);
                    }
                    Thread.Sleep(INTERVAL);
                }
            }
            catch (System.Net.WebException ex)
            {
                //Http connection lost
                MessageBox.Show(ex.Message);
                LogHelper.Error(ex);
                Thread logoutThread = new Thread(this.Logout);
                logoutThread.Start();
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
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

        public void RequestStopPing()
        {
            m_shouldStop = true;
        }

        public void StartPing()
        {
            m_shouldStop = false;
        }

        public void Vanish()
        {
            m_shouldVanish = true;
            m_informerThread.Join();
        }
    }
}
