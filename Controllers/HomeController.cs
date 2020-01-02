using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using BattlePlanner.Models;

namespace BattlePlanner.Controllers
{
    public class HomeController : Controller
    {
        private MyContext dbContext;
        public HomeController(MyContext context)
        {
            dbContext = context;
        }

        //Homepage
        [HttpGet("")]
        public IActionResult Index()
        {
            HttpContext.Session.Clear();
            return View();
        }

        //Login
        [HttpPost("")]
        public IActionResult LoginCheck(LoginUser fromForm)
        {   
            if(ModelState.IsValid)
            {
                User selectedUser = dbContext.UserTable.FirstOrDefault(u => u.Email == fromForm.Email);
                if (selectedUser == null)
                {
                    ModelState.AddModelError("Email", "Invalid Email/Password!");
                    return View("Index");
                }

                else
                {
                    var hasher = new PasswordHasher<LoginUser>();
                    var result = hasher.VerifyHashedPassword(fromForm, selectedUser.Password, fromForm.Password);
                    if(result == 0)
                    {
                        ModelState.AddModelError("Email", "Invalid Email/Password!");
                        return View("Index");
                    }
                }
                HttpContext.Session.SetInt32("userKey", selectedUser.UserId);
                return RedirectToAction("Dashboard");
            }
            return View("Index");
        }

        //User Registration
        [HttpGet("NewUser")]
        public IActionResult Register()
        {
            HttpContext.Session.Clear();
            return View();
        }

        [HttpPost("NewUser")]
        public IActionResult AddNewUser(User fromForm)
        {
            DateTime now = DateTime.Now;
            int years = (int)(now.Year - fromForm.Birthdate.Year);

            if(now.Month < fromForm.Birthdate.Month || (now.Month == fromForm.Birthdate.Month && now.Day < fromForm.Birthdate.Day))
            {
                years = years - 1;
            }

            if(!ModelState.IsValid || years < 18)
            {
                ModelState.AddModelError("Birthdate", "You are too young! Try a fight club for babies.");
                return View("Register");
            }
           if(dbContext.UserTable.Any(u => u.Email == fromForm.Email))
                {
                    ModelState.AddModelError("Email", "Someone has claimed this email!");
                    return View("Register");
                }

            PasswordHasher<User> Hasher = new PasswordHasher<User>();
            fromForm.Password = Hasher.HashPassword(fromForm, fromForm.Password);
            dbContext.Add(fromForm);
            dbContext.SaveChanges();

            User selectedUser = dbContext.UserTable.FirstOrDefault(u => u.Email == fromForm.Email);

            HttpContext.Session.SetInt32("userKey", selectedUser.UserId);
            return RedirectToAction("Dashboard");
        }

        //Dashboard
        [HttpGet("Dashboard")]
        public IActionResult Dashboard()
        {
            int? SessionId = (HttpContext.Session.GetInt32("userKey"));
            var LoggedUser = dbContext.UserTable.FirstOrDefault(u => u.UserId == SessionId);
            if(LoggedUser == null)
            {
                return RedirectToAction("Index");
            }
            List<Fight> myFights = dbContext.FightTable.Where(f => f.Creator == LoggedUser).ToList();
            List<Fight> AllFights = dbContext.FightTable.Include(f => f.Roster).ToList();
            ViewBag.myFights = myFights;
            ViewBag.AllFights = AllFights;
            ViewBag.Creator = "YES";
  
            return View(LoggedUser);
        }

        //Fights
        [HttpGet("Fights")]
        public IActionResult Fights()
        {
            List<Fight> AllFights = dbContext.FightTable.OrderBy(f => f.CreatedAt).ToList();
            ViewBag.AllFights = AllFights;
            return View("Fights", AllFights);
        }

        //FightViewer
        [HttpGet("Fights/{ID}")]
        public IActionResult FightViewer(int ID)
        {
            int? SessionId = HttpContext.Session.GetInt32("userKey");
            var LoggedUser = dbContext.UserTable.FirstOrDefault(u => u.UserId == SessionId);
            if(LoggedUser == null)
            {
                return RedirectToAction("Index");
            }
            Fight selectedFight = dbContext.FightTable.Include(f => f.Creator).FirstOrDefault(f => f.FightId == ID);
            List<Team> AllTeams = dbContext.TeamTable.Include(t => t.Participant).Where(t => t.FightId == ID).ToList();
            ViewBag.AllTeams = AllTeams;
            List<Taunt> AllTaunts = dbContext.TauntTable.Where(t => t.FightId == ID).OrderBy(t => t.CreatedAt).ToList();
            ViewBag.AllTaunts = AllTaunts;
            return View(selectedFight);
        }

        //FightAdd
        [HttpGet("Fights/Add")]
        public IActionResult FightForm()
        {
            int? SessionId = (HttpContext.Session.GetInt32("userKey"));
            ViewBag.SessionId = SessionId;
            var LoggedUser = dbContext.UserTable.FirstOrDefault(u => u.UserId == SessionId);
            if(LoggedUser == null)
            {
                return RedirectToAction("Index");
            }
            return View();
        }

        [HttpPost("Fights/Add")]
        public IActionResult FightFormAction(Fight fromForm)
        {   
            int? SessionId = (HttpContext.Session.GetInt32("userKey"));
            var LoggedUser = dbContext.UserTable.FirstOrDefault(u => u.UserId == SessionId);


            DateTime now = DateTime.Now;
            if(!ModelState.IsValid)
            {
                return View("FightForm");
            }

            if(fromForm.FightDate < now)
            {
                return View("FightForm");
            }
            var NewFight = new Fight()
            {
                Location = fromForm.Location,
                FightDate = fromForm.FightDate,
                UserId = LoggedUser.UserId
            };
            dbContext.Add(NewFight);
            dbContext.SaveChanges();
            return RedirectToAction("Dashboard");
        }

