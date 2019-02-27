using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CourtFinder.Models
{
//    Location(probably store lat / long coords for google api
//List of events
// Current court status (maybe open closed, current number of players, etc)

    public class Court
    {
        public Court()
        {
            this.Games = new HashSet<Game>();
        }
        public int CourtID { get; set; }
        public int FacilityID { get; set; }
        public string CourtName { get; set; }
        public virtual Facility Facility { get; set; }
        public virtual ICollection<Game> Games { get; set; }
    }
}