using System.Collections.Generic;

namespace BACS3403_Project.Models
{
    public class AnswerList
    {
        public int AnswerListID { get; set; }
        public int CandidateID { get; set; }
        public int RecordingID { get; set; }

        public RecordingList RecordingList { get; set; }
        public ICollection<Answer> Answers { get; set; }
    }
}