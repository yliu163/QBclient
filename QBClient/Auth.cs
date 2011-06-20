using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Utility.ModifyRegistry;
using System.Xml;
using System.Threading;

namespace QBClient
{
    class Auth
    {
        //test again
        XMPPCommandsHandler m_xmppCH;

        PresenceInformer m_presenceInformer;
        SyncHandler m_syncRequestHandler;

        const int XMPP_CONNECT_TIMEOUT = 30 * 1000;
        Account m_account;
        User m_user;

        //if the server side does not provide any company file information,
        //the account will be considered as a brand new one (true)
        bool m_isNewAccount;
        //if the server returns a token which is different from the local copy,
        //this variable will be true. If this account is new, this variable will also be true
        bool m_isNewToken;
        //if the token is newly authorized
        //if the company marker on the server side is empty
        //the server will generate a new company marker and store it in the company data extension on the client
        //else the company marker needs to be validated, send the request to the server
        //validation also happens before a sync

        bool m_isCompanyFileValidated;
        bool m_isSyncValidated;

        //bool m_needsProduceCompanyFileMarker;   //a new account needs new company file marker, thus is the same thing with m_isNewAccount

        //the initial account state should be true
        bool m_isAuthorized;

        public bool IsAuthorized
        {
            get
            {
                return m_isAuthorized;
            }
        }

        //whether the account has been synced or not. If not, a new sync marker needs to be issued
        bool m_neverSynced;

        //
        bool m_needsFullSync;

        public bool NeedsFullSync
        {
            get
            {
                return m_needsFullSync;
            }
            set
            {
                m_needsFullSync = value;
            }
        }
        //
        int m_agreedToFullSync;

        public int AgreedToFullSync
        {
            get
            {
                return m_agreedToFullSync;
            }
            set
            {
                m_agreedToFullSync = value;
            }
        }

        public Account Account
        {
            get
            {
                return m_account;
            }
        }
        public User User
        {
            get
            {
                return m_user;
            }

        }
        public bool CompanyFileValidated
        {
            get
            {
                return m_isCompanyFileValidated;
            }
            set
            {
                m_isCompanyFileValidated = value;
            }
        }
        public bool SyncValidated
        {
            get
            {
                return m_isSyncValidated;
            }
            set
            {
                m_isSyncValidated = value;
            }
        }
        public bool NeverSynced
        {
            get
            {
                return m_neverSynced;
            }
            set
            {
                m_neverSynced = value;
            }
        }
        public bool Authorized
        {
            get
            {
                return m_isAuthorized;
            }
        }

        Thread m_connectXMPPThread;
        Thread m_validateThread;
        QBClient m_qbClient;
        Label m_labelStatus;
        ListBox m_eventList;

        public bool IsXMPPConnected()
        {
            return m_xmppCH.IsOnline;
        }

        //xmppCH should be a member variable of Auth. It's kept in the arg list for debug purpose.

        public Auth(QBClient qbClient, Label labelStatus, ListBox eventList)
        {
            m_isNewAccount = false;
            m_isNewToken = false;
            m_isCompanyFileValidated = false;
            m_isSyncValidated = false;
            m_neverSynced = false;
            m_agreedToFullSync = (int)FullSyncConfirmed.NA;

            m_isAuthorized = false;

            m_qbClient = qbClient;
            m_labelStatus = labelStatus;
            m_eventList = eventList;
        }
 
        void LogoutWrapper()
        {
            if (this != null)
            {
                this.Logout();
            }
            else
            {
                MessageBox.Show("Already offline!");
                LogHelper.Debug("Log out failure: Already offline");
            }
        }

