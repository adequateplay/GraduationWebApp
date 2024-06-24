using GraduationWebApp.Data;
using GraduationWebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GraduationWebApp.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProfileController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var email = User.FindFirstValue(ClaimTypes.Name);
            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateEmail(string newEmail)
        {
            var email = User.FindFirstValue(ClaimTypes.Name);
            var user = _context.Users.FirstOrDefault(u => u.Email == email);

            if (user == null)
            {
                return NotFound();
            }

            user.Email = newEmail;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(string oldPassword, string newPassword)
        {
            var email = User.FindFirstValue(ClaimTypes.Name);
            var user = _context.Users.FirstOrDefault(u => u.Email == email);

            if (user == null || user.Password != oldPassword)
            {
                ViewBag.Message = "Current password is incorrect.";
                return View("Index", user);
            }

            user.Password = newPassword;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            ViewBag.Message = "Password changed successfully.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePersonalInfo(string firstName, string lastName)
        {
            var email = User.FindFirstValue(ClaimTypes.Name);
            var user = _context.Users.FirstOrDefault(u => u.Email == email);

            if (user == null)
            {
                return NotFound();
            }

            user.FirstName = firstName;
            user.LastName = lastName;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}
