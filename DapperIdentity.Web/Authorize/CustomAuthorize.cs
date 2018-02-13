using System.Web.Mvc;

namespace DapperIdentity.Web.Authorize
{
    public class CustomAuthorize : AuthorizeAttribute
    {
        protected override void HandleUnauthorizedRequest(AuthorizationContext context)
        {
            if (context.HttpContext.User.Identity.IsAuthenticated)
            {
                context.Result = new RedirectResult("/Home");
            }
            else
            {
                context.Result = new RedirectResult("/Account/Login");
            }
        }
    }
}