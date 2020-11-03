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
using BACS3403_Project.ViewModels;
using System.IO;

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
            ViewData["StatusMessage"] = "";
            ViewData["Status"] = "";

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
                .ThenInclude(q => q.MarkSchemes)
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
        public async Task<IActionResult> Create(RecordingCreateViewModel model)
        {
            if (ModelState.IsValid)
            {

                string uniqueFileName = null;
                string filePart;

                if(model.AudioRecording != null)
				{
                    filePart = FindFilePartPath(model.Part);

                    uniqueFileName = Guid.NewGuid().ToString() + "_" + model.AudioRecording.FileName;
                    string path = Path.Combine(filePart, uniqueFileName);
                    System.IO.FileStream fileStream = new FileStream(path, FileMode.Create);
                    model.AudioRecording.CopyTo(fileStream);
                    fileStream.Close();
                }

                Recording recording = new Recording
                {
                    Title = model.Title,
                    Part = model.Part,
                    Available = model.Available,
                    // Bind Examiner ID to object
                    ExaminerID = _userManager.GetUserId(User),
                    AudioURL = uniqueFileName
                };
       
                _context.Add(recording);
                await _context.SaveChangesAsync();

                TempData["StatusMessage"] = "New Recording: " + recording.Title + " has been added";
                TempData["Status"] = "success";

                return RedirectToAction("Index", new { part = recording.Part });
            }
            return View(model);
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

            RecordingEditViewModel recEditModel = new RecordingEditViewModel
            {
                RecordingId = recording.RecordingId,
                Title = recording.Title,
                Part = recording.Part,
                AudioURL = recording.AudioURL
            };

            return View(recEditModel);
        }

        // POST: Recordings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, RecordingEditViewModel recEditModel)
        {
            if (id != recEditModel.RecordingId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    string uniqueFileName = recEditModel.AudioURL;
                    string filePart;

                    if (recEditModel.AudioRecording != null)
                    {
                        //Get the file path that the recording part belongs
                        filePart = FindFilePartPath(recEditModel.Part);

                        //Remove existing audio recording
                        string existingAudioURL = Path.Combine(filePart, recEditModel.AudioURL);
						if (System.IO.File.Exists(existingAudioURL))
						{
                            System.IO.File.Delete(existingAudioURL);
                        }

                        //Create new audio recording
                        uniqueFileName = Guid.NewGuid().ToString() + "_" + recEditModel.AudioRecording.FileName;
                        string path = Path.Combine(filePart, uniqueFileName);
                        System.IO.FileStream fileStream = new FileStream(path, FileMode.Create);
                        recEditModel.AudioRecording.CopyTo(fileStream);
                        fileStream.Close();
                    }

                    //Update here db
                    var recording = await _context.Recordings.FindAsync(id);

                    recording.Title = recEditModel.Title;
                    recording.Part = recEditModel.Part;
                    recording.AudioURL = uniqueFileName;

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RecordingExists(recEditModel.RecordingId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction();
            }
            return View(recEditModel);
        }

        //NOT IN USE
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
            //Get find recording in db
            var recording = await _context.Recordings.FindAsync(id);

            //Get the file path that the recording part belongs
            string filePart = FindFilePartPath(recording.Part);

            //Remove existing audio recording
            string existingAudioURL = Path.Combine(filePart, recording.AudioURL);
            if (System.IO.File.Exists(existingAudioURL))
            {
                System.IO.File.Delete(existingAudioURL);
            }
                        
            _context.Recordings.Remove(recording);
            await _context.SaveChangesAsync();

            TempData["StatusMessage"] = "Recording: " + recording.Title + " has been deleted";
            TempData["Status"] = "success";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ActionName("UpdateStatus")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateAvailability(int id)
		{
            var recording = await _context.Recordings.FindAsync(id);

            if (recording.Available)
			{
                recording.Available = false;
                TempData["StatusMessage"] = "Recording " + recording.RecordingId + 
                                            ", Title: " + recording.Title + 
                                            " has been disabled";
                TempData["Status"] = "warning";
            }
			else
			{
                recording.Available = true;
                TempData["StatusMessage"] = "Recording " + recording.RecordingId +
                                            ", Title: " + recording.Title +
                                            " has been activated";
                TempData["Status"] = "success";
            }
            await _context.SaveChangesAsync();

			return RedirectToAction("Index", new { part = recording.Part });
        }

        private bool RecordingExists(int id)
        {
            return _context.Recordings.Any(e => e.RecordingId == id);
        }

        private string FindFilePartPath(int x)
		{
			string filePart = x switch
			{
				1 => "Storage\\AudioRecordings\\Part1\\",
				2 => "Storage\\AudioRecordings\\Part2\\",
				3 => "Storage\\AudioRecordings\\Part3\\",
				4 => "Storage\\AudioRecordings\\Part4\\",
				_ => "",
			};
			return filePart;
        }

        
    }
}
