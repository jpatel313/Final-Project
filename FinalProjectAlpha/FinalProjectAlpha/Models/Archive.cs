//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FinalProjectAlpha.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Archive
    {
        public string Link { get; set; }
        public string ArchiveLink { get; set; }
        public string RepoLink { get; set; }
        public string ShortDesc { get; set; }
        public string LongDesc { get; set; }
        public byte[] SnapShot { get; set; }

        public Archive()
            {
            }
        public Archive(string Link, string archiveLink, string repoLink, string shortDesc, string longDesc, byte[] snapShot)
        {
            this.Link = Link;
            ArchiveLink = archiveLink;
            RepoLink = repoLink;
            ShortDesc = shortDesc;
            LongDesc = longDesc;
            SnapShot = snapShot;
        }

    }




}
