using _1Pay;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Web.Helpers;

namespace LOLServices.Models
{
    public class _1PayService
    {
        static My1Pay _1PayLib = new My1Pay();
        static string _1PayUrl = "https://api.1pay.vn/card-charging/v2/topup";
        static string _1PayAKey = "rn0x45ko7h9if16rqdk2";
        static string _1PaySKey = "ttiyfkqztynh1pyi0grwhpv0hdk8hm2z";

        public struct _1PayCardRsult
        {
            public string status;
            public int amount;
            public string description;

            public _1PayCardRsult(string errMsg)
            {
                status = "-1";
                amount = 0;
                description = errMsg;
            }
        }

        public static _1PayCardRsult ChargeCard(string type, string pin, string serial)
        {
            string result = "";

            try
            {
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(_1PayUrl);
                webRequest.Headers.Add("Accept-Language", "en-US,en;q=0.5");
                webRequest.KeepAlive = false;
                webRequest.ProtocolVersion = HttpVersion.Version10;
                webRequest.Method = "POST";
                webRequest.ContentType = "application/x-www-form-urlencoded";
                webRequest.UserAgent = "Mozilla/5.0";

                string signature = _1PayLib.generateSignature_Card(_1PayAKey, pin, serial, type, _1PaySKey);
                string postParams = string.Format("access_key={0}&type={1}&pin={2}&serial={3}&signature={4}", _1PayAKey, type, pin, serial, signature);

                byte[] postBuffer = Encoding.ASCII.GetBytes(postParams);
                webRequest.ContentLength = postBuffer.Length;

                using (Stream requestStream = webRequest.GetRequestStream())
                {
                    requestStream.Write(postBuffer, 0, postBuffer.Length);
                }

                using (HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse())
                {
                    using (Stream responseStream = webResponse.GetResponseStream())
                    {
                        using (StreamReader resReader = new StreamReader(responseStream))
                        {
                            result = resReader.ReadToEnd();
                        }
                    }
                }

                _1PayCardRsult jsonResult = Json.Decode<_1PayCardRsult>(result);
                return jsonResult;
            }
            catch (Exception e)
            {
                result = e.GetBaseException().ToString();
                return new _1PayCardRsult(result);
            }
        }
    }
}
