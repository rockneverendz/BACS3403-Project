using System;

namespace BACS3403_Project.Models
{
    public class Seat
    {
        public int SeatID { get; set; }
        public DateTime? TimeClaimed { get; set; }
        public int? CandidateID { get; set; }
        public int TestID { get; set; }
        public Candidate Candidates { get; set; }
        public Test Test { get; set; }    
    }
}