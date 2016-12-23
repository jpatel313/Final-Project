using FinalProjectAlpha.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Threading;
using System.Drawing.Imaging;
using System.Windows.Forms;
using Microsoft.AspNet.Identity;
using System.Diagnostics;
using System.Security.Permissions;
using GrabzIt;
using GrabzIt.Parameters;

namespace FinalProjectAlpha.Controllers
{
    //[PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    
    public class ProjectController : Controller
    {

        #region Actions
        
        public ActionResult Details(string Link)    // returns one archived website in iframe, with details
        {
            
            
            
            //get db
            waybackdbEntities dbContext = new waybackdbEntities();

            //create list  
            List<Archive> archiveList = dbContext.Archives.ToList();

            // checks each item in db for matching primary key (to display selected record)
            foreach (var item in archiveList)
            {
                if (item.Link == Link)
                {//sends archive to page
                    ViewBag.Archive = item;
                }
            }
            return View();
        }
        
        //This is the page that contains the form for adding pages
        public ActionResult New()
        {//Background image can be changed from here.
            ViewBag.background = @Url.Content("~/Content/NewProject.gif");
            return View();
        }
       
       //Save() saves one Archive object into the database
       [HttpPost]
       [Authorize]
        public ActionResult Save(string ProjectName, string TeamName, string Link, string RepoLink, string ShortDesc, string LongDesc)
        {   
            //get db
            waybackdbEntities dbContext = new waybackdbEntities();
            //savelink() tries to save link and returns false if error... changes ViewBag.errorMessage, accordingly 
            if ((saveLink(Link)) == false) //If we get an error (false)     refactor to try/catch
            {

                return View("New");
            }

            //get wayback link from the wayback API
            string ArchiveLink = archiveLink(Link);

            //Call screenshot method, input users website link.
            // var SnapShot = SaveScreen(Link);

            var SnapShot = GetBytesFromImage(Link);

            //gets currently logged in user
            string UserID = User.Identity.GetUserId();

            //create Archive obj 
            Archive archive = new Archive(Link, ArchiveLink, RepoLink, ShortDesc, LongDesc, SnapShot, UserID, TeamName, ProjectName);

            //Add to db, save
            dbContext.Archives.Add(archive);
            dbContext.SaveChanges();
            
            //send user to Project/Details?Link=myurl.com
            return RedirectToAction("Details", "Project", new { Link = archive.Link });

        }

        [HttpPost]
        [Authorize]
        public ActionResult EditPage(string Link)
        {   //Background image can be changed from here.
            ViewBag.background = @Url.Content("~/Content/NewProject.gif");

            //get db
            waybackdbEntities dbContext = new waybackdbEntities();

            //Finds one matching archive
            Archive archive = dbContext.Archives.Find(Link);

           
            //sends the archive to the view
            return View(archive);
        }
        //attribute tells the routing engine(mvc) to send any POST requests to that action method to the one method over the other.Ask Kamel
        //Method that changes an archive in db. only sends info we want to edit.
        [HttpPost]
        [Authorize]
        public ActionResult Edit(Archive editedArchive)
        {   //
            waybackdbEntities dbContext = new waybackdbEntities();
            
            //Get original archive.
            Archive oldArchive = dbContext.Archives.Find(editedArchive.Link);
            
            //Get user that is logged in.
            string UserID = User.Identity.GetUserId();

            //Check if original archive Foreign key(userid) matches logged in user.
            if(oldArchive.UserID == UserID)
            {
              //change the values in original archive to the edited values.

                oldArchive.ProjectName = editedArchive.ProjectName;
                oldArchive.TeamName = editedArchive.TeamName;
                oldArchive.RepoLink = editedArchive.RepoLink;
                oldArchive.ShortDesc = editedArchive.ShortDesc;
                oldArchive.LongDesc = editedArchive.LongDesc;

               dbContext.SaveChanges();
                }

            //send user to /home/dashboard
            return RedirectToAction("Dashboard", "Home");

        }
        //to be called when a delete button is made

        [HttpPost]
        public ActionResult Delete(string Link)
        {
            waybackdbEntities dbContext = new waybackdbEntities();

            //create list  
            List<Archive> archiveList = dbContext.Archives.ToList();

            //populate list with select object (by the input Link)  (refactor: use orm .find?)
            foreach (var item in archiveList)
            {
                if (item.Link == Link)
                {
                    archiveList.Remove(item);
                }
            }
            return RedirectToAction("Index", "Home");
        }

