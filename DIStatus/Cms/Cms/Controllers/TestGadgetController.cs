using System.Web.Mvc;
using EPiServer.Shell.Gadgets;

namespace Cms.Controllers
{
    [Gadget]
    public class TestGadgetController : Controller
    {
        public ActionResult Index()
        {
            return Content("<h1>Test</h1>");
        }
    }
}