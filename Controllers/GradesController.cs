using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BACS3403_Project.Data;
using BACS3403_Project.Models;
using BACS3403_Project.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BACS3403_Project.Controllers
{
	public class GradesController : Controller
	{
		private readonly ApplicationDbContext _context;
		private readonly UserManager<Examiner> _userManager;
		public GradesController(ApplicationDbContext context, UserManager<Examiner> userManager)
		{
			_context = context;
			_userManager = userManager;
		}

		//NO INDEX VIEW FOR NOW
		public IActionResult Index()
		{
			return View();
		}

		public IActionResult ReviewTestAnswers()
		{
			//GET DATABASE DATA AND RETURN TO VIEW
			int i = 0;
			var result = (from tv in _context.Tests
							 select tv.Venue).Distinct();

			var venueList = result.ToList();

			List<SelectVenueViewModel> venueVM = new List<SelectVenueViewModel>();
			venueVM.Insert(i, new SelectVenueViewModel { VenueValue = "0", VenueName = "Select" });
			i++;

			foreach (var item in venueList)
			{
				venueVM.Insert(i, new SelectVenueViewModel { VenueValue = item, VenueName = item });
				i++;
			}

			ViewBag.ListOfVenue = venueVM;
			return View();
		}

		public JsonResult GetTestDate(string Venue)
		{
			int i = 0;
			var result = (from tv in _context.Tests
						  where tv.Venue == Venue
						  select tv.Date.Date).Distinct();

			/* select distinct FORMAT(t.Date, 'dd/mm/yyyy')
			 * from Test t 
			 * where t.Venue = 'British Council Kuala Lumpur'; 
			*/

			List<SelectVenueDateViewModel> venueDateVM = new List<SelectVenueDateViewModel>();
			venueDateVM.Insert(i, new SelectVenueDateViewModel { VenueDateValue = "0", VenueDateName = "Select" });
			i++;

			foreach (var item in result)
			{
				venueDateVM.Insert(i, new SelectVenueDateViewModel 
				{ 
					VenueDateValue = item.ToString("dd/MM/yyyy"),															
					VenueDateName = item.ToString("dd/MM/yyyy")
				});
				i++;
			}

			return Json(new SelectList(venueDateVM, "VenueDateValue", "VenueDateName"));
		}

		public JsonResult GetTestSessions(string Venue, string Date)
		{
			int i = 0;
			var result = _context.Tests
							.Where(t => t.Venue == Venue)
							.Where(t => t.Date.Date == Convert.ToDateTime(Date));

			List<SelectVenueSessionViewModel> venueSessionVM = new List<SelectVenueSessionViewModel>();
			venueSessionVM.Insert(i, new SelectVenueSessionViewModel { VenueSessionValue = "0", VenueSessionName = "Select" });
			i++;


			if(Date != "0")
			{
				foreach (var item in result)
				{
					venueSessionVM.Insert(i, new SelectVenueSessionViewModel
					{
						VenueSessionValue = item.Time.ToString("hh:mm tt"),
						VenueSessionName = item.Time.ToString("hh:mm tt")
					});
					i++;
				}
			}


			return Json(new SelectList(venueSessionVM, "VenueSessionValue", "VenueSessionName"));
		}

		public IActionResult GetGradeViewComponent(string TestVenue, string TestDate, string TestSession)
		{
			return ViewComponent("Grade", new { venue = TestVenue, date = TestDate, time = TestSession });
		}

		public async Task<IActionResult> CandidateAnswers(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			//Get Candidates Answers
			var answers = await _context.Candidates
						.Include(c => c.RecordingLists)
						.ThenInclude(rl => rl.Answers)
						.FirstOrDefaultAsync(c => c.CandidateID == id);

			//Get Test Mark Scheme
			var markScheme = from ms in _context.MarkSchemes
						  join q in _context.Questions on ms.QuestionGroupID equals q.QuestionGroupId
						  join r in _context.Recordings on q.RecordingID equals r.RecordingId
						  from c in _context.Candidates
						  join rl in _context.RecordingLists
							on new { r.RecordingId, c.CandidateID }
							equals new { RecordingId = rl.RecordingID, rl.CandidateID }
							into details
						  from d in details
						  where c.CandidateID == id
						  group new { ms.MarkSchemeID, ms.Index, ms.Answer }
						  by new { ms.MarkSchemeID, ms.Index, ms.Answer } into grp
						  orderby grp.Key.Index
						  select new
						  {
							  grp.Key.MarkSchemeID,
							  grp.Key.Index,
							  grp.Key.Answer
						  };

			//Get Test Total Mark
			var totalMark = _context.Answers
							.Where(a => a.Correctness == true)
							.Where(a => a.CandidateID == id).Count();
							
			// This block is to set the written and actual ans together a.k.a set MarkScheme Answer
			List<MsList> actualAnsList = new List<MsList>();
			foreach (var item in markScheme)
			{
				var ms = new MsList
				{
					Key = item.Index,
					Ans = item.Answer
				};
				actualAnsList.Add(ms);
			}
			List<MsList> sortedAnsList = actualAnsList.OrderBy(a => a.Key).ToList();  //Sort list accroding to the question Index

			//Relocate Data to View Model
			var candAnsViewModel = new CandidateAnswersViewModel
			{
				CandidateID = answers.CandidateID,
				TotalMarks = totalMark,
				Grade = answers.Grade
			};
			var ansByPartList = new List<AnswersByPartViewModel>();

			foreach (var recording in answers.RecordingLists)
			{
				var getRecordingDetails = _context.Recordings.Find(recording.RecordingID);

				var ansByPart = new AnswersByPartViewModel
				{
					RecordingID = getRecordingDetails.RecordingId,
					Title = getRecordingDetails.Title,
					Part = getRecordingDetails.Part
				};

				var ansWifMSList = new List<AnswersWithMarkSchemeViewModel>();

				foreach (var ans in recording.Answers)
				{
					var ansWithMarkScheme = new AnswersWithMarkSchemeViewModel
					{
						AnswerID = ans.AnswerID,
						Index = ans.Index,
						WrittenAnswer = ans.WrittenAnswer,
						Correctness = ans.Correctness,
						MarkSchemeAnswer = sortedAnsList[ans.Index - 1].Ans
					};

					//To Add the list of object of AnsWithMarkScheme with each iteration
					ansWifMSList.Add(ansWithMarkScheme);
				}

				ansByPart.AnswersWithMarkScheme = ansWifMSList;

				//To Add the list of object of AnsByPart iteration
				ansByPartList.Add(ansByPart);
			}

			candAnsViewModel.AnswersByPart = ansByPartList;

			return View(candAnsViewModel);
		}

		[HttpPost, ActionName("UpdateAns")]
		public async Task<IActionResult> UpdateCandidatesAnswer(int AnsID, int CandID)
		{
			var ans = await _context.Answers.FindAsync(AnsID);

			if (ans.Correctness)
			{
				ans.Correctness = false;
				TempData["StatusMessage"] = "Question " + ans.Index +
											" has been mark as wrong";
				TempData["Status"] = "warning";
			}
			else
			{
				ans.Correctness = true;
				TempData["StatusMessage"] = "Question " + ans.Index +
											" has been mark as correct";
				TempData["Status"] = "info";
			}

			await _context.SaveChangesAsync();

			return RedirectToAction("CandidateAnswers", new { id = CandID });
		}

		public IActionResult GenerateGradeReport()
		{
			//GET DATABASE DATA AND RETURN TO VIEW
			int i = 0;
			var result = (from tv in _context.Tests
						  select tv.Venue).Distinct();

			var venueList = result.ToList();

			List<SelectVenueViewModel> venueVM = new List<SelectVenueViewModel>();
			venueVM.Insert(i, new SelectVenueViewModel { VenueValue = "0", VenueName = "Select" });
			i++;

			foreach (var item in venueList)
			{
				venueVM.Insert(i, new SelectVenueViewModel { VenueValue = item, VenueName = item });
				i++;
			}

			ViewBag.ListOfVenue = venueVM;
			return View();
		}

		public IActionResult PrintGradeReport(string venue, string date, string time)
		{
			int i = 0;
			var GradeReportVM= new PrintGradeReportViewModel();
			List<CandidateGradesViewModel> candidateGrades = new List<CandidateGradesViewModel>();

			if (venue != null && date != null && time != null)
			{
				var testing = (from ans in _context.Answers
							   join rl in _context.RecordingLists
									 on new { ans.CandidateID, ans.RecordingID } equals new { rl.CandidateID, rl.RecordingID }
							   join cand in _context.Candidates on rl.CandidateID equals cand.CandidateID
							   join test in _context.Tests on cand.TestID equals test.TestID
							   where test.Venue == venue
							   where test.Date.Date == DateTime.Parse(date).Date
							   where test.Time.Hour == DateTime.Parse(time).Hour
							   where ans.Correctness == true
							   group new { cand.CandidateID, cand.Status, cand.Grade, ans.Correctness }
							   by new { cand.CandidateID, cand.Status, cand.Grade, ans.Correctness } into grp
							   select new
							   {
								   grp.Key.CandidateID,
								   grp.Key.Status,
								   grp.Key.Grade,
								   count = grp.Count()
							   });

				foreach (var item in testing)
				{
					i++;
					CandidateGradesViewModel cg = new CandidateGradesViewModel
					{
						Index = i,
						CandidateID = item.CandidateID,
						Status = item.Status,
						TotalMarks = item.count,
						Grade = item.Grade
					};
					candidateGrades.Add(cg);
				}

				GradeReportVM.Venue = venue;
				GradeReportVM.Date = date;
				GradeReportVM.Time = time;
				GradeReportVM.CandidateGradesViewModels = candidateGrades;
			}



			return View(GradeReportVM);
		}

		private class MsList
		{
			public int Key { get; set; }
			public string Ans { get; set; }
		}



	}
}
