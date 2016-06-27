using ExitGames.Logging;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using System.Web;

namespace DEngine.HeroServer
{
    public class HttpResult
    {
        public int Code;
        public string Description;

        public HttpResult()
        {
            Code = -1;
            Description = "";
        }
    }

    public class HttpService
    {
        private static ILogger Log = LogManager.GetCurrentClassLogger();

        public static string GetResponseXml(string requestUrl)
        {
            try
            {
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(requestUrl);
                webRequest.UserAgent = @"DEngine.HeroServer HttpWebRequest Client";

                using (HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse())
                {
                    using (Stream resStream = webResponse.GetResponseStream())
                    {
                        using (StreamReader resReader = new StreamReader(resStream))
                        {
                            string resStr = resReader.ReadToEnd();
                            return resStr;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Warn(ex.Message);
                Log.Warn(ex.StackTrace);
                return "";
            }
        }

        public static HttpResult GetResponse(string requestUrl)
        {
            HttpResult result = new HttpResult();

            try
            {
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(requestUrl);
                webRequest.UserAgent = @"DEngine.HeroServer HttpWebRequest Client";

                using (HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse())
                {
                    using (Stream resStream = webResponse.GetResponseStream())
                    {
                        using (StreamReader resReader = new StreamReader(resStream))
                        {
                            string resStr = resReader.ReadToEnd();
                            string[] allLines = resStr.Split('\n');
                            if (allLines.Length >= 2)
                            {
                                Int32.TryParse(allLines[0], out result.Code);
                                result.Description = allLines[1];
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Warn(ex.Message);
                Log.Warn(ex.StackTrace);
            }

            return result;
        }

        public static HttpResult GetResponse(string requestUrl,  Dictionary<string, string> formData)
        {
            HttpResult result = new HttpResult();

            try
            {
                NameValueCollection formValues = HttpUtility.ParseQueryString(String.Empty);
                foreach (var item in formData)
                    formValues.Add(item.Key, item.Value);

                ASCIIEncoding ascii = new ASCIIEncoding();
                byte[] postBytes = ascii.GetBytes(formValues.ToString());

                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(requestUrl);
                webRequest.UserAgent = @"DEngine.HeroServer HttpWebRequest Client";
                webRequest.Method = "POST";
                webRequest.ContentType = "application/x-www-form-urlencoded";
                webRequest.ContentLength = postBytes.Length;

                using (Stream postStream = webRequest.GetRequestStream())
                {
                    postStream.Write(postBytes, 0, postBytes.Length);
                    postStream.Flush();
                }

                using (HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse())
                {
                    using (Stream resStream = webResponse.GetResponseStream())
                    {
                        using (StreamReader resReader = new StreamReader(resStream))
                        {
                            string resStr = resReader.ReadToEnd();
                            string[] allLines = resStr.Split('\n');
                            if (allLines.Length >= 2)
                            {
                                Int32.TryParse(allLines[0], out result.Code);
                                result.Description = allLines[1];
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Warn(ex.Message);
                Log.Warn(ex.StackTrace);
            }

            return result;
        }
    }
}
