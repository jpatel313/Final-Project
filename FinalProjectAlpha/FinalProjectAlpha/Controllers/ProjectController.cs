using FinalProjectAlpha.ViewModels;
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

        public ActionResult Details(string Link)   // returns one archived website in iframe, with details
        {
            //Create Entity ORM object to access DB.
            waybackdbEntities dbContext = new waybackdbEntities();

            //Create list.  
            List<Archive> archiveList = dbContext.Archives.ToList();

            // Checks each item in db for matching primary key (to display selected record)
            //Only display if archive entity is marked public.
            foreach (var item in archiveList)
            {
                if (item.Link == Link && item.PrivateLink == false)
                {//sends archive to page
                    ViewBag.Archive = item;
                }
                else
                    RedirectToAction("Index", "Home");
            }
            return View();
        }
        //New view to display details of one private archive.
        [Authorize]
        public ActionResult _PrivateDetails(string Link)   
        {
            //Create Entity ORM object to access DB.
            waybackdbEntities dbContext = new waybackdbEntities();

            //Create list.  
            List<Archive> archiveList = dbContext.Archives.ToList();
            //Get the current logged in user id.
            string CurrentUserId = User.Identity.GetUserId();

            // Checks each item in db for matching primary key(Link) 
            //Only viewable to user logged in. [View "_PrivateDetails" displays button if bool is true]
            foreach (var item in archiveList)
            {   
                  
                if (item.Link == Link && item.UserID == CurrentUserId)

                {
                    ViewBag.Archive = item;
                }
                else if (item.Link == Link && item.UserID != CurrentUserId)
                {   //Redirects to homepage for now.   
                    RedirectToAction("Index", "Home");
                }
            }
            return View();
        }
        //This is the page that contains the form for adding new project pages.
        public ActionResult New()
        {
            //Background image can be changed from here.
            ViewBag.background = @Url.Content("~/Content/NewProject.gif");
            return View("ArchiveForm");
        }
        //Save() saves one Archive object into the database
        // Refactored to use ViewModel.  This will prevent the Archive model from breaking on rebuild and fix the problem wherein some projects did not save to db correctly.
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Save(Archive archive)
        {
            //Create Entity ORM object to access DB.
            waybackdbEntities dbContext = new waybackdbEntities();
            
            
            //Send user back to blank form with an empty archive obj if model state is not valid
            if (!ModelState.IsValid)
            {
                ArchiveFormViewModel viewModel = new ArchiveFormViewModel(archive);
                return View("ArchiveForm", viewModel);
            }


            //Savelink() tries to save link and returns false if error... changes ViewBag.errorMessage, accordingly 
            if ((saveLink(archive.Link)) == false) //If we get an error (false)     refactor to try/catch
            {
                return View("ArchiveForm");     //renders ArchiveForm view 
            }

            //Get the url that was archived using the wayback API.
            string ArchiveLink = archiveLink(archive.Link);

            //Call screenshot method, input users website link.
            var SnapShot = GetBytesFromImage(archive.Link);

            //Gets currently logged in user.
            string UserID = User.Identity.GetUserId();

            //Create the Archive object to determine what will be sent to DB from Project/New. 
            archive = new Archive(archive.Link, ArchiveLink, archive.RepoLink, archive.ShortDesc, archive.LongDesc, SnapShot, UserID, archive.ProjectName, archive.TeamName, archive.PrivateLink);

            //Adds changes to DB via Entity ORM.
            dbContext.Archives.Add(archive);

            //Saves changes to DB via Entity ORM.
            dbContext.SaveChanges();

            //send user to Project/Details?Link=myurl.com
            return RedirectToAction("Details", "Project", new { Link = archive.Link });
        }
       
        public ActionResult EditPage(string Link)
        {   
            //Background image can be changed from here.
            ViewBag.background = @Url.Content("~/Content/NewProject.gif");

            //get db
            waybackdbEntities dbContext = new waybackdbEntities();

            //Finds one matching archive
            Archive archive = dbContext.Archives.Find(Link);
 
            //sends the archive to the view
            return View(archive);
        }

        //Action method that edits archive's fields in DB. 
        //(Gives this Actionresult priority over other methods & handles only Post requests.)
        [HttpPost]
        public ActionResult Edit(Archive editedArchive)
        {
            waybackdbEntities dbContext = new waybackdbEntities();
            
            //Get original archive.
            Archive oldArchive = dbContext.Archives.Find(editedArchive.Link);
            
            //Get user that is logged in.
            string UserID = User.Identity.GetUserId();

            //Check if original archive Foreign key(userid) matches logged in user.
            if(oldArchive.UserID == UserID)
            {
                //Change the values in original archive to the edited values.

                oldArchive.ProjectName = editedArchive.ProjectName;
                oldArchive.TeamName = editedArchive.TeamName;
                oldArchive.RepoLink = editedArchive.RepoLink;
                oldArchive.ShortDesc = editedArchive.ShortDesc;
                oldArchive.LongDesc = editedArchive.LongDesc;
                oldArchive.PrivateLink = editedArchive.PrivateLink;

                //Saves changes to DB via Entity ORM.
                dbContext.SaveChanges();
            }

                //Send user to /home/dashboard.

            return RedirectToAction("Dashboard", "Home");

        }

        //To be called when a delete button is made.
        public ActionResult Delete(string Link)
        {
            waybackdbEntities dbContext = new waybackdbEntities();

            //Create list  
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