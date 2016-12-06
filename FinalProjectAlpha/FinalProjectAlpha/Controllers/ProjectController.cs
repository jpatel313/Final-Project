using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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
        public ActionResult saveLink()
        {
            // Create a request for the URL. 
            string url = "https://web.archive.org/save/" + "https://www.playbutterfly.com";


            HttpWebRequest request =
            (HttpWebRequest)WebRequest.Create(url);
            request.UserAgent = @"User-Agent: Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/53.0.2785.116 Safari/537.36";
            
            // Get the response.
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            ViewBag.response = response;
            return View();

        }

    }
}