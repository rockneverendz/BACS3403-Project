using BACS3403_Project.Data;
using BACS3403_Project.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BACS3403_Project.ViewComponents
{
	public class GradeViewComponent : ViewComponent
	{
		private readonly ApplicationDbContext _context;

		public GradeViewComponent(ApplicationDbContext context)
		{
			_context = context;
		}

		public IViewComponentResult Invoke(string venue, string date, string time)
		{

			//get candidates list frm DB
			/*	select c.CandidateID, c.Name, c.Status, COUNT(a.Correctness) as 'Total Marks', c.Grade
			 *	from Test t, Candidate c, RecordingList rl, Answer a
			 *	where t.TestID = c.TestID AND
			 *			rl.CandidateID = c.CandidateID AND
			 *			rl.CandidateID = a.CandidateID AND
			 *			rl.RecordingID = a.RecordingID AND
			 *			t.Venue = 'British Council Kuala Lumpur' AND
			 *			t.Date = CAST('2020-12-18 11:00:00' as datetime2) AND
			 *			a.Correctness = 1
			 *	group by c.CandidateID, c.Name, c.Status, c.Grade
			 */


			var i = 0;
			List<CandidateGradesViewModel> candidateGrades = new List<CandidateGradesViewModel>();

			if (date != "" && time != "")
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
			}
			

			return View(candidateGrades);
		}

	}
}
