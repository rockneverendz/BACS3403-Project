using BACS3403_Project.Data;
using BACS3403_Project.ViewModels;
using Microsoft.AspNetCore.Mvc;
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

		public IViewComponentResult Invoke()
		{
			List<CandidateGradesViewModel> candidateGrades = new List<CandidateGradesViewModel> 
			{
				new CandidateGradesViewModel
				{
					Index = 1,
					CandidateID = 1001,
					Status = "Present",
					TotalMarks = 90,
					Grade = "Band 5"
				},
				new CandidateGradesViewModel
				{
					Index = 2,
					CandidateID = 1002,
					Status = "Present",
					TotalMarks = 80,
					Grade = "Band 5"
				},
				new CandidateGradesViewModel
				{
					Index = 3,
					CandidateID = 1003,
					Status = "Present",
					TotalMarks = 75,
					Grade = "Band 4"
				}
			};

			return View(candidateGrades);
		}

	}
}
