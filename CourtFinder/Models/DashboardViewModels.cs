using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CourtFinder.Models
{
    public class SearchViewModel
    {
        public int Distance { get; set; }
        public string Sport { get; set; }
    }

    public class ProfileViewModel
    {
        public Player player { get; set; }
        public List<Team> teams { get; set; }
        public string newTeamName { get; set; }
        public string joinTeamID { get; set; }
    }

    public class FacilityViewModel
    {

    }

    public class TeamViewModel
    {
        public Team team { get; set; }
    }
}