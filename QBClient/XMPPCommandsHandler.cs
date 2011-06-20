using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;


using System.Threading;
using agsXMPP;
using agsXMPP.protocol;
using agsXMPP.protocol.client;


namespace QBClient
{
    //This class handles requests from the server
    //variables starting with "m_" implies a member variale
    //variables starting with "g_" implies a global variale
    class XMPPCommandsHandler
    {
        Thread m_onMessageThread;

        SyncHandler m_syncHandler;
        public SyncHandler SyncHandler
        {
            get
            {
                return m_syncHandler;
            }
            set
            {
                m_syncHandler = value;
            }
        }

        Auth m_auth;
        public Auth Auth
        {
            get
            {
                return m_auth;
            }
            set
            {
                m_auth = value;
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
        //delegate void UpdateEventListCallback(string text);

        XmppClientConnection m_xmppCon;
        QBClient m_qbClient;
        ListBox m_eventList;
        Label m_labelStatus;
        delegate void SetEventListCallback(agsXMPP.protocol.client.Message msg);
        delegate void AbortThreadDelegate(Object toBeAborted);
        const int m_timeout = 30 * 1000;    //in ms
        //Object thisLock = new Object();
        string m_token;
        bool m_isOnline;
        public bool IsOnline
        {
            get
            {
                return m_isOnline;
            }
        }


        public XMPPCommandsHandler(QBClient qbClient, Label labelStatus, ListBox eventList)
        {
            m_xmppCon = new XmppClientConnection();
            m_qbClient = qbClient;
            m_eventList = eventList;
            m_labelStatus = labelStatus;

            m_xmppCon.OnLogin += new ObjectHandler(OnLogin);
            m_xmppCon.OnRosterStart += new ObjectHandler(OnRosterStart);
            m_xmppCon.OnRosterEnd += new ObjectHandler(OnRosterEnd);
            m_xmppCon.OnRosterItem += new XmppClientConnection.RosterHandler(OnRosterItem);
            m_xmppCon.OnPresence += new agsXMPP.protocol.client.PresenceHandler(OnPresence);
            m_xmppCon.OnAuthError += new XmppElementHandler(OnAuthError);
            m_xmppCon.OnError += new ErrorHandler(OnError);
            m_xmppCon.OnClose += new ObjectHandler(OnClose);
            m_xmppCon.OnMessage += new agsXMPP.protocol.client.MessageHandler(OnMessage);
        }

        private void OnMessage(object sender, agsXMPP.protocol.client.Message msg)
        {
            // ignore empty messages (events)
            if (msg.Body == null)
                return;

            if (m_qbClient.InvokeRequired)
            {
                // Windows Forms are not Thread Safe, we need to invoke this :(
                // We're not in the UI thread, so we need to call m_qbClient.BeginInvoke				
                m_qbClient.BeginInvoke(new agsXMPP.protocol.client.MessageHandler(OnMessage), new object[] { sender, msg });
                return;
            }

            m_eventList.Items.Add(String.Format("Received Msg from:{0} type:{1}", msg.From.Bare, msg.Type.ToString()));
            m_eventList.Items.Add(msg.Body);
            m_eventList.SelectedIndex = m_eventList.Items.Count - 1;
            LogHelper.Debug(String.Format("Received Msg from:{0} type:{1}", msg.From.Bare, msg.Type.ToString()));
            LogHelper.Debug(msg.Body);

            XMPPMessage xMsg = new XMPPMessage(m_xmppCon, msg);

            //The getavailabethreads method will return the max the pool can hold, like 250 500
            //int a, b;
            //ThreadPool.GetAvailableThreads(out a, out b);

            //Here we avoid using threadpool to prevent the thread being recycled
            //ThreadPool.QueueUserWorkItem(this.ThreadPoolCallback, xMsg);

            ThreadStart starter = delegate { ThreadPoolCallback(xMsg); };
            if(m_onMessageThread != null && m_onMessageThread.IsAlive)
                m_onMessageThread.Join();
            m_onMessageThread = new Thread(starter);
            m_onMessageThread.Start();

            //Respond(msg);
        }

        private void OnClose(object sender)
        {
            if (m_qbClient.InvokeRequired)
            {
                // Windows Forms are not Thread Safe, we need to invoke this :(
                // We're not in the UI thread, so we need to call m_qbClient.BeginInvoke				
                m_qbClient.BeginInvoke(new ObjectHandler(OnClose), new object[] { sender });
                return;
            }

            m_isOnline = false;
            LogHelper.Debug("Disconnected to XMPP server");
            m_eventList.Items.Add("OnClose Connection closed");
            m_labelStatus.Text = "Offline";
            m_eventList.SelectedIndex = m_eventList.Items.Count - 1;

            
            Thread logoutThread = new Thread(this.Logout);
            logoutThread.Start();
        }

        private void OnError(object sender, Exception ex)
        {
            if (m_qbClient.InvokeRequired)
            {
                // Windows Forms are not Thread Safe, we need to invoke this :(
                // We're not in the UI thread, so we need to call m_qbClient.BeginInvoke				
                m_qbClient.BeginInvoke(new ErrorHandler(OnError), new object[] { sender, ex });
                return;
            }
            LogHelper.Debug("XMPP OnError");
            m_eventList.Items.Add("OnError");
            m_eventList.SelectedIndex = m_eventList.Items.Count - 1;
        }

        private void OnAuthError(object sender, agsXMPP.Xml.Dom.Element e)
        {
            if (m_qbClient.InvokeRequired)
            {
                // Windows Forms are not Thread Safe, we need to invoke this :(
                // We're not in the UI thread, so we need to call m_qbClient.BeginInvoke				
                m_qbClient.BeginInvoke(new XmppElementHandler(OnAuthError), new object[] { sender, e });
                return;
            }
            LogHelper.Debug("OnAuthError");
            m_eventList.Items.Add("OnAuthError");
            m_eventList.SelectedIndex = m_eventList.Items.Count - 1;
        }

        private void OnPresence(object sender, agsXMPP.protocol.client.Presence pres)
        {
            if (m_qbClient.InvokeRequired)
            {
                // Windows Forms are not Thread Safe, we need to invoke this :(
                // We're not in the UI thread, so we need to call m_qbClient.BeginInvoke				
                m_qbClient.BeginInvoke(new agsXMPP.protocol.client.PresenceHandler(OnPresence), new object[] { sender, pres });
                return;
            }
            LogHelper.Debug(String.Format("Received Presence from:{0} show:{1} status:{2}", pres.From.ToString(), pres.Show.ToString(), pres.Type.ToString()));
            m_eventList.Items.Add(String.Format("Received Presence from:{0} show:{1} status:{2}", pres.From.ToString(), pres.Show.ToString(), pres.Type.ToString()));
            m_eventList.SelectedIndex = m_eventList.Items.Count - 1;
        }

        private void OnRosterItem(object sender, agsXMPP.protocol.iq.roster.RosterItem item)
        {
            if (m_qbClient.InvokeRequired)
            {
                // Windows Forms are not Thread Safe, we need to invoke this :(
                // We're not in the UI thread, so we need to call m_qbClient.BeginInvoke				
                m_qbClient.BeginInvoke(new XmppClientConnection.RosterHandler(OnRosterItem), new object[] { sender, item });
                return;
            }
            LogHelper.Debug(String.Format("Received Contact {0}", item.Jid.Bare));
            m_eventList.Items.Add(String.Format("Received Contact {0}", item.Jid.Bare));
            m_eventList.SelectedIndex = m_eventList.Items.Count - 1;
        }

        private void OnRosterEnd(object sender)
        {
            if (m_qbClient.InvokeRequired)
            {
                // Windows Forms are not Thread Safe, we need to invoke this :(
                // We're not in the UI thread, so we need to call m_qbClient.BeginInvoke				
                m_qbClient.BeginInvoke(new ObjectHandler(OnRosterEnd), new object[] { sender });
                return;
            }
            LogHelper.Debug("OnRosterEnd");
            m_eventList.Items.Add("OnRosterEnd");
            m_eventList.SelectedIndex = m_eventList.Items.Count - 1;

            // Send our own presence to teh server, so other epople send us online
            // and the server sends us the presences of our contacts when they are
            // available
            m_xmppCon.SendMyPresence();
        }

        private void OnRosterStart(object sender)
        {
            if (m_qbClient.InvokeRequired)
            {
                // Windows Forms are not Thread Safe, we need to invoke this :(
                // We're not in the UI thread, so we need to call m_qbClient.BeginInvoke				
                m_qbClient.BeginInvoke(new ObjectHandler(OnRosterStart), new object[] { sender });
                return;
            }
            LogHelper.Debug("OnRosterStart");
            m_eventList.Items.Add("OnRosterStart");
            m_eventList.SelectedIndex = m_eventList.Items.Count - 1;
        }

        private void OnLogin(object sender)
        {
            if (m_qbClient.InvokeRequired)
            {
                // Windows Forms are not Thread Safe, we need to invoke this :(
                // We're not in the UI thread, so we need to call m_qbClient.BeginInvoke				
                m_qbClient.BeginInvoke(new ObjectHandler(OnLogin), new object[] { sender });
                return;
            }
            m_isOnline = true;
            LogHelper.Debug("Connected to XMPP server");
            m_eventList.Items.Add("OnLogin");
            m_labelStatus.Text = "Online";
            m_eventList.SelectedIndex = m_eventList.Items.Count - 1;
        }


        public void ThreadPoolCallback(Object threadContext)
        {
            XmppClientConnection m_xmppCon = ((XMPPMessage)threadContext).Connection;
            agsXMPP.protocol.client.Message msg = ((XMPPMessage)threadContext).Msg;
            //if (!Thread.CurrentThread.Name.Equals(msg.From.Resource))
            //{
            //Thread.CurrentThread.Name = msg.From.Resource;
            //Thread.CurrentThread.Name = msg.From.Resource + "@" + System.DateTime.Now.AddMilliseconds(m_timeout);
            //}
            Respond(msg);
        }

        void Respond(agsXMPP.protocol.client.Message msg)
        {
            QBCommandsHandler qbCH = null;
            bool responded = false;
            string toJid = msg.From.Bare + "/" + msg.From.Resource;
            string command = msg.Body.Trim();

            try
            {
                ThreadPool.QueueUserWorkItem(Waiter, Thread.CurrentThread);
                switch (command)
                {
                    case "/Status":
                        Send(toJid, "<Status>Alive</Status>");
                        responded = true;
                        break;
                    case "/ConnectorVersion":
                        //temporally hard coded
                        Send(toJid, "<ConnectorVersion>" + 0.1 + "</ConnectorVersion>");
                        responded = true;
                        break;
                    case "/Shutdown":
                        Send(toJid, "<Status>ShuttingDown</Status>");
                        responded = true;
                        break;
                    case "/SyncDone":
                        //temp solution
                        if (m_syncHandler != null)
                            m_syncHandler.SyncDone();
                        Send(toJid, "<Status>Alive</Status>");
                        responded = true;
                        break;
                    default:
                        if (command.StartsWith("/QB/"))
                        {
                            //To debug the timeout
                            //Thread.Sleep(m_timeout*2);

                            qbCH = new QBCommandsHandler();
                            if (qbCH.ConnectToQB())
                            {
                                QBSession(toJid, command, qbCH);
                                qbCH.DisconnectFromQB();
                            }
                            else
                                Send(toJid, "<QuickbooksError>Failed to begin QuickBook session!</QuickbooksError>");
                            responded = true;
                        }
                        break;
                }
                //abort from inside will only raise an exception 
                //Thread.CurrentThread.Abort();
            }
            catch (ThreadAbortException ex)
            {
                LogHelper.Error(ex);
                if (responded == false) //For thread is aborted from outside, e.Message.Equals("Timeout!") will not return true
                    Send(toJid, "<QuickbooksError>Timeout!</QuickbooksError>");
            }
            finally
            {
                if (qbCH != null)
                {
                    if (qbCH.Ticket != null)
                        qbCH.DisconnectFromQB();
                }
                //Thread.CurrentThread.Abort();
            }
        }

        private void Waiter(Object parentThread)
        {
            try
            {
                //Thread thrdTimer = new Thread(new ThreadStart(this.Timer));
                //thrdTimer.Start();
                //thrdTimer.Join();
                //Thread.Sleep(m_timeout);
                int count = 0;
                int checkInterval = 50; //check if the parentThread is done every half a second.
                bool isTimeout = true;
                while (count < m_timeout)
                {
                    if (((Thread)parentThread).IsAlive)
                    {
                        count += checkInterval;
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
                    if (((Thread)parentThread).IsAlive)
                    {
                        Exception timeoutEx = new Exception("Timeout!");
                        throw timeoutEx;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
                if (ex.Message.Equals("Timeout!"))
                {
                    /*When a thread calls Abort on itself, the effect is similar to throwing 
                     * an exception; the ThreadAbortException happens immediately, and the result 
                     * is predictable. However, if one thread calls Abort on another thread, the abort 
                     * interrupts whatever code is running. There is a chance that a static constructor
                     * could be aborted. In rare cases, this might prevent instances of that class from
                     * being created in that application domain. In the .NET Framework versions 1.0 and
                     * 1.1, there is a chance the thread could abort while a finally block is running, 
                     * in which case the finally block is aborted.*/
                    //lock (thisLock)
                    //{

                    //}
                    AbortThreadDelegate abd = new AbortThreadDelegate(AbortThread);
                    abd.Invoke(parentThread);
                    Thread.CurrentThread.Abort("Timeout!");
                }
            }
        }
        private void AbortThread(Object toBeAborted)
        {
            // do we need to switch threads?
            if (m_qbClient.InvokeRequired)
            {
                AbortThreadDelegate abd = new AbortThreadDelegate(AbortThread);
                object[] argu = { toBeAborted };
                m_qbClient.BeginInvoke(abd, argu);
                return;
            }

            //somehow aborting a thread could be dangerous
            ((Thread)toBeAborted).Abort("Timeout!");
        }

        private void Timer()
        {
            Thread.Sleep(5000);
        }

        private void QBSession(String toJid, string command, QBCommandsHandler qbCH)
        {
            if (qbCH.Ticket == null)
            {
                Send(toJid, "<QuickbooksError>Null m_ticket Error!</QuickbooksError>");
                return;
            }
            switch (command)
            {
                case "/QB/Status":
                    //Send(toJid, "<QBStatus>Alive</QBStatus>");
                    Send(toJid, qbCH.QBStatus());
                    break;
                case "/QB/QBXMLVersionsForSession":
                    Send(toJid, qbCH.QBXMLVersion());
                    break;
                case "/QB/CompanyFileName":
                    //Send(toJid, "<Status>Alive</Status>");
                    Send(toJid, qbCH.QBCompanyFileName());
                    break;
                default:
                    if (command.StartsWith("/QB/ProcessRequest"))
                        //Send(toJid, "<Status>Alive</Status>");
                        Send(toJid, qbCH.QBProcessRequest(command.Replace("/QB/ProcessRequest", "")));
                    /*
                     * the validation would be responsed through Http
                else if (command.StartsWith("/QB/CompanyMarkerValidation"))
                    //Send(toJid, "<Status>Alive</Status>");
                    qbCH.QBProcessPlainXML("CompanyMarkerValidation", command.Replace("/QB/CompanyMarkerValidation", ""));
                else if (command.StartsWith("/QB/SyncMarkerValidation"))
                    //Send(toJid, "<Status>Alive</Status>");
                    qbCH.QBProcessPlainXML("CompanyMarkerValidation", command.Replace("/QB/SyncMarkerValidation", ""));
                     */
                    //else if (command.StartsWith("/QB/SyncDone"))
                        //Send(toJid, "<Status>Alive</Status>");
                        //qbCH.QBProcessPlainXML("SyncDone", command.Replace("/QB/SyncDone", ""));
                        //wake up the timer of synchandler
                        //m_syncHandler.SyncDone();
                        //without receiving any response, the xmpp connection won't get poped from the queue
                        //Send(toJid, qbCH.QBStatus());
                    break;
            }
        }
        private void Send(String toJid, string message)
        {
            agsXMPP.protocol.client.Message msg = new agsXMPP.protocol.client.Message();
            msg.Type = agsXMPP.protocol.client.MessageType.chat;
            msg.To = new Jid(toJid);
            msg.Body = message;

            m_xmppCon.Send(msg);

            //m_eventList.Items.Add(String.Format("Sent Msg to:{0} type:{1}", msg.To.Bare, msg.Type.ToString()));
            //m_eventList.Items.Add(msg.Body);
            //m_eventList.SelectedIndex = m_eventList.Items.Count - 1;

            //The statements above are replaced by the method below, for m_eventList is not created in the thread
            SetEventList(msg);
        }
        private void SetEventList(agsXMPP.protocol.client.Message msg)
        {
            try
            {
                if (m_eventList.InvokeRequired)
                {
                    SetEventListCallback d = new SetEventListCallback(SetEventList);
                    m_eventList.Invoke(d, new object[] { msg });
                }
                else
                {
                    m_eventList.Items.Add(String.Format("Sent Msg to:{0} type:{1}", msg.To.Bare, msg.Type.ToString()));
                    m_eventList.Items.Add(msg.Body);
                    m_eventList.SelectedIndex = m_eventList.Items.Count - 1;
                    LogHelper.Debug(String.Format("Sent Msg to:{0} type:{1}", msg.To.Bare, msg.Type.ToString()));
                    LogHelper.Debug(msg.Body);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
            }

        }
        public void Connnect(string username, string password, string server, string token)
        {
            try
            {
                if (!m_isOnline)
                {
                    m_token = token;
                    Jid jidUser = new Jid(username + "@" + server);
                    m_xmppCon.Username = jidUser.User;
                    m_xmppCon.Server = jidUser.Server;
                    m_xmppCon.Password = password;
                    m_xmppCon.AutoResolveConnectServer = true;
                    m_xmppCon.Open();
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
            }
        }
        public void Disconnect()
        {
            try
            {
                if (m_isOnline)
                {
                    if (m_onMessageThread != null && m_onMessageThread.IsAlive)
                        m_onMessageThread.Join();
                    m_xmppCon.Close();
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
            }
        }
    }
}


