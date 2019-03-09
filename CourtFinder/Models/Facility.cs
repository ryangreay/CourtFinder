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
            this.Sports = new HashSet<Sport>();
            this.Players = new HashSet<Player>();
        }
        public int FacilityID { get; set; }
        public string FacilityName { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Address { get; set; }
        public virtual ICollection<Court> Courts { get; set; }
        public virtual ICollection<League> Leagues { get; set; }
        public virtual ICollection<Sport> Sports { get; set; }
        public virtual FacilityManager FacilityManager { get; set; }
        public virtual ICollection<Player> Players { get; set; }
    }
}