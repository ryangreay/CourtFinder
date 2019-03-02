using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CourtFinder.Models
{
    public class Time
    {
        public Time()
        {
            this.Brackets = new HashSet<Bracket>();
        }
        public int TimeID { get; set; }
        public TimeSpan Description { get; set; } 
        public virtual ICollection<Bracket> Brackets { get; set; }
    }
}