using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WeddingPlanner.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;


namespace WeddingPlanner.Controllers
{
    public class HomeController : Controller
    {
        private MyContext dbContext;
     
        public HomeController(MyContext context)
        {
            dbContext = context;
        }

        //Displaying Index page
        [Route("")]
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }


        // Register username
        [HttpPost]
        [Route("create")]
        public IActionResult Register(User reguser)
        {
            if(ModelState.IsValid)
            {

                if(dbContext.users.Any(u => u.Email == reguser.Email) )
                {
                    ModelState.TryAddModelError("Email", "Email is already in use!");
                    ViewBag.DuplicatateMessage = "Email is already being used";
                    return View("Index");
                }
                else
                {
                    PasswordHasher<User> Hasher = new PasswordHasher<User>();
                    reguser.Password = Hasher.HashPassword(reguser, reguser.Password);
                    dbContext.users.Add(reguser);
                    dbContext.SaveChanges();
                    return RedirectToAction("/");
                }
            }
            return View("index");
        }

         



        // Logging in
        [HttpPost]
        [Route("login")]    

        public IActionResult login(string Email, string Password)
        {
            if(ModelState.IsValid)
            {
                var DatabaseUser = dbContext.users.SingleOrDefault(u => u.Email ==  Email);
                if(DatabaseUser == null || Password == null)
                {
                    ModelState.AddModelError("Email", "Invalid Email");
                    return View("Index");
                }
               else
               {
                    var hasher = new PasswordHasher<User>();
                    
                    var notmatched = hasher.VerifyHashedPassword(DatabaseUser, DatabaseUser.Password, Password);
                    
                    if(notmatched == 0)
                    {
                        ModelState.AddModelError("Password", "Invalid Password");
                        return View("Index");
                    }
                    
               }
               HttpContext.Session.SetInt32("Id",DatabaseUser.UserId);
               return RedirectToAction("dashboard");
            }
            return View("Index");
        }





        // Dashboard (See everyone with date, number of guests, and action)
        [Route("dashboard")]
        [HttpGet]
        public IActionResult dashboard()
        {
           var display= HttpContext.Session.GetInt32("Id");
            
            System.Console.WriteLine(display);
            if(display==null){
               return RedirectToAction("Index");
            }
            User people = dbContext.users.SingleOrDefault(user => user.UserId == HttpContext.Session.GetInt32("Id"));
            List<Wedding> invitedPeople = dbContext.wedding
            .Include(a=>a.Users)
            .ThenInclude(b=>b.Oneuser)
            .ToList();
            ViewBag.Id = display;
            ViewBag.NewUser= people;
            
            return View("dashboard", invitedPeople);
        }


        // Wedding SignUp page
        [HttpGet]
        [Route("wedding")]
        public IActionResult wedding()
        {
            return View("wedding");
        }



       
        // Creating/Signing up new wedding
        [HttpPost]
        [Route("createWedding")]

        public IActionResult createWedding(Wedding newOne)
        {
            if(ModelState.IsValid){
                dbContext.wedding.Add(newOne);
                dbContext.SaveChanges();
                return RedirectToAction("show", new {WeddingId = newOne.WeddingId });
            }
            else{
                return View("wedding");
            }
        }




         // Showing person's wedding and it's details(details include: date & guests)
        [HttpGet]
        [Route("show/{WeddingId}")]
        public IActionResult show(int WeddingId){
            Wedding single = dbContext.wedding
            .Include(u=>u.Users)
            .ThenInclude(o=>o.Oneuser)
            .FirstOrDefault(w => w.WeddingId == WeddingId);
            ViewBag.AllGuest= single;
            

            return View("show",single);
        }




        [HttpGet]
        [Route("delete/{WeddingId}")]
        public IActionResult delete(int WeddingId){
            if(HttpContext.Session.GetInt32("UserId") == null) {
                return RedirectToAction("dashboard");
            }
            Wedding curWedding = dbContext.wedding
            .SingleOrDefault(wed => wed.WeddingId == WeddingId);
            dbContext.wedding.Remove(curWedding);
            dbContext.SaveChanges();
            return RedirectToAction("dashboard");
        }




        [HttpPost]
        [Route("rsvp/{WeddingId}")]
        public IActionResult rsvp(int WeddingId){
            Reservation rsvp = new Reservation{
                UserId = (int) HttpContext.Session.GetInt32("Id"),
                WeddingId = WeddingId
            };
                dbContext.reservation.Add(rsvp);
                dbContext.SaveChanges();
                return RedirectToAction("dashboard");
        }




        [HttpPost]
        [Route("unrsvp/{WeddingId}")]
        public IActionResult unrsvp(int WeddingId){
            Reservation rsvp = dbContext.reservation.Where(a=>a.UserId==(int) HttpContext.Session.GetInt32("Id"))
            .Where(b=>b.WeddingId==WeddingId).SingleOrDefault();
            dbContext.reservation.Remove(rsvp);
            dbContext.SaveChanges();
            return RedirectToAction("dashboard");
        }




        // Logout
        [Route("logout")]
        [HttpGet]
        public IActionResult logout(){
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

    }
}
