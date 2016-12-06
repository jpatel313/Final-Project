using Newtonsoft.Json.Linq;
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
        public ActionResult Save(string Link, string RepoLink, string ShortDesc, string LongDesc)
        {
            ArchiveDBEntities dbContext = new ArchiveBEntities();

            string ArchiveLink = archiveLink(Link);   //get Link from Jay's ArchiveLink()

            dbContext.Projects.Add(p);

            dbContext.SaveChanges();



           return RedirectToAction("Details", Link);
           
        }

        public void saveLink(string inputUrl)
        {     
            // Create a request for the URL. 
            string url = "http://archive.org/wayback/available?url=" + inputUrl;
            
            HttpWebRequest request =
            (HttpWebRequest)WebRequest.Create(url);
            request.UserAgent = @"User-Agent: Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/53.0.2785.116 Safari/537.36";
            
            // Get the response.
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            //create object 
            StreamReader rd = new StreamReader(response.GetResponseStream());
            string wbackResponse = rd.ReadToEnd();
            JObject urlRes = JObject.Parse(wbackResponse);
            string check = (string)urlRes["archived_snapshots"]["available"]; //Not sure, but if we get an error, this may throw an exception
            if (check == "false") //live but not archived
            {
                HttpWebRequest req = WebRequest.CreateHttp("http://archive.org/save/_embed/" + inputUrl);
                req.UserAgent =
                @"User-Agent: Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/53.0.2785.116 Safari/537.36";
            }
            else if (check == "true")
            {

                ViewBag.errormessage = "Sorry, this is archived already.";
            }
            else //All of the errors
            {
                ViewBag.errorMessage = "Sorry, there is something wrong with the link or server";
            }
           

        }



        public string archiveLink(string inputUrl)
        {
            //Use Wayback Machine Api + users project link
            HttpWebRequest request = WebRequest.CreateHttp("http://archive.org/wayback/available?url=" + inputUrl);
            request.UserAgent =
    @"User-Agent: Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/53.0.2785.116 Safari/537.36";

            //Cast Wayback request to Response.  
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            //Read response Url data and convert into string.
            StreamReader read = new StreamReader(response.GetResponseStream());
            string waybackResponse = read.ReadToEnd();

            //Parse from string to object.
            JObject urlResponse = JObject.Parse(waybackResponse);

            //cast exact part of return url needed and save as archiveUrl string
            string archiveUrl = (string)urlResponse["archived_snapshots"]["url"];

            return archiveUrl;

        }

    }
}