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

namespace BACS3403_Project.Controllers
{
    public class QuestionGroupsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Examiner> _userManager;

        public QuestionGroupsController(ApplicationDbContext context, UserManager<Examiner> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: QuestionGroups/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var questionGroup = await _context.Questions
                .Include(q => q.Recording)
                .FirstOrDefaultAsync(m => m.QuestionGroupId == id);
            if (questionGroup == null)
            {
                return NotFound();
            }

            return View(questionGroup);
        }

        // GET: QuestionGroups/Create
        
        public IActionResult Create(int? recordingId)
        {
            ViewData["RecordingID"] = recordingId;
            return View();
        }

        // POST: QuestionGroups/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("QuestionGroupId,TaskType,QuestionGroupURL,RecordingID,QuestionNoEnd,QuestionNoStart")] QuestionGroup questionGroup)
        {
            if (ModelState.IsValid)
            {
                // Check if user is authorized to create recording
                if (!IsOwner(questionGroup.RecordingID)) return Unauthorized();

                _context.Add(questionGroup);
                await _context.SaveChangesAsync();
                                                                    /*Need Revieew HERE*/
                return RedirectToAction("Details", "Recordings", new { id = questionGroup.RecordingID });
            }
            return View(questionGroup);
        }

        // GET: QuestionGroups/Edit/5
        public async Task<IActionResult> Edit(int? recordingId, int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var questionGroup = await _context.Questions.FindAsync(id);
            if (questionGroup == null)
            {
                return NotFound();
            }

            // Check if user is authorized to edit recording
            if (!IsOwner(questionGroup.RecordingID)) return Unauthorized();

            ViewData["RecordingID"] = questionGroup.RecordingID;

            return View(questionGroup);
        }

        // POST: QuestionGroups/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("QuestionGroupId,TaskType,QuestionGroupURL,RecordingID")] QuestionGroup questionGroup)
        {
            if (id != questionGroup.QuestionGroupId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // Check if user is authorized to edit recording
                if (!IsOwner(questionGroup.RecordingID)) return Unauthorized();

                try
                {
                    _context.Update(questionGroup);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!QuestionGroupExists(questionGroup.QuestionGroupId))
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
            ViewData["RecordingID"] = questionGroup.RecordingID;
            return View(questionGroup);
        }

        // GET: QuestionGroups/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var questionGroup = await _context.Questions
                .Include(q => q.Recording)
                .FirstOrDefaultAsync(m => m.QuestionGroupId == id);
            if (questionGroup == null)
            {
                return NotFound();
            }

            // Check if user is authorized to edit recording
            if (!IsOwner(questionGroup.RecordingID)) return Unauthorized();

            return View(questionGroup);
        }

        // POST: QuestionGroups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var questionGroup = await _context.Questions.FindAsync(id);
            _context.Questions.Remove(questionGroup);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool QuestionGroupExists(int id)
        {
            return _context.Questions.Any(e => e.QuestionGroupId == id);
        }

        private bool IsOwner(int? RecordingID)
        {
            Recording recording = _context.Recordings.FirstOrDefault(r => r.RecordingId == RecordingID);
            return recording.ExaminerID == _userManager.GetUserId(User);
        }
    }
}
