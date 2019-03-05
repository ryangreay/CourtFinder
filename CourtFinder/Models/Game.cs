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
        }
        public int GameID { get; set; }
        public DateTime GameStart { get; set; }
        public DateTime GameEnd { get; set; }
        public bool GameCompleted { get; set; }
        public int CourtID { get; set; }
        public virtual Court Court { get; set; }
        public virtual Team WinningTeam { get; set; }
        public virtual Team Team1 { get; set; }
        public virtual Team Team2 { get; set; }
    }
}