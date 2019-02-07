using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CourtFinder.Models
{
//    Title
//List of Courts they own / oversee

    public class CourtOwner
    {
        public CourtOwner()
        {
            this.Courts = new HashSet<Court>();
        }
        public int CourtOwnerID { get; set; }
        public string UserID { get; set; }
        [ForeignKey("UserID")]
        public ApplicationUser User { get; set; }
        public string Title { get; set; } //of the form UCR Recreation Center
        public virtual ICollection<Court> Courts { get; set; }
    }
}