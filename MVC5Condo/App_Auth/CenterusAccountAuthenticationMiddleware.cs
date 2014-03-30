using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Net.Http;
using Microsoft.Owin;
using Microsoft.Owin.Logging;
using Microsoft.Owin.Security.DataHandler;
using Microsoft.Owin.Security.DataProtection;
using Microsoft.Owin.Security.Infrastructure;
using Owin;
 
namespace MVC5Condo.App_Auth
{
    public class CenterusAccountAuthenticationMiddleware : AuthenticationMiddleware<CenterusAccountAuthenticationOptions>
    {
        private readonly ILogger _logger;
        private readonly HttpClient _httpClient;
 
        /// <summary>
        /// Initializes a <see cref="CenterusAccountAuthenticationMiddleware"/>
        /// </summary>
        /// <param name="next">The next middleware in the OWIN pipeline to invoke</param>
        /// <param name="app">The OWIN application</param>
        /// <param name="options">Configuration options for the middleware</param>
        public CenterusAccountAuthenticationMiddleware(
            OwinMiddleware next,
            IAppBuilder app,
            CenterusAccountAuthenticationOptions options)
            : base(next, options)
        {
            if (string.IsNullOrWhiteSpace(Options.ClientId))
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Properties.Resources.Exception_OptionMustBeProvided, "ClientId"));
            }
            if (string.IsNullOrWhiteSpace(Options.ClientSecret))
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Properties.Resources.Exception_OptionMustBeProvided, "ClientSecret"));
            }
 
            _logger = app.CreateLogger<CenterusAccountAuthenticationMiddleware>();
 
            if (Options.Provider == null)
            {
                Options.Provider = new CenterusAccountAuthenticationProvider();
            }
            if (Options.StateDataFormat == null)
            {
                IDataProtector dataProtecter = app.CreateDataProtector(
                    typeof(CenterusAccountAuthenticationMiddleware).FullName,
                    Options.AuthenticationType, "v1");
                Options.StateDataFormat = new PropertiesDataFormat(dataProtecter);
            }
 
            _httpClient = new HttpClient(ResolveHttpMessageHandler(Options));
            _httpClient.Timeout = Options.BackchannelTimeout;
            _httpClient.MaxResponseContentBufferSize = 1024 * 1024 * 10; // 10 MB
        }
 
        /// <summary>
        /// Provides the <see cref="AuthenticationHandler"/> object for processing authentication-related requests.
        /// </summary>
        /// <returns>An <see cref="AuthenticationHandler"/> configured with the <see cref="MicrosoftAccountAuthenticationOptions"/> supplied to the constructor.</returns>
        protected override AuthenticationHandler<CenterusAccountAuthenticationOptions> CreateHandler()
        {
            return new CenterusAccountAuthenticationHandler(_httpClient, _logger);
        }
 
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Managed by caller")]
        private static HttpMessageHandler ResolveHttpMessageHandler(CenterusAccountAuthenticationOptions options)
        {
            HttpMessageHandler handler = options.BackchannelHttpHandler ?? new WebRequestHandler();
 
            // If they provided a validator, apply it or fail.
            if (options.BackchannelCertificateValidator != null)
            {
                // Set the cert validate callback
                var webRequestHandler = handler as WebRequestHandler;
                if (webRequestHandler == null)
                {
                    throw new InvalidOperationException(Properties.Resources.Exception_ValidatorHandlerMismatch);
                }
                webRequestHandler.ServerCertificateValidationCallback = options.BackchannelCertificateValidator.Validate;
            }
 
            return handler;
        }
    }
}