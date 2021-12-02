using Demo2FAGooogleAuthenticator.Models;
using Google.Authenticator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Demo2FAGooogleAuthenticator.Controllers
{
    public class HomeController : Controller
    {
        private const string key = "qaz123!@@)(*";

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginModels login)
        {
            string message = "";
            bool status = false;
            if (login.Username == "Admin" && login.Password == "Password1")
            {
                status = true;
                message = "2FA Verification";
                Session["Username"] = login.Username;

                TwoFactorAuthenticator tfa = new TwoFactorAuthenticator();
                var setupInfo = tfa.GenerateSetupCode("Dotnet Awesome", login.Username, key, 300, 300);
                ViewBag.BarcodeImageUrl = setupInfo.QrCodeSetupImageUrl;
                ViewBag.SetupCode = setupInfo.ManualEntryKey;
            }
            else
            {
                message = "Invalid credential";
            }
            ViewBag.Message = message;
            ViewBag.Status = status;
            return View();
        }

        public ActionResult MyProfile()
        {
            if (Session["Username"] == null || Session["IsValid2FA"] == null || !(bool)Session["IsValid2FA"])
            {
                return RedirectToAction("Login");
            }
            ViewBag.Message = "Welcome " + Session["Username"].ToString();
            return View();
        }

        public ActionResult Verify2FA()
        {
            var token = Request["passcode"];
            TwoFactorAuthenticator tfa = new TwoFactorAuthenticator();
            bool isValid = tfa.ValidateTwoFactorPIN(key, token);
            if (isValid)
            {
                Session["IsValid2FA"] = true;
                return RedirectToAction("MyProfile", "Home");
            }
            return RedirectToAction("Login", "Home");
        }
    }
}