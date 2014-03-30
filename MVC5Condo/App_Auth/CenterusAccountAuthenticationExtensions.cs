using System;
using Microsoft.Owin.Security;
using Owin;

namespace MVC5Condo.App_Auth
{

    public static class CenterusAccountAuthenticationExtensions
    {
        /// <summary>
        /// Authenticate users using Microsoft Account
        /// </summary>
        /// <param name="app">The <see cref="IAppBuilder"/> passed to the configuration method</param>
        /// <param name="options">Middleware configuration options</param>
        /// <returns>The updated <see cref="IAppBuilder"/></returns>
        public static IAppBuilder UseCenterusAccountAuthentication(this IAppBuilder app, CenterusAccountAuthenticationOptions options)
        {
            if (app == null)
            {
                throw new ArgumentNullException("app");
            }
            if (options == null)
            {
                throw new ArgumentNullException("options");
            }

            app.Use(typeof(CenterusAccountAuthenticationMiddleware), app, options);
            return app;
        }

        /// <summary>
        /// Authenticate users using Microsoft Account
        /// </summary>
        /// <param name="app">The <see cref="IAppBuilder"/> passed to the configuration method</param>
        /// <param name="clientId">The application client ID assigned by the Microsoft authentication service</param>
        /// <param name="clientSecret">The application client secret assigned by the Microsoft authentication service</param>
        /// <returns></returns>
        public static IAppBuilder UseCenterusAccountAuthentication(
            this IAppBuilder app,
            string clientId,
            string clientSecret)
        {
            return UseCenterusAccountAuthentication(
                app,
                new CenterusAccountAuthenticationOptions
                {
                    ClientId = clientId,
                    ClientSecret = clientSecret,
                });
        }
    }
}