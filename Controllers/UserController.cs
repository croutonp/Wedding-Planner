using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WeddingPlanner.Models;
using Microsoft.AspNetCore.Identity;


namespace WeddingPlanner.Controllers;

public class UserController : Controller
{
    private readonly ILogger<UserController> _logger;

    private MyContext _context;

    public UserController(ILogger<UserController> logger, MyContext context)
    {
        _logger = logger;
        _context = context;
    }


    [HttpGet("")]
    public IActionResult Index()
    {
        return View();
    }


    [HttpPost("users/register")]
    public IActionResult Register(User newUser)
    {
        if (ModelState.IsValid)
        {

            PasswordHasher<User> Hasher = new();

            newUser.Password = Hasher.HashPassword(newUser, newUser.Password);

            _context.Add(newUser);
            _context.SaveChanges();

            HttpContext.Session.SetInt32("UUID", newUser.UserId);
            HttpContext.Session.SetString("LoggedUser", newUser.FirstName);
            return RedirectToAction("AllWeddings", "Wedding");
        }
        else
        {
            return View("Index");
        }
    }



    [HttpPost("users/login")]
    public IActionResult Login(Login newLog)

    {
        if (ModelState.IsValid)
        {       
            User? userInDb = _context.Users.FirstOrDefault(u => u.Email == newLog.LoginEmail);       
            if (userInDb == null)
            {           
                ModelState.AddModelError("LoginPassword", "Invalid Credentials");
                return View("Index");
            }       
            PasswordHasher<Login> hasher = new PasswordHasher<Login>();     
            var result = hasher.VerifyHashedPassword(newLog, userInDb.Password, newLog.LoginPassword);                                           
            if (result == 0)
            {  
                ModelState.AddModelError("LoginPassword", "Invalid Credentials");
                return View("Index");
            }

            HttpContext.Session.SetInt32("UUID", userInDb.UserId);
            HttpContext.Session.SetString("LoggedUser", userInDb.FirstName);
            return RedirectToAction("AllWeddings", "Wedding");
        }
        else
        {
            return View("Index");
        }
    }


    [HttpPost("users/logout")]
    public IActionResult LogoutUser()
    {
        HttpContext.Session.Remove("UUID");
        return RedirectToAction("Index");
    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
