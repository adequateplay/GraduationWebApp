using GraduationWebApp.Data;
using GraduationWebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PdfSharpCore.Drawing;
using PdfSharpCore.Drawing.Layout;
using PdfSharpCore.Pdf;
using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GraduationWebApp.Controllers
{
    [Authorize]
    public class JournalsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private const int PageSize = 5;

        public JournalsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Психолог, Админ")]
        public async Task<IActionResult> AdminIndex(int? userId, int pageNumber = 1)
        {
            if (userId == null)
            {
                return RedirectToAction("SelectUser");
            }

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var journals = await PaginatedList<Journals>.CreateAsync(_context.Journals
                .Where(j => j.UserId == userId).AsNoTracking(), pageNumber, PageSize);

            ViewBag.SelectedUser = user;
            ViewBag.PageNumber = pageNumber;
            ViewBag.TotalPages = journals.TotalPages;

            return View(journals);
        }

        [Authorize(Roles = "Участник")]
        public async Task<IActionResult> UserIndex(int pageNumber = 1)
        {
            var email = User.FindFirstValue(ClaimTypes.Name);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                return NotFound();
            }

            var journals = await PaginatedList<Journals>.CreateAsync(_context.Journals
                .Where(j => j.UserId == user.UserId).AsNoTracking(), pageNumber, PageSize);

            ViewBag.User = user;
            ViewBag.PageNumber = pageNumber;
            ViewBag.TotalPages = journals.TotalPages;

            return View(journals);
        }

        [Authorize(Roles = "Психолог, Админ")]
        public async Task<IActionResult> SelectUser(int pageNumber = 1)
        {
            var users = await PaginatedList<Users>.CreateAsync(_context.Users
                .Where(u => u.Role == "Участник").AsNoTracking(), pageNumber, PageSize);

            ViewBag.PageNumber = pageNumber;
            ViewBag.TotalPages = users.TotalPages;

            return View(users);
        }


        [Authorize(Roles = "Психолог, Админ, Участник")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Психолог, Админ, Участник")]
        public async Task<IActionResult> Create([Bind("Entry")] Journals journal)
        {
            var email = User.FindFirstValue(ClaimTypes.Name);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                return NotFound();
            }

            journal.UserId = user.UserId;
            journal.EntryDate = DateTime.Now;

            if (ModelState.IsValid)
            {
                _context.Add(journal);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(UserIndex));
            }
            return View(journal);
        }

        [Authorize(Roles = "Психолог, Админ, Участник")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var journal = await _context.Journals
                .FirstOrDefaultAsync(m => m.JournalId == id);
            if (journal == null)
            {
                return NotFound();
            }

            var email = User.FindFirstValue(ClaimTypes.Name);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (user == null || (user.Role == "Участник" && journal.UserId != user.UserId))
            {
                return Forbid();
            }

            return View(journal);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Психолог, Админ, Участник")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var journal = await _context.Journals.FindAsync(id);
            if (journal != null)
            {
                _context.Journals.Remove(journal);
                await _context.SaveChangesAsync();
            }

            if (User.IsInRole("Психолог") || User.IsInRole("Админ"))
            {
                return RedirectToAction(nameof(AdminIndex), new { userId = journal.UserId });
            }

            return RedirectToAction(nameof(UserIndex));
        }

        private bool JournalsExists(int id)
        {
            return _context.Journals.Any(e => e.JournalId == id);
        }

        #region DownloadPdf
        [Authorize(Roles = "Психолог, Админ")]
        public async Task<IActionResult> DownloadPDF(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var journals = await _context.Journals
                .Where(j => j.UserId == userId)
                .ToListAsync();

            using (MemoryStream stream = new MemoryStream())
            {
                PdfDocument pdf = new PdfDocument();
                PdfPage page = pdf.AddPage();
                XGraphics gfx = XGraphics.FromPdfPage(page);
                XFont font = new XFont("Verdana", 12, XFontStyle.Regular);
                XTextFormatter tf = new XTextFormatter(gfx);

                gfx.DrawString($"Журнал пользователя: {user.FirstName} {user.LastName}", font, XBrushes.Black,
                    new XRect(0, 0, page.Width, 40), XStringFormats.Center);

                int yPoint = 60;
                int margin = 40;
                int lineHeight = 20;
                int maxWidth = (int)page.Width - 2 * margin;

                foreach (var journal in journals)
                {
                    gfx.DrawString($"Дата: {journal.EntryDate.ToShortDateString()}", font, XBrushes.Black,
                        new XRect(margin, yPoint, maxWidth, lineHeight), XStringFormats.TopLeft);
                    yPoint += lineHeight;

                    string text = journal.Entry;
                    List<string> lines = SplitTextIntoLines(gfx, text, font, maxWidth);
                    int textHeight = lines.Count * lineHeight;

                    if (yPoint + textHeight + lineHeight > page.Height - margin)
                    {
                        page = pdf.AddPage();
                        gfx = XGraphics.FromPdfPage(page);
                        tf = new XTextFormatter(gfx);
                        yPoint = margin;
                    }

                    foreach (string line in lines)
                    {
                        tf.DrawString(line, font, XBrushes.Black, new XRect(margin, yPoint, maxWidth, lineHeight), XStringFormats.TopLeft);
                        yPoint += lineHeight;
                    }

                    gfx.DrawLine(XPens.Black, margin, yPoint, page.Width - margin, yPoint);
                    yPoint += lineHeight;

                    if (yPoint + lineHeight > page.Height - margin)
                    {
                        page = pdf.AddPage();
                        gfx = XGraphics.FromPdfPage(page);
                        tf = new XTextFormatter(gfx);
                        yPoint = margin;
                    }
                }

                if (yPoint + 2 * lineHeight > page.Height - margin)
                {
                    page = pdf.AddPage();
                    gfx = XGraphics.FromPdfPage(page);
                    tf = new XTextFormatter(gfx);
                    yPoint = margin;
                }

                yPoint = (int)page.Height - margin - 2 * lineHeight;
                gfx.DrawString($"Отчёт сгенерирован: {DateTime.Now.ToString("dd.MM.yyyy")}", font, XBrushes.Black,
                    new XRect(margin, yPoint, maxWidth, lineHeight), XStringFormats.TopLeft);
                yPoint += lineHeight;
                gfx.DrawString($"Сгенерировал: {User.Identity.Name}", font, XBrushes.Black,
                    new XRect(margin, yPoint, maxWidth, lineHeight), XStringFormats.TopLeft);

                pdf.Save(stream, false);
                byte[] bytes = stream.ToArray();
                stream.Close();

                return File(bytes, "application/pdf", $"Journal_{user.FirstName}_{user.LastName}.pdf");
            }
        }

        private List<string> SplitTextIntoLines(XGraphics gfx, string text, XFont font, int maxWidth)
        {
            List<string> lines = new List<string>();
            string[] words = text.Split(' ');

            string currentLine = string.Empty;
            foreach (var word in words)
            {
                string testLine = string.IsNullOrEmpty(currentLine) ? word : currentLine + " " + word;
                XSize size = gfx.MeasureString(testLine, font);

                if (size.Width > maxWidth)
                {
                    lines.Add(currentLine);
                    currentLine = word;
                }
                else
                {
                    currentLine = testLine;
                }
            }

            if (!string.IsNullOrEmpty(currentLine))
            {
                lines.Add(currentLine);
            }

            return lines;
        }
        #endregion

    }

}
