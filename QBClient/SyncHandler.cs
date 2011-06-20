using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Windows.Forms;

namespace QBClient
{
    //this class will send sync request to the server periodically. If the sync is being processed, stop timer
    class SyncHandler
    {
        volatile Stopwatch stopwatch;

        //Note: now the real interval is the sum of the intervals below, plus http communication (should not be included)
        const int CHECK_INTERVAL = 1000;
        const int TIMER_INTERVAL = 19 * 1000;

        Thread m_timerThread;
        volatile Auth m_auth;
        Account m_account;
        /*
         * * The volatile keyword indicates that a field can be modified in the program 
         * by something such as the operating system, the hardware, or a concurrently executing thread.
         */
        volatile bool m_shouldStop;
        volatile bool m_shouldVanish;

        public SyncHandler(Auth auth)
        {
            stopwatch = new Stopwatch();
            m_auth = auth;
            m_account = auth.Account;
            m_account.Id = auth.Account.Id.ToString();
            m_timerThread = new Thread(SendSyncRequestCycler);
            m_shouldStop = true;
            m_timerThread.Start();
            m_shouldVanish = false;
        }

        public bool IsRunning()
        {
            if (!m_shouldVanish && !m_shouldStop)
                return true;
            return false;
        }

        public void SendSyncRequestCycler()
        {
            try
            {
                while (!m_shouldVanish)
                {
                    if (!m_shouldStop && stopwatch.ElapsedMilliseconds > TIMER_INTERVAL)
                    {

                        //if validation fails
                        if (m_auth.IsXMPPConnected())
                        {
                            CompanyFileValidator companyFileValidator = new CompanyFileValidator(m_auth);
                            
                            //the send interval calculation should plus the check time (will be fixed)
                            try
                            {
                                m_auth.CompanyFileValidated = companyFileValidator.CheckCompanyMarker();
                            }
                            catch (System.Net.WebException ex)
                            {
                                //Http connection lost
                                LogHelper.Error(ex);
                                MessageBox.Show(ex.Message);
                                Thread logoutThread = new Thread(this.Logout);
                                logoutThread.Start();
                                return;
                            }
                            if (!m_auth.CompanyFileValidated)
                            {
                                using (NotPassCompanyFileValidationDialog dialog = new NotPassCompanyFileValidationDialog(m_auth))
                                {
                                    dialog.ShowDialog();
                                }
                                if (m_auth.Authorized && !m_auth.CompanyFileValidated)
                                {
                                    //In case the user click on the "close" button of the dialog
                                    //  m_auth.Logout();  this will make the two threads lock each other
                                    Thread logoutThread = new Thread(this.Logout);
                                    logoutThread.Start();
                                    return;
                                }
                                else if (m_auth.Authorized && m_auth.CompanyFileValidated)
                                {
                                    //not likely to happen any way
                                    MessageBox.Show("Validated");
                                    LogHelper.Debug("Validated");
                                    //StartPingAndSyncRequest();
                                }
                            }
                            //below will assume the company marker is validated, or it won't hit here.
                            if (m_auth.NeverSynced)
                            {
                                m_auth.NeedsFullSync = true;
                                m_auth.AgreedToFullSync = (int)FullSyncConfirmed.NA;  //never synced, it doesn't need confirmation from user
                                /*
                                if (syncMarkerValidator.MarkSync() == (int)Status.SUCCESS)
                                    m_auth.NeverSynced = false;
                                else
                                {
                                    m_auth.NeverSynced = true;
                                    MessageBox.Show("Mark sync failed.");
                                    LogHelper.Debug("Mark sync failed.");
                                }
                                 */
                                m_auth.SyncValidated = true;
                            }
                            else
                            {
                                SyncMarkerValidator syncMarkerValidator = new SyncMarkerValidator(m_auth);
                                try
                                {
                                    m_auth.SyncValidated = syncMarkerValidator.CheckSyncMarker();
                                }
                                catch (System.Net.WebException ex)
                                {
                                    //Http connection lost
                                    LogHelper.Error(ex);
                                    MessageBox.Show(ex.Message);
                                    Thread logoutThread = new Thread(this.Logout);
                                    logoutThread.Start();
                                    return;
                                }
                                m_auth.NeedsFullSync = m_auth.SyncValidated?false:true;
                            }

                            if (!m_auth.SyncValidated)
                            {
                                m_auth.NeedsFullSync = true;
                                m_auth.AgreedToFullSync = (int)FullSyncConfirmed.No;
                                using (AgreeToFullSyncDialog dialog = new AgreeToFullSyncDialog(m_auth))
                                {
                                    dialog.ShowDialog();
                                }
                                if (m_auth.AgreedToFullSync == (int)FullSyncConfirmed.No)
                                {
                                    //In case the user click on the "close" button of the dialog
                                    MessageBox.Show("No full sync, no choice. Have to log out.");
                                    LogHelper.Debug("No full sync, no choice. Have to log out.");
                                    //  m_auth.Logout();  this will make the two threads lock each other
                                    Thread logoutThread = new Thread(this.Logout);
                                    logoutThread.Start();
                                    return;
                                }
                                else
                                {
                                    string xmlResponse = m_auth.SendSyncRequest(m_auth.NeedsFullSync);  //m_auth.NeedsFullSync = true

                                    if (xmlResponse != null || xmlResponse.Length != 0)
                                    {
                                        //once get confirmation from server, stop timer to wait for the sync done signal
                                        RestartTimer();
                                        PauseTimer();
                                        //if no confirmation is received, go on cycling
                                    }
                                }
                            }
                            else
                            {
                                //m_auth.NeedsFullSync = false;
                                string xmlResponse = m_auth.SendSyncRequest(m_auth.NeedsFullSync); //m_auth.NeedsFullSync = false

                                if (xmlResponse != null || xmlResponse.Length != 0)
                                {
                                    //once get confirmation from server, stop timer to wait for the sync done signal
                                    RestartTimer();
                                    PauseTimer();
                                    //if no confirmation is received, go on cycling
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show("XMPP connection failed. Will log out");
                            //m_auth.Logout();
                        }
                    }
                    Thread.Sleep(CHECK_INTERVAL);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
            }
        }

        public void SyncDone()
        {
            //SyncMarkerValidator syncMarkerValidator = new SyncMarkerValidator(m_auth.Account);
            //syncMarkerValidator.MarkSync();

            //after finishing the sync, NeverSynced should be false
            m_auth.NeverSynced = false;
            ResumeTimer();
        }
        public void Vanish()
        {
            m_shouldVanish = true;
            m_timerThread.Join();
        }
        public void StartSending()
        {
            m_shouldStop = false;
            if (stopwatch.IsRunning == false)
            {
                stopwatch.Reset();
                stopwatch.Start();
            }
        }
        public void StopSending()
        {
            m_shouldStop = true;
            if (stopwatch.IsRunning == true)
                stopwatch.Stop();
        }
        public void PauseTimer()
        {
            if (stopwatch.IsRunning == true)
                stopwatch.Stop();
        }
        public void ResumeTimer()
        {
            if (stopwatch.IsRunning == false)
            {
                stopwatch.Start();
            }
        }
        private void RestartTimer()
        {
            stopwatch.Reset();
            stopwatch.Start();
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
