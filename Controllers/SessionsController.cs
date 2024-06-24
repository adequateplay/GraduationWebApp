using GraduationWebApp.Data;
using GraduationWebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GraduationWebApp.Controllers
{
    [Authorize(Roles = "Психолог")]
    public class SessionsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private const int PageSize = 5;

        public SessionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Sessions
        public async Task<IActionResult> Index(int pageNumber = 1)
        {
            var sessions = await _context.Sessions
                .Include(s => s.Group)
                .OrderBy(s => s.SessionDate)
                .Skip((pageNumber - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            ViewBag.PageNumber = pageNumber;
            ViewBag.TotalPages = (int)Math.Ceiling((double)_context.Sessions.Count() / PageSize);

            return View(sessions);
        }

        // GET: Sessions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var session = await _context.Sessions
                .Include(s => s.Group)
                .FirstOrDefaultAsync(m => m.SessionId == id);
            if (session == null)
            {
                return NotFound();
            }

            return View(session);
        }

        // GET: Sessions/Create
        public IActionResult Create()
        {
            ViewData["Groups"] = new SelectList(_context.Groups, "GroupId", "Name");
            return View();
        }

        // POST: Sessions/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SessionId,GroupId,SessionDate,StartTime,EndTime")] Sessions session, string GroupName)
        {
            session.GroupId = _context.Groups.FirstOrDefault(g => g.Name == GroupName)?.GroupId;

            if (ModelState.IsValid)
            {
                _context.Add(session);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Groups"] = new SelectList(_context.Groups, "GroupId", "Name", session.GroupId);
            return View(session);
        }

        // GET: Sessions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var session = await _context.Sessions.FindAsync(id);
            if (session == null)
            {
                return NotFound();
            }
            ViewData["Groups"] = new SelectList(_context.Groups, "GroupId", "Name", session.GroupId);
            return View(session);
        }

        // POST: Sessions/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SessionId,GroupId,SessionDate,StartTime,EndTime")] Sessions session, string GroupName)
        {
            session.GroupId = _context.Groups.FirstOrDefault(g => g.Name == GroupName)?.GroupId;

            if (id != session.SessionId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(session);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SessionExists(session.SessionId))
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
            ViewData["Groups"] = new SelectList(_context.Groups, "GroupId", "Name", session.GroupId);
            return View(session);
        }

        private bool SessionExists(int id)
        {
            return _context.Sessions.Any(e => e.SessionId == id);
        }
    }
}
