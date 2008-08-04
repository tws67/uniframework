using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml;

namespace Uniframework
{
    /// <summary>
    /// Web端异常帮助类（AF吴頔）
    /// </summary>
    public class ExceptionUtility
    {
        /// <summary>
        /// Wraps the exception.
        /// </summary>
        /// <param name="ex">The ex.</param>
        /// <returns></returns>
        public static SoapException WrapException(Exception ex)
        {
            //serialize the exception object
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            formatter.Serialize(ms, ex);
            ms.Close();
            byte[] bs = ms.GetBuffer();
            //construct the string represents the exception(in bytes)
            string exceptionString = null;
            for (int i = 0; i < bs.Length; i++)
            {
                exceptionString += bs[i] + ".";
            }
            exceptionString = exceptionString.Substring(0, exceptionString.Length - 1);

            //put the exception string into an xml node
            XmlDocument doc = new XmlDocument();
            XmlNode node = doc.CreateNode(XmlNodeType.Element, SoapException.DetailElementName.Name, "");
            node.InnerXml = exceptionString;

            string url = string.Empty;
            if (HttpContext.Current != null)
                url = HttpContext.Current.Request.Url.ToString();

            //the real exception will be send to the client
            SoapException soapEx = new SoapException(ex.Message, SoapException.ClientFaultCode, url, node);

            return soapEx;
        }

        /// <summary>
        /// Uns the wrap exception.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ex">The ex.</param>
        /// <returns></returns>
        public static T UnWrapException<T>(SoapException ex) where T : Exception, new()
        {
            //get the string represents the exception(in bytes)
            string exceptionString = ex.Detail.InnerText;
            //get each byte for the exception
            string[] bstr = exceptionString.Split('.');
            byte[] bs = new byte[bstr.Length];
            for (int i = 0; i < bstr.Length; i++)
            {
                bs[i] = byte.Parse(bstr[i]);
            }
            //deserialize the exception object
            MemoryStream ms = new MemoryStream(bs);
            BinaryFormatter formatter = new BinaryFormatter();
            T exception = (T)formatter.Deserialize(ms);
            ms.Close();
            return exception;
        }
    }
}
