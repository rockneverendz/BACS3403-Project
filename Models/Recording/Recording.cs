using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BACS3403_Project.Models
{
    public class Recording
    {
        public int RecordingId { get; set; }
        
        /*
            Part 1 – a conversation between two people set in an everyday social context.
            Part 2 - a monologue set in an everyday social context, 
                     e.g. a speech about local facilities.
            Part 3 – a conversation between up to four people set in an educational or training context, 
                     e.g. a university tutor and a student discussing an assignment.
            Part 4 - a monologue on an academic subject, 
                     e.g. a university lecture.
        */
        public int Part { get; set; }
        public string AudioURL { get; set; }
        public QuestionGroup[] Question { get; set; }
    }
}
