using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CourtFinder.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.AspNet.Identity.EntityFramework;
using System.IO;
using System.Globalization;
using System.Device;
using System.Device.Location;
using System.Net;
using System.Configuration;
using System.Xml.Linq;

namespace CourtFinder.Controllers
{
    [RequireHttps]
    [Authorize]
    public class DashboardController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();

        //this is used for creating a random teamid
        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        //contact google maps rest api and send address, bring into xml document, 
        //pull the lat and long coords out from the document
        public static GeoCoordinate GetCoordinates(string address)
        {
            var requestUri = string.Format("https://maps.googleapis.com/maps/api/geocode/xml?address={0}&key=AIzaSyDccK65hnsCsuS5VmBDWaWo6BQoBwgcH78", Uri.EscapeDataString(address));

            var request = WebRequest.Create(requestUri);
            var response = request.GetResponse();
            var xdoc = XDocument.Load(response.GetResponseStream());

            var result = xdoc.Element("GeocodeResponse").Element("result");
            var locationElement = result.Element("geometry").Element("location");
            var lat = locationElement.Element("lat").Value;
            var lng = locationElement.Element("lng").Value;
            return new GeoCoordinate(Convert.ToDouble(lat), Convert.ToDouble(lng));
        }

        public bool scheduleGame(Bracket bracket, List<Court> availableCourts, Game game, Team team1, Team team2, DateTime gameStartDate)
        {
            bool scheduled = false;
            TimeSpan gameLength = bracket.GameLength;
            //schedule game with this team starting at daysbetweenRounds.
            //loop on available days and available times, start at bracketstartdate
            //loop on court that support league sport
            //move by gamelength and check if court is available
            //if so set available court, available start
            for (DateTime date = gameStartDate; date <= (gameStartDate.AddDays(bracket.daysBetweenRounds)); date = date.AddDays(1))
            {
                if (bracket.Days.Any(val => val.Description == date.DayOfWeek.ToString())) //if this day is a selected bracket day
                {
                    //loop on times in selected bracket times
                    foreach (TimeSpan time in bracket.Times.Select(val => val.Description))
                    {
                        DateTime currDate = new DateTime(date.Year, date.Month, date.Day, time.Hours, time.Minutes, 0);
                        foreach (Court court in availableCourts)
                        {
                            //check if currdate to currDate + gametime has no scheduled game, if so grab time and court
                            //and schedule game
                            bool courtOverlap = court.Games.Any(val => currDate < val.GameEnd &&
                                (currDate.Add(bracket.GameLength) > val.GameStart));
                            if (!courtOverlap)
                            {
                                game = new Game
                                {
                                    Court = court, //available court
                                    CourtID = court.CourtID,
                                    GameStart = currDate, //available day and time
                                    GameEnd = currDate.Add(gameLength) //GameStart + game length 
                                };
                                game.Team1 = team1;
                                game.Team2 = team2;
                                bracket.Games.Add(game);
                                court.Games.Add(game);
                                scheduled = true;
                                goto sheduleSuccess;
                            }
                        }
                    }
                }
            }

            sheduleSuccess:;
            return scheduled;
        }

        [HttpGet]
        public ActionResult UserProfile(string playerID)
        {
            ProfileViewModel model = new ProfileViewModel();

            Player player;
            if (playerID == null || playerID == "")
            {
                string userid = User.Identity.GetUserId();
                player = db.Players.Where(val => val.UserID == userid).FirstOrDefault();               
            }
            else
            {
                int intPlayerID = int.Parse(playerID);
                player = db.Players.Where(val => val.PlayerID == intPlayerID).FirstOrDefault();
            }
            
            List<int> teamids = db.Teams.Where(val => val.Players.Select(p => p.PlayerID).Contains(player.PlayerID)).Select(val => val.TeamID).ToList();
            List<Team> teams = db.Teams.Where(val => teamids.Contains(val.TeamID)).ToList();
            model.player = player;
            model.teams = teams;
            return View(model);
        }

