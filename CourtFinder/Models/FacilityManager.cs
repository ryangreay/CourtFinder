using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CourtFinder.Models
{
//    Title
//List of Courts they own / oversee

    public class FacilityManager
    {
        public FacilityManager()
        {
            this.Facilities = new HashSet<Facility>();
        }
        public int FacilityManagerID { get; set; }
        public string UserID { get; set; }
        [ForeignKey("UserID")]
        public ApplicationUser User { get; set; }
        public string ManagerTitle { get; set; } //of the form UCR Recreation Center
        public virtual ICollection<Facility> Facilities { get; set; }
    }
}