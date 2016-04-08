using IdentityServer3.Core.Extensions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SocialNetwork.OAuth.Controllers
{
    public class TermsController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Index(bool acceptTerms)
        {
            if (!acceptTerms)
            {
                return View();
            }
            var partialLogin = await Request.GetOwinContext()
                .Environment.GetIdentityServerPartialLoginAsync();

            if (partialLogin == null)
            {
                return Redirect("/");
            }

            var resumeUrl = await Request.GetOwinContext()
                .Environment.GetPartialLoginResumeUrlAsync();

            return Redirect(resumeUrl);
        }
    }
}