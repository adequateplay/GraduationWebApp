using GraduationWebApp.Data;
using GraduationWebApp.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

public class AccessController : Controller
{
    private readonly ApplicationDbContext _context;

    public AccessController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult Login()
    {
        ClaimsPrincipal claimUser = HttpContext.User;
        if (claimUser.Identity.IsAuthenticated)
            return RedirectToAction("Index", "Home");

        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Login(Users model)
    {
        // Проверка на данные хостинга
        if (model.Email == "11183693" && model.Password == "60-dayfreetrial")
        {
            ViewData["ValidateMessage"] = "Invalid user credentials.";
            return View();
        }

        var user = _context.Users.FirstOrDefault(u => u.Email == model.Email && u.Password == model.Password);

        if (user != null)
        {
            List<Claim> claims = new List<Claim>() {
                new Claim(ClaimTypes.Name, model.Email),
                new Claim(ClaimTypes.NameIdentifier, model.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            AuthenticationProperties properties = new AuthenticationProperties()
            {
                AllowRefresh = true,
                IsPersistent = true, // Ensure persistent login
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30) // Custom expiration time
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), properties);

            return RedirectToAction("Index", "Home");
        }

        ViewData["ValidateMessage"] = "Пользователь не найден";
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login", "Access");
    }
}
