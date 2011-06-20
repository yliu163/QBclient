using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;
using System.Web; //windows form has no ref to System.Web. Right click project, add reference, go to .net tab and select it

namespace QBClient
{
    class RestComm
    {
        //192.168.1.4
        //10.10.13.124
        //POST request login
        public const string URL_ACCOUNT_LOGIN = "http://10.10.13.124:3000/accounts/login.xml";
        //POST request explicit log out
        public const string URL_ACCOUNT_LOGOUT = "http://10.10.13.124:3000/accounts/logout.xml";
        //POST timely inform the server of the presence of the client
        public const string URL_ACCOUNT_PING = "http://10.10.13.124:3000/accounts/ping.xml";
        //POST request to check the company maker in the company data extention matches the record on the server side
        public const string URL_ACCOUNT_VALIDATE_COMPANY_MARKER = "http://10.10.13.124:3000/accounts/validatecompanymarker.xml";
        //POST send the company file location to server and request generate a company marker
        public const string URL_ACCOUNT_NEW_COMPANY_FILE = "http://10.10.13.124:3000/accounts/markcompanyfile.xml";
        //POST request to check the sync maker in the company data extention matches the record on the server side
        public const string URL_ACCOUNT_VALIDATE_SYNC_MARKER = "http://10.10.13.124:3000/accounts/validatesyncmarker.xml";
        //POST send the sync file location to server and request generate a sync marker
        public const string URL_ACCOUNT_NEW_SYNC_MARKER = "http://10.10.13.124:3000/accounts/marksync.xml";
        //POST send the sync request
        public const string URL_ACCOUNT_REQUEST_SYNC = "http://10.10.13.124:3000/accounts/requestsync.xml";

        //POST request to check the company maker in the company data extention matches the record on the server side not using queue
        public const string URL_ACCOUNT_VALIDATE_COMPANY_MARKER_NO_QUEUEING = "http://10.10.13.124:3000/accounts/validatecompanymarker_no_queueing.xml";
        //POST agree to sync or not
        public const string URL_AGREE_TO_FULL_SYNC = "http://10.10.13.124:3000/accounts/agree_to_full_sync.xml";


        /* Only the post method is employed to send Rest request
         * 
         * 
        public static string HttpGet(string url)
        {
            string result = null;
            try
            {
                HttpWebRequest req = WebRequest.Create(url)
                                     as HttpWebRequest;
                using (HttpWebResponse resp = req.GetResponse()
                                              as HttpWebResponse)
                {
                    StreamReader reader =
                        new StreamReader(resp.GetResponseStream());
                    result = reader.ReadToEnd();
                }
                return result;
            }
            catch (Exception e)
            {
                return result;
            }
        }

        public static string HttpAuth(string url,string username, string password)
        {
            string result = null;
            try
            {
                HttpWebRequest request
                    = WebRequest.Create(url) as HttpWebRequest;
                // Add authentication to request  
                request.Credentials = new NetworkCredential(username, password);

                // Get response  
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    // Get the response stream  
                    StreamReader reader = new StreamReader(response.GetResponseStream());
                    result = reader.ReadToEnd();
                    // Console application output  
                    //Console.WriteLine(reader.ReadToEnd());
                }
                return result;
            }
            catch (Exception e)
            {
                return result;
            }
        }
         */

        //public static bool isHttpAlive;

        private const int TIMEOUT = 1000 * 1000;  //default value of HttpWebResponse is 100*1000 ms

        public static string HttpPost(string url,
            string[] paramName, string[] paramVal)
        {
            string result = null;
            try
            {
                HttpWebRequest req = WebRequest.Create(new Uri(url))
                                     as HttpWebRequest;
                req.Timeout = TIMEOUT;
                req.Method = "POST";
                req.ContentType = "application/x-www-form-urlencoded";

                // Build a string with all the params, properly encoded.
                // We assume that the arrays paramName and paramVal are
                // of equal length:
                StringBuilder paramz = new StringBuilder();
                for (int i = 0; i < paramName.Length; i++)
                {
                    paramz.Append(paramName[i]);
                    paramz.Append("=");
                    paramz.Append(HttpUtility.UrlEncode(paramVal[i]));
                    paramz.Append("&");
                }

                // Encode the parameters as form data:
                byte[] formData =
                    UTF8Encoding.UTF8.GetBytes(paramz.ToString());
                req.ContentLength = formData.Length;

                // Send the request:
                using (Stream post = req.GetRequestStream())
                {
                    post.Write(formData, 0, formData.Length);
                }

                // Pick up the response:
                using (HttpWebResponse resp = req.GetResponse()
                                              as HttpWebResponse)
                {
                    StreamReader reader =
                        new StreamReader(resp.GetResponseStream());
                    result = reader.ReadToEnd();
                }

                return result;
            }
            catch (WebException ex)
            {
                //Http connection lost
                LogHelper.Error(ex);
                throw (ex);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
                return result;
            }
            finally
            {

            }
        }
        public static string SendRestRequest(string url, string[] paraName, string[] paraValue)
        {
            string xmlResponse = null;
            try
            {
                LogHelper.Debug(String.Format("Send Rest request through:{0}", url));
                LogHelper.Debug("Parameters:");
                for (int i = 0; i < paraName.Length; i++)
                {
                    LogHelper.Debug(String.Format("{0}:{1}", paraName[i], paraValue[i]));
                }
                xmlResponse = RestComm.HttpPost(url, paraName, paraValue);
                LogHelper.Debug(String.Format("Received response from server:{0}", xmlResponse));
                //isHttpAlive = true;
                if (xmlResponse == null)
                {
                    return null;
                }
                else
                {
                    return xmlResponse;
                }
            }
            catch (WebException ex)
            {
                //Http connection lost
                //isHttpAlive = false;
                LogHelper.Error(ex);
                throw (ex);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
                return null;
            }
            finally
            {

            }
        }
      
    }
}
