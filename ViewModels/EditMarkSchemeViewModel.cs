using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BACS3403_Project.ViewModels
{
	public class EditMarkSchemeViewModel
	{
        public int MarkSchemeID { get; set; }
        public int Index { get; set; }

        // Only accessible for examiners
        public string Answer { get; set; }
        public int QuestionGroupID { get; set; }
    }
}
