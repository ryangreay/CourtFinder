using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CourtFinder.Models
{
    public class League
    {
        public League()
        {
            this.Teams = new HashSet<Team>();
        }
        public int LeagueID { get; set; }
        public string LeagueName { get; set; }
        public virtual Sport Sport { get; set; }
        public DateTime RegisterStartPeriod { get; set; }
        public DateTime RegisterEndPeriod { get; set; }
        public int TeamSize { get; set; }
        public int MinTeams { get; set; }
        public int MaxTeams { get; set; }
        public virtual Bracket Bracket { get; set; }
        public virtual ICollection<Team> Teams { get; set; }
    }
}