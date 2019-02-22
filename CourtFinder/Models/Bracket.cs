using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CourtFinder.Models
{
    public class Bracket
    {
        public Bracket()
        {
            this.Games = new HashSet<Game>();
        }
        public int BracketID { get; set; }
        public virtual ICollection<Game> Games { get; set; }
    }
}