using GraduationWebApp.Data;
using GraduationWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GraduationWebApp.Controllers
{
    public class GroupParticipantsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GroupParticipantsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: GroupParticipants
        public async Task<IActionResult> Index()
        {
            return View(await _context.GroupParticipants.ToListAsync());
        }

        // GET: GroupParticipants/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var groupParticipants = await _context.GroupParticipants
                .FirstOrDefaultAsync(m => m.ParticipantId == id);
            if (groupParticipants == null)
            {
                return NotFound();
            }

            return View(groupParticipants);
        }

        // GET: GroupParticipants/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: GroupParticipants/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ParticipantId,UserId,GroupId,JoinDate")] GroupParticipants groupParticipants)
        {
            if (ModelState.IsValid)
            {
                _context.Add(groupParticipants);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(groupParticipants);
        }

        // GET: GroupParticipants/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var groupParticipants = await _context.GroupParticipants.FindAsync(id);
            if (groupParticipants == null)
            {
                return NotFound();
            }
            return View(groupParticipants);
        }

        // POST: GroupParticipants/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ParticipantId,UserId,GroupId,JoinDate")] GroupParticipants groupParticipants)
        {
            if (id != groupParticipants.ParticipantId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(groupParticipants);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GroupParticipantsExists(groupParticipants.ParticipantId))
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
            return View(groupParticipants);
        }

        // GET: GroupParticipants/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var groupParticipants = await _context.GroupParticipants
                .FirstOrDefaultAsync(m => m.ParticipantId == id);
            if (groupParticipants == null)
            {
                return NotFound();
            }

            return View(groupParticipants);
        }

        // POST: GroupParticipants/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var groupParticipants = await _context.GroupParticipants.FindAsync(id);
            if (groupParticipants != null)
            {
                _context.GroupParticipants.Remove(groupParticipants);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GroupParticipantsExists(int id)
        {
            return _context.GroupParticipants.Any(e => e.ParticipantId == id);
        }
    }
}
