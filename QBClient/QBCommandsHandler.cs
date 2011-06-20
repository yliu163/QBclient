using System;
using System.Collections.Generic;
using System.Text;

using Interop.QBXMLRP2;


namespace QBClient
{
    class QBCommandsHandler
    {
        const int ERROR_CODE_MODAL_DIALOG = -2147220460;    //0x80040414
        RequestProcessor2 rp = null;
        string m_ticket = null;
        string m_appID = "LevionQuickBookClient";
        string m_appName = "LevionQuickBookClient";
        string m_maxVersion;
        string[] m_versions;

        private const int m_maxIterations = 10;

        public string Ticket
        {
            get
            {
                return m_ticket;
            }
        }
        public QBCommandsHandler()
        {
             //ConnectToQB();
        }
        ~QBCommandsHandler()
        {
             DisconnectFromQB();
        }
        
        //if connection esists or established, return true; else return false
        public bool ConnectToQB() {
            if (m_ticket == null)
            {
                rp = new RequestProcessor2Class();
                rp.OpenConnection(m_appID, m_appName);

                for (int i = 0; i < m_maxIterations; i++ )
                {
                    try
                    {
                        m_ticket = rp.BeginSession("", QBFileMode.qbFileOpenDoNotCare);
                    }
                    catch (System.Runtime.InteropServices.COMException ex)
                    {
                        LogHelper.Error(ex);
                        if (ex.ErrorCode == ERROR_CODE_MODAL_DIALOG)
                        {
                            int isClosed = WindowOperation.CloseModalDialog("qbw32");
                            //if (isClosed != WindowOperation.NOTCLOSED)
                            //{
                            //m_ticket = rp.BeginSession("", QBFileMode.qbFileOpenDoNotCare);
                            //}
                        }
                        //return "<QBStatus>" + ex.Message + "</Q>";
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Error(ex);
                    }
                    finally
                    {
                        if (m_ticket != null)
                        {
                            m_versions = rp.get_QBXMLVersionsForSession(m_ticket);
                            m_maxVersion = m_versions[m_versions.Length - 1];
                        }
                    }

                    if (m_ticket != null)
                        return true;
                }
                return false;
            }
            //return "ConnectToQB: Max Version:" + m_maxVersion;
            return true;
        }
        
        public void DisconnectFromQB() {
            if (m_ticket != null) {
                try {
                    rp.EndSession(m_ticket);
                    m_ticket = null;
                    rp.CloseConnection();
                }
                catch (Exception ex) {
                    LogHelper.Error(ex);
                    //MessageBox.Show(e.Message);
                }
            }
        }

        public string QBStatus()
        {
            string response = null;
            try
            {
                if (m_ticket != null)
                {
                    response = "<QBStatus>Alive</QBStatus>";
                }
                else
                    return "<QuickbooksError>Null m_ticket Error!</QuickbooksError>";
            }
            catch (System.Runtime.InteropServices.COMException ex)
            {
                LogHelper.Error(ex);
                //MessageBox.Show("COM Error Description = " + ex.Message, "COM error");
                return "<QuickbooksError>" + ex.Message + "</QuickbooksError>";
            }
            finally
            {
                if (m_ticket != null)
                {
                     rp.EndSession(m_ticket);
                     m_ticket = null;
                }
                if (rp != null)
                {
                    rp.CloseConnection();
                }
            };
            return response;
        }
        public string QBXMLVersion()
        {
            string response = null;
            try
            {
                if (m_ticket != null)
                {
                    response = "<QBXMLVersionsForSession>" + string.Join(",", m_versions) + "</QBXMLVersionsForSession>";
                }
                else
                    return "<QuickbooksError>Null m_ticket Error!</QuickbooksError>";
            }
            catch (System.Runtime.InteropServices.COMException ex)
            {
                LogHelper.Error(ex);
                //MessageBox.Show("COM Error Description = " + ex.Message, "COM error");
                return "<QuickbooksError>" + ex.Message + "</QuickbooksError>";
            }
            finally
            {
                if (m_ticket != null)
                {
                    rp.EndSession(m_ticket);
                    m_ticket = null;
                }
                if (rp != null)
                {
                    rp.CloseConnection();
                }
            };
            return response;
        }
        public string QBCompanyFileName()
        {
            string response = null;
            try
            {
                if (m_ticket != null)
                {
                    response = "<CompanyFileName>" + rp.GetCurrentCompanyFileName(m_ticket) + "</CompanyFileName>";
                }
                else
                    return "<QuickbooksError>Null m_ticket Error!</QuickbooksError>";
            }
            catch (System.Runtime.InteropServices.COMException ex)
            {
                LogHelper.Error(ex);
                //MessageBox.Show("COM Error Description = " + ex.Message, "COM error");
                return "<QuickbooksError>" + ex.Message + "</QuickbooksError>";
            }
            finally
            {
                if (m_ticket != null)
                {
                    rp.EndSession(m_ticket);
                    m_ticket = null;
                }
                if (rp != null)
                {
                    rp.CloseConnection();
                }
            };
            return response;
        }
        public string QBProcessRequest(string qbxml)
        {
            string response = null;
            try
            {
                 if (m_ticket != null)
                 {
                     response = rp.ProcessRequest(m_ticket, qbxml.Trim());
                 }
                 else
                     return "<QuickbooksError>Null m_ticket Error!</QuickbooksError>";
            }
            catch (System.Runtime.InteropServices.COMException ex)
            {
                LogHelper.Error(ex);
                //MessageBox.Show("COM Error Description = " + ex.Message, "COM error");
                return "<QuickbooksError>" + ex.Message + "</QuickbooksError>";
            }
            finally
            {
                if (m_ticket != null)
                {
                    rp.EndSession(m_ticket);
                    m_ticket = null;
                }
                if (rp != null)
                {
                    rp.CloseConnection();
                }
            };
            return response;
        }

        //extract the attribute value according to the tag Name
        public string QBProcessPlainXML(string tagName, string plainXML)
        {
            try
            {
               return XMLParser.ExtractValueByTagName(plainXML, tagName);
            }
            catch (System.Runtime.InteropServices.COMException ex)
            {
                //MessageBox.Show("COM Error Description = " + ex.Message, "COM error");
                LogHelper.Error(ex);
                return "<QuickbooksError>" + ex.Message + "</QuickbooksError>";
            }
            finally
            {
                if (m_ticket != null)
                {
                    rp.EndSession(m_ticket);
                    m_ticket = null;
                }
                if (rp != null)
                {
                    rp.CloseConnection();
                }
            };
        }

    }
}
