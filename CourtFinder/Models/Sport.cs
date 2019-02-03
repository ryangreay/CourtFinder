using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CourtFinder.Models
{
//    ID
//Description

    public class Sport
    {
        public int SportID { get; set; }
        public string Description { get; set; } //of the form #Basketball, Soccer, etc
        public string SportImageURL { get; set; } //save an icon image to display alongside the sport
        public virtual ICollection<Player> Players { get; set; }
    }
}