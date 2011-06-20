using System;
using System.Collections.Generic;
using System.Text;

using agsXMPP;
using agsXMPP.protocol;
using agsXMPP.protocol.client;

namespace QBClient
{
    //This class contains not only the message, but the connection
    //variables starting with "m_" implies a member variale
    //variables starting with "g_" implies a global variale
    class XMPPMessage
    {
        XmppClientConnection m_xmppCon;
        agsXMPP.protocol.client.Message m_msg;
        public XMPPMessage(XmppClientConnection xmppCon, agsXMPP.protocol.client.Message msg)
        {
            m_msg = msg;
            m_xmppCon = xmppCon;
        }
        public XmppClientConnection Connection { get { return m_xmppCon; } }
        public agsXMPP.protocol.client.Message Msg { get { return m_msg; } }
    }
}
