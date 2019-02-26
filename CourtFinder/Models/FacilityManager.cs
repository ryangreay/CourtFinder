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
        public int FacilityManagerID { get; set; }
        public string UserID { get; set; }
        [ForeignKey("UserID")]
        public ApplicationUser User { get; set; }
        public string ProfileImage { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}