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

        public ActionResult Dashboard(string Link)
        {
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
            //Make list that has all archives of currentuserid viewable.
            // ViewBag.userArchiveList = userArchiveList.ToArray();
            ViewBag.userArchiveList = userArchiveList;

            //foreach (var item in ViewBag.userArchiveList.ToArray())
            //{
            //    if (item.Link == ArchiveLink.)
            //    {
            //        userArchiveList.Add(item);
            //    }
            //}

            return View();
        }

    }
}