using System;
using System.Collections.Generic;
using System.Text;

using System.Xml;

namespace QBClient
{
    class User
    {
        string m_username;
        string m_password;
        string m_salt;
        string m_pcId;
        string m_status;
        string m_id;
        string m_loginTime;
        string m_type;
        string m_syncTime;
        string m_token;
        public User(string username, string password)
        {
            m_username = username;
            m_password = password;
            m_token = "999";
            m_status = "Offline";
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
        public string Username
        {
            get
            {
                return m_username;
            }
            set
            {
                m_username = value;
            }
        }
        public string Password
        {
            get
            {
                return m_password;
            }
            set
            {
                m_password = value;
            }
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
        public void AssignAccount(string xmlResponse)
        {
            XmlDocument xDoc = new XmlDocument();
            xDoc.LoadXml(xmlResponse);

            XmlNodeList xToken = xDoc.GetElementsByTagName("token");
            this.Token = xToken[0].InnerText;

            XmlNodeList xId = xDoc.GetElementsByTagName("id");
            this.Id = xId[0].InnerText;
        }
    }
}
