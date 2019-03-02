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
        public League league { get; set; }

        //court settings
        public Court court { get; set; }
        public List<string> courtSports {get; set;}

        //League settings form
        public string LeagueName { get; set; }
        public string sport { get; set; }
        public int? registerStartMonth { get; set; }
        public int? registerStartDay { get; set; }
        public int? registerStartYear { get; set; }
        public int? registerEndMonth { get; set; }
        public int? registerEndDay { get; set; }
        public int? registerEndYear { get; set; }
        public int? teamSize { get; set; }
        public int? minTeams { get; set; }
        public int? maxTeams { get; set; }
    }

    public class TeamViewModel
    {
        public Team team { get; set; }
    }

    public class GameViewModel
    {
        public Game game { get; set; }
        public League league { get; set; }
        public int winningTeamID { get; set; }
    }

    public class CourtViewModel
    {
        public Court court { get; set; }
    }

    public class LeagueViewModel
    {
        //general league info
        public League league { get; set; }
        public Facility facility { get; set; }

        //User join league
        public int teamSignupID { get; set; }
        public List<Team> myTeams { get; set; }

        //Build bracket form
        public int? gameStartMonth { get; set; }
        public int? gameStartDay { get; set; }
        public int? gameStartYear { get; set; }
        public int? gameLengthHour { get; set; }
        public int? gameLengthMin { get; set; }
        public List<string> daysOfWeek { get; set; }
        public List<string> hoursOfDay { get; set; }
        public int? daysBetweenRounds { get; set; }

        //League settings form
        public string sport { get; set; }
        public List<string> sports { get; set; }
        public int? registerStartMonth { get; set; }
        public int? registerStartDay { get; set; }
        public int? registerStartYear { get; set; }
        public int? registerEndMonth { get; set; }
        public int? registerEndDay { get; set; }
        public int? registerEndYear { get; set; }
        public int? teamSize { get; set; }
        public int? minTeams { get; set; }
        public int? maxTeams { get; set; }
    }

}