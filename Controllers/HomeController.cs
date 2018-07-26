﻿using System;
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
            // Console.WriteLine("Email is: " + CheckEmail);
            // Console.WriteLine("Password is: " + CheckPassword);

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

                // List<ActivityEvent> rsvps = _context.activities.Include(u => u.rsvps).ToList();
                // List<ActivityEvent> allActivityEvents = _context.activities.Include(a => a.creator).OrderBy(a => a.date).Where(a => a.date > today).ToList(); 
                User currentUser = _context.users.SingleOrDefault(u => u.id == UserId);
                List<Mimic> usersMimics = _context.mimics.Include(m => m.owner).Where(m => m.owner.id == UserId).ToList();

                // ViewBag.allActivityEvents = allActivityEvents;
                ViewBag.currentUser = currentUser;
                ViewBag.usersMimics = usersMimics;
                // ViewBag.rsvps_cool = rsvps_cool;

                return View("Dashboard");
            }
            else
            {
                return View("Index");
            }
        }

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

        // // Join Event

        // [HttpPost]
        // [Route("rsvp/{activityId}")]
        // public IActionResult rsvpActivityEvent(int activityId)
        // {
        //     int? UserId = HttpContext.Session.GetInt32("UserId");
        //         if (UserId != null)
        //         {

        //             User currentUser = _context.users.SingleOrDefault(u => u.id == UserId);

        //             ActivityEvent currentActivityEvent = _context.activities.SingleOrDefault(w => w.id == activityId);

        //             List<RSVP> thisUsersEvents = _context.rsvps.Where(r => r.user.id == currentUser.id).Include(r => r.activity).ToList();

        //             RSVP checkDuplicate = _context.rsvps.SingleOrDefault(r => r.user.id == currentUser.id && r.activity.id == currentActivityEvent.id);

        //             RSVP checkConflict = _context.rsvps.SingleOrDefault(r => r.user.id == currentUser.id && r.activity.date == currentActivityEvent.date && r.activity.id != currentActivityEvent.id);

        //             string checkConflict2 = null;

        //             int thiseventfulldurationmin = 0;
                    
        //             if (currentActivityEvent.durationtime == "Minutes"){
        //                 thiseventfulldurationmin = currentActivityEvent.duration;
        //                 }
        //             else if (currentActivityEvent.durationtime == "Hours"){
        //                 thiseventfulldurationmin = currentActivityEvent.duration * 60;
        //                 }
        //             else {
        //                 thiseventfulldurationmin = currentActivityEvent.duration * 1440;
        //                 }

        //             foreach (var e in thisUsersEvents){
        //                 if (e.activity.durationtime == "Minutes"){
        //                     int fulldurationminutes = e.activity.duration;
        //                     DateTime starttime = e.activity.date;
        //                     DateTime endtime = starttime.AddMinutes(fulldurationminutes);

        //                     DateTime thisStarttime = currentActivityEvent.date;
        //                     DateTime thisEndtime = thisStarttime.AddMinutes(thiseventfulldurationmin);

        //                     if(currentActivityEvent.date >= starttime && currentActivityEvent.date <= endtime && currentActivityEvent.id != e.activity.id){
        //                         checkConflict2 = "YUP!";
        //                     }

        //                     else if(e.activity.date >= thisStarttime && e.activity.date <= thisEndtime && currentActivityEvent.id != e.activity.id){
        //                         checkConflict2 = "YUP!";
        //                     }
                        
        //                 }
        //                 else if (e.activity.durationtime == "Hours"){
        //                     int fulldurationminutes = e.activity.duration * 60;
        //                     DateTime starttime = e.activity.date;
        //                 DateTime endtime = starttime.AddMinutes(fulldurationminutes);
                        
        //                 DateTime thisStarttime = currentActivityEvent.date;
        //                     DateTime thisEndtime = thisStarttime.AddMinutes(thiseventfulldurationmin);

        //                     if(currentActivityEvent.date >= starttime && currentActivityEvent.date <= endtime && currentActivityEvent.id != e.activity.id){
        //                         checkConflict2 = "YUP!";
        //                     }

        //                     else if(e.activity.date >= thisStarttime && e.activity.date <= thisEndtime && currentActivityEvent.id != e.activity.id){
        //                         checkConflict2 = "YUP!";
        //                     }
        //                 }
        //                 else {
        //                     int fulldurationminutes = e.activity.duration * 1440;
        //                     DateTime starttime = e.activity.date;
        //                 DateTime endtime = starttime.AddMinutes(fulldurationminutes);

        //                 DateTime thisStarttime = currentActivityEvent.date;
        //                     DateTime thisEndtime = thisStarttime.AddMinutes(thiseventfulldurationmin);

        //                     if(currentActivityEvent.date >= starttime && currentActivityEvent.date <= endtime && currentActivityEvent.id != e.activity.id){
        //                         checkConflict2 = "YUP!";
        //                     }

        //                     else if(e.activity.date >= thisStarttime && e.activity.date <= thisEndtime && currentActivityEvent.id != e.activity.id){
        //                         checkConflict2 = "YUP!";
        //                     }
        //                 }
        //             }
                    

        //             if (checkConflict != null || checkConflict2 != null){

        //                 TempData["message"] = "Schedule conflict! You are already attending an event at this time!";

        //                 return RedirectToAction("Dashboard");

        //             }

        //             else if (checkDuplicate != null){
        //                 _context.rsvps.Remove(checkDuplicate);
        //                 _context.SaveChanges();

        //                 TempData["message"] = "You have left the event!";

        //                 return RedirectToAction("Dashboard");
        //             }

        //             else{

        //                 RSVP newRSVP = new RSVP();

        //                 newRSVP.user = currentUser;
        //                 newRSVP.activity = currentActivityEvent;
        //                 _context.Add(newRSVP);
        //                 _context.SaveChanges();

        //                 currentUser.rsvps.Add(newRSVP);
        //                 currentActivityEvent.rsvps.Add(newRSVP);
        //                 _context.SaveChanges();

        //                 TempData["message"] = "You have joined the event!";

        //                 return RedirectToAction("Dashboard");
        //             }
        //         }

        //         else
        //         {
        //             return View("Index");
        //         }
        // }

        // [HttpPost]
        // [Route("rsvpactivity/{activityId}")]
        // public IActionResult rsvpActivityEventPage(int activityId)
        // {
        //     int? UserId = HttpContext.Session.GetInt32("UserId");
        //         if (UserId != null)
        //         {

        //                                User currentUser = _context.users.SingleOrDefault(u => u.id == UserId);

        //             ActivityEvent currentActivityEvent = _context.activities.SingleOrDefault(w => w.id == activityId);

        //             List<RSVP> thisUsersEvents = _context.rsvps.Where(r => r.user.id == currentUser.id).Include(r => r.activity).ToList();

        //             RSVP checkDuplicate = _context.rsvps.SingleOrDefault(r => r.user.id == currentUser.id && r.activity.id == currentActivityEvent.id);

        //             RSVP checkConflict = _context.rsvps.SingleOrDefault(r => r.user.id == currentUser.id && r.activity.date == currentActivityEvent.date && r.activity.id != currentActivityEvent.id);

        //             string checkConflict2 = null;

        //             int thiseventfulldurationmin = 0;
                    
        //             if (currentActivityEvent.durationtime == "Minutes"){
        //                 thiseventfulldurationmin = currentActivityEvent.duration;
        //                 }
        //             else if (currentActivityEvent.durationtime == "Hours"){
        //                 thiseventfulldurationmin = currentActivityEvent.duration * 60;
        //                 }
        //             else {
        //                 thiseventfulldurationmin = currentActivityEvent.duration * 1440;
        //                 }

        //             foreach (var e in thisUsersEvents){
        //                 if (e.activity.durationtime == "Minutes"){
        //                     int fulldurationminutes = e.activity.duration;
        //                     DateTime starttime = e.activity.date;
        //                     DateTime endtime = starttime.AddMinutes(fulldurationminutes);

        //                     DateTime thisStarttime = currentActivityEvent.date;
        //                     DateTime thisEndtime = thisStarttime.AddMinutes(thiseventfulldurationmin);

        //                     if(currentActivityEvent.date >= starttime && currentActivityEvent.date <= endtime && currentActivityEvent.id != e.activity.id){
        //                         checkConflict2 = "YUP!";
        //                     }

        //                     else if(e.activity.date >= thisStarttime && e.activity.date <= thisEndtime && currentActivityEvent.id != e.activity.id){
        //                         checkConflict2 = "YUP!";
        //                     }
                        
        //                 }
        //                 else if (e.activity.durationtime == "Hours"){
        //                     int fulldurationminutes = e.activity.duration * 60;
        //                     DateTime starttime = e.activity.date;
        //                 DateTime endtime = starttime.AddMinutes(fulldurationminutes);
                        
        //                 DateTime thisStarttime = currentActivityEvent.date;
        //                     DateTime thisEndtime = thisStarttime.AddMinutes(thiseventfulldurationmin);

        //                     if(currentActivityEvent.date >= starttime && currentActivityEvent.date <= endtime && currentActivityEvent.id != e.activity.id){
        //                         checkConflict2 = "YUP!";
        //                     }

        //                     else if(e.activity.date >= thisStarttime && e.activity.date <= thisEndtime && currentActivityEvent.id != e.activity.id){
        //                         checkConflict2 = "YUP!";
        //                     }
        //                 }
        //                 else {
        //                     int fulldurationminutes = e.activity.duration * 1440;
        //                     DateTime starttime = e.activity.date;
        //                 DateTime endtime = starttime.AddMinutes(fulldurationminutes);

        //                 DateTime thisStarttime = currentActivityEvent.date;
        //                     DateTime thisEndtime = thisStarttime.AddMinutes(thiseventfulldurationmin);

        //                     if(currentActivityEvent.date >= starttime && currentActivityEvent.date <= endtime && currentActivityEvent.id != e.activity.id){
        //                         checkConflict2 = "YUP!";
        //                     }

        //                     else if(e.activity.date >= thisStarttime && e.activity.date <= thisEndtime && currentActivityEvent.id != e.activity.id){
        //                         checkConflict2 = "YUP!";
        //                     }
        //                 }
        //             }
                    

        //             if (checkConflict != null || checkConflict2 != null){

        //                 TempData["message"] = "Schedule conflict! You are already attending an event at this time!";

        //                 return RedirectToAction("activity", new { id = currentActivityEvent.id });

        //             }

        //             else if (checkDuplicate != null){
        //                 _context.rsvps.Remove(checkDuplicate);
        //                 _context.SaveChanges();

        //                 TempData["message"] = "You have left the event!";

        //                 return RedirectToAction("activity", new { id = currentActivityEvent.id });
        //             }

        //             else{

        //                 RSVP newRSVP = new RSVP();

        //                 newRSVP.user = currentUser;
        //                 newRSVP.activity = currentActivityEvent;
        //                 _context.Add(newRSVP);
        //                 _context.SaveChanges();

        //                 currentUser.rsvps.Add(newRSVP);
        //                 currentActivityEvent.rsvps.Add(newRSVP);
        //                 _context.SaveChanges();

        //                 TempData["message"] = "You have joined the event!";

        //                 return RedirectToAction("activity", new { id = currentActivityEvent.id });
        //             }
        //         }

        //         else
        //         {
        //             return View("Index");
        //         }
        // }

        // // Delete Event

        // [HttpPost]
        // [Route("delete/{activityId}")]
        // public IActionResult deleteActivityEvent(int activityId)
        // {
        //     int? UserId = HttpContext.Session.GetInt32("UserId");
        //         if (UserId != null)
        //         {

        //             List<RSVP> removePeople = _context.rsvps.Where(r => r.activity.id == activityId).ToList();
        //                 foreach( var participant in removePeople)
        //                 {
        //                     _context.Remove(participant);
        //                 }
        //             _context.SaveChanges();

        //             ActivityEvent activity = _context.activities.Where(w => w.id == activityId).SingleOrDefault();

        //             _context.activities.Remove(activity);
        //             _context.SaveChanges();

        //             TempData["message"] = "You have deleted the event!";

        //             return RedirectToAction("Dashboard");

        //         }

        //         else
        //         {
        //             return View("Index");
        //         }
        // }

        // View Mimic

        [HttpGet]
        [Route("Home/mimic/{mimicId}")]
        public IActionResult viewMimic(int mimicId)
        {
            int? UserId = HttpContext.Session.GetInt32("UserId");

                User currentUser = _context.users.SingleOrDefault(u => u.id == UserId);
                Mimic currentMimic = _context.mimics.Include(m => m.owner).SingleOrDefault(m => m.id == mimicId);

                Console.WriteLine("THIS IS WHAT YOU WANT::::::::::");
                Console.WriteLine(DateTime.Now-currentMimic.created_at);

                ViewBag.currentMimic = currentMimic;
                ViewBag.currentUser = currentUser;

                return View("Mimic");
        }
    }
}