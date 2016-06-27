using Google.Apis.AndroidPublisher.v2;
using Google.Apis.AndroidPublisher.v2.Data;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using LOLServices.Helpers;
using SimpleJSON;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace LOLServices.Models
{
    public class StoreService
    {
        public struct ApplePurchase
        {
            public int Status;
            public string ProductId;
            public string TransactionId;
        }

        public static ProductPurchase GetGooglePurchaseState(string productId, string tokenId)
        {
            X509Certificate2 certificate = new X509Certificate2(Common.GoogleStorePrivateKey, "notasecret", X509KeyStorageFlags.Exportable);

            ServiceAccountCredential credential = new ServiceAccountCredential(
               new ServiceAccountCredential.Initializer(Common.GoogleStoreServiceAccount)
               {
                   Scopes = new[] { AndroidPublisherService.Scope.Androidpublisher }
               }.FromCertificate(certificate));

            // Create the service.
            AndroidPublisherService service = new AndroidPublisherService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "Google Play Android Developer",
            });

            try
            {
                var request = service.Purchases.Products.Get(Common.GoogleStoreBundleId, productId, tokenId);
                return request.Execute();
            }
            catch (Exception ex)
            {
                LogService.WriteLine(ex);
                return null;
            }
        }

        public static ApplePurchase GetApplePurchaseState(string receiptData)
        {
            ApplePurchase applePurchase = new ApplePurchase() { Status = -1, ProductId = "", TransactionId = "" };

            try
            {
                WebRequest webRequest = WebRequest.Create(Common.AppleStoreVerifyUrl);
                webRequest.Method = "POST";
                webRequest.ContentType = "application/json; charset=utf-8";

                string jSonContent = string.Format("{{ \"receipt-data\": \"{0}\", \"password\": \"{1}\" }}", receiptData, Common.AppleStoreVerifyPassword);
                webRequest.ContentLength = jSonContent.Length;

                using (StreamWriter oneStreamWriter = new StreamWriter(webRequest.GetRequestStream(), Encoding.ASCII))
                {
                    oneStreamWriter.Write(jSonContent);
                }

                using (WebResponse webResponse = webRequest.GetResponse())
                {
                    using (StreamReader sr = new StreamReader(webResponse.GetResponseStream()))
                    {
                        string respose = sr.ReadToEnd().Trim();
                        JSONNode jSon = JSON.Parse(respose);

                        int statusCode = Int32.Parse(jSon["status"].Value);
                        applePurchase.Status = statusCode;

                        if (statusCode == 0)
                        {
                            JSONNode receipt = jSon["receipt"];
                            JSONNode in_app = receipt["in_app"];
                            JSONNode transaction = in_app.AsArray[0];

                            applePurchase.ProductId = transaction["product_id"].Value;
                            applePurchase.TransactionId = transaction["transaction_id"].Value;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.WriteLine(ex);
            }

            return applePurchase;
        }
    }
}
