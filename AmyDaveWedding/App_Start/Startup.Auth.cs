using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using System.Configuration;
using EisnelShared;
using System.Diagnostics;
using AmyDaveWedding.Helpers;

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
                    //Debug.WriteLine("Facebook AppId encrypted: " + facebookCredentials.Key.Encrypt());
                    //Debug.WriteLine("Facebook AppSecret encrypted: " + facebookCredentials.Secret.Encrypt());
                    app.UseFacebookAuthentication(
                       appId: facebookCredentials.Key,
                       appSecret: facebookCredentials.Secret);
                }
            }

            app.UseGoogleAuthentication();
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