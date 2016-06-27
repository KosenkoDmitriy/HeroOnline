using LOLServices.Models;
using System.Configuration;
using System.Web.Mvc;
using LukeSkywalker.IPNetwork;
using System;
using System.Collections.Generic;
using System.Net;

namespace LOLServices.Helpers
{
    public class RightActionFilter : ActionFilterAttribute
    {
        private static char[] splitChars = new char[] { ';' };
        protected static List<IPNetwork> AdminIPList;
        protected static List<IPNetwork> ManagerIPList;

        static RightActionFilter()
        {
            AdminIPList = new List<IPNetwork>();
            string adminIPs = ConfigurationManager.AppSettings["AdminIPs"];
            string[] adminAdds = adminIPs.Split(splitChars, StringSplitOptions.RemoveEmptyEntries);
            foreach (string address in adminAdds)
            {
                IPNetwork network = IPNetwork.Parse(address);
                AdminIPList.Add(network);
            }

            ManagerIPList = new List<IPNetwork>();
            string ManagerIPs = ConfigurationManager.AppSettings["ManagerIPs"];
            string[] managerAdds = ManagerIPs.Split(splitChars, StringSplitOptions.RemoveEmptyEntries);
            foreach (string address in managerAdds)
            {
                IPNetwork network = IPNetwork.Parse(address);
                ManagerIPList.Add(network);
            }
        }

        protected bool IsValidIP(List<IPNetwork> networkList, string address)
        {
            IPAddress remoteIP = IPAddress.Parse(address);
            foreach (var item in networkList)
            {
                if (IPNetwork.Contains(item, remoteIP))
                    return true;
            }

            return false;
        }
    }

    public class AdminRight : RightActionFilter
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var session = filterContext.HttpContext.Session;
            var request = filterContext.HttpContext.Request;
            var response = filterContext.HttpContext.Response;

            string remoteIP = request.UserHostAddress;
            if (!IsValidIP(AdminIPList, remoteIP))
            {
                filterContext.Result = new RedirectResult("/Home/InvalidRight");
                return;
            }

            string curUser = filterContext.HttpContext.User.Identity.Name + ";";
            string adminUsers = ConfigurationManager.AppSettings["AdminUsers"];
            if (!adminUsers.Contains(curUser))
            {
                filterContext.Result = new RedirectResult("/Home/InvalidRight");
                return;
            }

            base.OnActionExecuting(filterContext);
        }
    }

    public class ServiceRight : RightActionFilter
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var request = filterContext.HttpContext.Request;

            string remoteIP = request.UserHostAddress;
            if (!IsValidIP(AdminIPList, remoteIP))
            {
                filterContext.Result = RetCode.ErrorCodes[-4];
                return;
            }

            base.OnActionExecuting(filterContext);
        }
    }

    public class ManagerRight : RightActionFilter
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var session = filterContext.HttpContext.Session;
            var request = filterContext.HttpContext.Request;
            var response = filterContext.HttpContext.Response;

            string remoteIP = request.UserHostAddress;
            if (!IsValidIP(ManagerIPList, remoteIP))
            {
                filterContext.Result = new RedirectResult("/Home/InvalidRight");
                return;
            }

            string curUser = filterContext.HttpContext.User.Identity.Name + ";";
            string managerUsers = ConfigurationManager.AppSettings["ManagerUsers"];
            if (!managerUsers.Contains(curUser))
            {
                filterContext.Result = new RedirectResult("/Home/InvalidRight");
                return;
            }

            base.OnActionExecuting(filterContext);
        }
    }
}