        #endregion
        //checks the DB for another project with the same name
        #region Methods
        public bool saveLink(string Link)
        //checks the DB for another project with the same name //Returns false if there is any error
        {
            //get db
            try
            {//if db is empty, we catch and skip this error handling.
                waybackdbEntities dbContext = new waybackdbEntities();

                List<Archive> archiveList = dbContext.Archives.ToList();

                List<string> Links = new List<string>();

                //Use primary key to separate lists
                foreach (var item in archiveList)
                {
                    Links.Add(item.Link);
                }

                //If link already in db, message.
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

            catch
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

            //avoid 403 error for logged in users. auth check
            //request.UseDefaultCredentials = true;
            //request.Proxy.Credentials = System.Net.CredentialCache.DefaultCredentials;
            //request.AllowAutoRedirect = true;
            
            
            // Get the response.
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            
            //create object from response.
            StreamReader rd = new StreamReader(response.GetResponseStream());
            
            //read the object.
            string wbackResponse = rd.ReadToEnd();

            //If there is an error, 
            if (checkBefore(wbackResponse))
            {
                HttpWebRequest req = WebRequest.CreateHttp("http://web.archive.org/save/" + inputUrl);
                req.UserAgent =
                    
                @"User-Agent: Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/53.0.2785.116 Safari/537.36";
                req.GetResponse();
                
            }
            else
            {
                return false;
            }


            request =
            (HttpWebRequest)WebRequest.Create(url);
            request.UserAgent = @"User-Agent: Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/53.0.2785.116 Safari/537.36";
            request.UseDefaultCredentials = true;
            // Get the response.
            response = (HttpWebResponse)request.GetResponse();

            //create object from response.
            rd = new StreamReader(response.GetResponseStream());

            //read the object.
            wbackResponse = rd.ReadToEnd();

            return checkAfter(wbackResponse); //false is unsucessful save


        } //checks the wayback machine for a archive already. //False = error
        private bool checkBefore(string wbackResponse) //true = continue with saving, false = already saved or error
        {
            JObject urlRes = JObject.Parse(wbackResponse);
            //Checks the wayback machine if there is already an archive
            if (urlRes["archived_snapshots"]["closest"] == null)
            {
                return true;

            }
            return true;

            //currently it doesnt matter what is in the Wayback Archive, we will save a new archive anyway.

            //later we can add the API to prevent saving a new archive if its already in the db.
            
        }

        //Checks if the url was bad, or some oddball error (False is a bad save and returns an error message)
        private bool checkAfter(string wbackResponse) //false is unsucessful save and a error message
        {
            JObject urlRes = JObject.Parse(wbackResponse);
            if (urlRes["archived_snapshots"]["closest"] == null)
            {
                //Most likely the person entered a url of a offline website
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

        public string archiveLink(string inputUrl)
        {
            //Use Wayback Machine Api + users project link
            HttpWebRequest request = WebRequest.CreateHttp("http://archive.org/wayback/available?url=" + inputUrl);
            request.UserAgent =
    @"User-Agent: Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/53.0.2785.116 Safari/537.36";
            //request.UseDefaultCredentials = true;
            //request.Proxy.Credentials = System.Net.CredentialCache.DefaultCredentials;
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

        //saves a screenshot to db.
        public byte[] SaveScreen(string inputUrl)
        { 

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

        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        private Bitmap ExportUrlToImage(string url, int width, int height)
        {
            // Load the webpage into a WebBrowser control
            WebBrowser wb = new WebBrowser();
            
            wb.ScrollBarsEnabled = false;
            wb.ScriptErrorsSuppressed = true;

           // wb.Navigate(url);
            wb.Navigate(url, null, null,
                    "User-Agent: Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/53.0.2785.116 Safari/537.36");
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

        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        //This method takes the users project link and gets s
        private byte[] GetBytesFromImage(string url)
        {

            GrabzItClient grabzIt = GrabzItClient.Create("OTc4YWU2YjFlMGQxNGI1YmEyNWJiYThjNTAyYWEzODc=", "P2w/Jz80FTMICj9DP2IQOyJ3dkdoPz8/Yj8/P1RQPz8=");

            ImageOptions m = new ImageOptions();

            m.CustomWaterMarkId = "";

            m.Quality = 100;

            m.BrowserHeight = 1280;

            m.BrowserWidth = 1024;

            grabzIt.URLToImage(url, m);

            byte[] bytes = grabzIt.SaveTo().Bytes;
            return bytes; 


        }
        #endregion


    }
}