using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FinalProjectAlpha.Controllers
{
    public class ProjectController : Controller
    {
        // GET: Project
        public ActionResult Details(string PK_Link)
        {
            return View();
        }
        public ActionResult New()
        {
            return View();
        }
        public ActionResult Save(/*Data types and stuff here.*/)
        {

            //creates new Archive
            //adds archive to DB
            //redirects to Main/Index (Or whatever the homepage is)

            return View();
        }
    }
}