// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.Owin.Security.Provider;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Newtonsoft.Json.Linq;

namespace MVC5Condo.App_Auth
{
    /// <summary>
    /// Provides context information to middleware providers.
    /// </summary>
    public class CenterusAccountReturnEndpointContext : ReturnEndpointContext
    {
        /// <summary>
        /// Initializes a new <see cref="CenterusAccountReturnEndpointContext"/>.
        /// </summary>
        /// <param name="context">OWIN environment</param>
        /// <param name="ticket">The authentication ticket</param>
        public CenterusAccountReturnEndpointContext(
            IOwinContext context,
            AuthenticationTicket ticket)
            : base(context, ticket)
        {
        }
    }


    /// <summary>
    /// Contains information about the login session as well as the user <see cref="System.Security.Claims.ClaimsIdentity"/>.
    /// </summary>
    public class CenterusAccountAuthenticatedContext : BaseContext
    {
        /// <summary>
        /// Initializes a <see cref="MicrosoftAccountAuthenticatedContext"/>
        /// </summary>
        /// <param name="context">The OWIN environment</param>
        /// <param name="user">The JSON-serialized user</param>
        /// <param name="accessToken">The access token provided by the Microsoft authentication service</param>
        public CenterusAccountAuthenticatedContext(IOwinContext context, JObject user, string accessToken)
            : base(context)
        {
            IDictionary<string, JToken> userAsDictionary = user;

            User = user;
            AccessToken = accessToken;

            Id = User["ID"].ToString();
            Name = PropertyValueIfExists("user_login", userAsDictionary);
            FirstName = PropertyValueIfExists("first_name", userAsDictionary);
            LastName = PropertyValueIfExists("last_name", userAsDictionary);
            Email = PropertyValueIfExists("user_email", userAsDictionary);
        }

        /// <summary>
        /// Gets the JSON-serialized user
        /// </summary>
        public JObject User { get; private set; }

        /// <summary>
        /// Gets the access token provided by the Microsoft authenication service
        /// </summary>
        public string AccessToken { get; private set; }

        /// <summary>
        /// Gets the Microsoft Account user ID
        /// </summary>
        public string Id { get; private set; }

        /// <summary>
        /// Gets the user name
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the user first name
        /// </summary>
        public string FirstName { get; private set; }

        /// <summary>
        /// Gets the user last name
        /// </summary>
        public string LastName { get; private set; }

        /// <summary>
        /// Gets the user email address
        /// </summary>
        public string Email { get; private set; }

        /// <summary>
        /// Gets the <see cref="ClaimsIdentity"/> representing the user
        /// </summary>
        public ClaimsIdentity Identity { get; set; }

        /// <summary>
        /// Gets or sets a property bag for common authentication properties
        /// </summary>
        public AuthenticationProperties Properties { get; set; }

        private static string PropertyValueIfExists(string property, IDictionary<string, JToken> dictionary)
        {
            return dictionary.ContainsKey(property) ? dictionary[property].ToString() : null;
        }
    }
}
