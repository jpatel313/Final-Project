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

namespace FinalProjectAlpha.Controllers
{
    public class ProjectController : Controller
    {
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

            ViewBag.ss = SaveScreen("http://wayne.edu");
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
            saveLink(newLink);


            //get Link from Jay's ArchiveLink()
            string ArchiveLink = archiveLink(Link);
            //add Archive obj to db
            Archive archive = new Archive(newLink, ArchiveLink, RepoLink, ShortDesc, LongDesc);

            
            dbContext.Archives.Add(archive);


            //save to db
           dbContext.SaveChanges();
            //send user to Project/Details 



            return RedirectToAction("Details", "Project", new { Link = archive.Link });

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
            string check = (string)urlRes["archived_snapshots"]["closest"]["available"]; //Not sure, but if we get an error, this may throw an exception
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
            string archiveUrl = (string)urlResponse["archived_snapshots"]["closest"]["url"];

            return archiveUrl;

        }

        public byte[] SaveScreen(string url)
        {
           // var url = "https://wayne.edu";

            FileContentResult result = null;
            Bitmap bitmap = null;

            var thread = new Thread(
            () =>
            {
                bitmap = ExportUrlToImage(url, 1280, 1024);
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

        //screenshot via Freezer Nuget package
        //public Freezer.Core.Screenshot screenShot(string inputUrl)
        //{//return type:Freezer.Core.ScreenshotJob
        //        //convert user link to screenshotjob byte array
        //var screenshotJob = ScreenshotJobBuilder.Create(inputUrl)

        //      .SetBrowserSize(1366, 768)
        //       // Set what should be captured
        //      .SetCaptureZone(CaptureZone.FullPage) 
        //       // Set when the picture is taken
        //      .SetTrigger(new WindowLoadTrigger())
        //       // Capture the screenshot 100 ms after DOMContentLoaded is fired
        //      .SetTrigger(new DomReadyTrigger(100));

        //    System.IO.File.WriteAllBytes("Website Screenshot", screenshotJob.Freeze());

        //  return screenshotJob.Freeze("Details");
        //}


    }
}