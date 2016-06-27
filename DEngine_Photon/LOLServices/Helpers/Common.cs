using System;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace LOLServices.Helpers
{
    public static class Common
    {
        public static readonly string AppRootDir;
        public static readonly string LogFileName;

        public static readonly string GoogleStoreBundleId;
        public static readonly string GoogleStoreServiceAccount;
        public static readonly string GoogleStorePrivateKey;

        public static readonly string AppleStoreVerifyUrl;
        public static readonly string AppleStoreVerifyPassword;

        static Common()
        {
            AppRootDir = AppDomain.CurrentDomain.BaseDirectory;
            LogFileName = AppRootDir + "Content\\LOLServices.log";

            GoogleStorePrivateKey = AppRootDir + "Content\\GoogleStore_BlueSky.p12";

            GoogleStoreBundleId = ConfigurationManager.AppSettings["GoogleBundleId"];
            GoogleStoreServiceAccount = ConfigurationManager.AppSettings["GoogleServiceAccount"];

            AppleStoreVerifyUrl = ConfigurationManager.AppSettings["AppleStoreVerifyUrl"];
            AppleStoreVerifyPassword = ConfigurationManager.AppSettings["AppleStoreVerifyPassword"];

            LogService.WriteLine("Common Initialized!");
        }

        public static string GetHashString(string input, string hashname)
        {
            using (HashAlgorithm hash = HashAlgorithm.Create(hashname))
            {
                byte[] data = hash.ComputeHash(Encoding.UTF8.GetBytes(input));
                StringBuilder strBld = new StringBuilder();
                for (int i = 0; i < data.Length; i++)
                    strBld.AppendFormat("{0:X2}", data[i]);
                return strBld.ToString();
            }
        }
    }
}