        [HttpPost]
        public ActionResult SaveProfile(ProfileViewModel model, string month, string day, string year, 
            string feetHeight, string inchesHeight, string lbWeight, string gender)
        {
            string userid = User.Identity.GetUserId();
            Player player = db.Players.Where(val => val.UserID == userid).FirstOrDefault();
            if (model.profilePhoto != null)
            {
                string pic = System.IO.Path.GetFileName(model.profilePhoto.FileName);
                string path = System.IO.Path.Combine(Server.MapPath("~/images/profile"), pic);
                model.profilePhoto.SaveAs(path);
                player.ProfileImage = "/images/profile/" + pic;
            }
            player.FirstName = model.player.FirstName;
            player.LastName = model.player.LastName;
            if (month != "" && day != "" && year != "")
                player.BirthDate = DateTime.Parse(month + "/" + day + "/" + year);
            if (feetHeight != "" && inchesHeight != "")
                player.Height = (int.Parse(feetHeight) * 12) + int.Parse(inchesHeight);
            if (lbWeight != "")
                player.Weight = int.Parse(lbWeight);
            if (gender != "")
                player.Gender = gender;
            if (player.Gender == "Male" && player.ProfileImage == "/Graphics/default_avatar.png")
                player.ProfileImage = "/Graphics/male_avatar.png";
            else if (player.Gender == "Female" && player.ProfileImage == "/Graphics/default_avatar.png")
                player.ProfileImage = "/Graphics/female_avatar.png";
            db.SaveChanges();
            return RedirectToAction("UserProfile", "Dashboard");
        }

        [HttpPost]
        public ActionResult CreateTeam(ProfileViewModel model)
        {
            string userid = User.Identity.GetUserId();
            Player me = db.Players.Where(val => val.UserID == userid).FirstOrDefault();
            string privateid = RandomString(5);
            while (db.Teams.Any(val => val.PrivateTeamID == privateid))
            {
                privateid = RandomString(5);
            }

            Team team = new Team { TeamName = model.newTeamName, Standing = 1.00, Wins = 0, Losses = 0, PrivateTeamID = privateid };
            team.Players.Add(me);
            db.Teams.Add(team);
            db.SaveChanges();

            return RedirectToAction("UserProfile", "Dashboard");
        }

        [HttpPost]
        public ActionResult JoinTeam(ProfileViewModel model)
        {
            //if (guid != (string)TempData["guid"])
            //    return RedirectToAction("UserProfile", "Dashboard");
            string userid = User.Identity.GetUserId();
            Player me = db.Players.Where(val => val.UserID == userid).FirstOrDefault();
            Team team = db.Teams.Where(val => val.PrivateTeamID == model.joinTeamID).FirstOrDefault();
            if (team != null)
            {
                team.Players.Add(me);
                db.SaveChanges();
            }
            return RedirectToAction("UserProfile", "Dashboard");
        }       

        [HttpPost]
        public ActionResult LeaveTeam(ProfileViewModel model, string teamID)
        {
            string userid = User.Identity.GetUserId();
            int intTeamID = int.Parse(teamID);
            Player me = db.Players.Where(val => val.UserID == userid).FirstOrDefault();
            Team team = db.Teams.Where(val => val.TeamID == intTeamID).FirstOrDefault();

            team.Players.Remove(me);
            if (team.Players.Count() == 0 || team.Players == null)
            {
                db.Teams.Remove(team);
            }

            db.SaveChanges();
            //model.player = me;
            //model.teams.Remove(team);            

            return RedirectToAction("UserProfile", "Dashboard");
        }

