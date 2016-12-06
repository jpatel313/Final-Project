﻿using FinalProjectAlpha.Models;
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
            //get db
            waybackdbEntities dbContext = new waybackdbEntities();
            //get Link from Jay's ArchiveLink()
            string ArchiveLink = archiveLink(Link);
            //add Archive obj to db
            Archive archive = new Archive(Link, ArchiveLink, RepoLink, ShortDesc, LongDesc);
            dbContext.Archives.Add(archive);
            //save to db
            dbContext.SaveChanges();
            //send user to Project/Details 
            return RedirectToAction("Details", Link);

        }

        public void saveLink(string inputUrl)
        {     
            // Create a request for the URL. 
            string url = "https://web.archive.org/save/" + inputUrl;


            HttpWebRequest request =
            (HttpWebRequest)WebRequest.Create(url);
            request.UserAgent = @"User-Agent: Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/53.0.2785.116 Safari/537.36";
            
            // Get the response.
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            ViewBag.response = response;
           

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