using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BACS3403_Project.ViewModels
{
	public class CandidateAttendanceViewModel
	{
		public int Index { get; set; }
		public int Seat { get; set; }
		public string TimeRegistered { get; set; }
		public int CandidateID { get; set; }
		public string CandidateName { get; set; }
		public string Status { get; set; }
	}
}