        //FightJoin
        [HttpGet("Fights/{ID}/Red")]
        public IActionResult JoinFightRed(int ID)
        {
            int? SessionId = HttpContext.Session.GetInt32("userKey");
            var LoggedUser = dbContext.UserTable.FirstOrDefault(u => u.UserId == SessionId);
            if(LoggedUser == null)
            {
                return RedirectToAction("Index");
            }
            Team TeamCheck = dbContext.TeamTable.FirstOrDefault((t => t.UserId == LoggedUser.UserId && t.FightId == ID));
            if(TeamCheck != null && TeamCheck.TeamColor == "Red")
            {
                return RedirectToAction("FightViewer", new {ID = ID});
            }
            if(TeamCheck != null && TeamCheck.TeamColor == "Blue")
            {
                dbContext.TeamTable.Remove(TeamCheck);
            }
            Fight selectedFight = dbContext.FightTable.FirstOrDefault(f => f.FightId == ID);

            var newTeam = new Team()
            {
                UserId = LoggedUser.UserId,
                FightId = selectedFight.FightId,
                TeamColor = "Red"
            };
            dbContext.Add(newTeam);
            dbContext.SaveChanges();
            return RedirectToAction("FightViewer", new {ID = ID});

        }

        [HttpGet("Fights/{ID}/Blue")]
        public IActionResult JoinFightBlue(int ID)
        {
            int? SessionId = HttpContext.Session.GetInt32("userKey");
            var LoggedUser = dbContext.UserTable.FirstOrDefault(u => u.UserId == SessionId);
            if(LoggedUser == null)
            {
                return RedirectToAction("Index");
            }
            Team TeamCheck = dbContext.TeamTable.FirstOrDefault((t => t.UserId == LoggedUser.UserId && t.FightId == ID));
            if(TeamCheck != null && TeamCheck.TeamColor == "Blue")
            {
                return RedirectToAction("FightViewer", new {ID = ID});
            }
            if(TeamCheck != null && TeamCheck.TeamColor == "Red")
            { 
                dbContext.TeamTable.Remove(TeamCheck);
            }
            Fight selectedFight = dbContext.FightTable.FirstOrDefault(f => f.FightId == ID);

            var newTeam = new Team()
            {
                UserId = LoggedUser.UserId,
                FightId = selectedFight.FightId,
                TeamColor = "Blue"
            };
            dbContext.Add(newTeam);
            dbContext.SaveChanges();
            return RedirectToAction("FightViewer", new {ID = ID});
        }

        [HttpGet("Fights/{ID}/Leave")]
        public IActionResult LeaveFight(int ID)
        {
            int? SessionId = HttpContext.Session.GetInt32("userKey");
            var LoggedUser = dbContext.UserTable.FirstOrDefault(u => u.UserId == SessionId);
            if(LoggedUser == null)
            {
                return RedirectToAction("Index");
            }
            Team TeamCheck = dbContext.TeamTable.FirstOrDefault((t => t.UserId == LoggedUser.UserId && t.FightId == ID));
            if(TeamCheck != null)
            {
                dbContext.TeamTable.Remove(TeamCheck);
                dbContext.SaveChanges();
                return RedirectToAction("FightViewer", new {ID = ID});
            }

            return RedirectToAction("FightViewer", new {ID = ID});

        }

        //AddTaunt
        [HttpPost("Fights/{ID}")]
        public IActionResult AddTaunt(int ID, Taunt fromForm)
        {
            int? SessionId = HttpContext.Session.GetInt32("userKey");
            var LoggedUser = dbContext.UserTable.FirstOrDefault(u => u.UserId == SessionId);
            if(LoggedUser == null)
            {
                return RedirectToAction("Index");
            }
           var NewTaunt = new Taunt()
           { 
            Message = fromForm.Message,
            UserId = LoggedUser.UserId,
            FightId = ID
        };
            dbContext.Add(NewTaunt);
            dbContext.SaveChanges();
            return RedirectToAction("FightViewer", new {ID = ID});
        }

        [HttpGet("Fights/{ID}/Edit")]
        public IActionResult EditViewer(int ID)
        {
            var EditedFight = dbContext.FightTable.FirstOrDefault(f => f.FightId == ID);
            return RedirectToAction ("FightForm");
        }

        [HttpPost("Fights/{ID}/Edit")]
        public IActionResult EditAction(int ID, Fight fromForm)
        {
            int? SessionId = (HttpContext.Session.GetInt32("userKey"));
            var LoggedUser = dbContext.UserTable.FirstOrDefault(u => u.UserId == SessionId);
            var EditedFight = dbContext.FightTable.FirstOrDefault(f => f.FightId == ID);

            DateTime now = DateTime.Now;
            if(!ModelState.IsValid)
            {
                return View("FightForm");
            }

            if(fromForm.FightDate < now)
            {
                return View("FightForm");
            }

            EditedFight.Location = fromForm.Location;
            EditedFight.FightDate = fromForm.FightDate;
            dbContext.SaveChanges();
            return RedirectToAction("FightViewer", new {ID = ID});
        }

//----------------------------------------------------//
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
