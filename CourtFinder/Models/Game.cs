using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CourtFinder.Models
{
    public class Game
    {
        public Game()
        {
            this.Teams = new HashSet<Team>();
        }
        public int GameID { get; set; }
        public DateTime GameStart { get; set; }
        public DateTime GameEnd { get; set; }
        public int CourtID { get; set; }
        public virtual Court Court { get; set; }
        public virtual Team WinningTeam { get; set; }
        public virtual ICollection<Team> Teams { get; set; }
    }
}