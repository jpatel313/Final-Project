using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FinalProjectAlpha.ViewModels;

namespace FinalProjectAlpha.ViewModels
{
    public class ArchiveFormViewModel
    {
        //Archive properties that appear in ArchiveForm.  Decorated with annotations to support form validation. 
        [Required]
        [Display(Name ="Project Name")]
        public string ProjectName { get; set; }

        [Required]
        [Url]
        [Display(Name ="Project URL")]
        public string Link { get; set; }

        [Display(Name ="Team Name")]
        public string TeamName { get; set; }

        [Required]
        [MinLength(10)]
        [StringLength(200)]
        [Display(Name ="Short Description")]
        public string ShortDesc { get; set; }

        [Required]
        public bool? PrivateLink { get; set; }

        [Url]
        [Display(Name ="Repository Link")]
        public string RepoLink { get; set; }

        [StringLength(500)]
        [Display(Name ="Long Description")]
        public string LongDesc { get; set; }



        
        //public string UserID { get; set; }      //set hidden field in View?

        //public virtual AspNetUser AspNetUser { get; set; }

        

        //constructor to build archive obj that will be passed from form to Project/Save
        public ArchiveFormViewModel(Archive archive)
        {
            ProjectName = archive.ProjectName;
            Link = archive.Link;
            ShortDesc = archive.ShortDesc;
            PrivateLink = archive.PrivateLink;
            RepoLink = archive.RepoLink;
            TeamName = archive.TeamName;
            LongDesc = archive.LongDesc;
        }
        
        //Default Constructor (to generate new Archive)
        public ArchiveFormViewModel()
        {
            Link = null; 
        }
}
}