using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WeddingPlanner.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;

namespace WeddingPlanner.Controllers;

[SessionCheck]
public class WeddingController : Controller
{
    private readonly ILogger<WeddingController> _logger;

    private MyContext _context;

    public WeddingController(ILogger<WeddingController> logger, MyContext context)
    {
        _logger = logger;
        _context = context;
    }


    [HttpGet("weddings")]
    public IActionResult AllWeddings()
    {
        List<Wedding> AllWeddings = _context.Weddings.Include(a => a.Planner).Include(a => a.UsersGuest).ToList();
        return View(AllWeddings);
    }





    [HttpGet("weddings/new")]
    public ViewResult PlanWedding()
    {
        return View();
    }

    [HttpPost("weddings/create")]
    public IActionResult CreateWedding(Wedding CreateWedding)
    {
        if (!ModelState.IsValid)
        {
            return View("PlanWedding");
        }

        CreateWedding.UserId = (int)HttpContext.Session.GetInt32("UUID");
        _context.Add(CreateWedding);
        _context.SaveChanges();
        return OneWedding(CreateWedding.WeddingId);
    }



    [HttpPost("weddings/{id}")]
    public IActionResult Register(User newUser)
    {
        if (ModelState.IsValid)
        {

            PasswordHasher<User> Hasher = new();

            newUser.Password = Hasher.HashPassword(newUser, newUser.Password);

            _context.Add(newUser);
            _context.SaveChanges();

            HttpContext.Session.SetInt32("UUID", newUser.UserId);
            return RedirectToAction("Success");
        }
        else
        {
            return View("Index");
        }
    }


    [HttpPost("weddings/{id}/delete")]
    public RedirectToActionResult Delete(int id)
    {
        Wedding? toDelete = _context.Weddings.SingleOrDefault(w => w.WeddingId == id);
        if (toDelete != null)
        {
            _context.Remove(toDelete);
            _context.SaveChanges();
        }

        return RedirectToAction("AllWeddings");
    }

    [HttpPost("weddings/{id}/rsvp")]
    public RedirectToActionResult RSVP(int id)
    {
        int UUID = (int)HttpContext.Session.GetInt32("UUID");
        UserRSVP currentRSVP = _context.UserRSVPs.FirstOrDefault(r => r.WeddingId == id && r.UserId == UUID);
        if (currentRSVP == null)
        {
            UserRSVP newRSVP = new()
            {
                UserId = UUID,
                WeddingId = id
            };
            _context.Add(newRSVP);
        }
        else
        {
            _context.Remove(currentRSVP);
        }
        _context.SaveChanges();
        return RedirectToAction("AllWeddings");

    }

    [HttpGet("weddings/{id}/view")]
    public IActionResult OneWedding(int id)
    {
        Wedding? oneWedding = _context.Weddings.Include(w => w.Planner).Include(w => w.UsersGuest).ThenInclude(ug => ug.Guest).FirstOrDefault(w => w.WeddingId == id);

        if (oneWedding == null)
        {
            return RedirectToAction("AllWeddings");
        }
        return View("OneWedding", oneWedding);
    }



    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