        public int Logout()
        {
            try
            {
                if (m_account != null)
                {
                    LogHelper.Debug("Try Log out");
                    //note these two threads below may call this logout, so after they are done, check m_account != null
                    if(m_connectXMPPThread != null)
                        m_connectXMPPThread.Join();
                    if (m_validateThread != null)
                        m_validateThread.Join();
                    if (m_presenceInformer != null)
                    {
                        m_presenceInformer.RequestStopPing();
                        m_presenceInformer.Vanish();
                        m_presenceInformer = null;
                    }
                    if (m_syncRequestHandler != null)
                    {
                        m_syncRequestHandler.StopSending();
                        m_syncRequestHandler.Vanish();
                        m_syncRequestHandler = null;
                    }
                    //should be disconnected after m_presenceInformer and m_syncRequestHandler stopped
                    if (m_xmppCH != null && m_xmppCH.IsOnline)
                    {
                        m_xmppCH.Disconnect();
                        m_xmppCH = null;
                    }
                    //set member variable that might be needed by other variables at last
                    if (m_account != null)
                    {
                        string token = m_account.Token;

                        m_account = null;
                        m_user = null;
                        m_isAuthorized = false;

                        //once m_isAuthorized is false, the account is offline
                        MessageBox.Show("Now offline!");

                        string[] paraName = new string[] { "user_id", "token", "client_version" };
                        string[] paraValue = new string[] { m_account.Id, token, ClientInfo.VERSION };
                        string xmlResponse = RestComm.SendRestRequest(RestComm.URL_ACCOUNT_LOGOUT, paraName, paraValue);

                        if (xmlResponse != null)
                        {
                            LogHelper.Debug("Log out successfully");
                            return (int)Status.SUCCESS;
                        }
                        else
                        {
                            MessageBox.Show("No account found or server's dead!");
                            LogHelper.Debug("Log out failure: No account found");
                            //m_account.Status = "Offline";
                            //m_user.Status = "Offline";
                            return (int)Status.FAILURE;
                        }
                    }
                }
                MessageBox.Show("Already offline!");
                LogHelper.Debug("Log out failure: Already offline");
                return (int)Status.FAILURE;
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
                return (int)Status.EXCEPTION;
            }
        }

        public void LoginAndValidate()
        {
            if (!m_isAuthorized)
            {
                int loginStatus = Login();
                if (m_isAuthorized && loginStatus == (int)Status.SUCCESS)
                {
                    //newly authorized account needs validation
                    //Validate require connecting to the xmpp server
                    m_xmppCH = new XMPPCommandsHandler(m_qbClient, m_labelStatus, m_eventList);
                    m_xmppCH.Auth = this;
                    if (m_xmppCH != null && !m_xmppCH.IsOnline)
                    {
                        m_connectXMPPThread = new Thread(
                            unused => m_xmppCH.Connnect("clientdemo", "secret", "yangzi-lius-macbook-pro.local", m_account.Token)
                        );
                        m_connectXMPPThread.Start();
                    }

                    m_validateThread = new Thread(ValidateCompanyMarker);
                    m_validateThread.Start();
                }
                else
                {
                    LogHelper.Debug("Log in failed.");
                    MessageBox.Show("Log in failed.");
                }
            }
            else
            {
                LogHelper.Debug("One user is online. Make it log out if you can.");
                MessageBox.Show("One user is online. Make it log out if you can.");
            }
        }