        [HttpGet]
        public ActionResult Team(int teamID)
        {
            string userID = User.Identity.GetUserId();
            TeamViewModel model = new TeamViewModel();
            model.games = new List<Game>();
            if (teamID != 0)
            {
                //we need to jump through some hoops to get the games here. We need to see what leagues our team is a part
                //of, then what brackets are associated with those leagues, and then for each bracket, what games do we
                //have scheduled
                Team team = db.Teams.Where(val => val.TeamID == teamID).FirstOrDefault();
                //List<Bracket> brackets1 = db.Leagues.Where(val => val.Teams.Select(t => t.TeamID).Contains(teamID)).Select(l => l.Bracket).ToList();
                List<int> bracketIDs = db.Leagues
                    .Where(val => val.Teams.Select(t => t.TeamID).Contains(teamID) && val.Bracket != null)
                    .Select(l => l.Bracket.BracketID).ToList();
                List<Bracket> brackets = db.Brackets.Where(val => bracketIDs.Contains(val.BracketID)).ToList();
                foreach (Bracket bracket in brackets)
                {
                    model.games.AddRange(bracket.Games.Where( val => val.Team1.TeamID == teamID || val.Team2.TeamID == teamID).ToList());
                }
                model.games = model.games.OrderBy(val => val.GameCompleted).ThenBy(val => val.GameStart).ToList();
                model.team = team;
            }

            List<Player> players = db.Players.Where(val => val.UserID != userID ).ToList();
            model.allPlayers = players;

            return View(model);
        }

        [HttpPost]
        public ActionResult UpdateTeamName(TeamViewModel model)
        {
            Team team = db.Teams.Where(val => val.TeamID == model.team.TeamID).FirstOrDefault();
            team.TeamName = model.team.TeamName;
            db.SaveChanges();
            return RedirectToAction("Team", "Dashboard", new { teamID = team.TeamID });
        }

        [HttpPost]
        public ActionResult AddTeammate(TeamViewModel model, int teamID, int playerID)
        {
            Team team = db.Teams.Where(val => val.TeamID == teamID).FirstOrDefault();
            Player playerToAdd = db.Players.Where(val => val.PlayerID == playerID).FirstOrDefault();

            team.Players.Add(playerToAdd);
            playerToAdd.Teams.Add(team);

            db.SaveChanges();

            return RedirectToAction("Team", "Dashboard", new { teamID = team.TeamID });
        }

        [HttpGet]
        public ActionResult Search(string distance, string sport)
        {
            string userid = User.Identity.GetUserId();
            Player player = db.Players.Where(val => val.UserID == userid).FirstOrDefault();
            FacilityManager manager = db.FacilityManagers.Where(val => val.UserID == userid).FirstOrDefault();
            SearchViewModel model = new SearchViewModel();
            //get all facilities within default 10 mile radius from my location
            model.sports = db.Sports.ToList();
            if (distance != null && distance != "")
                model.Distance = int.Parse(distance);
            else
                model.Distance = 10;
            if (sport != null && sport != "" && sport != "Sport")
                model.sport = db.Sports.Where(val => val.Description == sport).FirstOrDefault();
            else
                model.sport = null;

            List<Facility> facilities = new List<Facility>();
            if ((player != null && player.Latitude != 0) || (manager != null && manager.Latitude != 0))
            {
                GeoCoordinate coord = new GeoCoordinate(Convert.ToDouble(player != null ? player.Latitude : manager.Latitude),
                                                        Convert.ToDouble(player != null ? player.Longitude : manager.Longitude));
                facilities = db.Facility.AsEnumerable()
                    .Where(val => new GeoCoordinate(val.Latitude, val.Longitude).GetDistanceTo(coord) < (model.Distance * 1609.344)).ToList();
                facilities = facilities.Where(val =>
                    (model.sport != null && val.Sports.Select(s => s.SportID).Contains(model.sport.SportID)) ||
                    (model.sport == null)).ToList();
            }

            model.facilities = facilities;
            return View(model);
        }

