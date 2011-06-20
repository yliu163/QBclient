using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace QBClient
{
    class Account
    {
        string m_account_name;
        string m_userId;
        string m_status;
        string m_id;
        string m_loginTime;
        string m_companyFile;
        string m_companyMarker;
        string m_syncMarker;
        string m_syncTime;
        string m_token;

        public Account(string token)
        {
            m_token = token;
            m_status = "Offline";
        }

        public string Id
        {
            get
            {
                return m_id;
            }
            set
            {
                m_id = value;
            }
        }
        public string Token
        {
            get
            {
                return m_token;
            }
            set
            {
                m_token = value;
            }
        }
        public string Status
        {
            get
            {
                return m_status;
            }
            set
            {
                m_status = value;
            }
        }
        public string CompanyFile
        {
            get
            {
                return m_companyFile;
            }
            set
            {
                m_companyFile = value;
            }
        }
        public string CompanyMarker
        {
            get
            {
                return m_companyMarker;
            }
            set
            {
                m_companyMarker = value;
            }
        }
        public string LoginTime
        {
            get
            {
                return m_loginTime;
            }
            set
            {
                m_loginTime = value;
            }
        }
        public void AssignAccount(string xmlResponse)
        {
            XmlDocument xDoc = new XmlDocument();
            xDoc.LoadXml(xmlResponse);

            XmlNodeList xToken = xDoc.GetElementsByTagName("token");
            this.Token = xToken[0].InnerText;
            XmlNodeList xCompanyFile = xDoc.GetElementsByTagName("company-file");
            this.CompanyFile = xCompanyFile[0].InnerText;
            XmlNodeList xCompanyMarker = xDoc.GetElementsByTagName("company-marker");
            this.CompanyMarker = xCompanyMarker[0].InnerText;
            XmlNodeList xId = xDoc.GetElementsByTagName("id");
            this.Id = xId[0].InnerText;
        }
        public void ClearAccount()
        {
            this.Token = "";
            this.CompanyFile = "";
            this.CompanyMarker = "";
            this.Id = "";
        }

    }
}
