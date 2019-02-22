using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CourtFinder.Models
{
//    fullname
//age
//bdate
//gender
//height
//weight
//preferred sports
//list of events this person signed up for

    public class Player
    {
        public Player()
        { 
            this.Teams = new HashSet<Team>();
        }
        public int PlayerID { get; set; }
        public string UserID { get; set; }
        [ForeignKey("UserID")]
        public ApplicationUser User { get; set; }
        public DateTime BirthDate { get; set; }
        public string FullName { get; set; }
        public string Gender { get; set; } //maybe something like "Male" "Female" or "M" "F"
        public int Height { get; set; } //maybe store as centimeters then do conversion to feet inch
        public int Weight { get; set; }
        public virtual ICollection<Team> Teams { get; set; }
    }
}