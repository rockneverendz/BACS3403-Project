using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BACS3403_Project.Models
{
    public class Examiner
    {
        public int ExaminerId { get; set; }

        // user ID from AspNetUser table.
        public string OwnerID { get; set; }
        public string Name { get; set; }
    }
}
