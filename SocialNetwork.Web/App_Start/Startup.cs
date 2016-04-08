using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.IdentityModel.Protocols;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using Owin;
using Thinktecture.IdentityModel.Clients;

[assembly: OwinStartup(typeof(SocialNetwork.Web.Startup))]

namespace SocialNetwork.Web
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            JwtSecurityTokenHandler.InboundClaimTypeMap
                = new Dictionary<string, string>();

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie
            });

            app.UseOpenIdConnectAuthentication(new OpenIdConnectAuthenticationOptions
            {
                ClientId = "socialnetwork_code",
                Authority = "http://localhost:22710",
                RedirectUri = "http://localhost:28037/",
                ResponseType = "code id_token",
                Scope = "openid profile offline_access",
                PostLogoutRedirectUri = "http://localhost:28037",
                SignInAsAuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,

                Notifications = new OpenIdConnectAuthenticationNotifications
                {
                    AuthorizationCodeReceived = async notification =>
                    {
                        var requestResponse = await OidcClient.CallTokenEndpointAsync(
                            new Uri("http://localhost:22710/connect/token"),
                            new Uri("http://localhost:28037/"),
                            notification.Code,
                            "socialnetwork_code",
                            "secret");

                        var identity = notification.AuthenticationTicket.Identity;

                        identity.AddClaim(new Claim("access_token", requestResponse.AccessToken));
                        identity.AddClaim(new Claim("id_token", requestResponse.IdentityToken));
                        identity.AddClaim(new Claim("refresh_token", requestResponse.RefreshToken));

                        notification.AuthenticationTicket = new AuthenticationTicket(
                            identity, notification.AuthenticationTicket.Properties);
                    },
                    RedirectToIdentityProvider = notification =>
                    {
                        if (notification.ProtocolMessage.RequestType !=
                            OpenIdConnectRequestType.LogoutRequest)
                        {
                            return Task.FromResult(0);
                        }

                        notification.ProtocolMessage.IdTokenHint =
                            notification.OwinContext.Authentication.User.FindFirst("id_token").Value;

                        return Task.FromResult(0);
                    }
                }
            });
        }
    }
}

