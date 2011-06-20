using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Threading;

namespace QBClient
{
    class CompanyFileValidator
    {
        //enum Status { EXCEPTION = -2, FAILURE, SUCCESS };

        bool m_isValidated;
        Account m_account;
        volatile Auth m_auth;
 
        public CompanyFileValidator(Auth auth)
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
        //request the server to mark the company file
        public int MarkCompanyFile()
        {
            try
            {
                LogHelper.Debug("Mark Company File");
                string[] paraName = new string[] { "account_id", "company_file", "client_version"};
                string[] paraValue = new string[] { m_account.Id, m_account.CompanyFile, ClientInfo.VERSION};
                string xmlResponse = RestComm.SendRestRequest(RestComm.URL_ACCOUNT_NEW_COMPANY_FILE, paraName, paraValue);
                if (xmlResponse != null)
                {
                    //Once a new company maker is generated, the new marker does not need to be validated
                    m_isValidated = true;
                    m_account.AssignAccount(xmlResponse);
                    MessageBox.Show(m_account.CompanyFile);
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

        public bool CheckCompanyMarker()
        {
            try
            {
                LogHelper.Debug("Check Company File");
                string[] paraName = new string[] { "account_id", "client_version" };
                string[] paraValue = new string[] { m_account.Id, ClientInfo.VERSION };

                //Since the marker validation might take some time, the http communication will time out even before getting response from the server
                //The default timeout of HttpWebResponse is said to be 100,000 milliseconds (100 seconds).
                string xmlResponse = RestComm.SendRestRequest(RestComm.URL_ACCOUNT_VALIDATE_COMPANY_MARKER_NO_QUEUEING, paraName, paraValue);
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
                //return false;
                throw (ex);
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
                    return false;
                }
                else if( XMLParser.MatchValue(plainXML, "CompanyMarkerValidation", "Validated"))
                {
                    m_isValidated = true;
                    LogHelper.Debug("Company Marker Validation Passed!");
                    return true;
                }
                else if (XMLParser.MatchValue(plainXML, "CompanyMarkerValidation", "Not Validated"))
                {
                    m_isValidated = false;
                    LogHelper.Debug("Company Marker Validation Failed!");
                    return false;
                }
                else
                {
                    m_isValidated = false;
                    LogHelper.Warn("Invalid Validation Response!");
                    return false;
                }
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
