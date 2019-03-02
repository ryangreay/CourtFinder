using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CourtFinder.Models
{
    public class Day
    {
        public Day()
        {
            this.Brackets = new HashSet<Bracket>();
        }
        public int DayID { get; set; }
        public string Description { get; set; }
        public virtual ICollection<Bracket> Brackets {get; set;}

    }
}