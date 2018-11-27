using System.Web.Mvc;

namespace App.Client.Web.Controllers
{
    public class _ConfigController : _BaseController
    {
        //
        // GET: /_Config/
        public ActionResult Index()
        {
            Response.ContentType = "text/javascript";
            return View("~/Scripts/global/system.configurations.cshtml");
        }
    }
}