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
        public ActionResult Save(string Link, string RepoLink, string ShortDesc, string LongDesc)
        {
            ArchiveDBEntities dbContext = new ArchiveBEntities();

            string ArchiveLink = archiveLink(Link);   //get Link from Jay's ArchiveLink()

            dbContext.Projects.Add(p);

            dbContext.SaveChanges();

           return RedirectToAction("Details", Link);
           
        }
    }
}