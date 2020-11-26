using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BACS3403_Project.ViewModels
{
	public class CandidateAnswersViewModel
	{
		public int CandidateID { get; set; }
		public int TotalMarks { get; set; }
		public string Grade { get; set; }
		public IList<AnswersByPartViewModel> AnswersByPart { get; set; }
	}
}
