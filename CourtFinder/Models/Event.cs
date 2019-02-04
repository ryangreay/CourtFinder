using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CourtFinder.Models
{
//    DateTimeStart
//DateTimeEnd
//Result
//Time
//Event Coordinator
//Location / Court / Court number
//Sport
//Capacity
//#people that signed up

    public class Event
    {
        public Event()
        {
            this.Teams = new HashSet<Team>();
        }
        public int EventID { get; set; }
        public DateTime EventStart { get; set; }
        public DateTime EventEnd { get; set; }
        public Team Winner { get; set; }
        public CourtOwner Coordinator { get; set; }
        public int CourtNumber { get; set; }
        public Sport Sport { get; set; }
        public int Capacity { get; set; }      
        public Court Court { get; set; }
        public virtual ICollection<Team> Teams { get; set; }
    }
}