        public int Login()
        {
            if (!m_isAuthorized)
            {
                try
                {
                    LogHelper.Debug("Try Log in");
                    m_account = new Account(RetrieveToken("TOKEN"));
                    m_user = new User("test", "123");
                    //m_isNewAccount = false;
                    //m_isNewToken = false;
                    //m_isCompanyFileValidated = false;

                    string syncMarker = null;
                    string token = m_account.Token;
                    string companyFile = null;
                    string username = m_user.Username;
                    string password = m_user.Password;
                    string[] paraName = new string[] { "username", "password", "token", "client_version" };
                    string[] paraValue = new string[] { username, password, token, ClientInfo.VERSION };

                    string xmlResponse = RestComm.SendRestRequest(RestComm.URL_ACCOUNT_LOGIN, paraName, paraValue);
                    
                    //if no response from server
                    if (xmlResponse == null)
                    {
                        m_isAuthorized = false;
                        m_account = null;
                        m_user = null;
                        MessageBox.Show("No response from server!");
                        LogHelper.Debug("Log in failure: No response from server");
                        return (int)Status.FAILURE;
                    }

                    string error = XMLParser.ExtractValueByTagName(xmlResponse, "error");

                    if (error == null || error.Length == 0)
                    {
                        companyFile = XMLParser.ExtractValueByTagName(xmlResponse, "company-file");
                        token = XMLParser.ExtractValueByTagName(xmlResponse, "token");

                        if (companyFile == null || companyFile.Length == 0)
                        {
                            //this accout has no company file record on the server side, thus is a new account
                            m_isNewAccount = true;
                            m_isNewToken = true;
                            m_neverSynced = true;
                            //a new account does not need validation
                            m_isCompanyFileValidated = true;
                            //new account needs full sync
                            m_needsFullSync = true;
                        }
                        else
                        {
                            m_isNewAccount = false;
                            syncMarker = XMLParser.ExtractValueByTagName(xmlResponse, "sync-marker");
                            if (syncMarker == null || syncMarker.Length == 0)
                                m_neverSynced = true;
                            else
                                m_neverSynced = false;
                            if (token.Equals(m_account.Token))
                            {
                                m_isNewToken = false;
                                m_isCompanyFileValidated = true;
                            }
                            else
                            {
                                m_isNewToken = true;
                                m_isCompanyFileValidated = false;
                            }
                        }

                        //once a new token is generated, the user needs to locate the company file
                        if (m_isNewToken)
                        {
                            companyFile = SelectCompanyFileDialog();
                            StroeToken("TOKEN", token);
                        }

                        LogHelper.Debug(String.Format("Comapny File: {0}", companyFile));

                        if (companyFile == null || companyFile.Length == 0)
                        {
                            m_isAuthorized = false;
                            m_account = null;
                            m_user = null;
                            MessageBox.Show("Invalid Company File!");
                            LogHelper.Debug("Log in failure: Invalid Company File");
                            return (int)Status.FAILURE;
                        }
                        else
                        {
                            //note that if it's the first time login, the company file must be assigned from the location provided by the user
                            m_account.AssignAccount(xmlResponse);
                            if (m_isNewAccount)
                                m_account.CompanyFile = companyFile;

                            m_isAuthorized = true;
                            MessageBox.Show("Login Success!");
                            LogHelper.Debug(String.Format("Login Success"));

                            //the company file and token will stroed in the local register on the client side
                            m_account.Status = "Online";
                            m_user.Status = "Online";
                            m_account.Token = token;
                            m_user.Token = token;
                            return (int)Status.SUCCESS;
                        }
                    }
                    else
                    {
                        m_isAuthorized = false;
                        m_account = null;
                        m_user = null;
                        MessageBox.Show(error);
                        LogHelper.Debug(String.Format("Log in failure: {0}", error));
                        return (int)Status.FAILURE;
                    }
                }
                catch (System.Net.WebException ex)
                {
                    //Http connection lost
                    LogHelper.Error(ex);
                    MessageBox.Show(ex.Message);
                    Thread logoutThread = new Thread(this.LogoutWrapper);
                    logoutThread.Start();
                    return (int)Status.EXCEPTION;
                }
                catch (Exception ex)
                {
                    m_isAuthorized = false;
                    m_account = null;
                    m_user = null;
                    LogHelper.Error(ex);
                    MessageBox.Show(ex.Message);
                    return (int)Status.EXCEPTION;
                }
            }
            else
            {
                MessageBox.Show("Already online!");
                LogHelper.Debug("Alredy online");
                //the company file and token will stored in the local register on the client side
                m_account.Status = "Online";
                m_user.Status = "Online";
                return (int)Status.FAILURE;
            }
        }