        [HttpPost]
        public ActionResult Search(SearchViewModel model, string distance, string sport, string lat, string lon)
        {
            if (distance != null && distance != "")
                model.Distance = int.Parse(distance);
            else
                model.Distance = 10;

            if (sport != null && sport != "" && sport != "Sport")
                model.sport = db.Sports.Where(val => val.Description == sport).FirstOrDefault();

            model.sports = db.Sports.ToList();
            //get all facilities with distance mile radius and supporting sport.
            //if facility does not have lat long set then dont add it to the list
            List<Facility> facilities = new List<Facility>();
            if (lat != null && lat != "" && lon != null && lon != "")
            {
                GeoCoordinate coord = new GeoCoordinate(Convert.ToDouble(lat), Convert.ToDouble(lon));
                facilities = db.Facility.AsEnumerable()
                    .Where(val => new GeoCoordinate(val.Latitude, val.Longitude).GetDistanceTo(coord) < (model.Distance * 1609.344)).ToList();
                facilities = facilities.Where(val => 
                    (model.sport != null && val.Sports.Select(s => s.SportID).Contains(model.sport.SportID)) ||
                    (model.sport == null)).ToList();
            }

            model.facilities = facilities;

            return RedirectToAction("Search", "Dashboard", new { distance, sport});
        }  

        [HttpGet]
        public ActionResult Facility(string facilityID)
        {
            int intFacilityID = 0;
            string userid = User.Identity.GetUserId();
            FacilityViewModel model = new FacilityViewModel();
            if (facilityID != null && facilityID != "")
            {
                intFacilityID = int.Parse(facilityID);
                model.facility = db.Facility.Where(val => val.FacilityID == intFacilityID).FirstOrDefault();
            }
            else
            {
                model.facility = db.Facility.Where(val => val.FacilityManager.UserID == userid).FirstOrDefault();
            }

            if (model.facility.Address != null)
            {
                model.address = model.facility.Address.Split(',')[0];
                model.state = model.facility.Address.Split(',')[1];
                model.zipCode = model.facility.Address.Split(',')[2];
            }
            model.facilitySports = model.facility.Sports.Select(val => val.Description).ToList();
            model.sports = db.Sports.Select(val => val.Description).ToList();
            Player me = db.Players.Where(val => val.UserID == userid).FirstOrDefault();
            model.isPinned = (me != null ? me.Facilities.Any(val => val.FacilityID == intFacilityID) : false);
            return View(model);
        }

        [HttpPost]
        public ActionResult PinFacility(FacilityViewModel model, int facilityID)
        {
            string userid = User.Identity.GetUserId();
            Player me = db.Players.Where(val => val.UserID == userid).FirstOrDefault();
            Facility facility = db.Facility.Where(val => val.FacilityID == facilityID).FirstOrDefault();
            if (me.Facilities.Any(val => val.FacilityID == facilityID))
            {
                me.Facilities.Remove(facility);
                model.isPinned = false;
            }
            else
            {
                me.Facilities.Add(facility);
                model.isPinned = true;
            }
            db.SaveChanges();

            return RedirectToAction("Facility", "Dashboard", new { facilityID = facilityID.ToString() });
        }

        [HttpPost]
        public ActionResult Facility(FacilityViewModel model, string state, int facilityID)
        {
            Facility me = db.Facility.Where(val => val.FacilityID == facilityID).FirstOrDefault();
            if (model.address != "" && model.address != null &&
                model.zipCode != "" && model.zipCode != null &&
                state != "" && state != null)
            {
                string address = model.address + ", " + state + ", " + model.zipCode;
                GeoCoordinate coords = GetCoordinates(address);
                me.Latitude = coords.Latitude;
                me.Longitude = coords.Longitude;
                me.Address = address;
            }
            if (model.facilitySports != null)
            {
                me.Sports.Clear();
                //db.Sports.Where(val => val.Facilities.Select(f => f.FacilityID).Contains(me.FacilityID))
                me.Sports = db.Sports.Where(val => model.facilitySports.Contains(val.Description)).ToList();
            }

            me.FacilityName = model.facility.FacilityName;
            db.SaveChanges();
            
            return RedirectToAction("Facility", "Dashboard", new { facilityID = facilityID.ToString() });
        }

