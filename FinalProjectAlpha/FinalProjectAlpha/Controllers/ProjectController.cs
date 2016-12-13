using FinalProjectAlpha.Models;
using Freezer.Core;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Freezer;
using System.Threading;
using System.Drawing.Imaging;

using System.Windows.Forms;
using Microsoft.AspNet.Identity;

namespace FinalProjectAlpha.Controllers
{
    public class ProjectController : Controller
    {
        public string Short { get; private set; }

        // GET: Project
        [Authorize(Roles = "Admin, Alumni")]
        public ActionResult Details(string Link)
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

            ViewBag.ss = SaveScreen(Link);
            return View();
        }
        [Authorize(Roles = "Admin")]
        public ActionResult New()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Save(string Link, string RepoLink, string ShortDesc, string LongDesc)
        {
            //get db
            waybackdbEntities dbContext = new waybackdbEntities();
            string newLink = "http://" + Link;


            if ((saveLink(newLink))== false) //If we get an error (false)
            {
                return View("New");
            }

            //get Link from Jay's ArchiveLink()
            string ArchiveLink = archiveLink(Link);

            //Call screenshot method, input users website link.
            var SnapShot = SaveScreen(Link);

            string UserID = User.Identity.GetUserId();

            //add Archive obj to db
            Archive archive = new Archive(newLink, ArchiveLink, RepoLink, ShortDesc, LongDesc, SnapShot, UserID);


            dbContext.Archives.Add(archive);


            //save to db
            dbContext.SaveChanges();
            //send user to Project/Details 

            return RedirectToAction("Details", "Project", new { Link = archive.Link });

        }

        public bool saveLink(string Link) //this is the "newLink" //Returns false if there is any error
        {
            //get db
            waybackdbEntities dbContext = new waybackdbEntities();

            List<Archive> archiveList = dbContext.Archives.ToList();

            List<String> Links = new List<string>();
            foreach (var item in archiveList)
            {
                Links.Add(item.Link);
            }

            if (Links.Exists(x => x == Link))
            {
                ViewBag.errormessage = "Link exists!";
                return false;
            }
            else
            { 
                return saveLinkInDB(Link);
            }
        }

        public bool saveLinkInDB(string inputUrl)
        {
            // Create a request for the API. 
            string url = "http://archive.org/wayback/available?url=" + inputUrl;

            HttpWebRequest request =
            (HttpWebRequest)WebRequest.Create(url);
            request.UserAgent = @"User-Agent: Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/53.0.2785.116 Safari/537.36";

            // Get the response.
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            //create object 
            StreamReader rd = new StreamReader(response.GetResponseStream());

            string wbackResponse = rd.ReadToEnd();

            if (checkBefore(wbackResponse))
            {
                HttpWebRequest req = WebRequest.CreateHttp("http://archive.org/save/_embed/" + inputUrl);
                req.UserAgent =
                @"User-Agent: Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/53.0.2785.116 Safari/537.36";
            }
            else
            {
                return false;
            }

            return checkAfter(wbackResponse); //false is unsucessful save


        }
        private bool checkAfter(string wbackResponse) //false is unsucessful save and a error message
        {
            JObject urlRes = JObject.Parse(wbackResponse);
            if (urlRes["archived_snapshots"]["closest"] == null)
            {
                ViewBag.errormessage = "Something went wrong, the url could not be saved.";

                return false;


            }
            else if ((bool)urlRes["archived_snapshots"]["closest"]["available"] == true)
            {

                //archive sucessfully saved
                return true;

            }
            ViewBag.errormessage = "Something went wrong, closest not null but available not true.";
            return false;//we had an error
        }
        private bool checkBefore(string wbackResponse) //true = continue with saving, false = already saved or error
        {
            JObject urlRes = JObject.Parse(wbackResponse);
            if (urlRes["archived_snapshots"]["closest"] == null)
            {
                return true;

            }
            return true;

            //currently it doesnt matter what is in the Wayback Archive, we will save a new archive anyway.

            //later we can add the API to prevent saving a new archive if its already in the db.
            
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
            string archiveUrl = (string)urlResponse["archived_snapshots"]["closest"]["url"];

            return archiveUrl;

        }

        public byte[] SaveScreen(string inputUrl)
        {
            // var url = "https://wayne.edu";

            FileContentResult result = null;
            Bitmap bitmap = null;

            var thread = new Thread(
            () =>
            {   //call ExportToImage Method, set screen capture size
                bitmap = ExportUrlToImage(inputUrl, 1280, 1024);
            });

            thread.SetApartmentState(ApartmentState.STA); //Set the thread to STA
            thread.Start();
            thread.Join();

            if (bitmap != null)
            {
                using (var memstream = new MemoryStream())
                {
                    bitmap.Save(memstream, ImageFormat.Jpeg);
                    result = this.File(memstream.GetBuffer(), "image/jpeg");
                }
            }

            return result.FileContents;
        }

        private Bitmap ExportUrlToImage(string url, int width, int height)
        {
            // Load the webpage into a WebBrowser control
            WebBrowser wb = new WebBrowser();
            wb.ScrollBarsEnabled = false;
            wb.ScriptErrorsSuppressed = true;

            wb.Navigate(url);
            while (wb.ReadyState != WebBrowserReadyState.Complete)
            {
                Application.DoEvents();
            }
            // Set the size of the WebBrowser control
            wb.Width = width;
            wb.Height = height;

            Bitmap bitmap = new Bitmap(wb.Width, wb.Height);
            wb.DrawToBitmap(bitmap, new System.Drawing.Rectangle(0, 0, wb.Width, wb.Height));
            wb.Dispose();

            return bitmap;
        }



    }
}