        //This method will encapsulated in a thread and complete the potential validation and start the ping and sync req threads
        public void ValidateCompanyMarker()
        {
            //must wait till the xmpp connection established
            CompanyFileValidator companyFileValidator = new CompanyFileValidator(this);
            bool isTimeout = true;
            int count = 0;
            int checkInterval = 100;
            while (count < XMPP_CONNECT_TIMEOUT)
            {
                if (m_xmppCH != null && !m_xmppCH.IsOnline)
                {
                    count += 100;
                    Thread.Sleep(checkInterval);
                }
                else
                {
                    isTimeout = false;
                    break;
                }
            }
            if (isTimeout)
            {
                LogHelper.Debug("XMPP Connection timeout");
                //companyFileValidator.IsValidated = false;
                MessageBox.Show("XMPP Connection timeout! Will log out");
                Thread logoutThread = new Thread(this.LogoutWrapper);
                logoutThread.Start();
                return;
            }

            if (m_xmppCH != null && m_xmppCH.IsOnline)
            {

                if (m_isNewAccount)
                {
                    //mark the file if this account is new
                    if (companyFileValidator.MarkCompanyFile() == (int)Status.SUCCESS)
                    {
                        m_isCompanyFileValidated = true;
                    }
                    else
                    {
                        m_isCompanyFileValidated = false;
                    }
                }
                //if the company file has just been marked, no need to validate it
                else if (!m_isCompanyFileValidated)
                {
                    try
                    {
                        m_isCompanyFileValidated = companyFileValidator.CheckCompanyMarker();
                    }
                    catch (System.Net.WebException ex)
                    {
                        //Http connection lost
                        LogHelper.Error(ex);
                        MessageBox.Show(ex.Message);
                        Thread logoutThread = new Thread(this.LogoutWrapper);
                        logoutThread.Start();
                        return;
                    }
                }

                if (m_isCompanyFileValidated)
                {
                    //Once validated
                    //start ping thread and sending sync request
                    MessageBox.Show("Validated");
                    LogHelper.Debug("Validated");
                    StartPingAndSyncRequest();
                }
                else
                {
                    using (NotPassCompanyFileValidationDialog dialog = new NotPassCompanyFileValidationDialog(this))
                    {
                        dialog.ShowDialog();
                    }
                    if (!m_isCompanyFileValidated)
                    {
                        //In case the user click on the "close" button of the dialog
                        Thread logoutThread = new Thread(this.LogoutWrapper);
                        logoutThread.Start();
                        return;
                    }
                    else if (m_isAuthorized && m_isCompanyFileValidated)
                    {
                        //not likely to happen any way
                        MessageBox.Show("Validated");
                        LogHelper.Debug("Validated");
                        StartPingAndSyncRequest();
                    }
                }
            }
            else
            {
                MessageBox.Show("XMPP offline. Can't validate");
                LogHelper.Debug("XMPP offline. Can't validate");
            }
        }

        public void StartPing()
        {
            if (m_presenceInformer == null)
            {
                m_presenceInformer = new PresenceInformer(this);
                m_presenceInformer.StartPing();
            }
            else if (!m_presenceInformer.IsRunning())
                m_presenceInformer.StartPing();
        }

        public void StartSyncReq()
        {
            if (m_syncRequestHandler == null)
            {
                m_syncRequestHandler = new SyncHandler(this);
                m_xmppCH.SyncHandler = m_syncRequestHandler;
                m_syncRequestHandler.StartSending();
            }
            else if (!m_syncRequestHandler.IsRunning())
                m_syncRequestHandler.StartSending();
        }

        public void StartPingAndSyncRequest()
        {
            StartSyncReq();
            StartPing();
        }

