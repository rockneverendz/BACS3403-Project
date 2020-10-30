using System.Collections.Generic;

namespace BACS3403_Project.Models
{
    public class MarkScheme
    {
        public int MarkSchemeID { get; set; }
        public int Index { get; set; }

        // Only accessible for examiners
        public string Answer { get; set; }
        public int QuestionGroupID { get; set; }
        public QuestionGroup QuestionGroup { get; set; }
    }
}