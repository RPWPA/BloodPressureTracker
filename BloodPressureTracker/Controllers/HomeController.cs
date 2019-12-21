using System.Web;
using System.Web.Mvc;
using BloodPressureTracker.Models;
using Microsoft.AspNet.Membership.OpenAuth;
using System.Collections.Specialized;
using DotNetOpenAuth.GoogleOAuth2;
using System.Web.Security;

namespace BloodPressureTracker.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Authorize(user User)
        {
            try
            {
                SignInServiceReference.SignInSoapClient S = new SignInServiceReference.SignInSoapClient();
                int userID = S.signingIn(User.name, User.password);
                if (userID == -1)
                {
                    User.LoginErrorMsg = "Invalid User Name or Password";
                    return View("Index", User);
                }
                else
                {
                    Session["UserID"] = userID;
                    return RedirectToAction("userHome", "User");
                }
            }
            catch { return View("Index", User); }
        }

        public ActionResult LogOut()
        {
            Session.Abandon();
            LogOff();
            return RedirectToAction("Index", "Home");
        }

        public ActionResult RedirectToGoogle()
        {
            string provider = "google";
            string returnUrl = "";
            return new ExternalLoginResult(provider, Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl }));
        }

        internal class ExternalLoginResult : ActionResult
        {
            public ExternalLoginResult(string provider, string returnUrl)
            {
                Provider = provider;
                ReturnUrl = returnUrl;
            }

            public string Provider { get; private set; }
            public string ReturnUrl { get; private set; }

            public override void ExecuteResult(ControllerContext context)
            {
                OpenAuth.RequestAuthentication(Provider, ReturnUrl);
            }
        }

        [AllowAnonymous]
        public ActionResult ExternalLoginCallback(string returnUrl)
        {
            string ProviderName = OpenAuth.GetProviderNameFromCurrentRequest();

            if (ProviderName == null || ProviderName == "")
            {
                NameValueCollection nvs = Request.QueryString;
                if (nvs.Count > 0)
                {
                    if (nvs["state"] != null)
                    {
                        NameValueCollection provideritem = HttpUtility.ParseQueryString(nvs["state"]);
                        if (provideritem["__provider__"] != null)
                        {
                            ProviderName = provideritem["__provider__"];
                        }
                    }
                }
            }

            GoogleOAuth2Client.RewriteRequest();

            var redirectUrl = Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl });
            var retUrl = returnUrl;
            var authResult = OpenAuth.VerifyAuthentication(redirectUrl);

            if (!authResult.IsSuccessful)
            {
                return Redirect(Url.Action("Index", "Home"));
            }

            // User has logged in with provider successfully
            // Check if user is already registered locally
            //You can call you user data access method to check and create users based on your model
            if (OpenAuth.Login(authResult.Provider, authResult.ProviderUserId, createPersistentCookie: false))
            {
                SignUpServiceReference.SignUpSoapClient S = new SignUpServiceReference.SignUpSoapClient();
                user User = new user();
                User.name = authResult.UserName;
                User.email = authResult.Provider;
                User.password = "1234";
                User.weight = User.age = 0;
                User.gender = "not set";
                User.BPsample = "120/80";

                int userID = S.signingUp(User.name, User.weight, User.age, User.gender[0], User.BPsample, User.email, User.password);
                if (userID != -1)
                {
                    Session["UserID"] = userID;

                    return RedirectToAction("userHome", "User");
                }
                else
                {
                    SignInServiceReference.SignInSoapClient Se = new SignInServiceReference.SignInSoapClient();
                    int userid = Se.signingIn(User.name, User.password);
                    if (userid == -1)
                    {
                        User.LoginErrorMsg = "Invalid User Name or Password";
                        return View("Index", User);
                    }
                    else
                    {
                        Session["UserID"] = userid;
                        return RedirectToAction("userHome", "User");
                    }
                }
            }

            //Get provider user details
            string ProviderUserId = authResult.ProviderUserId;
            string ProviderUserName = authResult.UserName;

            string Email = null;
            if (Email == null && authResult.ExtraData.ContainsKey("email"))
            {
                Email = authResult.ExtraData["email"];
            }

            if (User.Identity.IsAuthenticated)
            {
                // User is already authenticated, add the external login and redirect to return url
                OpenAuth.AddAccountToExistingUser(ProviderName, ProviderUserId, ProviderUserName, User.Identity.Name);
                return Redirect(Url.Action("userHome", "User"));
            }
            else
            {
                // User is new, save email as username
                string membershipUserName = Email ?? ProviderUserId;
                var createResult = OpenAuth.CreateUser(ProviderName, ProviderUserId, ProviderUserName, membershipUserName);

                if (!createResult.IsSuccessful)
                {
                    ViewBag.Message = "User cannot be created";
                    return View();
                }
                else
                {
                    // User created
                    if (OpenAuth.Login(ProviderName, ProviderUserId, createPersistentCookie: false))
                    {
                        return Redirect(Url.Action("userHome", "User"));
                    }
                }
            }
            return View();
        }

        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            //Call https://www.google.com/accounts/Logout if you want to logoff at provider
            return Redirect(Url.Action("Index", "Home"));
        }
    }
}