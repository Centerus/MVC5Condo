﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Infrastructure;
using Microsoft.Owin.Infrastructure;
using Microsoft.Owin.Logging;
using Newtonsoft.Json.Linq;

namespace MVC5Condo.App_Auth
{
    internal class CenterusAccountAuthenticationHandler : AuthenticationHandler<CenterusAccountAuthenticationOptions>
    {
        private const string TokenEndpoint = "https://www.centerus.com/oauth/request_token";
        private const string GraphApiEndpoint = "https://www.centerus.com/oauth/request_access";
 
        private readonly ILogger _logger;
        private readonly HttpClient _httpClient;
 
        public CenterusAccountAuthenticationHandler(HttpClient httpClient, ILogger logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }
 
        public override async Task<bool> InvokeAsync()
        {
            if (Options.CallbackPath.HasValue && Options.CallbackPath == Request.Path)
            {
                return await InvokeReturnPathAsync();
            }
            return false;
        }
 
        protected override async Task<AuthenticationTicket> AuthenticateCoreAsync()
        {
            // debug
            RequestCookieCollection cookies = HttpContext.Current.GetOwinContext().Request.Cookies;
            int count = cookies.Count();
            // end debug

            AuthenticationProperties properties = null;
            try
            {
                string code = null;
                string state = null;
 
                IReadableStringCollection query = Request.Query;
                IList<string> values = query.GetValues("code");
                if (values != null && values.Count == 1)
                {
                    code = values[0];
                }
                values = query.GetValues("state");
                if (values != null && values.Count == 1)
                {
                    state = values[0];
                }
 
                properties = Options.StateDataFormat.Unprotect(state);
                if (properties == null)
                {
                    return null;
                }
 
                // OAuth2 10.12 CSRF
                if (!ValidateCorrelationId(properties, _logger))
                {
                    return new AuthenticationTicket(null, properties);
                }
 
                var tokenRequestParameters = new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("client_id", Options.ClientId),
                    //new KeyValuePair<string, string>("redirect_uri", GenerateRedirectUri()),
                    new KeyValuePair<string, string>("client_secret", Options.ClientSecret),
                    new KeyValuePair<string, string>("code", code),
                    new KeyValuePair<string, string>("grant_type", "authorization_code"),
                };
 
                var requestContent = new FormUrlEncodedContent(tokenRequestParameters);
 
                HttpResponseMessage response = await _httpClient.PostAsync(TokenEndpoint, requestContent, Request.CallCancelled);
                response.EnsureSuccessStatusCode();
                string oauthTokenResponse = await response.Content.ReadAsStringAsync();
 
                JObject oauth2Token = JObject.Parse(oauthTokenResponse);
                var accessToken = oauth2Token["access_token"].Value<string>();
 
                if (string.IsNullOrWhiteSpace(accessToken))
                {
                    _logger.WriteWarning("Access token was not found");
                    return new AuthenticationTicket(null, properties);
                }
 
                HttpResponseMessage graphResponse = await _httpClient.GetAsync(
                    GraphApiEndpoint + "?access_token=" + Uri.EscapeDataString(accessToken), Request.CallCancelled);
                graphResponse.EnsureSuccessStatusCode();
                string accountString = await graphResponse.Content.ReadAsStringAsync();
                JObject accountInformation = JObject.Parse(accountString);
 
                var context = new CenterusAccountAuthenticatedContext(Context, accountInformation, accessToken);
                context.Identity = new ClaimsIdentity(
                    new[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, context.Id, "http://www.w3.org/2001/XMLSchema#string", Options.AuthenticationType),
                        new Claim(ClaimTypes.Name, context.Name, "http://www.w3.org/2001/XMLSchema#string", Options.AuthenticationType),
                        new Claim("urn:centerusaccount:id", context.Id, "http://www.w3.org/2001/XMLSchema#string", Options.AuthenticationType),
                        new Claim("urn:centerusaccount:name", context.Name, "http://www.w3.org/2001/XMLSchema#string", Options.AuthenticationType)
                    },
                    Options.AuthenticationType,
                    ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType);
                if (!string.IsNullOrWhiteSpace(context.Email))
                {
                    context.Identity.AddClaim(new Claim(ClaimTypes.Email, context.Email, "http://www.w3.org/2001/XMLSchema#string", Options.AuthenticationType));
                }

