namespace BACS3403_Project.Models
{
    public class Answer
    {
        public int AnswerID { get; set; }
        public string WrittenAnswer { get; set; }
        public int AnswerListID { get; set; }

        public AnswerList AnswerList { get; set; }
    }
}