using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CourtFinder.Models
{
    public class BracketResult
    {
        public int BracketResultID { get; set; }
        public int BracketID { get; set; }
        [ForeignKey("BracketID")]
        public Bracket Bracket { get; set; }
        public int TeamID { get; set; }
        [ForeignKey("TeamID")]
        public Team Team { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
    }
}