using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CourtFinder.Models
{
    public class Facility
    {
        public Facility()
        {
            this.Courts = new HashSet<Court>();
            this.Leagues = new HashSet<League>();
        }
        public int FacilityID { get; set; }
        public string FacilityName { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public virtual ICollection<Court> Courts { get; set; }
        public virtual ICollection<League> Leagues { get; set; }
    }
}