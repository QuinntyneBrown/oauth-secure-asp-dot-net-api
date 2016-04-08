using System.Security.Claims;
using System.Web.Http;

namespace SocialNetwork.Api.Controllers
{
    [Authorize]
    public abstract class SocialNetworkApiController : ApiController
    {
        public string GetUsernameFromClaims()
        {
            var claimsPrincipal = User as ClaimsPrincipal;

            return  claimsPrincipal?.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").Value;
        }
    }
}