        private string SelectCompanyFileDialog()
        {
            try
            {
                //Make sure you're not trying to open the dialog from any other thread than the main one (the UI thread/has the run loop).
                //Or 
                /*
                 * Error: ThreadStateException was Unhandled.
                 * Description: Current thread must be set to single thread apartment (STA) mode before OLE calls can be made.
                 * Ensure that your Main function has STAThreadAttribute marked on it.
This exception is only raised if a debugger is attached to the process.
                 */

                LogHelper.Debug("User selected company file");
                OpenFileDialog companyFileDialog = new OpenFileDialog();

                companyFileDialog.Title = "Open Image Files";
                companyFileDialog.Filter = "QuickBook Company Files|*.QBW";
                companyFileDialog.InitialDirectory = @"C:\Documents and Settings\AllUsers\Shared Documents\Intuit\QuickBooks\";
                companyFileDialog.AddExtension = true;
                companyFileDialog.CheckFileExists = true;
                companyFileDialog.CheckPathExists = true;

                if (companyFileDialog.ShowDialog() == DialogResult.OK)
                {
                    return companyFileDialog.FileName.ToString();
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        //stroe the token in registry
        private void StroeToken(string keyName, string keyValue)
        {
            try
            {
                ModifyRegistry myRegistry = new ModifyRegistry();
                //myRegistry.SubKey = "SOFTWARE\\RTF_SHARP_EDIT\\RECENTFILES";
                myRegistry.ShowError = true;
                myRegistry.DeleteSubKeyTree();
                myRegistry.Write(keyName, keyValue);
                LogHelper.Debug(String.Format("Stroed token: {0} in registry with subkey: {1} and key name: {2}", keyValue, myRegistry.SubKey, keyName));
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
            }
        }

        //get the token from registry
        private string RetrieveToken(string keyName)
        {
            try
            {
                ModifyRegistry myRegistry = new ModifyRegistry();
                //myRegistry.SubKey = "SOFTWARE\\RTF_SHARP_EDIT\\RECENTFILES";
                myRegistry.ShowError = true;
                string token = myRegistry.Read(keyName);
                LogHelper.Debug(String.Format("Get token: {0} in registry with subkey: {1} and key name: {2}", token, myRegistry.SubKey, keyName));
                return token;
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
                return null;
            }
        }

        public string ConfirmFullSync(bool agreed)
        {
            try
            {
                string[] paraName = new string[] { "account_id", "client_version", "agree_to_full_sync" };
                string[] paraValue;
                if (agreed)
                {
                    paraValue = new string[] { m_account.Id, ClientInfo.VERSION, "Yes" };
                    m_agreedToFullSync = (int)FullSyncConfirmed.Yes;
                }
                else
                {
                    paraValue = new string[] { m_account.Id, ClientInfo.VERSION, "No" };
                    m_agreedToFullSync = (int)FullSyncConfirmed.No;
                }

                LogHelper.Debug("Send confirm full sync");

                try
                {
                    string xmlResponse = RestComm.SendRestRequest(RestComm.URL_AGREE_TO_FULL_SYNC, paraName, paraValue);
                    return xmlResponse;
                }
                catch (System.Net.WebException ex)
                {
                    //Http connection lost
                    LogHelper.Error(ex);
                    MessageBox.Show(ex.Message);
                    Thread logoutThread = new Thread(this.LogoutWrapper);
                    logoutThread.Start();
                    return null;
                }
            }

            catch (Exception ex)
            {
                LogHelper.Error(ex);
                return null;
            }
        }

        public string SendSyncRequest(bool isFullSync)
        {
            try
            {
                string[] paraName = new string[] { "account_id", "client_version", "needs_full_sync" };
                string[] paraValue;
                if (isFullSync)
                {
                    paraValue = new string[] { m_account.Id, ClientInfo.VERSION, "yes" };
                    m_agreedToFullSync = (int)FullSyncConfirmed.Yes;
                }
                else
                {
                    paraValue = new string[] { m_account.Id, ClientInfo.VERSION, "no" };
                    m_agreedToFullSync = (int)FullSyncConfirmed.No;
                }

                LogHelper.Debug("Send sync request");
                string xmlResponse = RestComm.SendRestRequest(RestComm.URL_ACCOUNT_REQUEST_SYNC, paraName, paraValue);
                return xmlResponse;
            }

            catch (System.Net.WebException ex)
            {
                //Http connection lost
                LogHelper.Error(ex);
                MessageBox.Show(ex.Message);
                Thread logoutThread = new Thread(this.LogoutWrapper);
                logoutThread.Start();
                return null;
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
                return null;
            }
        }
    }
}
