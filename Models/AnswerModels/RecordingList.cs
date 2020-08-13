using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BACS3403_Project.Models
{
    public class RecordingList
    {
        public int CandidateID { get; set; }
        public int RecordingID { get; set; }
        public Candidate Candidate { get; set; }
        public Recording Recording { get; set; }
        public ICollection<Answer> Answers { get; set; }
    }
}
