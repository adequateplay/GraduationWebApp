using GraduationWebApp.Data;
using GraduationWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GraduationWebApp.Controllers
{
    public class GroupsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GroupsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Groups
        public async Task<IActionResult> Index()
        {
            return View(await _context.Groups.ToListAsync());
        }

        // GET: Groups/Details/5
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

        // GET: Groups/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Groups/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
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

        // GET: Groups/Edit/5
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

        // POST: Groups/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
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

        // GET: Groups/Delete/5
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

        // POST: Groups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var groups = await _context.Groups.FindAsync(id);
            if (groups != null)
            {
                _context.Groups.Remove(groups);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GroupsExists(int id)
        {
            return _context.Groups.Any(e => e.GroupId == id);
        }
    }
}
