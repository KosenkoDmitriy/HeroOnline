using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LOLServices.Models
{
    public static class RetCode
    {
        public static readonly Dictionary<int, ContentResult> ErrorCodes;

        static RetCode()
        {
            ErrorCodes = new Dictionary<int, ContentResult>();
            ErrorCodes[0] = new ContentResult() { Content = "0\nSuccess" };
            ErrorCodes[-1] = new ContentResult() { Content = "-1\nInvalidParams" };
            ErrorCodes[-2] = new ContentResult() { Content = "-2\nInvalidWorld" };
            ErrorCodes[-3] = new ContentResult() { Content = "-3\nInvalidUser" };
            ErrorCodes[-4] = new ContentResult() { Content = "-4\nInvalidUserRight" };
            ErrorCodes[-5] = new ContentResult() { Content = "-5\nDuplicatePurchase" };
            ErrorCodes[-6] = new ContentResult() { Content = "-6\nInvalidGooglePurchase" };
            ErrorCodes[-7] = new ContentResult() { Content = "-7\nInvalidApplePurchase" };
        }
    }
}