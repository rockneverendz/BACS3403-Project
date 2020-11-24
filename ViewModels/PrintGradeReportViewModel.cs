using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BACS3403_Project.ViewModels
{
	public class PrintGradeReportViewModel
	{
		public string Venue { get; set; }
		public string Date { get; set; }
		public string Time { get; set; }
		public IList<CandidateGradesViewModel> CandidateGradesViewModels { get; set; }
	}
}
