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
            this.Events = new HashSet<Event>();
        }
        public int CourtID { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Address { get; set; }        
        public bool Status { get; set; }
        public int CurrentOccupancy { get; set; }
        public CourtOwner Owner { get; set; }
        public virtual ICollection<Event> Events { get; set; }
    }
}