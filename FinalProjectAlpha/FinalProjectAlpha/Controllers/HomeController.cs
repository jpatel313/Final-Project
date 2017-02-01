using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FinalProjectAlpha.ViewModels;
using Microsoft.AspNet.Identity;
using System.Activities.Statements;

namespace FinalProjectAlpha.Controllers
{
    public class HomeController : Controller
    {
        // create ORM obj to access the database
        waybackdbEntities dbContext = new waybackdbEntities();

        public ActionResult Index()
        {
            //Change background image of homepage here.
            ViewBag.background = @Url.Content("~/Content/Keeper1.jpg");

            //Create ORM Object to access database
            waybackdbEntities dbContext = new waybackdbEntities();

            ////Turn Object into a List. Get all archives in it.
            List<Archive> archiveList = dbContext.Archives.ToList();
           
            //Displays only links that are marked public(PrivateLink=false)
            ViewBag.ArList =  dbContext.Archives.Where(x => x.PrivateLink.Equals(false));
          
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
            //Displays only links that are marked public(PrivateLink=false)
            List<Archive> arList = dbContext.Archives.Where(x => x.PrivateLink.Equals(false)).ToList();
                        
            //find the archive record(s) and put them in the bag                                    
            ViewBag.Found = arList.Where(x => x.ProjectName.Contains(searchTerm) || x.ShortDesc.Contains(searchTerm) || x.UserID.Contains(searchTerm));
            
            //sort search results by default
            //ViewBag.Found.Sort("ArchiveDate", "Descending");
           
            //check for null in cshtml (if viewbag.Found = empty, show errorMessage)
            ViewBag.errorMessage = "No Project with " + searchTerm + " in the name or description could be found.  Try another search.";

            return View("Index");
        }
        public ActionResult Dashboard(string Link)
        {   
            //Change background image here.
            ViewBag.background = @Url.Content("~/Content/dashboard.jpg");
            
            //Create ORM Object to access database.
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