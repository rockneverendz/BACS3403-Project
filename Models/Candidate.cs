using System.Collections.Generic;

namespace BACS3403_Project.Models
{
    public class Candidate
    {
        public int CandidateID { get; set; }
        public string Name { get; set; }
        public string Token { get; set; }
        public string RecentPicture { get; set; }
        public string IdentificationCard { get; set; }
        public int TestID { get; set; }
        public string Status { get; set; }
        public string Grade { get; set; }

        public ICollection<RecordingList> RecordingLists { get; set; }
        public Test Test { get; set; }

        public Seat Seat { get; set; }
    }
}