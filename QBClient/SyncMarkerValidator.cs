using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Threading;

namespace QBClient
{
    class SyncMarkerValidator
    {
        //enum Status { EXCEPTION = -2, FAILURE, SUCCESS };

        bool m_isValidated;
        Account m_account;
        volatile Auth m_auth;

        public SyncMarkerValidator(Auth auth)
        {
            m_isValidated = false;
            m_auth = auth;
            m_account = auth.Account;
        }

        public bool IsValidated
        {
            get
            {
                return m_isValidated;
            }
            set
            {
                m_isValidated = value;
            }
        }
         //request the server to mark the sync file
        public int MarkSync()
        {
            try
            {
                LogHelper.Debug("Mark Sync");
                string[] paraName = new string[] { "account_id",  "client_version"};
                string[] paraValue = new string[] { m_account.Id, ClientInfo.VERSION };
                string xmlResponse = RestComm.SendRestRequest(RestComm.URL_ACCOUNT_NEW_SYNC_MARKER, paraName, paraValue);
                if (xmlResponse != null)
                {
                    //Once a new sync maker is generated, the new marker does not need to be validated
                    m_isValidated = true;
                    m_account.AssignAccount(xmlResponse);
                    return (int)Status.SUCCESS;
                }
                else
                {
                    return (int)Status.FAILURE;
                }
            }
            catch (System.Net.WebException ex)
            {
                //Http connection lost
                LogHelper.Error(ex);
                MessageBox.Show(ex.Message);
                Thread logoutThread = new Thread(this.Logout);
                logoutThread.Start();
                return (int)Status.EXCEPTION;
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
                return (int)Status.EXCEPTION;
            }
        }

        public bool CheckSyncMarker()
        {
            try
            {
                LogHelper.Debug("Check sync");
                string[] paraName = new string[] { "account_id", "client_version" };
                string[] paraValue = new string[] { m_account.Id, ClientInfo.VERSION };

                //Since the marker validation might take some time, the http communication will time out even before getting response from the server
                //The default timeout of HttpWebResponse is said to be 100,000 milliseconds (100 seconds).
                string xmlResponse = RestComm.SendRestRequest(RestComm.URL_ACCOUNT_VALIDATE_SYNC_MARKER, paraName, paraValue);
                return GetValidationResult(xmlResponse);
                //return false;
            }
            catch (System.Net.WebException ex)
            {
                //Http connection lost
                LogHelper.Error(ex);
                MessageBox.Show(ex.Message);
                //Thread logoutThread = new Thread(this.Logout);
                //logoutThread.Start();

                throw (ex);
                //return false;
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
                return false;
            }
        }
        public bool GetValidationResult(string plainXML)
        {
            try
            {
                if (plainXML.Length == 0 || plainXML == null)
                {
                    m_isValidated = false;
                    LogHelper.Warn("No Response from server!");
                    //return false;
                }
                else if( XMLParser.MatchValue(plainXML, "SyncMarkerValidation", "Validated"))
                {
                    m_isValidated = true;
                    LogHelper.Debug("Sync Marker Validation Passed!");
                    //return true;
                }
                else if (XMLParser.MatchValue(plainXML, "SyncMarkerValidation", "Not Validated"))
                {
                    m_isValidated = false;
                    LogHelper.Debug("Sync Marker Validation Failed!");
                    //return false;
                }
                else
                {
                    m_isValidated = false;
                    LogHelper.Warn("Invalid Validation Response!");
                    //return false;
                }
                return m_isValidated;
            }
            catch (Exception ex)
            {
                m_isValidated = false;
                LogHelper.Error(ex);
                return false;
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
