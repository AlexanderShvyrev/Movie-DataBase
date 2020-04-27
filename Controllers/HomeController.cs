using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.EntityFrameworkCore;

namespace MyProject.Controllers
{
    public class HomeController : Controller
    {
        private MyContext dbContext;
        private IHostingEnvironment _hostingEnvironment;
        public HomeController(MyContext context, IHostingEnvironment hostingEnvironment)
		{
			dbContext = context;
            _hostingEnvironment=hostingEnvironment;
		}

        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost("register")]
        public IActionResult Register(User newUser)
        {
            if(ModelState.IsValid)
            {
                if(dbContext.Users.Any(u => u.Email == newUser.Email))
                {
                    ModelState.AddModelError("Email", "Email already in use");
                    return View("Index");
                }
                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                newUser.Password = Hasher.HashPassword(newUser, newUser.Password);
                dbContext.Users.Add(newUser);
                dbContext.SaveChanges();
                HttpContext.Session.SetInt32("UserId", newUser.UserId);
                HttpContext.Session.SetString("FirstName", newUser.FirstName);
                //HttpContext.Session.SetInt32("UserId", newUser.UserId)
                return RedirectToAction("Dash");
            }
            else
            {
                return View("Index");
            }
        }

        [HttpGet("signup")]
        public IActionResult SignUp()
        {
            return View("Login");
        }

        [HttpPost("login")]
        public IActionResult Login(LoginUser userSubmission)
        {
            if(ModelState.IsValid)
            {
                // List<User>AllUsers=dbContext.Users.ToList();
                var hasher = new PasswordHasher<LoginUser>();
                var signedInUser = dbContext.Users.FirstOrDefault(u => u.Email == userSubmission.Email);
                
                if(signedInUser == null)
                {
                    ViewBag.Message="Email/Password is invalid";
                    return View("Login");
                }
                else
                {
                    var result = hasher.VerifyHashedPassword(userSubmission, signedInUser.Password, userSubmission.Password);
                    if(result==0)
                    {
                        ViewBag.Message="Email/Password is invalid";
                        return View("Login");
                    }
                }
                
                HttpContext.Session.SetInt32("UserId", signedInUser.UserId);
                HttpContext.Session.SetString("FirstName", signedInUser.FirstName);
                return RedirectToAction("Dash");
            }
            else
            {
                return View("Login");
            }
            
        }

        [HttpGet("success")]
        public IActionResult Dash()
        {
            
            int? userid = HttpContext.Session.GetInt32("UserId");
            ViewBag.AllMovies=dbContext.Movies
                                .Include(m=>m.Comments)
                                .ThenInclude(u=>u.NavUser)
                                .ToList();
            if (userid == null)
            {
                return View("Login");
            }
            else
            {
                ViewBag.AllMovies=dbContext.Movies
                                .Include(m=>m.Comments)
                                .ThenInclude(u=>u.NavUser)
                                .OrderByDescending(m=>m.MovieId)
                                .Take(3)
                                .ToList();
                ViewBag.FirstName=HttpContext.Session.GetString("FirstName");
                ViewBag.UserId = (int) userid;
                return View("Dash");
            }
        }
        [HttpGet("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return View("Login");
        }

        [HttpGet("add")]
        public IActionResult Add()
        {
            return View();
        }
        [HttpPost("create")]
        public IActionResult Create(MovieCreate model)
        {
            if(ModelState.IsValid)
            {
                string uniqueFileName=null;
                if (model.Image != null)
                {
                    string uploadsFolder=Path.Combine(_hostingEnvironment.WebRootPath, "images");
                    uniqueFileName=Guid.NewGuid().ToString() + "_" + model.Image.FileName;
                    string filePath=Path.Combine(uploadsFolder, uniqueFileName);
                    model.Image.CopyTo(new FileStream(filePath, FileMode.Create));
                }
                Movie newMovie=new Movie
                {
                    Title=model.Title,
                    Year=model.Year,
                    Director=model.Director,
                    Rating=model.Rating,
                    Stars=model.Stars,
                    Description=model.Description,
                    ImagePath=uniqueFileName
                };
                dbContext.Add(newMovie);
                dbContext.SaveChanges();
                return RedirectToAction("Browse");
            }
            else
            {
                return View("Add");
            }
            
        }

        [HttpGet("browse")]
        public IActionResult Browse()
        {
            List<Movie>AllMovies=dbContext.Movies.OrderByDescending(m => m.CreatedAt).ToList();
            return View(AllMovies);
        }
        [HttpGet("search")]
        public IActionResult Search()
        {
            return View("Search");
        }

        [HttpGet("show")]
        public IActionResult Movie()
        {
            return View();
        }










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
