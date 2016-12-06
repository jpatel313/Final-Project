using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FinalProjectAlpha.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
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


        public string Myreturnstringmethod(string myvariable)
        {
            string hello = myvariable;

            return myvariable;
        }

        public string Myreturnstringmethod2(string myvariable, string secondvariable)
        {
            string hello = myvariable;

            return myvariable;
        }

        public ActionResult Savepage(string link, string githublink)
        {

            Myreturnstringmethod(link);

            Myreturnstringmethod2(link, githublink);

            ViewBag.Message = "My ViewBag.Message";

            return View();
        }
    }
}