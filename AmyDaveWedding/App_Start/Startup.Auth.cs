using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using System.Configuration;
using System.Diagnostics;
using AmyDaveWedding.Helpers;
using EisnelShared;
using System.Text;
using Microsoft.Owin.Security.Facebook;

namespace AmyDaveWedding
{
    public partial class Startup
    {
        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            // Enable the application to use a cookie to store information for the signed in user
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login")
            });
            // Use a cookie to temporarily store information about a user logging in with a third party login provider
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Uncomment the following lines to enable logging in with third party login providers
            //app.UseMicrosoftAccountAuthentication(
            //    clientId: "",
            //    clientSecret: "");

            {
                var twitterCredentials = ApiCredentialSource.TwitterCredentials;
                if (twitterCredentials != null)
                {
                    //Debug.WriteLine("Twitter AppId encrypted: " + twitterKey.Encrypt());
                    //Debug.WriteLine("Twitter AppSecret encrypted: " + twitterSecret.Encrypt());
                    //Debug.WriteLine("Twitter AppId: " + twitterCredentials.Key);
                    //string keyDpApi = twitterCredentials.Key.EncryptDpApi(Encoding.UTF8);
                    //string secretDpApi = twitterCredentials.Secret.EncryptDpApi(Encoding.UTF8);
                    //string keyAes = twitterCredentials.Key.EncryptAes(ApiCredentialSource.GetAesKey(), Encoding.UTF8);
                    //string secretAes = twitterCredentials.Secret.EncryptAes(ApiCredentialSource.GetAesKey(), Encoding.UTF8);
                    //string keyRoundTrip = keyAes.DecryptAes(ApiCredentialSource.GetAesKey(), Encoding.UTF8);

                    // Management portal: https://dev.twitter.com/
                    app.UseTwitterAuthentication(
                       consumerKey: twitterCredentials.Key,
                       consumerSecret: twitterCredentials.Secret);
                }
            }

            {
                //var facebookId = GetAppSetting("Facebook.AppId", "Facebook.AppIdEncrypted");
                //var facebookSecret = GetAppSetting("Facebook.AppSecret", "Facebook.AppSecretEncrypted");
                var facebookCredentials = ApiCredentialSource.FacebookCredentials;
                if (facebookCredentials != null)
                {
                    // Management portal: https://developers.facebook.com/apps

                    //Debug.WriteLine("Facebook AppId encrypted: " + facebookCredentials.Key.Encrypt());
                    //Debug.WriteLine("Facebook AppSecret encrypted: " + facebookCredentials.Secret.Encrypt());
                    //Debug.WriteLine("Facebook AppId: " + facebookCredentials.Key);
                    //string keyDpApi = facebookCredentials.Key.EncryptDpApi(Encoding.UTF8);
                    //string secretDpApi = facebookCredentials.Secret.EncryptDpApi(Encoding.UTF8);
                    //string keyAes = facebookCredentials.Key.EncryptAes(ApiCredentialSource.GetAesKey(), Encoding.UTF8);
                    //string secretAes = facebookCredentials.Secret.EncryptAes(ApiCredentialSource.GetAesKey(), Encoding.UTF8);
                    //string keyRoundTrip = keyAes.DecryptAes(ApiCredentialSource.GetAesKey(), Encoding.UTF8);

                    // What follows is a more complicated way of setting this up,
                    // but necessary to remove the default "user_friends" scope/permission.
                    // http://forums.asp.net/t/1927914.aspx?Adding+Facebook+scope+permissions+when+authenticating+users+Owin+
                    FacebookAuthenticationOptions fbao = new FacebookAuthenticationOptions();
                    fbao.AppId = facebookCredentials.Key;
                    fbao.AppSecret = facebookCredentials.Secret;
                    fbao.Scope.Add("email");
                    fbao.Scope.Add("public_profile");
                    // fbao.Scope.Add("user_friends");
                    fbao.SignInAsAuthenticationType = Microsoft.Owin.Security.AppBuilderSecurityExtensions.GetDefaultSignInAsAuthenticationType(app);
                    app.UseFacebookAuthentication(fbao);
                }
            }

            {
                var googleCredentials = ApiCredentialSource.GoogleCredentials;
                if (googleCredentials != null)
                {
                    //Debug.WriteLine("Google ClientId: " + googleCredentials.Key);
                    //string keyDpApi = googleCredentials.Key.EncryptDpApi(Encoding.UTF8);
                    //string secretDpApi = googleCredentials.Secret.EncryptDpApi(Encoding.UTF8);
                    //Debug.WriteLine("Google googleCredentials.Key encrypted: " + keyDpApi);
                    //Debug.WriteLine("Google ClientSecret encrypted: " + secretDpApi);
                    //string keyAes = googleCredentials.Key.EncryptAes(ApiCredentialSource.GetAesKey(), Encoding.UTF8);
                    //string secretAes = googleCredentials.Secret.EncryptAes(ApiCredentialSource.GetAesKey(), Encoding.UTF8);
                    //string keyRoundTrip = keyAes.DecryptAes(ApiCredentialSource.GetAesKey(), Encoding.UTF8);

                    // Management portal: https://console.developers.google.com/project
                    // http://www.asp.net/mvc/tutorials/mvc-5/create-an-aspnet-mvc-5-app-with-facebook-and-google-oauth2-and-openid-sign-on
                    // This requires Microsoft.Owin.Security.Google 2.1.0.
                    // Originally I just had 2.0.0 installed.
                    // Upgrading involved runninng this in the Package Manager Console:
                    // Update-Package Microsoft.Owin.Security.Google
                    // http://www.mattburkedev.com/app-dot-usegoogleauthentication-does-not-accept-2-arguments-azure-tutorial/
                    app.UseGoogleAuthentication(
                     clientId: googleCredentials.Key,
                     clientSecret: googleCredentials.Secret);
                    // app.UseGoogleAuthentication();
                }
            }
        }

        //private string GetAppSetting( string unencryptedName, string encryptedName )
        //{
        //    var value = ConfigurationManager.AppSettings[unencryptedName];
        //    if (string.IsNullOrWhiteSpace(value))
        //    {
        //        value = ConfigurationManager.AppSettings[encryptedName];
        //        if( !string.IsNullOrWhiteSpace(value) )
        //        {
        //            value = value.Decrypt();
        //        }
        //    }
        //    return value;
        //}
    }
}