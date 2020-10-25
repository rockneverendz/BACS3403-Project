using Microsoft.CodeAnalysis.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BACS3403_Project.Models
{
    public class QuestionGroup
    {
        /*
            Task type 1 – Multiple choice
            Task type 2 – Matching
            Task type 3 – Plan, map, diagram labelling
            Task type 4 – Form, note, table, flow-chart, summary completion
            Task type 5 – Sentence completion
            Task type 6 – Short-answer questions
        */

        public int QuestionGroupId { get; set; }
        public int QuestionNoStart { get; set; }
        public int QuestionNoEnd { get; set; }
        public int TaskType { get; set; }
        public string QuestionGroupURL { get; set; }
        public int RecordingID { get; set; }

        public Recording Recording { get; set; }
        public ICollection<MarkScheme> MarkSchemes { get; set; }
    }
}
