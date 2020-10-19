using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BACS3403_Project.Models
{
    public class Examiner : IdentityUser
    {
        public string Name { get; set; }
        public ICollection<Recording> Recordings { get; set; }
    }
}
