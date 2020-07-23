using System.Collections.Generic;

namespace BACS3403_Project.Models
{
    public class Question
    {
        public int QuestionId { get; set; }

        public int Index { get; set; }
        public List<OptionList> Option { get; set; }
        // Only accessible for examiners
        public List<AnswerList> Answer { get; set; }
    }

    public class OptionList
    {
        public int Id { get; set; }
        public string Option { get; set; }
    }

    public class AnswerList
    {
        public int Id { get; set; }
        public string Answer { get; set; }
    }
}