        [HttpPost]
        public ActionResult CreateLeague(FacilityViewModel model, int facilityID)
        {
            Facility facility = db.Facility.Where(val => val.FacilityID == facilityID).FirstOrDefault();
            if (model.LeagueName != null && model.LeagueName != "")
            {
                League league = new League
                {
                    LeagueName = model.LeagueName,
                    MaxTeams = model.maxTeams.GetValueOrDefault(),
                    MinTeams = model.minTeams.GetValueOrDefault(),
                    TeamSize = model.teamSize.GetValueOrDefault(),
                    Sport = db.Sports.Where(val => val.Description == model.sport).FirstOrDefault(),
                    RegisterStartPeriod = DateTime.Parse(model.registerStartMonth + "/" + model.registerStartDay + "/" + model.registerStartYear),
                    RegisterEndPeriod = DateTime.Parse(model.registerEndMonth + "/" + model.registerEndDay + "/" + model.registerEndYear)
                };
                facility.Leagues.Add(league);
                db.SaveChanges();
            }

            return RedirectToAction("Facility", "Dashboard", new { facilityID = facilityID.ToString() });
        }

        [HttpPost]
        public ActionResult AddCourt(FacilityViewModel model, int facilityID)
        {
            Facility facility = db.Facility.Where(val => val.FacilityID == facilityID).FirstOrDefault();
            List<Sport> sports = db.Sports.Where(val => model.courtSports.Contains(val.Description)).ToList();

            Court court = new Court {
                CourtName = model.court.CourtName,
                Facility = facility,
                Sports = sports
            };
            db.Courts.Add(court);
            db.SaveChanges();
            
            return RedirectToAction("Facility", "Dashboard", new { facilityID = facilityID.ToString() });
        }

        [HttpGet]
        public ActionResult League(int leagueID)
        {
            string userID = User.Identity.GetUserId();
            LeagueViewModel model = new LeagueViewModel();
            model.league = db.Leagues.Where(val => val.LeagueID == leagueID).FirstOrDefault();
            List<int> leagueTeams = model.league.Teams.Select(t => t.TeamID).ToList();
            if (model.league.Bracket != null)
                model.results = db.BracketResults.Where(val => leagueTeams.Contains(val.TeamID) && val.BracketID == model.league.Bracket.BracketID).ToList();
            model.facility = db.Facility.Where(val => val.Leagues.Select(l => l.LeagueID).Contains(model.league.LeagueID)).FirstOrDefault();
            model.sports = db.Sports.Select(val => val.Description).ToList();
            Player me = db.Players.Where(val => val.UserID == userID).FirstOrDefault();
            if (me != null)
            {
                model.myTeams = me.Teams.ToList();
            }

            model.sport = db.Sports.Where(val => val.Description == model.league.Sport.Description)
                .Select( val => val.Description).FirstOrDefault();
            model.registerStartMonth = model.league.RegisterStartPeriod.Month;
            model.registerStartDay = model.league.RegisterStartPeriod.Day;
            model.registerStartYear = model.league.RegisterStartPeriod.Year;
            model.registerEndMonth = model.league.RegisterEndPeriod.Month;
            model.registerEndDay = model.league.RegisterEndPeriod.Day;
            model.registerEndYear = model.league.RegisterEndPeriod.Year;
            ViewBag.SignupError = TempData["SignupError"] != null ? TempData["SignupError"].ToString() : "";
            ViewBag.BracketError = TempData["BracketError"] != null ? TempData["BracketError"].ToString() : "";
            return View(model);
        }

