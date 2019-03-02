using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CourtFinder.Models
{
//    ID
//Description

    public class Sport
    {
        public Sport()
        {
            this.Players = new HashSet<Player>();
            this.Facilities = new HashSet<Facility>();
            this.Courts = new HashSet<Court>();
        }
        public int SportID { get; set; }
        public string Description { get; set; } //of the form #Basketball, Soccer, etc
        public string SportImageURL { get; set; } //save an icon image to display alongside the sport
        public virtual ICollection<Player> Players { get; set; }
        public virtual ICollection<Facility> Facilities { get; set; }
        public virtual ICollection<Court> Courts { get; set; }
    }
}