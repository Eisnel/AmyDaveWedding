﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using AmyDaveWedding.Models;
using Microsoft.AspNet.Identity.Owin;
using System.Net.Http;
using System.Net.Http.Headers;
using AmyDaveWedding.Helpers;
using System.Text;
using System.Diagnostics;

namespace AmyDaveWedding.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        public AccountController()
            : this(new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext())))
        {
            this.WeddingContext = new WeddingContext();
            this.WeddingContext.Database.Log = s => System.Diagnostics.Debug.WriteLine(s);
        }

        public AccountController(UserManager<ApplicationUser> userManager)
        {
            UserManager = userManager;
        }

        public UserManager<ApplicationUser> UserManager { get; private set; }

        private WeddingContext WeddingContext { get; private set; }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindAsync(model.UserName, model.Password);
                if (user != null)
                {
                    await SignInAsync(user, model.RememberMe);
                    return RedirectToLocal(returnUrl);
                }
                else
                {
                    ModelState.AddModelError("", "Invalid username or password.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser() { UserName = model.UserName, Email = model.Email };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    AddErrors(result);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // POST: /Account/Disassociate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Disassociate(string loginProvider, string providerKey)
        {
            ManageMessageId? message = null;
            IdentityResult result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));
            if (result.Succeeded)
            {
                message = ManageMessageId.RemoveLoginSuccess;
            }
            else
            {
                message = ManageMessageId.Error;
            }
            return RedirectToAction("Manage", new { Message = message });
        }

        //
        // GET: /Account/Manage
        public ActionResult Manage(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : message == ManageMessageId.Error ? "An error has occurred."
                : "";
            ViewBag.HasLocalPassword = HasPassword();
            ViewBag.ReturnUrl = Url.Action("Manage");
            return View();
        }

        //
        // POST: /Account/Manage
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Manage(ManageUserViewModel model)
        {
            bool hasPassword = HasPassword();
            ViewBag.HasLocalPassword = hasPassword;
            ViewBag.ReturnUrl = Url.Action("Manage");
            if (hasPassword)
            {
                if (ModelState.IsValid)
                {
                    IdentityResult result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
                    }
                    else
                    {
                        AddErrors(result);
                    }
                }
            }
            else
            {
                // User does not have a password so remove any validation errors caused by a missing OldPassword field
                ModelState state = ModelState["OldPassword"];
                if (state != null)
                {
                    state.Errors.Clear();
                }

                if (ModelState.IsValid)
                {
                    IdentityResult result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
                    }
                    else
                    {
                        AddErrors(result);
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var user = await UserManager.FindAsync(loginInfo.Login);
            if (user != null)
            {
                await SignInAsync(user, isPersistent: false);
                return RedirectToLocal(returnUrl);
            }
            else
            {
                var result = await AuthenticationManager.AuthenticateAsync(DefaultAuthenticationTypes.ExternalCookie);

                // If the user does not have an account, then prompt the user to create an account
                ViewBag.ReturnUrl = returnUrl;
                ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                var model = await GetExternalLoginModel(result, loginInfo);
                return View("ExternalLoginConfirmation", model);
            }
        }

        private async Task<ExternalLoginConfirmationViewModel> GetExternalLoginModel(AuthenticateResult result, ExternalLoginInfo loginInfo)
        {
            var model = new ExternalLoginConfirmationViewModel() { UserName = loginInfo.DefaultUserName };

            if (loginInfo.Login.LoginProvider == "Facebook")
            {
                model.Name = GetClaimValue(result.Identity, "urn:facebook:name");
            }
            else if (loginInfo.Login.LoginProvider == "Google")
            {
                model.Name = GetClaimValue(result.Identity, "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name");
                model.Email = GetClaimValue(result.Identity, "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress");
            }
            else if (loginInfo.Login.LoginProvider == "Twitter")
            {
                var userInfo = await GetUserDataFromTwitter(model.UserName);
                if (userInfo != null)
                {
                    model.Name = userInfo.FullName;
                }
            }
            return model;
        }

        private string GetClaimValue( ClaimsIdentity ci, string type )
        {
            var claim = ci.Claims.FirstOrDefault(c => c.Type == type);
            return claim != null ? claim.Value : null;
        }

        private async Task<SocialUserInfo> GetUserDataFromTwitter(string screenName)
        {
            ApiCredential twitterCredentials = ApiCredentialSource.TwitterCredentials;

            string accessToken = twitterCredentials.AccessToken;
            if (string.IsNullOrWhiteSpace(accessToken))
            {
                accessToken = await GetTwitterAccessToken();
                if (string.IsNullOrWhiteSpace(accessToken))
                {
                    return null;
                }
                twitterCredentials.AccessToken = accessToken;
            }

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(@"https://api.twitter.com");

                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                // client.DefaultRequestHeaders.Add("Accept", "application/json");
                // client.DefaultRequestHeaders.Add("Content-Type", "application/x-www-form-urlencoded;charset=UTF-8");
                
                //client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Get, "/1.1/users/show.json?screen_name=" + screenName);
                //req.Properties.Add("screen_name", screenName);

                // HttpResponseMessage response = await client.GetAsync("/1.1/users/show.json?screen_name=" + screenName);
                HttpResponseMessage response = await client.SendAsync(req);
                if (response.IsSuccessStatusCode)
                {
                    dynamic result = await response.Content.ReadAsAsync<dynamic>();

                    //string name = result.name;
                    //string profileImageUrl = result.profile_image_url;
                    return new SocialUserInfo { FullName = result.name, ProfileImageUrl = result.profile_image_url };
                }
                else
                {
                    //dynamic result = await response.Content.ReadAsAsync<dynamic>();
                    //string resultJsonStr = Convert.ToString(result);
                    string result = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine("Errored calling {0}: {1}", req.RequestUri, result);
                    return null;
                }
            }
        }

        private async Task<string> GetTwitterAccessToken()
        {
            //string auth = "xvz1evFS4wEEPTGEFPHBog" + ":" + "L8qq9PZyRg6ieKGEKhZolGC0vJWLw8iEJ88DRdyOg";
            var twitterCredentials = ApiCredentialSource.TwitterCredentials;
            string auth = HttpUtility.UrlEncode(twitterCredentials.Key) + ":" + HttpUtility.UrlEncode(twitterCredentials.Secret);
            byte[] authBytes = Encoding.UTF8.GetBytes(auth);
            auth = Convert.ToBase64String(authBytes);

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(@"https://api.twitter.com");
                
                client.DefaultRequestHeaders.Accept.Clear();
                // client.DefaultRequestHeaders.Add("Accept", "application/json");
                //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //client.DefaultRequestHeaders.Add("Authorization", "Basic " + auth);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", auth);

                //client.DefaultRequestHeaders.Add("Content-Type", "application/x-www-form-urlencoded;charset=UTF-8");
                HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Post, "oauth2/token");
                //req.Content = new StringContent("{\"name\":\"John Doe\",\"age\":33}", Encoding.UTF8, "application/json");
                req.Content = new StringContent("grant_type=client_credentials", Encoding.UTF8, "application/x-www-form-urlencoded");

                //var contentList = new List<KeyValuePair<string, string>> { new KeyValuePair<string, string>("grant_type", "client_credentials") };
                //var content = new FormUrlEncodedContent(contentList);
                //HttpResponseMessage response = await client.PostAsync("oauth2/token", content);
                HttpResponseMessage response = await client.SendAsync(req);
                if (response.IsSuccessStatusCode)
                {
                    dynamic result = await response.Content.ReadAsAsync<dynamic>();

                    string tokenType = result.token_type;
                    string accessToken = result.access_token;

                    return accessToken;

                    //Product product = await response.Content.ReadAsAsync<Product>();
                    //Console.WriteLine("{0}\t${1}\t{2}", product.Name, product.Price, product.Category);
                }
                else
                {
                    //dynamic result = await response.Content.ReadAsAsync<dynamic>();
                    //string resultJsonStr = Convert.ToString(result);
                    string result = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine("Errored calling {0}: {1}", req.RequestUri, result);
                    return null;
                }
            }
        }

        //
        // POST: /Account/LinkLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LinkLogin(string provider)
        {
            // Request a redirect to the external login provider to link a login for the current user
            return new ChallengeResult(provider, Url.Action("LinkLoginCallback", "Account"), User.Identity.GetUserId());
        }

        //
        // GET: /Account/LinkLoginCallback
        public async Task<ActionResult> LinkLoginCallback()
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.GetUserId());
            if (loginInfo == null)
            {
                return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
            }
            var result = await UserManager.AddLoginAsync(User.Identity.GetUserId(), loginInfo.Login);
            if (result.Succeeded)
            {
                return RedirectToAction("Manage");
            }
            return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Manage");
            }

            if (ModelState.IsValid)
            {
                // TODO: Verify that this user is actually invited.
                // Ask for a last name and zip code.
                // Compare to known Invitees in our database.
                // If no match, set an error message and proceed to
                // the end of this method to redisplay the previous form.
                // If a match is found, attach that Invitee
                // to the ApplicationUser created below.
                // ...

                var zipCode = "55555";
                var lastName = "Smith";

                // var invitees = WeddingContext.Invitees.Where(i => i.ZipCode == zipCode).Where(i => i.Name.Contains(lastName));
                var query = from i in WeddingContext.Invitees
                            where i.LockedFromUserAssignment == false
                            && i.ZipCode == zipCode
                            && i.Name.Contains(lastName)
                            select i;
                // var sql = ((System.Data.Objects.ObjectQuery)query).ToTraceString();
                var invitees = await query.ToListAsync();

                if (invitees.Count() == 1)
                {
                    // Get the information about the user from the external login provider
                    var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                    if (info == null)
                    {
                        return View("ExternalLoginFailure");
                    }
                    Invitee invitee = invitees.First();
                    var user = new ApplicationUser() { Invitee = invitee, UserName = model.UserName, Email = model.Email };
                    var result = await UserManager.CreateAsync(user);
                    if (result.Succeeded)
                    {
                        result = await UserManager.AddLoginAsync(user.Id, info.Login);
                        if (result.Succeeded)
                        {
                            await SignInAsync(user, isPersistent: false);
                            return RedirectToLocal(returnUrl);
                        }
                    }
                    AddErrors(result);
                }
                else if( invitees.Count() > 1 )
                {
                    // Multiple Invitees matched this criteria.
                    // Redisplay the form and request that they pick an Invitee.
                    ViewBag.MatchingInvitees = invitees;
                }
                else
                {
                    // Then no matching Invitees were found.
                    // TODO: Set an error then redisplay the form.

                    // ModelState.AddModelError("", "error description");
                    ViewBag.NoInviteeFound = true;
                }
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        [ChildActionOnly]
        public ActionResult RemoveAccountList()
        {
            var linkedAccounts = UserManager.GetLogins(User.Identity.GetUserId());
            ViewBag.ShowRemoveButton = HasPassword() || linkedAccounts.Count > 1;
            return (ActionResult)PartialView("_RemoveAccountPartial", linkedAccounts);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && UserManager != null)
            {
                UserManager.Dispose();
                UserManager = null;
            }
            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private async Task SignInAsync(ApplicationUser user, bool isPersistent)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            var identity = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, identity);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            Error
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        private class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri) : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties() { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}