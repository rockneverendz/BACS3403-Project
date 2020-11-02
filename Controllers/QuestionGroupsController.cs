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
            var recording = _context.Recordings
                            .FirstOrDefault(r => r.RecordingId == recordingId);

            ViewData["RecordingModel"] = recording;

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


                /* Find the Recording on based on QuestionGroup -> Recording ID*/
                var recording = await _context.Recordings
                                .FirstOrDefaultAsync(m => m.RecordingId == questionGroup.RecordingID);

                string filePath = "not found";
                /*string filePathPdf = "not found";*/
                string fileName = "";

                /* Save the textarea content b4 update the filePath */
                string content = questionGroup.QuestionGroupURL;
                questionGroup.QuestionGroupURL = "";

                switch (recording.Part)
                {
                    case 1:
                        filePath = "Storage\\QuestionsText\\Part1\\";
                        break;
                    case 2:
                        filePath = "Storage\\QuestionsText\\Part2\\";
                        break;
                    case 3:
                        filePath = "Storage\\QuestionsText\\Part3\\";
                        break;
                    case 4:
                        filePath = "Storage\\QuestionsText\\Part4\\";
                        break;
                    default:
                        break;
                }

                /* Append File Name with QuestionGrp Details */
                fileName = Guid.NewGuid().ToString() + "_" + 
                                recording.RecordingId + "_" +
                                recording.Part + "_" +
                                questionGroup.QuestionNoStart + "_To_" +
                                questionGroup.QuestionNoEnd;

                /* Combine File Path and File Name */
                filePath += fileName + ".html";
                /* filePathPdf += fileName + ".pdf";*/

                /* Write content to file */
                System.IO.File.WriteAllText(filePath, content);

                /*Create and save to pdf*/
                /*convertToPdf(filePath, content);*/

                /* Update Question Group URL */
                questionGroup.QuestionGroupURL = filePath;

                _context.Add(questionGroup);
                await _context.SaveChangesAsync();

                /*Query the recently added questiongroup and save as ViewData*/
                var addedQuestionGroup = _context.Questions
                                .OrderByDescending(x => x.QuestionGroupId)
                                .FirstOrDefault();

                /* Then redirect to create CreateMarkScheme*/
                return RedirectToAction("CreateMarkScheme", "QuestionGroups", new { id = addedQuestionGroup.QuestionGroupId } );

                //return RedirectToAction("Details", "Recordings", new { id = questionGroup.RecordingID });
            }
            return View(questionGroup);
        }

        // GET: QuestionGroups/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var questionGroup = await _context.Questions
                .Include(q => q.MarkSchemes)
                .FirstOrDefaultAsync(m => m.QuestionGroupId == id);

            if (questionGroup == null)
            {
                return NotFound();
            }

            // Check if user is authorized to edit recording
            /*if (!IsOwner(questionGroup.RecordingID)) return Unauthorized();*/

            var recording = _context.Recordings
                .FirstOrDefault(r => r.RecordingId == questionGroup.RecordingID);

            ViewData["RecordingModel"] = recording;

            // Relocate data to get markscheme in List not collection ...

            var questionGrpWifListMarkScheme = new EditQuestionGroupViewModel();
            var markSchemeList = new List<EditMarkSchemeViewModel>();

            questionGrpWifListMarkScheme.QuestionGroupId = questionGroup.QuestionGroupId;
            questionGrpWifListMarkScheme.QuestionNoStart = questionGroup.QuestionNoStart;
            questionGrpWifListMarkScheme.QuestionNoEnd = questionGroup.QuestionNoEnd;
            questionGrpWifListMarkScheme.TaskType = questionGroup.TaskType;
            questionGrpWifListMarkScheme.QuestionGroupURL = questionGroup.QuestionGroupURL;
            questionGrpWifListMarkScheme.RecordingID = questionGroup.RecordingID;

			foreach (var x in questionGroup.MarkSchemes)
			{
				var ms = new EditMarkSchemeViewModel
				{
					MarkSchemeID = x.MarkSchemeID,
					Index = x.Index,
					Answer = x.Answer,
					QuestionGroupID = x.QuestionGroupID
				};
				markSchemeList.Add(ms);

            }
            questionGrpWifListMarkScheme.MarkSchemes = markSchemeList;

            return View(questionGrpWifListMarkScheme);
        }

        // POST: QuestionGroups/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditQuestionGroupViewModel questionGroupViewModel)
        {
            if (id != questionGroupViewModel.QuestionGroupId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // Check if user is authorized to edit recording
                /*if (!IsOwner(questionGroup.RecordingID)) return Unauthorized();*/
                try
                {
                    //get the old questionGrp question html content...
                    var currQG = _context.Questions
                                      .FirstOrDefault(q => q.QuestionGroupId == questionGroupViewModel.QuestionGroupId);
                    var existingURLPath = currQG.QuestionGroupURL;

                    //Replace the old content with the new content
                    var newContent = questionGroupViewModel.QuestionGroupURL; //<-- the URL stores the content in the view

                    /* Write content to file */
                    System.IO.File.WriteAllText(existingURLPath, newContent);

                    //Relocate QuestionGrp Data
                    currQG.TaskType = questionGroupViewModel.TaskType;
                    
                    // Save QuestionGrp
                    await _context.SaveChangesAsync();

                    //Relocate MarkScheme Data
                    if (questionGroupViewModel.MarkSchemes != null)
                    {
                        foreach (var item in questionGroupViewModel.MarkSchemes)
                        {
                            var record = _context.MarkSchemes.Find(item.MarkSchemeID);
                            if (record != null)
                            {
                                record.Answer = item.Answer;
                            }
                        }
                        //Save all
                        await _context.SaveChangesAsync();
                    }

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!QuestionGroupExists(questionGroupViewModel.QuestionGroupId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Details", "Recordings", new { id = questionGroupViewModel.RecordingID });
                
            }
            return View(questionGroupViewModel);
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
            var questionGroup = _context.Questions.Find(id);
            var recID = questionGroup.RecordingID;

            _context.Questions.Remove(questionGroup);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Recordings", new { id = recID });
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

        // GET: QuestionGroups/CreateMarkScheme
        public async Task<IActionResult> CreateMarkScheme(int? id)
        {
            /* Find Question Group and Set to ViewData*/          
            if (id == null)
            {
                return NotFound();
            }

            var questionGroup = _context.Questions.Find(id);
            if (questionGroup == null)
            {
                return NotFound();
            }

            ViewData["QuestionGroup"] = questionGroup;

            /* Add the MarkScheme */
            var msList = new List<MarkScheme>();

			for (int i = questionGroup.QuestionNoStart; i <= questionGroup.QuestionNoEnd; i++)
			{
				var ms = new MarkScheme
				{
					Index = i,
					Answer = "",
					QuestionGroupID = questionGroup.QuestionGroupId
				};
				msList.Add(ms);

                _context.Add(ms);
                await _context.SaveChangesAsync();
            }

            var markScheme = await _context.MarkSchemes
                                .Where(m => m.QuestionGroupID == questionGroup.QuestionGroupId)
                                .ToListAsync();

            return View(markScheme);
        }


        // POST: QuestionGroups/CreateMarkScheme
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateMarkScheme(List<MarkScheme> markScheme)
		{
            /* Working codes*/
            if (ModelState.IsValid)
			{
                if( markScheme != null)
				{
					foreach (var item in markScheme)
					{
                        var record = _context.MarkSchemes.Find(item.MarkSchemeID);
                        if (record != null)
                        {
                            record.Answer = item.Answer;
                        }
                    }
                    await _context.SaveChangesAsync();
				}
            }

            /*Reverse serach back question group to return the view*/
            var addedMarkScheme = await _context.MarkSchemes
                .OrderByDescending(x => x.MarkSchemeID)
                .FirstOrDefaultAsync();

            var reverseFindQuestionGrp = await _context.Questions
                .FirstOrDefaultAsync(q => q.QuestionGroupId == addedMarkScheme.QuestionGroupID);

            return RedirectToAction("Details", "Recordings", new { id = reverseFindQuestionGrp.RecordingID });
            /*return View(markScheme);*/
        }


        /*  Testing DinkToPdf*/
        /*public void convertToPdf(string path, string content)
		{
            var converter = new SynchronizedConverter(new PdfTools());

            var doc = new HtmlToPdfDocument()
            {
                GlobalSettings = {
                    ColorMode = ColorMode.Color,
                    Orientation = Orientation.Landscape,
                    PaperSize = PaperKind.A4Plus,
                    Out = path,
                },
                Objects = {
                    new ObjectSettings() {
                        PagesCount = true,
                        HtmlContent = content,
                        WebSettings = { DefaultEncoding = "utf-8" },
                        HeaderSettings = { FontSize = 9, Right = "Page [page] of [toPage]", Line = true, Spacing = 2.812 }
                    }
                }
            };

            converter.Convert(doc);

        }*/


    }
}
