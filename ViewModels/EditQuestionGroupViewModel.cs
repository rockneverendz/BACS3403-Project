using BACS3403_Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BACS3403_Project.ViewModels
{
	public class EditQuestionGroupViewModel
	{
        public int QuestionGroupId { get; set; }
        public int QuestionNoStart { get; set; }
        public int QuestionNoEnd { get; set; }
        public int TaskType { get; set; }
        public string QuestionGroupURL { get; set; }
        public int RecordingID { get; set; }

        public IList<EditMarkSchemeViewModel> MarkSchemes { get; set; }
    }
}
