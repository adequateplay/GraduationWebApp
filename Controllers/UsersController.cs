using GraduationWebApp.Data;
using GraduationWebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace GraduationWebApp.Controllers
{
    [Authorize(Roles = "Админ")]
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private const int PageSize = 5;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Users
        public async Task<IActionResult> Index(int pageNumber = 1)
        {
            var users = await PaginatedList<Users>.CreateAsync(_context.Users.AsNoTracking(), pageNumber, PageSize);
            ViewBag.PageNumber = pageNumber;
            ViewBag.TotalPages = users.TotalPages;
            return View(users);
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var users = await _context.Users
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (users == null)
            {
                return NotFound();
            }

            return View(users);
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            ViewBag.Roles = new SelectList(new[] { "Админ", "Участник", "Психолог" });
            return View();
        }

        // POST: Users/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserId,FirstName,LastName,Email,Password,Role,RegistrationDate")] Users users)
        {
            if (ModelState.IsValid)
            {
                // Проверка уникальности email
                if (_context.Users.Any(u => u.Email == users.Email))
                {
                    ModelState.AddModelError("Email", "Этот email уже используется.");
                    ViewBag.Roles = new SelectList(new[] { "Админ", "Участник", "Психолог" });
                    return View(users);
                }

                _context.Add(users);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Roles = new SelectList(new[] { "Админ", "Участник", "Психолог" });
            return View(users);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var users = await _context.Users.FindAsync(id);
            if (users == null)
            {
                return NotFound();
            }
            ViewBag.Roles = new SelectList(new[] { "Админ", "Участник", "Психолог" }, users.Role);
            return View(users);
        }

        // POST: Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserId,FirstName,LastName,Email,Password,Role,RegistrationDate")] Users users)
        {
            if (id != users.UserId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // Проверка уникальности email
                if (_context.Users.Any(u => u.Email == users.Email && u.UserId != users.UserId))
                {
                    ModelState.AddModelError("Email", "Этот email уже используется.");
                    ViewBag.Roles = new SelectList(new[] { "Админ", "Участник", "Психолог" }, users.Role);
                    return View(users);
                }

                try
                {
                    _context.Update(users);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsersExists(users.UserId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Roles = new SelectList(new[] { "Админ", "Участник", "Психолог" }, users.Role);
            return View(users);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var users = await _context.Users
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (users == null)
            {
                return NotFound();
            }

            return View(users);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var users = await _context.Users.FindAsync(id);
            if (users != null)
            {
                _context.Users.Remove(users);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool UsersExists(int id)
        {
            return _context.Users.Any(e => e.UserId == id);
        }
    }

    public class PaginatedList<T> : List<T>
    {
        public int PageIndex { get; private set; }
        public int TotalPages { get; private set; }

        public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);

            this.AddRange(items);
        }

        public bool HasPreviousPage
        {
            get
            {
                return (PageIndex > 1);
            }
        }

        public bool HasNextPage
        {
            get
            {
                return (PageIndex < TotalPages);
            }
        }

        public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize)
        {
            var count = await source.CountAsync();
            var items = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PaginatedList<T>(items, count, pageIndex, pageSize);
        }
    }
}
