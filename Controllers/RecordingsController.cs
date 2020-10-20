using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BACS3403_Project.Data;
using BACS3403_Project.Models;
using Microsoft.AspNetCore.Identity;

namespace BACS3403_Project.Controllers.Question
{
    public class RecordingsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Examiner> _userManager;

        public RecordingsController(ApplicationDbContext context, UserManager<Examiner> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Recordings
        // GET: Recordings/Index
        // GET: Recordings/Index/1
        public async Task<IActionResult> Index(int? part)
        {
            if (part == null) part = 1;
            
            return View(await _context.Recordings
                .Where(r => r.Part == part)
                .ToListAsync());
        }

        // GET: Recordings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recording = await _context.Recordings
                .Include(r => r.QuestionGroups)
                .FirstOrDefaultAsync(m => m.RecordingId == id);
            if (recording == null)
            {
                return NotFound();
            }

            return View(recording);
        }

        // GET: Recordings/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Recordings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RecordingId,Part,AudioURL,Available,Title")] Recording recording)
        {
            if (ModelState.IsValid)
            {
                // Bind Examiner ID to object
                recording.ExaminerID = _userManager.GetUserId(User);

                _context.Add(recording);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(recording);
        }

        // GET: Recordings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recording = await _context.Recordings.FindAsync(id);
            if (recording == null)
            {
                return NotFound();
            }
            return View(recording);
        }

        // POST: Recordings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RecordingId,Part,AudioURL,Available,Title")] Recording recording)
        {
            if (id != recording.RecordingId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(recording);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RecordingExists(recording.RecordingId))
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
            return View(recording);
        }

        // GET: Recordings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recording = await _context.Recordings
                .FirstOrDefaultAsync(m => m.RecordingId == id);
            if (recording == null)
            {
                return NotFound();
            }

            return View(recording);
        }

        // POST: Recordings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var recording = await _context.Recordings.FindAsync(id);
            _context.Recordings.Remove(recording);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RecordingExists(int id)
        {
            return _context.Recordings.Any(e => e.RecordingId == id);
        }
        
        
        
        /*
        public async Task<IActionResult> TestIndex(int? part)
        {
            if (part == null) part = 1;

            return View(await _context.Recordings
                .Where(r => r.Part == part)
                .ToListAsync());
        }*/


    }
}
