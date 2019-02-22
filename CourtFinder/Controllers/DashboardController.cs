using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CourtFinder.Models;

namespace CourtFinder.Controllers
{
    [RequireHttps]
    public class DashboardController : Controller
    {
        public ActionResult UserProfile()
        {
            return View();
        }

        [HttpPost]
        public ActionResult UserProfile(ProfileViewModel model)
        {
            return View();
        }

        public ActionResult Search()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Search(SearchViewModel model)
        {
            return View();
        }

        public ActionResult Facility()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Facility(FacilityViewModel model)
        {
            return View(model);
        }
    }
}