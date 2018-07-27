using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using DefaultProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DefaultProject.Controllers
{
    public class HomeController : Controller
    {
        private ProjectContext _context;
        public HomeController (ProjectContext context)
        {
            _context = context;
        }

       [HttpGet]
        public IActionResult Index()
        {
           int? UserId = HttpContext.Session.GetInt32("UserId");
            if (UserId != null)
            {
                return RedirectToAction("Dashboard");
            }
            else
            {
                return View("Index");
            }
        }

        [HttpPost]
        [Route("create")]
        public IActionResult Create(User user)
        {

            var checkusername = user.username;
            var checkemail = user.email;
            User checkDuplicate = _context.users.SingleOrDefault(u => u.username == checkusername && u.email == checkemail);

                if(ModelState.IsValid){
                    if(checkDuplicate != null){
                        Console.WriteLine("Username or email already in use!");
                        return View("Index");
                    }
                    else{
                    PasswordHasher<User> Hasher = new PasswordHasher<User>();
                    user.password = Hasher.HashPassword(user,user.password);
                    user.confirm_password = Hasher.HashPassword(user,user.confirm_password);
                    
                        _context.Add(user);
                        _context.SaveChanges();
                        
                    return View("Success");
                }
                }
                else{

                    return View("Index");

                }
            }

        [HttpPost]
        [Route("login")]
        public IActionResult Login(string CheckEmail, string CheckPassword)
        {
            User checkUser = _context.users.SingleOrDefault(user => user.email == CheckEmail);
            if (checkUser != null && CheckPassword != null){
                var Hasher = new PasswordHasher<User>();
                if(0 != Hasher.VerifyHashedPassword(checkUser, checkUser.password, CheckPassword))
                {
                    Console.WriteLine("GOOD!");
                    HttpContext.Session.SetInt32("UserId", checkUser.id);
                    return RedirectToAction("Dashboard");
                }
                else{
                    Console.WriteLine("BAD!");
                    TempData["error"] = "Incorrect credentials!";
                    return View("Index");
                }
            }
            else{
                Console.WriteLine("MISSING INFO!");
                TempData["error"] = "Please fill in all fields!";
                return View("Index");
            }
            
        }

        [HttpGet]
        [Route("logout")]
        public IActionResult LogOut()
        {
            HttpContext.Session.Clear();
            return View("Index");
        }

        // Dashboard Stuff

        
        [HttpGet]
        [Route("dashboard")]
        public IActionResult Dashboard()
        {
            int? UserId = HttpContext.Session.GetInt32("UserId");
            if (UserId != null)
            {

                DateTime today = DateTime.Now;
                string todayFormat = today.ToString("yyyy-MM-dd");
                string timeFormat = today.ToString("hh:mm");

                ViewBag.today = todayFormat;
                ViewBag.time = timeFormat;

                User currentUser = _context.users.SingleOrDefault(u => u.id == UserId);
                List<Mimic> usersMimics = _context.mimics.Include(m => m.owner).Where(m => m.owner.id == UserId).ToList();

                ViewBag.currentUser = currentUser;
                ViewBag.usersMimics = usersMimics;

                return View("Dashboard");
            }
            else
            {
                return View("Index");
            }
        }

        // Profile

        [HttpGet]
        [Route("profile")]
        public IActionResult ProfileRe(int userId)
        {

                int? UserId = HttpContext.Session.GetInt32("UserId");
                if (UserId != null)
                {

                User currentUser = _context.users.SingleOrDefault(u => u.id == UserId);

                return RedirectToAction("profile", new { currentUser.id });

                }

            else
            {
                return View("Notlogged");
            }
        }


        [HttpGet]
        [Route("Home/profile/{userId}")]
        public IActionResult ProfileView(int userId)
        {

                //int? UserId = HttpContext.Session.GetInt32("sUserId");

                User profileUser = _context.users.SingleOrDefault(u => u.id == userId);
                List<Mimic> usersMimics = _context.mimics.Include(m => m.owner).Where(m => m.owner.id == profileUser.id).ToList();

                ViewBag.profileUser = profileUser;
                ViewBag.usersMimics = usersMimics;

                return View("Profile");


        }



        // Adventures!

        [HttpGet]
        [Route("adventure")]
        public IActionResult Adventure()
        {
            int? UserId = HttpContext.Session.GetInt32("UserId");
            if (UserId != null)
            {

                User currentUser = _context.users.SingleOrDefault(u => u.id == UserId);
                List<Mimic> usersMimics = _context.mimics.Include(m => m.owner).Where(m => m.owner.id == UserId && m.hp > 0 && m.hunger > 0).ToList();

                ViewBag.currentUser = currentUser;
                ViewBag.usersMimics = usersMimics;

                return View("Adventure");
            }
            else
            {
                return View("Notlogged");
            }
        }

        [HttpGet]
        [Route("adventure/go/{mimicId}")]
        public IActionResult Adventure(int mimicId)
        {
            int? UserId = HttpContext.Session.GetInt32("UserId");
            if (UserId != null)
            {

                User currentUser = _context.users.SingleOrDefault(u => u.id == UserId);
                Mimic currentMimic = _context.mimics.SingleOrDefault(m => m.id == mimicId);

                if (currentMimic.hp <= 0 || currentMimic.hunger <= 0){
                    return View("Error");
                }

                else{

                        Random random = new Random();
                        int randomGold = random.Next(0, (currentMimic.str+currentMimic.dex+currentMimic.inte));
                        int randomFood = random.Next(0, (currentMimic.str+currentMimic.dex+currentMimic.inte)/8);
                        int randomPotion = random.Next(0, (currentMimic.str+currentMimic.dex+currentMimic.inte)/8);
                        int randomSpecial = random.Next(1,100);

                        int randomHP = random.Next(0,4);
                        int randomHunger = random.Next(0,4);

                        int randomInte = random.Next(0,4);
                        int randomDex = random.Next(1,4);
                        int randomStr = random.Next(1,4);

                        currentUser.gold += randomGold;
                        currentUser.food += randomFood;
                        currentUser.potions += randomPotion;

                        if(randomSpecial == 33){
                            currentUser.specialEgg += 1;
                            ViewBag.special = "Wow! You collected a special egg ticket! Click adopt to see the rare species options!";
                        }

                        else{
                            ViewBag.special = "";
                        }

                        currentMimic.hp -= randomHP;
                        currentMimic.hunger -= randomHunger;
                        currentMimic.xp += 1;

                        _context.SaveChanges();

                        if (currentMimic.hp < 0){
                            currentMimic.hp = 0;
                        }

                        if (currentMimic.hunger < 0){
                            currentMimic.hunger = 0;
                        }

                        if (currentMimic.xp >= (currentMimic.lvl * 5)){
                            currentMimic.xp = 0;
                            currentMimic.lvl += 1;
                            currentMimic.str += randomStr;
                            currentMimic.inte += randomInte;
                            currentMimic.dex += randomDex;
                            ViewBag.leveledUp = "Your mimic gained a level! Stats increased.";
                        }
                        else{
                            ViewBag.leveledUp = "";
                        }

                        _context.SaveChanges();

                        ViewBag.currentUser = currentUser;
                        ViewBag.currentMimic = currentMimic;

                        ViewBag.gainedGold = randomGold;
                        ViewBag.gainedFood = randomFood;
                        ViewBag.gainedPotions = randomPotion;

                        ViewBag.lostHP = randomHP;
                        ViewBag.lostHunger = randomHunger;

                return View("Adventureresults");
                }
            }
            else
            {
                return View("Notlogged");
            }
        }

        // Inventory

        [HttpGet]
        [Route("inventory")]
        public IActionResult Inventory()
        {
            int? UserId = HttpContext.Session.GetInt32("UserId");
            if (UserId != null)
            {
                User currentUser = _context.users.SingleOrDefault(u => u.id == UserId);
                
                ViewBag.currentUser = currentUser;

                return View("Inventory");
            }
            else
            {
                return View("Notlogged");
            }
        }

        // News

        [HttpGet]
        [Route("news")]
        public IActionResult News()
        {

            int? UserId = HttpContext.Session.GetInt32("UserId");

            User currentUser = _context.users.SingleOrDefault(u => u.id == UserId);
                
            ViewBag.currentUser = currentUser;

            return View("News");

        }

        // Healing and Feeding

        [HttpGet]
        [Route("heal/{mimicId}")]
        public IActionResult Heal(int mimicId)
        {
            int? UserId = HttpContext.Session.GetInt32("UserId");
            if (UserId != null)
            {

                User currentUser = _context.users.SingleOrDefault(u => u.id == UserId);
                Mimic currentMimic = _context.mimics.Include(m => m.owner).SingleOrDefault(m => m.id == mimicId);

                if (currentMimic.owner.potions <= 0){
                    return View("Error");
                }

                else{
                        currentMimic.hp += 1;
                        currentUser.potions -=1;

                        _context.SaveChanges();

                return RedirectToAction("mimic", new { currentMimic.id });
                }
            }
            else
            {
                return View("Notlogged");
            }
        }

        [HttpGet]
        [Route("feed/{mimicId}")]
        public IActionResult Feed(int mimicId)
        {
            int? UserId = HttpContext.Session.GetInt32("UserId");
            if (UserId != null)
            {

                User currentUser = _context.users.SingleOrDefault(u => u.id == UserId);
                Mimic currentMimic = _context.mimics.Include(m => m.owner).SingleOrDefault(m => m.id == mimicId);

                if (currentMimic.owner.food <= 0){
                    return View("Error");
                }

                else{
                        currentMimic.hunger += 1;
                        currentUser.food -=1;

                        _context.SaveChanges();

                return RedirectToAction("mimic", new { currentMimic.id });

                }
            }
            else
            {
                return View("Notlogged");
            }
        }

        // Not Logged In

        [HttpGet]
        [Route("not-logged")]
        public IActionResult pleaseLogIn()
        {
            int? UserId = HttpContext.Session.GetInt32("UserId");
            if (UserId != null)
            {
                return View("Dashboard");
            }
            else
            {
                return View("Notlogged");
            }
        }

        // Adoption Creation Stuff

        [HttpGet]
        [Route("adopt")]
        public IActionResult Adopt()
        {
            int? UserId = HttpContext.Session.GetInt32("UserId");
            if (UserId != null)
            {
                User currentUser = _context.users.SingleOrDefault(u => u.id == UserId);
                ViewBag.currentUser = currentUser;
                return View("Adopt");
            }
            else
            {
                return View("Notlogged");
            }
        }

        [HttpPost]
        [Route("createMimic")]
        public IActionResult createMimic(string name, string species)
        {
            int? UserId = HttpContext.Session.GetInt32("UserId");
                if (UserId != null)
                {
    
                    if(ModelState.IsValid){
                        User currentUser = _context.users.SingleOrDefault(u => u.id == UserId);

                        if(species == "spike" && currentUser.specialEgg < 1){
                            return View("Error");
                        }

                        else if(species == "spike" && currentUser.specialEgg >= 1){
                            currentUser.specialEgg -= 1;
                        }

                        Random random = new Random();
                        int randomColor = random.Next(1, 4);
                        int randomStr = random.Next(1,16);
                        int randomInt = random.Next(1,16);
                        int randomDex = random.Next(1,16);

                        Mimic newMimic = new Mimic();
                        newMimic.owner = currentUser;
                        newMimic.name = name;
                        newMimic.species = species;
                        newMimic.color = randomColor;

                        newMimic.xp = 0;
                        newMimic.lvl = 1;

                        newMimic.hp = 10;
                        newMimic.hunger = 10;
                        newMimic.str = randomStr;
                        newMimic.inte = randomInt;
                        newMimic.dex = randomDex;

                        newMimic.created_at =  DateTime.Now;
                        newMimic.updated_at =  DateTime.Now;

                        _context.Add(newMimic);
                        _context.SaveChanges();

                        currentUser.mimics.Add(newMimic);
                        _context.SaveChanges();

                        return RedirectToAction("mimic", new { id = newMimic.id });
                    }

                    else{
                         return RedirectToAction("adopt");
                    }
                }

                else
                {
                    return View("Notlogged");
                }
        }

        // View Mimic

        [HttpGet]
        [Route("Home/mimic/{mimicId}")]
        public IActionResult viewMimic(int mimicId)
        {
            int? UserId = HttpContext.Session.GetInt32("UserId");

                User currentUser = _context.users.SingleOrDefault(u => u.id == UserId);
                Mimic currentMimic = _context.mimics.Include(m => m.owner).SingleOrDefault(m => m.id == mimicId);

                double xppercent = ((double)currentMimic.xp/((double)currentMimic.lvl*5.00))*100.00;

                int hppercent = currentMimic.hp * 10;

                int hungerpercent = currentMimic.hunger * 10;

                ViewBag.currentMimic = currentMimic;
                ViewBag.currentUser = currentUser;
                ViewBag.percentXP = Math.Round(xppercent, 2);
                ViewBag.percentHP = hppercent;
                ViewBag.percentHunger = hungerpercent;

                return View("Mimic");
        }
    }
}
