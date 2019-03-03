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
            this.Days = new HashSet<Day>();
            this.Times = new HashSet<Time>();
            this.UnscheduledTeams = new HashSet<Team>();
        }
        public int BracketID { get; set; }
        public DateTime BracketStartDate { get; set; }
        public TimeSpan GameLength { get; set; }
        public int daysBetweenRounds { get; set; }
        public virtual ICollection<Day> Days { get; set; }
        public virtual ICollection<Time> Times { get; set; }
        public virtual ICollection<Game> Games { get; set; }
        public virtual ICollection<Team> UnscheduledTeams { get; set; }
    }
}