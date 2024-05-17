using GraduationWebApp.Data;
using GraduationWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GraduationWebApp.Controllers
{
    public class SessionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SessionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Sessions
        public async Task<IActionResult> Index()
        {
            return View(await _context.Sessions.ToListAsync());
        }

        // GET: Sessions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sessions = await _context.Sessions
                .FirstOrDefaultAsync(m => m.SessionId == id);
            if (sessions == null)
            {
                return NotFound();
            }

            return View(sessions);
        }

        // GET: Sessions/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Sessions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SessionId,GroupId,SessionDate,StartTime,EndTime")] Sessions sessions)
        {
            if (ModelState.IsValid)
            {
                _context.Add(sessions);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(sessions);
        }

        // GET: Sessions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sessions = await _context.Sessions.FindAsync(id);
            if (sessions == null)
            {
                return NotFound();
            }
            return View(sessions);
        }

        // POST: Sessions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SessionId,GroupId,SessionDate,StartTime,EndTime")] Sessions sessions)
        {
            if (id != sessions.SessionId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sessions);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SessionsExists(sessions.SessionId))
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
            return View(sessions);
        }

        // GET: Sessions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sessions = await _context.Sessions
                .FirstOrDefaultAsync(m => m.SessionId == id);
            if (sessions == null)
            {
                return NotFound();
            }

            return View(sessions);
        }

        // POST: Sessions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sessions = await _context.Sessions.FindAsync(id);
            if (sessions != null)
            {
                _context.Sessions.Remove(sessions);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SessionsExists(int id)
        {
            return _context.Sessions.Any(e => e.SessionId == id);
        }
    }
}
