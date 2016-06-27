using LOLServices.Models;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;
using Recaptcha.Web;
using Recaptcha.Web.Mvc;
using System.Web.Security;

namespace LOLServices.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult InvalidRight()
        {
            return View();
        }

        public ActionResult SignIn(string username, string password, string returnUrl)
        {
            if (Request.HttpMethod.Equals("GET"))
            {
                if (Request.IsAuthenticated)
                    return RedirectToAction("Index");

                return View();
            }

            Session["UserData"] = null;

            UserModel userModel = UserModel.SignIn(username, password);
            if (userModel.UserId == -1)
                ModelState.AddModelError("_FORM", "Invalid Account");
            else if (userModel.UserId == -2)
                ModelState.AddModelError("_FORM", "Invalid Password");
            else
            {
                Session["UserData"] = userModel;
                FormsAuthentication.SetAuthCookie(username, false);
                return RedirectToAction("Index");
            }

            return View();
        }

        public ActionResult SignOut()
        {
            Session.Clear();
            FormsAuthentication.SignOut();
            return RedirectToAction("Index");
        }

        public ActionResult Register(string username, string password, string passwordconfirm)
        {
            /*if (Request.HttpMethod == "POST")
            {
                RecaptchaVerificationHelper recaptchaHelper = this.GetRecaptchaVerificationHelper();

                if (String.IsNullOrEmpty(recaptchaHelper.Response))
                {
                    ModelState.AddModelError("", "Captcha answer cannot be empty.");
                    return View();
                }

                RecaptchaVerificationResult recaptchaResult = recaptchaHelper.VerifyRecaptchaResponse();
                if (recaptchaResult != RecaptchaVerificationResult.Success)
                {
                    ModelState.AddModelError("", "Incorrect captcha answer.");
                }
            }*/

            return View();
        }
   }
}
