using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BACS3403_Project.Models
{
    public class Test
    {
        public int TestID { get; set; }
        public string Venue { get; set; }
        public DateTime Date { get; set; }
        public DateTime Time { get; set; }

        public ICollection<Candidate> Candidates { get; set; }
        public ICollection<Seat> Seats { get; set; }

        public DateTime DateTime()
        {
            return Date.Date.Add(Time.TimeOfDay);
        }
    }
}
