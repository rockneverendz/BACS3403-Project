using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BACS3403_Project.ViewModels
{
	public class AnswersByPartViewModel
	{
		public int RecordingID { get; set; }
		public string Title { get; set; }
		public int Part { get; set; }
		public IList<AnswersWithMarkSchemeViewModel> AnswersWithMarkScheme { get; set; }

	}
}
