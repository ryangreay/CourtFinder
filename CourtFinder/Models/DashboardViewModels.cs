using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CourtFinder.Models
{
    public class SearchViewModel
    {
        public int Distance { get; set; }
        public Sport sport { get; set; }
        public List<Sport> sports { get; set; }
        public List<Facility> facilities { get; set; }
    }

    public class ProfileViewModel
    {
        public Player player { get; set; }
        public List<Team> teams { get; set; }
        public HttpPostedFileBase profilePhoto { get; set; }
        public string newTeamName { get; set; }
        public string joinTeamID { get; set; }
    }

    public class FacilityViewModel
    {
        public FacilityManager facilityManager { get; set; }
        public Facility facility { get; set; }
        public List<string> sports { get; set; }
        public List<string> facilitySports { get; set; }
        public string address { get; set; }
        public string state { get; set; }
        public string zipCode { get; set; }
    }

    public class TeamViewModel
    {
        public Team team { get; set; }
    }

    public class GameViewModel
    {
        public Game game { get; set; }
    }

}