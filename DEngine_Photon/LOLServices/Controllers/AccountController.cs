using LOLServices.Helpers;
using LOLServices.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;

namespace LOLServices.Controllers
{
    [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
    public class AccountController : Controller
    {
        private static readonly DateTime utcRootTime = new DateTime(1970, 1, 1);
        private static readonly DateTime itcRootTime = new DateTime(1970, 1, 1, 7, 0, 0);
        private static readonly Dictionary<string, int> storeChargeAmount;

        static AccountController()
        {
            storeChargeAmount = new Dictionary<string, int>();

            storeChargeAmount.Add("gold20", 20);
            storeChargeAmount.Add("gold50", 60);
            storeChargeAmount.Add("gold100", 120);
            storeChargeAmount.Add("gold200", 250);
            storeChargeAmount.Add("gold500", 688);
            storeChargeAmount.Add("silver25000", 25000);
        }

        private static string GetHashString(string input, string hashname)
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

        [HttpGet]
        [ServiceRight]
        public ContentResult Register(string username, string password, string email, string nickname)
        {
            try
            {
                if (username == null || password == null || email == null)
                    return RetCode.ErrorCodes[-1];

                using (MainDBDataContext dbContext = new MainDBDataContext())
                {
                    Account theAccount = dbContext.Accounts.FirstOrDefault(acc => acc.UserName == username);
                    if (theAccount != null)
                        return Content("-2\nDuplicateUserName");

                    theAccount = dbContext.Accounts.FirstOrDefault(acc => acc.NickName == nickname);
                    if (theAccount != null)
                        return Content("-3\nDuplicateNickName");

                    theAccount = new Account();
                    theAccount.UserName = username;
                    theAccount.Password = GetHashString(password, "MD5");
                    theAccount.Email = email;
                    theAccount.NickName = nickname?? "Guest";
                    theAccount.CreateTime = DateTime.Now;

                    dbContext.Accounts.InsertOnSubmit(theAccount);
                    dbContext.SubmitChanges();

                    return Content(string.Format("{0}\nSuccess", theAccount.UserId));
                }
            }
            catch (Exception ex)
            {
                return Content(string.Format("-1\nException: {0}", ex.Message));
            }
        }

        [HttpGet]
        [ServiceRight]
        public ContentResult SignIn(string username, string password)
        {
            try
            {
                if (username == null || password == null)
                    return RetCode.ErrorCodes[-1];

                using (MainDBDataContext dbContext = new MainDBDataContext())
                {
                    Account theAccount = dbContext.Accounts.FirstOrDefault(acc => acc.UserName == username);
                    if (theAccount == null)
                        return Content("-2\nUserNotFound");

                    if (theAccount.Password != GetHashString(password, "MD5"))
                        return Content("0\nInvalidPassword");

                    return Content(string.Format("{0}\n{1}", theAccount.UserId, theAccount.NickName));
                }
            }
            catch (Exception ex)
            {
                return Content(string.Format("-1\nException: {0}", ex.Message));
            }
        }

        [HttpGet]
        [ServiceRight]
        public ContentResult ChangePass(string username, string oldpass, string newpass)
        {
            try
            {
                if (username == null || oldpass == null || newpass == null)
                    return RetCode.ErrorCodes[-1];

                using (MainDBDataContext dbContext = new MainDBDataContext())
                {
                    Account theAccount = dbContext.Accounts.FirstOrDefault(acc => acc.UserName == username);
                    if (theAccount == null)
                        return Content("-2\nUserNotFound");

                    if (theAccount.Password != GetHashString(oldpass, "MD5"))
                        return Content("-3\nInvalidPassword");

                    theAccount.Password = GetHashString(newpass, "MD5");
                    dbContext.SubmitChanges();

                    return RetCode.ErrorCodes[0];
                }
            }
            catch (Exception ex)
            {
                return Content(string.Format("-1\nException: {0}", ex.Message));
            }
        }

        [HttpGet]
        [ServiceRight]
        public ContentResult ChargeCard(int? worldid, int? userid, int? cardtype, string cardSeri, string cardCode)
        {
            try
            {
                if (worldid == null || userid == null || cardtype == null)
                    return RetCode.ErrorCodes[-1];

                string cardType = "";
                switch (cardtype.Value)
                {
                    //vinaphone, mobifone, viettel, gate, vcoin, zing, bit
                    case 1:
                        cardType = "vinaphone";
                        break;
                    case 2:
                        cardType = "mobifone";
                        break;
                    case 3:
                        cardType = "viettel";
                        break;
                    default:
                        return RetCode.ErrorCodes[-1];
                }

                using (MainDBDataContext dbContext = new MainDBDataContext())
                {
                    World world = dbContext.Worlds.FirstOrDefault(w => w.WorldId == worldid);
                    if (world == null)
                        return RetCode.ErrorCodes[-2];

                    Account acc = dbContext.Accounts.FirstOrDefault(a => a.UserId == userid);
                    if (acc == null)
                        return RetCode.ErrorCodes[-3];

                    var payResult = _1PayService.ChargeCard(cardType, cardCode, cardSeri);
                    int retCode = Int32.Parse(payResult.status);

                    CashLog log = new CashLog()
                    {
                        LogTime = DateTime.Now,
                        UserId = acc.UserId,
                        WorldId = worldid.Value,
                        VenderId = "1PAY",
                        CardType = cardType,
                        CardSeri = cardSeri,
                        CardCode = cardCode,
                        Amount = payResult.amount,
                        RetCode = retCode,
                    };

                    dbContext.CashLogs.InsertOnSubmit(log);
                    dbContext.SubmitChanges();

                    if (retCode == 0)
                        return Content(string.Format("{0}\nSuccess", payResult.amount));
                    else
                        return Content(string.Format("0\nChargeCard failed! Error: {0}", payResult.description));
                }
            }
            catch (Exception ex)
            {
                return Content(string.Format("-1\nException: {0}", ex.Message));
            }
        }

        [ServiceRight]
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ContentResult GoogleStore(int? worldid, int? userid, string product, string token)
        {
            try
            {
                if (worldid == null || userid == null)
                    return RetCode.ErrorCodes[-1];

                if (!storeChargeAmount.ContainsKey(product))
                    return RetCode.ErrorCodes[-1];

                using (MainDBDataContext dbContext = new MainDBDataContext())
                {
                    World world = dbContext.Worlds.FirstOrDefault(w => w.WorldId == worldid);
                    if (world == null)
                        return RetCode.ErrorCodes[-2];

                    Account acc = dbContext.Accounts.FirstOrDefault(a => a.UserId == userid);
                    if (acc == null)
                        return RetCode.ErrorCodes[-3];

                    CashLog curLog = dbContext.CashLogs.FirstOrDefault(l => l.UserId == userid.Value && l.CardCode == token);
                    if (curLog != null)
                        return RetCode.ErrorCodes[-5];

                    var purchaseState = StoreService.GetGooglePurchaseState(product, token);
                    if (purchaseState == null || purchaseState.PurchaseState != 0)
                        return RetCode.ErrorCodes[-6];

                    int cashAmount = storeChargeAmount[product];

                    string cardTye = "Gold";
                    if (cashAmount > 1000)
                        cardTye = "Silver";

                    CashLog log = new CashLog()
                    {
                        LogTime = DateTime.Now,
                        UserId = acc.UserId,
                        WorldId = worldid.Value,
                        VenderId = "GOOGLE",
                        CardType = cardTye,
                        CardSeri = product,
                        CardCode = token,
                        Amount = cashAmount,
                        RetCode = 0,
                    };

                    dbContext.CashLogs.InsertOnSubmit(log);
                    dbContext.SubmitChanges();

                    return Content(string.Format("{0}\nSuccess", cashAmount));
                }
            }
            catch (Exception ex)
            {
                LogService.WriteLine(ex);
                return Content(string.Format("-1\nException: {0}", ex.Message));
            }
        }

        [ServiceRight]
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ContentResult AppleStore(int? worldid, int? userid, string product, string receipt)
        {
            try
            {
                if (worldid == null || userid == null)
                    return RetCode.ErrorCodes[-1];

                if (!storeChargeAmount.ContainsKey(product))
                    return RetCode.ErrorCodes[-1];

                using (MainDBDataContext dbContext = new MainDBDataContext())
                {
                    World world = dbContext.Worlds.FirstOrDefault(w => w.WorldId == worldid);
                    if (world == null)
                        return RetCode.ErrorCodes[-2];

                    Account acc = dbContext.Accounts.FirstOrDefault(a => a.UserId == userid);
                    if (acc == null)
                        return RetCode.ErrorCodes[-3];

                    var purchaseState = StoreService.GetApplePurchaseState(receipt);
                    if (purchaseState.Status != 0)
                        return RetCode.ErrorCodes[-7];

                    if (!purchaseState.ProductId.StartsWith(product))
                        return RetCode.ErrorCodes[-1];

                    CashLog curLog = dbContext.CashLogs.FirstOrDefault(l => l.UserId == userid.Value && l.CardCode == purchaseState.TransactionId);
                    if (curLog != null)
                        return RetCode.ErrorCodes[-5];

                    int cashAmount = storeChargeAmount[product];

                    string cardTye = "Gold";
                    if (cashAmount > 1000)
                        cardTye = "Silver";

                    CashLog log = new CashLog()
                    {
                        LogTime = DateTime.Now,
                        UserId = acc.UserId,
                        WorldId = worldid.Value,
                        VenderId = "APPLE",
                        CardType = cardTye,
                        CardSeri = purchaseState.ProductId,
                        CardCode = purchaseState.TransactionId,
                        Amount = cashAmount,
                        RetCode = 0,
                    };

                    dbContext.CashLogs.InsertOnSubmit(log);
                    dbContext.SubmitChanges();

                    return Content(string.Format("{0}\nSuccess", cashAmount));
                }
            }
            catch (Exception ex)
            {
                LogService.WriteLine(ex);
                return Content(string.Format("-1\nException: {0}", ex.Message));
            }
        }

        [HttpGet]
        [ServiceRight]
        public ContentResult MicrosoftStore(int worldid, string username, string token)
        {
            return RetCode.ErrorCodes[-1];
        }
    }
}