        //if it is register end period and min teams have signed up
        //auto create bracket (build games and schedule on courts)
        //based on scheduling alogrithm
        [HttpPost]
        public ActionResult League(LeagueViewModel model, int leagueID)
        {
            string userID = User.Identity.GetUserId();
            League league = db.Leagues.Where(val => val.LeagueID == leagueID).FirstOrDefault();
            //check if it is league settings and update those form fields
            if (model.league.LeagueName != null && model.league.LeagueName != "")
            {
                league.LeagueName = model.league.LeagueName;
                league.MaxTeams = model.league.MaxTeams;
                league.MinTeams = model.league.MinTeams;
                league.TeamSize = model.league.TeamSize;
                league.Sport = db.Sports.Where(val => val.Description == model.sport).FirstOrDefault();
                league.RegisterStartPeriod = DateTime.Parse(model.registerStartMonth + "/" + model.registerStartDay + "/" + model.registerStartYear);
                league.RegisterEndPeriod = DateTime.Parse(model.registerEndMonth + "/" + model.registerEndDay + "/" + model.registerEndYear);
                db.SaveChanges();
            }

            return RedirectToAction("League", "Dashboard", new { leagueID });
        }

        [HttpPost]
        public ActionResult TeamSignup(LeagueViewModel model, int leagueID)
        {
            string userID = User.Identity.GetUserId();
            League league = db.Leagues.Where(val => val.LeagueID == leagueID).FirstOrDefault();
            Team team = db.Teams.Where(val => val.TeamID == model.teamSignupID).FirstOrDefault();
            //if we dont go over max teams, 
            //if it is during the sign up process, 
            //if our teamsize fits league setting,
            //if none of our team members are on another team in the league
            //the last check might fail because it wants us to compare ids not objects
            if (league.MaxTeams >= (league.Teams.Count + 1) &&
                (league.RegisterStartPeriod < DateTime.Now && league.RegisterEndPeriod > DateTime.Now) &&
                (league.TeamSize == team.Players.Count) &&
                (league.Teams.Where(t => team.Players.Any(tp => t.Players.Contains(tp))).Count() == 0))
            {
                league.Teams.Add(team);
                db.SaveChanges();
                ViewBag.SignupError = "";
            }
            else if (league.MaxTeams < (league.Teams.Count + 1))
            {
                TempData["SignupError"] = "The maximum amount of teams are already registered.";
            }
            else if (league.RegisterStartPeriod > DateTime.Now || league.RegisterEndPeriod < DateTime.Now)
            {
                TempData["SignupError"] = "It is not during the register sign up period.";
            }
            else if (league.TeamSize != team.Players.Count)
            {
                TempData["SignupError"] = "Your team member count does not match the league requirement.";
            }
            else if (league.Teams.Where(t => team.Players.Any(tp => t.Players.Contains(tp))).Count() != 0)
            {
                TempData["SignupError"] = "Your team is already signed up or you have members on your team on another team.";
            }

            return RedirectToAction("League", "Dashboard", new { leagueID });
        }

