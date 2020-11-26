using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BACS3403_Project.ViewModels
{
	public class AnswersWithMarkSchemeViewModel
	{
		public int Index { get; set; }
		public string WrittenAnswer { get; set; }
		public bool Correctness { get; set; }
		public string MarkSchemeAnswer { get; set; }
		public int AnswerID { get; set; }
	}
}
