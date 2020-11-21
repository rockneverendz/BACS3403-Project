namespace BACS3403_Project.Models
{
    public class Answer
    {
        public int AnswerID { get; set; }
        public int Index { get; set; }
        public string WrittenAnswer { get; set; }
        public bool Correctness { get; set; }
        public int CandidateID { get; set; }
        public int RecordingID { get; set; }

        public RecordingList RecordingList{ get; set; }
    }
}