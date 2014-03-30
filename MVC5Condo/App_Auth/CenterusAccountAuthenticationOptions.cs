using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Owin;
using Microsoft.Owin.Security;


namespace MVC5Condo.App_Auth
{
    public interface ICenterusAccountAuthenticationProvider
    {
        Task Authenticated(CenterusAccountAuthenticatedContext context);
        Task ReturnEndpoint(CenterusAccountReturnEndpointContext context);
    }


    public static class Constants
    {
        public const string DefaultAuthenticationType = "www.Centerus.com";
    }


    public class CenterusAccountAuthenticationOptions : AuthenticationOptions
    {
        /// <summary>
        /// Initializes a new <see cref="CenterusAccountAuthenticationOptions"/>.
        /// </summary>
        [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters",
            MessageId = "Microsoft.Owin.Security.MicrosoftAccount.MicrosoftAccountAuthenticationOptions.set_Caption(System.String)", Justification = "Not localizable")]
        public CenterusAccountAuthenticationOptions() : base(Constants.DefaultAuthenticationType)
        {
            Caption = Constants.DefaultAuthenticationType;
            CallbackPath = new PathString("/Account/ExternalLoginCallback");
            AuthenticationMode = AuthenticationMode.Passive;
            Scope = new List<string>();
            BackchannelTimeout = TimeSpan.FromSeconds(60);
            SignInAsAuthenticationType = Constants.DefaultAuthenticationType;
        }
 
        /// <summary>
        /// Gets or sets the a pinned certificate validator to use to validate the endpoints used
        /// in back channel communications belong to Centerus Account.
        /// </summary>
        /// <value>
        /// The pinned certificate validator.
        /// </value>
        /// <remarks>If this property is null then the default certificate checks are performed,
        /// validating the subject name and if the signing chain is a trusted party.</remarks>
        public ICertificateValidator BackchannelCertificateValidator { get; set; }
 
        /// <summary>
        /// Get or sets the text that the user can display on a sign in user interface.
        /// </summary>
        /// <remarks>
        /// The default value is 'www.Centerus.com'
        /// </remarks>
        public string Caption
        {
            get { return Description.Caption; }
            set { Description.Caption = value; }
        }
 
        /// <summary>
        /// The application client ID assigned by the Centerus OAuth2 authentication service.
        /// </summary>
        public string ClientId { get; set; }
 
        /// <summary>
        /// The application client secret assigned by the Centerus authentication service.
        /// </summary>
        public string ClientSecret { get; set; }
 
        /// <summary>
        /// Gets or sets timeout value in milliseconds for back channel communications with Centerus.
        /// </summary>
        /// <value>
        /// The back channel timeout.
        /// </value>
        public TimeSpan BackchannelTimeout { get; set; }
 
        /// <summary>
        /// The HttpMessageHandler used to communicate with Centerus.
        /// This cannot be set at the same time as BackchannelCertificateValidator unless the value 
        /// can be downcast to a WebRequestHandler.
        /// </summary>
        public HttpMessageHandler BackchannelHttpHandler { get; set; }
 
        /// <summary>
        /// A list of permissions to request. (Not used.)
        /// </summary>
        public IList<string> Scope { get; private set; }
 
        /// <summary>
        /// The request path within the application's base path where the user-agent will be returned.
        /// The middleware will process this request when it arrives.
        /// (Not used.)
        /// </summary>
        public PathString CallbackPath { get; set; }
 
        /// <summary>
        /// Gets or sets the name of another authentication middleware which will be responsible for actually issuing a user <see cref="System.Security.Claims.ClaimsIdentity"/>.
        /// </summary>
        public string SignInAsAuthenticationType { get; set; }
 
        /// <summary>
        /// Gets or sets the <see cref="ICenterusAccountAuthenticationProvider"/> used to handle authentication events.
        /// </summary>
        public ICenterusAccountAuthenticationProvider Provider { get; set; }
 
        /// <summary>
        /// Gets or sets the type used to secure data handled by the middleware.
        /// </summary>
        public ISecureDataFormat<AuthenticationProperties> StateDataFormat { get; set; }
    }
}