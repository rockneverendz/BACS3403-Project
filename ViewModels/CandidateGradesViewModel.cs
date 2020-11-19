using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BACS3403_Project.ViewModels
{
	public class CandidateGradesViewModel
	{
		public int Index { get; set; }
		public int CandidateID { get; set; }
		public string Status { get; set; }
		public int TotalMarks{ get; set; }
		public string Grade { get; set; }
	}
}
