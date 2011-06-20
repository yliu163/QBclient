using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace QBClient
{
    class XMLParser
    {
        public static string ExtractValueByTagName(string xmlResponse, string tagName)
        {
            try
            {
                LogHelper.Debug(String.Format("Extracting tag: {0} from xml string: {1}", tagName, xmlResponse));
                XmlDocument xDoc = new XmlDocument();
                xDoc.LoadXml(xmlResponse);
                string result = null;
                XmlNodeList xResult = xDoc.GetElementsByTagName(tagName);

                if (xResult.Count > 0)
                {
                    result = xResult[0].InnerText;
                    LogHelper.Debug(String.Format("Value of tag: {0} is: {1}", tagName, result));
                    return result;
                }
                else
                {
                    LogHelper.Debug(String.Format("Can't find tag: {0} in xml string: {1}", tagName, xmlResponse));
                    return result;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
                return null;
            }
        }
        public static bool MatchValue(string xmlResponse, string tagName, string toBeMatched)
        {
            try
            {
                LogHelper.Debug(String.Format("Extracting tag: {0} from xml string: {1}", tagName, xmlResponse));
                LogHelper.Debug(String.Format("To be compared with {0}", toBeMatched));
                XmlDocument xDoc = new XmlDocument();
                xDoc.LoadXml(xmlResponse);
                string result = null;
                XmlNodeList xResult = xDoc.GetElementsByTagName(tagName);

                if (xResult.Count > 0)
                {
                    result = xResult[0].InnerText;
                    LogHelper.Debug(String.Format("Value of tag: {0} is: {1}", tagName, result));
                    if (result.Equals(toBeMatched))
                    {
                        LogHelper.Debug(String.Format("Value of tag: {0} matches: {1}", tagName, toBeMatched));
                        return true;
                    }
                    else
                    {
                        LogHelper.Debug(String.Format("Value of tag: {0} doesn't match: {1}", tagName, toBeMatched));
                        return false;
                    }

                }
                else
                {
                    LogHelper.Debug(String.Format("Can't find tag: {0} in xml string: {1}", tagName, xmlResponse));
                    return false;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
                return false;
            }
        }
    }
}
