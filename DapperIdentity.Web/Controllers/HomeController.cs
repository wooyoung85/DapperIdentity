using DapperIdentity.Web.Authorize;
using System.Web.Mvc;

namespace DapperIdentity.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        [CustomAuthorize(Roles ="Admin")]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}