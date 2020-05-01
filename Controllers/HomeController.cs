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
using cloudscribe.Pagination.Models;


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
            var x = View();
            for(var i=0;i<10;i++){
            Console.WriteLine(x);
            };
            
            return x;
        }
        [HttpGet("test")]
        public IActionResult Test()
        {

            return View(dbContext.Movies.ToList());
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
            List<Banana> watchlist = dbContext.Bananas.Include(b => b.NavMovie).ToList();
            int? userid = HttpContext.Session.GetInt32("UserId");
            ViewBag.AllMovies=dbContext.Movies
                                .Include(m=>m.Comments)
                                .ThenInclude(u=>u.NavCUser)
                                .ToList();
            if (userid == null)
            {
                return View("Login");
            }
            else
            {
                // ViewBag.AllMovies=dbContext.Movies
                //                 .Include(m=>m.Comments)
                //                 .ThenInclude(u=>u.NavCUser)
                //                 .OrderByDescending(m=>m.MovieId)
                //                 .Take(3)
                //                 .ToList();
                ViewBag.FirstName=HttpContext.Session.GetString("FirstName");
                ViewBag.User = dbContext.Users.Include(u=>u.MyActions).ThenInclude(b=>b.NavMovie).ThenInclude( m => m.Comments ).ThenInclude( c => c.NavCUser ).FirstOrDefault(u=>u.UserId==userid);
                ViewBag.UserId=userid;
                return View(watchlist);
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
                User userInDb=dbContext.Users.FirstOrDefault(u=>u.UserId==(int)HttpContext.Session.GetInt32("UserId"));
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
                    Creator=userInDb,
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

        
        public IActionResult Browse2()
        {
            User userInDb=dbContext.Users.FirstOrDefault(u=>u.UserId==(int)HttpContext.Session.GetInt32("UserId"));
            List<Movie> AllMovies=dbContext.Movies
                                .Include(m=>m.Creator)
                                .Include(m=>m.Comments)
                                .Include(m=>m.Actions)
                                .ThenInclude(u=>u.NavUser)
                                .OrderByDescending(m => m.CreatedAt).ToList();
            ViewBag.AllUsers = dbContext.Users.ToList();
            if (userInDb == null)
            {
                return View("Login");
            }
            else
            {
                ViewBag.User=userInDb;
                return View(AllMovies);
            }
        }
        [HttpGet("browse")]
        public IActionResult Browse(int pageNumber=1, int pageSize=3)
        {
            int ExcludeRecords=(pageSize * pageNumber)-pageSize;
            User userInDb=dbContext.Users.FirstOrDefault(u=>u.UserId==(int)HttpContext.Session.GetInt32("UserId"));
            var AllMovies=dbContext.Movies
                                .Include(m=>m.Creator)
                                .Include(m=>m.Comments)
                                .Include(m=>m.Actions)
                                .ThenInclude(u=>u.NavUser)
                                .OrderBy(m => m.Title).Skip(ExcludeRecords).Take(pageSize);
            ViewBag.AllUsers = dbContext.Users.ToList();
            var result=new PagedResult<Movie>
            {
                Data=AllMovies.AsNoTracking().ToList(),
                TotalItems=dbContext.Movies.Count(),
                PageNumber=pageNumber,
                PageSize=pageSize
            };
            if (userInDb == null)
            {
                return View("Login");
            }
            else
            {
                ViewBag.User=userInDb;
                return View(result);
            }
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

        [HttpPost("CreateComment/{MovieId}")]

        public IActionResult CreateComment (Comment new_comment, int MovieId)
        {
            if(ModelState.IsValid)
            {
                int? userIndb=HttpContext.Session.GetInt32("UserId");
                
                dbContext.Comments.Add(new_comment);
                new_comment.UserId=(int)userIndb;
                new_comment.MovieId=MovieId;
                dbContext.SaveChanges();
                return Redirect("/success");
            }
            else
            {
                ViewBag.AllMovies=dbContext.Movies
                                .Include(m=>m.Comments)
                                .ThenInclude(u=>u.NavCUser)
                                .OrderByDescending(m=>m.MovieId)
                                .ToList();
                int? userIndb=HttpContext.Session.GetInt32("UserId");
                ViewBag.User = dbContext.Users.Include(u=>u.MyActions).ThenInclude(b=>b.NavMovie).ThenInclude( m => m.Comments ).ThenInclude( c => c.NavCUser ).FirstOrDefault(u=>u.UserId==userIndb);
            return View("Dash");
            }
        }
    
        [HttpPost("Comment/{MovieId}")]
        public IActionResult Comment (Comment new_comment, int MovieId)
        {
            if (ModelState.IsValid)
            {
                int? userIndb=HttpContext.Session.GetInt32("UserId");
                
                dbContext.Comments.Add(new_comment);
                new_comment.UserId=(int)userIndb;
                new_comment.MovieId=MovieId;
                dbContext.SaveChanges();
                return Redirect("/browse");
            }
            else
            {
                ViewBag.AllMovies=dbContext.Movies
                                .Include(m=>m.Comments)
                                .ThenInclude(u=>u.NavCUser)
                                .OrderByDescending(m=>m.MovieId)
                                .ToList();
            return View("Browse");
            }
        }

        [HttpGet("add/{MovieId}/{UserId}")]
        public IActionResult AddToWatchlist(int MovieId, int UserId)
        {
            ViewBag.Content=dbContext.Movies.Include(m=>m.Comments).ThenInclude(c=>c.Content).Where(m=>m.MovieId==MovieId);
            Banana added=new Banana();
            added.UserId=UserId;
            added.MovieId=MovieId;
            dbContext.Bananas.Add(added);
            dbContext.SaveChanges();
        
            return RedirectToAction("Browse");
        }
        [HttpGet("delete/{MovieId}")]
        public IActionResult Delete(int MovieId)
        {
            Movie ToBeDeleted=dbContext.Movies.FirstOrDefault(m=>m.MovieId==MovieId);
            dbContext.Movies.Remove(ToBeDeleted);
            dbContext.SaveChanges();
            return RedirectToAction("Browse");
        }

        [HttpGet("unwatch/{MovieId}/{UserId}")]
        public IActionResult UnWatch(int MovieId, int UserId)
        {
            Banana toUnWatch=dbContext.Bananas.FirstOrDefault(b=>b.UserId==UserId && b.MovieId==MovieId);
            dbContext.Bananas.Remove(toUnWatch);
            dbContext.SaveChanges();
            return RedirectToAction("Browse");
        }

        [HttpGet("google")]
        public IActionResult Google()
        {
            return View();
        }

        [HttpPost("search")]

        public IActionResult Search(Query data)
        {
            List<Movie> movies = new List<Movie>();
            if(data.query != "all")
            {
                movies = dbContext.Movies.Where(m =>m.Title.Contains(data.query)).ToList();
            }
            return View("_Filter", movies);
        }


        [HttpGet("forum")]
        public IActionResult Forum()
        {
            User userInDb=dbContext.Users.FirstOrDefault(u=>u.UserId==(int)HttpContext.Session.GetInt32("UserId"));
            ViewBag.AllMessages = dbContext.Messages
                                        .Include(fd=>fd.MessageCreator)
                                        .Include(ds=>ds.PostedComments)
                                        .ThenInclude(u=>u.MUser)
                                        .OrderByDescending(f=>f.MessageId)
                                        .ToList();
            if(userInDb==null)
            {
                return RedirectToAction("Logout");
            }
            else
            {
                ViewBag.AllMessages = dbContext.Messages
                                        .Include(fd=>fd.MessageCreator)
                                        .Include(ds=>ds.PostedComments)
                                        .ThenInclude(u=>u.MUser)
                                        .OrderByDescending(f=>f.MessageId)
                                        .ToList();
                int? userIndb=HttpContext.Session.GetInt32("UserId");
                ViewBag.User = dbContext.Users.Include(u=>u.MyActions).ThenInclude(b=>b.NavMovie).ThenInclude( m => m.Comments ).ThenInclude( c => c.NavCUser ).FirstOrDefault(u=>u.UserId==userIndb);
            return View();
            }
        }
        [HttpPost("Forum")]
        public IActionResult CreateMessage(Message NewMessage)
        {
            if (ModelState.IsValid){
                int? userInDb=HttpContext.Session.GetInt32("UserId");
                NewMessage.UserId=(int)userInDb;
                dbContext.Messages.Add(NewMessage);
                dbContext.SaveChanges();
                return Redirect("/forum");
            }
            else{
                ViewBag.AllMessages = dbContext.Messages
                                    .Include(fd=>fd.MessageCreator)
                                    .Include(ds=>ds.PostedComments)
                                    .ThenInclude(u=>u.MUser)
                                    .OrderByDescending(f=>f.MessageId)
                                    .ToList();
                int? userIndb=HttpContext.Session.GetInt32("UserId");
                ViewBag.User = dbContext.Users.Include(u=>u.MyActions).ThenInclude(b=>b.NavMovie).ThenInclude( m => m.Comments ).ThenInclude( c => c.NavCUser ).FirstOrDefault(u=>u.UserId==userIndb);
                
                return View("Forum");
            }
        }
        


        [HttpPost("PostComment/{MessageId}")]

        public IActionResult ForumComments(MComment new_comment, int MessageId)
        {
            if (ModelState.IsValid)
            {
                int? userIndb=HttpContext.Session.GetInt32("UserId");
                
                dbContext.MComments.Add(new_comment);
                new_comment.UserId=(int)userIndb;
                new_comment.MessageId=MessageId;
                dbContext.SaveChanges();
                return Redirect("/forum");
            }
            else
            {
                ViewBag.AllMessages = dbContext.Messages
                                        .Include(fd=>fd.MessageCreator)
                                        .Include(ds=>ds.PostedComments)
                                        .ThenInclude(u=>u.MUser)
                                        .OrderByDescending(f=>f.MessageId)
                                        .ToList();
                int? userIndb=HttpContext.Session.GetInt32("UserId");
                ViewBag.User = dbContext.Users.Include(u=>u.MyActions).ThenInclude(b=>b.NavMovie).ThenInclude( m => m.Comments ).ThenInclude( c => c.NavCUser ).FirstOrDefault(u=>u.UserId==userIndb);

            return View("Forum");
            }
        }

        [HttpGet("destroy/{MessageId}")]
        public IActionResult Delete_Message(int MessageId)
        {
            Message ToBeDeleted=dbContext.Messages.FirstOrDefault(m=>m.MessageId==MessageId);
            dbContext.Messages.Remove(ToBeDeleted);
            dbContext.SaveChanges();
            return RedirectToAction("Forum");
        }
        [HttpGet("kill/{MCommentId}")]
        public IActionResult Delete_Comment(int MCommentId)
        {
            MComment ToBeDeleted=dbContext.MComments.FirstOrDefault(c=>c.MCommentId==MCommentId);
            dbContext.MComments.Remove(ToBeDeleted);
            dbContext.SaveChanges();
            return RedirectToAction("Forum");
        }

        [HttpGet("bomb/{CommentId}")]

        public IActionResult Bomb_Comment(int CommentId)
        {
            Comment ToBeDeleted=dbContext.Comments.FirstOrDefault(c=>c.CommentId==CommentId);
            dbContext.Comments.Remove(ToBeDeleted);
            dbContext.SaveChanges();
            return RedirectToAction("Dash");
        }

        [HttpGet("team")]
        public IActionResult Team()
        {
            return View();
        }

    }
}
