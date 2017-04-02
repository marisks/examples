using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using EPiServer.Cms.UI.AspNetIdentity;
using EPiServer.Security;
using EPiServer.ServiceLocation;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security.OAuth;

namespace Cms.Business
{
    public class IdentityAuthorizationProvider : OAuthAuthorizationServerProvider
    {
        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
            return Task.FromResult(0);
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            var signInManager = ServiceLocator.Current.GetInstance<SignInManager<ApplicationUser, string>>();
            var result =
                await
                    signInManager.PasswordSignInAsync(
                        context.UserName,
                        context.Password,
                        isPersistent: false,
                        shouldLockout: false);
            if (result == SignInStatus.Success)
            {
                var identity = new ClaimsIdentity(context.Options.AuthenticationType);
                var principal = PrincipalInfo.CreatePrincipal(context.UserName);
                if (principal is GenericPrincipal)
                {
                    var generic = principal as GenericPrincipal;
                    identity.AddClaims(generic.Claims);
                }

                context.Validated(identity);
            }
            else
            {
                context.Rejected();
            }
        }
    }
}