        //abstract scheduling algorithm and append to when the gamewinner gets
        //set on the game page
        [HttpPost]
        public ActionResult CreateBracket(LeagueViewModel model, int leagueID)
        {
            string userID = User.Identity.GetUserId();
            League league = db.Leagues.Where(val => val.LeagueID == leagueID).FirstOrDefault();
            if (model.gameStartMonth != 0)
            {
                //if create bracket, check number of teams fits min && max
                if (league.Teams.Count <= league.MaxTeams && league.Teams.Count >= league.MinTeams)
                {
                    List<Day> days = db.Days.Where(val => model.daysOfWeek.Contains(val.Description)).ToList();
                    List<Time> times = new List<Time>();// db.Times.ToList();
                    foreach (string selectedHour in model.hoursOfDay)
                    {
                        DateTime hr = DateTime.Parse(selectedHour);
                        times.AddRange(db.Times.Where(val => val.Description.Hours == hr.TimeOfDay.Hours).ToList());
                    }

                    Bracket bracket = new Bracket
                    {
                        BracketStartDate = DateTime.Parse(model.gameStartMonth + "/" + model.gameStartDay + "/" + model.gameStartYear),
                        GameLength = new TimeSpan(model.gameLengthHour.GetValueOrDefault(), model.gameLengthMin.GetValueOrDefault(), 0),
                        Days = days,
                        Times = times, //Available times //list of hours, 
                        daysBetweenRounds = model.daysBetweenRounds.GetValueOrDefault()
                    };

                    foreach (Team team in league.Teams)
                    {
                        BracketResult result = new BracketResult
                        {
                             Bracket = bracket,
                             Team = team, 
                             Losses = 0,
                             Wins = 0,
                             TeamID = team.TeamID
                        };
                        db.BracketResults.Add(result);
                    }

                    //take all registered teams put into list, while list is not empty pull best vs worst
                    //create game based on timing specified in form and court availability
                    List<Team> unscheduledTeams = league.Teams.OrderBy(val => (val.Wins / (val.Wins == 0 && val.Losses == 0 ? 1 : val.Wins + val.Losses) ) ).ToList();
                    List<Court> availableCourts = db.Courts.Where(val => val.Sports.Select(s => s.Description).Contains(league.Sport.Description)).ToList();
                    Team best, worst;
                    Game game = new Game();
                    TimeSpan gameLength = bracket.GameLength;
                    bool scheduled = false;
                    while (unscheduledTeams.Count != 0)
                    {
                        //uneven team count, list top team as bye week
                        if (unscheduledTeams.Count % 2 != 0)
                        {
                            bracket.UnscheduledTeams.Add(unscheduledTeams[0]);
                            unscheduledTeams.RemoveAt(0); 
                        }

                        //schedule best vs worse on available facility court, 
                        //and during bracket available days and times starting at bracketstartdate
                        best = unscheduledTeams[0];
                        worst = unscheduledTeams[unscheduledTeams.Count - 1];

                        scheduled = scheduleGame(bracket, availableCourts, game, best, worst, bracket.BracketStartDate);

                        if (scheduled)
                        {
                            unscheduledTeams.RemoveAt(0);
                            unscheduledTeams.RemoveAt(unscheduledTeams.Count - 1);
                        }
                        else
                        {
                            TempData["BracketError"] = "Not enough available times to schedule games. Open up available days and times to schedule games.";
                            goto doneScheduling; //leave scheduling algorithm without saving if any teams cant be scheduled. initial scheduling is atomic.
                        }
                    }

                    league.Bracket = bracket;
                    db.SaveChanges();
                }           
            }
            doneScheduling:;
            return RedirectToAction("League", "Dashboard", new { leagueID });
        }

