using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CourtFinder.Models
{
    //    List of players
    //Events
    //History
    //Standing(good-bad) (record)

    public class Team
    {
        public Team()
        {
            this.Games = new HashSet<Game>();
            this.Players = new HashSet<Player>();
            this.Leagues = new HashSet<League>();
        }
        public int TeamID { get; set; }
        public string PrivateTeamID { get; set; }
        public string TeamName { get; set; }        
        public int Wins { get; set; }
        public int Losses { get; set; }
        //this is not win loss. Its a reputation measure for other teams (how many scheduled games did they miss etc)
        public double Standing { get; set; } 
        public virtual ICollection<Player> Players { get; set; }       
        public virtual ICollection<Game> Games { get; set; }
        public virtual ICollection<League> Leagues { get; set; }
        //History ??
    }
}