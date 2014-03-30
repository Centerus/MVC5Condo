using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Microsoft.Owin.Security.Cookies;
using MVC5Condo.App_Auth;
using Owin;

namespace MVC5Condo
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
            //    clientId: "ec9dadd2c1d29f9a707ff36194c0eafd7274c264",
            //    clientSecret: "d09459cb6961f75fea02");

            //app.UseTwitterAuthentication(
            //   consumerKey: "",
            //   consumerSecret: "");

            //app.UseFacebookAuthentication(
            //   appId: "484835288289451",
            //   appSecret: "88c48ed2f8bc745592062a3b1db7634e");

            //app.UseGoogleAuthentication();

            app.UseCenterusAccountAuthentication(
                clientId: "88acd98d45edbd1125f68072c44dcd758a1a3bad",
                clientSecret: "4a2c999e5713ebcbc348");
        }
    }
}