        [HttpGet]
        public ActionResult Game(string gameID)
        {
            GameViewModel model = new GameViewModel();
            int intGameID = int.Parse(gameID);
            model.game = db.Games.Where(val => val.GameID == intGameID).FirstOrDefault();
            //we need the facility so we know if we are the facility manager and can mark who won
            //the game. We need the bracket from the gameid, the league from the bracket, and the facility
            //from the league
            Bracket bracket = db.Brackets.Where(val => val.Games.Select(g => g.GameID).Contains(model.game.GameID)).FirstOrDefault();
            League league = db.Leagues.Where(val => val.Bracket.BracketID == bracket.BracketID).FirstOrDefault();
            if (league != null)
            {
                Facility facility = db.Facility.Where(val => val.Leagues.Select(l => l.LeagueID).Contains(league.LeagueID)).FirstOrDefault();
                model.facility = facility;
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult Game(GameViewModel model, int gameID)
        {
            //here we need to check list of unscheduled teams and schedule a new game
            //for each team. Dont allow same teams to play eachother check previous games in bracket
            //schedule next game daysbetweenrounds after this game. Check if every team has played all other teams
            //then we are done. if not done but cant schedule games yet, just add teams to unscheduled list
            //gonna set bracket results here
            Team winningTeam = db.Teams.Where(val => val.TeamID == model.winningTeamID).FirstOrDefault();
            Game game = db.Games.Where(val => val.GameID == gameID).FirstOrDefault();
            Bracket bracket = db.Brackets.Where(val => val.Games.Select(g => g.GameID).Contains(game.GameID)).FirstOrDefault();
            League league = db.Leagues.Where(val => val.Bracket.BracketID == bracket.BracketID).FirstOrDefault();
            for (int i = 0; i < 2; i++)// (Team team in game.Teams)
            {
                Team team = (i == 0 ? game.Team1 : game.Team2);
                //check if bracketresult exists already and update wins / losses
                //if not create a new bracketresult. The relationship should be 1 bracketresult for each team
                BracketResult result = db.BracketResults
                    .Where(val => val.BracketID == bracket.BracketID && val.TeamID == team.TeamID)
                    .FirstOrDefault();
                if (result == null)
                {
                    //now that we are initializing the bracketresults for each team in create bracket
                    //this should never be null
                    result = new BracketResult
                    {
                        Team = team,
                        Wins = (winningTeam.TeamID == team.TeamID ? 1 : 0),
                        Losses = (winningTeam.TeamID == team.TeamID ? 0 : 1),
                        Bracket = bracket
                    };
    
                    db.BracketResults.Add(result);
                }
                else
                {
                    result.Wins = (winningTeam.TeamID == team.TeamID) ? result.Wins + 1 : result.Wins;
                    result.Losses = (winningTeam.TeamID == team.TeamID) ? result.Losses : result.Losses + 1;
                }

                team.Wins = (winningTeam.TeamID == team.TeamID ? team.Wins + 1 : team.Wins);
                team.Losses = (winningTeam.TeamID == team.TeamID ? team.Losses : team.Losses + 1);

                //check if they have played every other team in the league, maybe do count of games == count of teams - 1
                //if so, we are done here
                List<Game> ourGames;
                Team scheduledTeam = null;
                bool scheduled = false;
                List<Court> availableCourts = db.Courts.Where(val => val.Sports.Select(s => s.Description).Contains(league.Sport.Description)).ToList();
                Game newGame = new Game();
                if (!(bracket.Games.Where(g => g.Team1.TeamID == team.TeamID || g.Team2.TeamID == team.TeamID).Count() == (league.Teams.Count() - 1)) )
                {
                    //if not, loop unscheduled list and choose the first team we have not already played. 
                    foreach (Team unscheduledTeam in bracket.UnscheduledTeams)
                    {
                        ourGames = bracket.Games.Where(val => val.Team1.TeamID == team.TeamID || val.Team2.TeamID == team.TeamID).ToList();
                        if (ourGames.Where(val => val.Team1.TeamID == unscheduledTeam.TeamID || val.Team2.TeamID == unscheduledTeam.TeamID).Count() == 0)
                        {
                            //schedule game with this team starting at daysbetweenRounds.
                            scheduled = scheduleGame(bracket, availableCourts, newGame, team, unscheduledTeam, game.GameStart.AddDays(bracket.daysBetweenRounds));
                            if (scheduled)
                            {
                                scheduledTeam = unscheduledTeam;
                                //bracket.UnscheduledTeams.Remove(unscheduledTeam);
                                goto scheduleSuccess;
                            }
                        }
                    }

                    //if we cant find anyone to play just add ourselves to unscheduled list
                    bracket.UnscheduledTeams.Add(team);
                    scheduleSuccess:;
                    if (scheduled)
                        bracket.UnscheduledTeams.Remove(scheduledTeam);
                }
            }

            game.WinningTeam = winningTeam;
            game.GameCompleted = true;
            db.SaveChanges();

            return RedirectToAction("Game", "Dashboard", new { gameID = gameID.ToString() });
        }

        [HttpGet]
        public ActionResult Court(string courtID, string facilityID)
        {
            CourtViewModel model = new CourtViewModel();
            int intCourtID = int.Parse(courtID);
            int intFacilityID = int.Parse(facilityID);
            model.court = db.Courts.Where(val => val.CourtID == intCourtID).FirstOrDefault();
            Facility facility = db.Facility.Where(val => val.FacilityID == intFacilityID).FirstOrDefault();
            model.leagues = facility.Leagues.ToList();
            return View(model);
        }

    }
}