using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;

namespace AmyDaveWedding.Controllers
{
    public class HomeController : Controller
    {
        public async Task<ActionResult> Index()
        {
            ViewBag.GoogleAnalyticsTrackingID = ConfigurationManager.AppSettings["GoogleAnalyticsTrackingID"];

            var user = await new AccountController().FindUserByIdAsync(User.Identity.GetUserId());
            ViewBag.Invitee = user != null ? user.Invitee : null;

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}