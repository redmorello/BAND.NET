using System.Web.Mvc;

namespace BAND.ExampleApi.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Band");
        }
    }
}
