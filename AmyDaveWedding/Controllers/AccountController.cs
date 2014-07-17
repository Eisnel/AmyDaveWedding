using System;
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
using EisnelShared;
using System.Text.RegularExpressions;

namespace AmyDaveWedding.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        public AccountController()
        {
            ApplicationContext = new ApplicationDbContext();
            // Setting LazyLoadingEnabled to false breaks UserManager.GetLogins(userId).
            //ApplicationContext.Configuration.LazyLoadingEnabled = false;
            ApplicationContext.Database.Log = s => System.Diagnostics.Debug.WriteLine(s);

            UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(ApplicationContext));
        }

        //public AccountController(UserManager<ApplicationUser> userManager)
        //{
        //    UserManager = userManager;
        //}

        private ApplicationDbContext ApplicationContext { get; set; }

        private UserManager<ApplicationUser> UserManager { get; set; }

        private async Task<ApplicationUser> LoadCurrentUserAsync()
        {
            return await FindUserByIdAsync(User.Identity.GetUserId());
        }

        public async Task<ApplicationUser> FindUserByIdAsync(string userId)
        {
            if( string.IsNullOrEmpty(userId) )
            {
                return null;
            }
            var user = await ApplicationContext.Users.Include(u => u.Invitee).FirstOrDefaultAsync(u => u.Id == userId);
            //var user = await ApplicationContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
            //await ApplicationContext.Entry(user).Reference(u => u.Invitee).LoadAsync();
            return user;
        }

        //
        // GET: /Account/Rsvp
        public async Task<ActionResult> Rsvp()
        {
            RsvpModel model;

            // string userId = User.Identity.GetUserId();
            //var user = await UserManager.FindByIdAsync(userId);
            // var user = await ApplicationContext.Users.Include("Invitee").FirstOrDefaultAsync(u => u.Id == userId);
            var user = await LoadCurrentUserAsync();
            var invitee = user != null ? user.Invitee : null;
            if (invitee == null)
            {
                ViewBag.UserId = User.Identity.GetUserId();
                return View("NoInvitee");
            }
            else
            {
                ViewBag.Invitee = invitee;
                // ViewBag.InviteeName = invitee.Name;

                var groupedInvitees = await GetGroupedInvitees(invitee);
                ViewBag.GroupedInvitees = groupedInvitees;
                ViewBag.PlusOneKnown = groupedInvitees.Any(i => i.IsKnown);

                model = new RsvpModel()
                {
                    Attending = invitee.Attending,
                    ChildCount = invitee.ChildCount,
                    InterestedInChildCare = invitee.InterestedInChildCare,
                    AttendingRehearsal = invitee.AttendingRehearsal,
                    // RsvpDate = user.RsvpDate,
                    Note = invitee.Note
                };

                // Only set model.PlusOneAttending to a non-null value if there's
                // at least one grouped Invitee with a non-null Attending value:
                if (groupedInvitees.Any(i => i.Attending == true))
                {
                    model.PlusOneAttending = true;
                }
                else if (groupedInvitees.Any(i => i.Attending == false))
                {
                    model.PlusOneAttending = false;
                }
            }

            return View(model);
        }

        //
        // POST: /Account/Rsvp
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Rsvp(RsvpModel model)
        {
            var user = await LoadCurrentUserAsync();
            var invitee = user != null ? user.Invitee : null;
            ViewBag.Invitee = invitee;
            if (ModelState.IsValid)
            {
                if (invitee == null)
                {
                    ViewBag.UserId = User.Identity.GetUserId();
                    return View("NoInvitee");
                }
                else
                {
                    user.Attending = model.Attending;
                    user.RsvpDate = DateTime.Now;

                    invitee.Attending = model.Attending;
                    invitee.RsvpDate = user.RsvpDate;

                    invitee.ChildCount = model.ChildCount;
                    invitee.InterestedInChildCare = model.InterestedInChildCare;
                    invitee.AttendingRehearsal = model.AttendingRehearsal;
                    invitee.Note = model.Note;

                    var groupedInvitees = await GetGroupedInvitees(invitee);
                    ViewBag.GroupedInvitees = groupedInvitees;
                    ViewBag.PlusOneKnown = groupedInvitees.Any(i => i.IsKnown);
                    if (model.PlusOneAttending != null)
                    {
                        // Update groups Invitees based on model.PlusOneAttending:
                        foreach (var i in groupedInvitees)
                        {
                            i.Attending = model.PlusOneAttending;
                        }
                    }

                    await ApplicationContext.SaveChangesAsync();

                    ViewBag.Attending = invitee.Attending;
                    return View("RsvpConfirm", model);
                }
            }

            // If we got this far, something failed, so redisplay form
            return View(model);
        }

        private async Task<IEnumerable<Invitee>> GetGroupedInvitees(Invitee invitee)
        {
            //bool b = invitee.Group.NullOrWhiteSpace();
            if (string.IsNullOrWhiteSpace(invitee.Group))
            {
                return Enumerable.Empty<Invitee>();
            }
            var query = ApplicationContext.Invitees.Where(i => i.Group == invitee.Group && i.Id != invitee.Id);
            return await query.ToListAsync();
        }

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
            if( true )
            {
                // This site doesn't support registration with a username/password.
                // We're only allowing social network logins.
                return View(model);
            }

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
            ViewBag.Username = User.Identity.GetUserName();
            ViewBag.UserLoginProviderTypes = GetUserLoginProviderTypes();
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
            ViewBag.UserLoginProviderTypes = GetUserLoginProviderTypes();
            return View(model);
        }

        private IEnumerable<string> GetUserLoginProviderTypes()
        {
            IList<UserLoginInfo> linkedAccounts = UserManager.GetLogins(User.Identity.GetUserId());
            return linkedAccounts.Select(l => l.LoginProvider).ToList();
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
        // A number of vendor-specific URIs are routed here,
        // such as: /signin-google
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
            string name = null;
            string email = null;
            if (loginInfo.Login.LoginProvider == "Facebook")
            {
                name = GetClaimValue(result.Identity, "urn:facebook:name");
            }
            else if (loginInfo.Login.LoginProvider == "Google")
            {
                name = GetClaimValue(result.Identity, "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name");
                email = GetClaimValue(result.Identity, "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress");
            }
            else if (loginInfo.Login.LoginProvider == "Twitter")
            {
                var userInfo = await GetUserDataFromTwitter(loginInfo.DefaultUserName);
                if (userInfo != null)
                {
                    name = userInfo.FullName;
                }
            }

            var userName = loginInfo.DefaultUserName + loginInfo.Login.LoginProvider;
            userName += name ?? "";
            Regex rgx = new Regex("[^a-zA-Z0-9]");
            userName = rgx.Replace(userName, "");

            return new ExternalLoginConfirmationViewModel() { UserName = userName, Name = name, Email = email };
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
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl, String loginProvider)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Manage");
            }

            if (ModelState.IsValid)
            {
                // Verify that this user is actually invited.
                // We asked for their full name and zip code.
                // Verify that there's an Invitee with that zip code
                // who either has:
                // 1. The correct last name.
                // 2. The correct full name.
                // Try to strip off their last name and check to see 
                // Ask for a last name and zip code.
                // Compare to known Invitees in our database.
                // If no match, set an error message and proceed to
                // the end of this method to redisplay the previous form.
                // If a match is found, attach that Invitee
                // to the ApplicationUser created below.
                // ...

                var fullName = model.Name.Trim().ToLower();
                var lastName = fullName.Split().Last().Trim();
                if( lastName.Length < 3 )
                {
                    lastName = null;
                }
                var zipCode = model.ZipCode.Trim();

                var query = ApplicationContext.Invitees.Where(i => !i.LockedFromUserAssignment && i.IsKnown && i.ZipCode == zipCode);
                if (!string.IsNullOrWhiteSpace(lastName))
                {
                    query = query.Where(i => i.Name.Contains(lastName));
                }
                else
                {
                    query = query.Where(i => i.Name.Contains(fullName) || fullName.Contains(i.Name));
                }
                //var query = from i in WeddingContext.Invitees
                //            where i.LockedFromUserAssignment == false
                //            && i.ZipCode == zipCode
                //            && i.Name.Contains(lastName)
                //            select i;
                // var sql = ((System.Data.Objects.ObjectQuery)query).ToTraceString();
                var invitees = await query.ToListAsync();

                if (invitees.Count() > 1 && model.InviteeId != null && model.InviteeId > 0)
                {
                    // Even if an Invitee Id was passed, we still perform the query above.
                    // That prevents somebody from simply guessing in an Invitee Id.
                    // Now we search the results for an Invitee with the passed Id:
                    var matchesId = invitees.Where(i => i.Id == model.InviteeId).ToList();
                    if (matchesId.Any())
                    {
                        invitees = matchesId;
                    }
                }
                else if( invitees.Count() > 1 && lastName != null )
                {
                    // Then we did a search by lastName and found multiple matches.
                    // Let's see if any of these were exact matches of the fullName,
                    // and if so use that instead.
                    var matchesFullName = invitees.Where(i => string.Equals(i.Name, fullName, StringComparison.OrdinalIgnoreCase)).ToList();
                    if (matchesFullName.Any())
                    {
                        invitees = matchesFullName;
                    }
                }

                // Let's see if we found a matching Invitee that we can use
                // without asking the user for further confirmation.
                bool matchFound = false;
                if (invitees.Count() == 1 && model.InviteeId != null)
                {
                    // Then we found one match, and it's the InviteeId
                    // that was passed in the model. That means that we
                    // already showed the user a confirmation page asking
                    // him/her to verify which Invitee they are, and they
                    // selected one. So we're good to go.
                    matchFound = true;
                }
                else if (invitees.Count() == 1)
                {
                    // Then we found one matching Invitee,
                    // and there was no model.InviteeId passed
                    // (which means that we haven't already asked
                    // this user to pick which invitee they are).
                    // Our search is fairly permissive.
                    // If the match isn't exact, we'll simply ask
                    // the user to verify that we've got it right.
                    // This may happen due to people using shortened
                    // forms of their names, in which case it won't be
                    // a big deal. But this could also happen because
                    // we've gotten a name wrong. They'll still be able
                    // to login and attach to that Invitee, but at least
                    // this will prompt them to contact us and make
                    // a correction to our records.
                    var match = invitees.First();
                    if (string.Equals(match.Name, fullName, StringComparison.OrdinalIgnoreCase))
                    {
                        // It's an exact match (case insensitive).
                        matchFound = true;
                    }
                    // If it's not an exact match, that doesn't meant that we can't use it.
                    // It means that we want to show it to the user for verification.
                    // Maybe they searched for "Jack Doe" at zip code 55555
                    // and we found the Invitee for "John Doe" at the same zip code.
                    // So show this to the user so that they can verify that this is
                    // the same person (matchFound is already false, so leave it that way).
                }

                if (matchFound)
                {
                    var invitee = invitees.First();

                    // Get the information about the user from the external login provider
                    var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                    if (info == null)
                    {
                        return View("ExternalLoginFailure");
                    }

                    var user = new ApplicationUser()
                    {
                        Invitee = invitee,
                        UserName = model.UserName,
                        Email = model.Email,
                        Name = model.Name
                    };
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
                else if( invitees.Any() )
                {
                    // Then we don't have an excact match, but we've got at lesat one potential match.
                    // Redisplay the form and request that they pick an Invitee.
                    // If there's only one Invitee then we're just asking them to verify.
                    ViewBag.MatchingInvitees = invitees;
                }
                else
                {
                    // Then no matching Invitees were found.
                    ModelState.AddModelError("Name", "Cannot find an invitee with this name/zip combo.");
                    ModelState.AddModelError("ZipCode", "Cannot find an invitee with this name/zip combo.");
                    ViewBag.NoInviteeFound = true;
                }
            }

            ViewBag.ReturnUrl = returnUrl;
            ViewBag.LoginProvider = loginProvider;
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
            var userId = User.Identity.GetUserId();
            var linkedAccounts = UserManager.GetLogins(userId);
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
                string modelField = "";
                if (error != null)
                {
                    if (error.Contains(" is already taken"))
                    {
                        modelField = "UserName";
                    }
                    else if (error.StartsWith("User name "))
                    {
                        modelField = "UserName";
                    }
                }
                ModelState.AddModelError(modelField, error);
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