                await Options.Provider.Authenticated(context);
 
                context.Properties = properties;

                // debug
                cookies = HttpContext.Current.GetOwinContext().Request.Cookies;
                count = cookies.Count();
                // end debug

 
                return new AuthenticationTicket(context.Identity, context.Properties);
            }
            catch (Exception ex)
            {
                _logger.WriteWarning("Authentication failed", ex);
                return new AuthenticationTicket(null, properties);
            }
        }
 
        protected override Task ApplyResponseChallengeAsync()
        {
            if (Response.StatusCode != 401)
            {
                return Task.FromResult<object>(null);
            }
 
            AuthenticationResponseChallenge challenge = Helper.LookupChallenge(Options.AuthenticationType, Options.AuthenticationMode);
 
            if (challenge != null)
            {
                string baseUri = Request.Scheme + Uri.SchemeDelimiter + Request.Host + Request.PathBase;
 
                string currentUri = baseUri + Request.Path + Request.QueryString;
 
                string redirectUri = baseUri + Options.CallbackPath;
 
                AuthenticationProperties extra = challenge.Properties;
                if (string.IsNullOrEmpty(extra.RedirectUri))
                {
                    extra.RedirectUri = currentUri;
                }
 
                // OAuth2 10.12 CSRF
                GenerateCorrelationId(extra);
 
                // OAuth2 3.3 space separated                
                string scope = string.Join(" ", Options.Scope);
                // Centerus does not require a scope string, so scope can be null or whitespace.
 
                string state = Options.StateDataFormat.Protect(extra);
 
                string authorizationEndpoint =
                    "https://www.centerus.com/oauth/authorize" +
                        "?client_id=" + Uri.EscapeDataString(Options.ClientId) +
                        //"&scope=" + Uri.EscapeDataString(scope) + 
                        "&response_type=code" +
                        //"&redirect_uri=" + Uri.EscapeDataString(redirectUri) +
                        "&state=" + Uri.EscapeDataString(state);
 
                Response.StatusCode = 302;
                Response.Headers.Set("Location", authorizationEndpoint);
            }
 
            return Task.FromResult<object>(null);
        }
 
        public async Task<bool> InvokeReturnPathAsync()
        {
            // debug
            RequestCookieCollection cookies = HttpContext.Current.GetOwinContext().Request.Cookies;
            int count = cookies.Count();
            // end debug

            AuthenticationTicket model = await AuthenticateAsync();
            if (model == null)
            {
                _logger.WriteWarning("Invalid return state, unable to redirect.");
                Response.StatusCode = 500;
                return true;
            }
 
            var context = new CenterusAccountReturnEndpointContext(Context, model);
            context.SignInAsAuthenticationType = Options.SignInAsAuthenticationType;
            context.RedirectUri = model.Properties.RedirectUri;
            model.Properties.RedirectUri = null;
 
            await Options.Provider.ReturnEndpoint(context);
 
            if (context.SignInAsAuthenticationType != null && context.Identity != null)
            {
                ClaimsIdentity signInIdentity = context.Identity;
                if (!string.Equals(signInIdentity.AuthenticationType, context.SignInAsAuthenticationType, StringComparison.Ordinal))
                {
                    signInIdentity = new ClaimsIdentity(signInIdentity.Claims, context.SignInAsAuthenticationType, signInIdentity.NameClaimType, signInIdentity.RoleClaimType);
                }
                context.Properties.IsPersistent = true;
                Context.Authentication.SignIn(context.Properties, signInIdentity);
            }
 
            if (!context.IsRequestCompleted && context.RedirectUri != null)
            {
                if (context.Identity == null)
                {
                    // add a redirect hint that sign-in failed in some way
                    context.RedirectUri = WebUtilities.AddQueryString(context.RedirectUri, "error", "access_denied");
                }
                Response.Redirect(context.RedirectUri);
                context.RequestCompleted();
                Options.CallbackPath = PathString.Empty;
            }
 
            return context.IsRequestCompleted;
        }
 
        private string GenerateRedirectUri()
        {
            string requestPrefix = Request.Scheme + "://" + Request.Host;
 
            string redirectUri = requestPrefix + RequestPathBase + Options.CallbackPath; // + "?state=" + Uri.EscapeDataString(Options.StateDataFormat.Protect(state));            
            return redirectUri;
        }
    }
}