using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FinalProjectAlpha.Models;
using Microsoft.AspNet.Identity;

namespace FinalProjectAlpha.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.background = @Url.Content("~/Content/Keeper1.jpg");
            waybackdbEntities dbContext = new waybackdbEntities();

            List<Archive> archiveList = dbContext.Archives.ToList();

            ViewBag.ArList = archiveList;   // return project records to the Index

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
        //[HttpGet]
        public ActionResult Search(string searchTerm)
        {
            waybackdbEntities dbContext = new waybackdbEntities();      // create ORM obj
                                                                        //find the archive record(s) and put in the bag
            ViewBag.Found = dbContext.Archives.Where(x => x.ProjectName.Contains(searchTerm) || x.ShortDesc.Contains(searchTerm) || x.UserID.Contains(searchTerm));

            //check for null in cshtml
            ViewBag.errorMessage = "No Project with " + searchTerm + " in the name or description could be found.  Try another search.";

            return View("Index");
        }

        public ActionResult Dashboard(string Link)
        {   
            //Change background image here.
            ViewBag.background = @Url.Content("~/Content/dashboard.jpg");
            
            //Create DB Object. Get database
            waybackdbEntities Archives = new waybackdbEntities();

            //Turn Object into a List. Get all archives in it.
            List<Archive> ArchiveList = Archives.Archives.ToList();

            //Get the current logged in user id.
            string CurrentUserId = User.Identity.GetUserId();

            //New list that will represent the archives archived by user (foreign key)
            List<Archive> userArchiveList = new List<Archive>();

            //Get all userid that match current userid logged in.
            foreach (var item in ArchiveList)
            {   //Matching IDs get thrown in list.
                if (item.UserID == CurrentUserId)
                {
                    userArchiveList.Add(item);
                }
            }
            
            //Make list that has all of the current users archive.
            ViewBag.userArchiveList = userArchiveList;

            return View();
        }

    }
}