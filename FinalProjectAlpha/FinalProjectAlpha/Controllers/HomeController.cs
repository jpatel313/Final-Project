using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FinalProjectAlpha.Models;

namespace FinalProjectAlpha.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult ViewArchive(string Link)
        {
            waybackdbEntities dbContext = new waybackdbEntities();




            List<Archive> archiveList = dbContext.Archives.ToList();



            foreach (var item in archiveList)
            {
                if (item.Link == Link)
                {
                    ViewBag.Archive = item;
                }
            }

            return View();
        }

        public ActionResult Index()
        {
            waybackdbEntities dbContext = new waybackdbEntities();
            List<Archive> archiveList = dbContext.Archives.ToList();

            ViewBag.ArList = archiveList;

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