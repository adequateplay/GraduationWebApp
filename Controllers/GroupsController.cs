using GraduationWebApp.Data;
using GraduationWebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace GraduationWebApp.Controllers
{
    [Authorize]
    public class GroupsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private const int PageSize = 5;

        public GroupsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int pageNumber = 1)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == User.Identity.Name);
            bool isAuthorized = User.IsInRole("Психолог") || User.IsInRole("Админ");

            var groupsQuery = _context.Groups
                .Include(g => g.Leader)
                .OrderByDescending(g => g.StartDate);

            if (user == null || user.Role == "Участник")
            {
                groupsQuery = (IOrderedQueryable<Groups>)groupsQuery.Select(g => new Groups
                {
                    GroupId = g.GroupId,
                    Name = g.Name,
                    Description = g.Description,
                    StartDate = g.StartDate,
                    EndDate = g.EndDate,
                    Leader = new Users
                    {
                        FirstName = g.Leader.FirstName + " ",
                        LastName = g.Leader.LastName
                    }
                });
            }

            var groups = await groupsQuery
                .Skip((pageNumber - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            ViewBag.PageNumber = pageNumber;
            ViewBag.TotalPages = (int)Math.Ceiling((double)_context.Groups.Count() / PageSize);
            ViewBag.IsAuthorized = isAuthorized;

            return View(groups);
        }

        public async Task<IActionResult> Calendar(int pageNumber = 1)
        {
            var groups = await _context.Groups
                .Include(g => g.Leader)
                .OrderByDescending(g => g.StartDate)
                .Skip((pageNumber - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            ViewBag.PageNumber = pageNumber;
            ViewBag.TotalPages = (int)Math.Ceiling((double)_context.Groups.Count() / PageSize);

            return View(groups);
        }

        [Authorize(Roles = "Психолог, Админ")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var groups = await _context.Groups
                .FirstOrDefaultAsync(m => m.GroupId == id);
            if (groups == null)
            {
                return NotFound();
            }

            return View(groups);
        }

        [Authorize(Roles = "Психолог, Админ")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Психолог, Админ")]
        public async Task<IActionResult> Create([Bind("GroupId,Name,Description,StartDate,EndDate,LeaderId")] Groups groups)
        {
            if (ModelState.IsValid)
            {
                _context.Add(groups);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(groups);
        }

        [Authorize(Roles = "Психолог, Админ")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var groups = await _context.Groups.FindAsync(id);
            if (groups == null)
            {
                return NotFound();
            }
            return View(groups);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Психолог, Админ")]
        public async Task<IActionResult> Edit(int id, [Bind("GroupId,Name,Description,StartDate,EndDate,LeaderId")] Groups groups)
        {
            if (id != groups.GroupId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(groups);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GroupsExists(groups.GroupId))
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
            return View(groups);
        }

        [Authorize(Roles = "Психолог, Админ")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var groups = await _context.Groups
                .FirstOrDefaultAsync(m => m.GroupId == id);
            if (groups == null)
            {
                return NotFound();
            }

            return View(groups);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Психолог, Админ")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var groups = await _context.Groups.FindAsync(id);
            if (groups != null)
            {
                _context.Groups.Remove(groups);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool GroupsExists(int id)
        {
            return _context.Groups.Any(e => e.GroupId == id);
        }
    }
}