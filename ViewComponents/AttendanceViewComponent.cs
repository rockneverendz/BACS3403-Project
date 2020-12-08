using BACS3403_Project.Data;
using BACS3403_Project.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BACS3403_Project.ViewComponents
{
	public class AttendanceViewComponent : ViewComponent
	{
		private readonly ApplicationDbContext _context;
		public AttendanceViewComponent(ApplicationDbContext context)
		{
			_context = context;
		}

		public IViewComponentResult Invoke(string venue, string date, string time)
		{
			/*
			 *	Select s.SeatID, s.TimeClaimed, c.CandidateID, c.Name,  c.Status
			 *	From Seat s, Test t, Candidate c
			 *	Where c.TestID = t.TestID AND
			 *	s.TestID = t.TestID AND
			 *	s.CandidateID = c.CandidateID AND
			 *	t.Venue = 'British Council Kuala Lumpur' AND
			 *	t.Date = CAST('2020-12-18 11:00:00' as datetime2)
			*/

			var i = 0;
			List<CandidateAttendanceViewModel> TestAttendance = new List<CandidateAttendanceViewModel>();

			if (date != "" && time != "")
			{
				var query = (from s in _context.Seats
							 join t in _context.Tests on s.TestID equals t.TestID
							 join c in _context.Candidates on s.CandidateID equals c.CandidateID
							 where t.Venue == venue
							 where t.Date.Date == DateTime.Parse(date).Date
							 where t.Time.Hour == DateTime.Parse(time).Hour
							 select new
							 {
								 s.SeatID,
								 s.TimeClaimed,
								 c.CandidateID,
								 c.Name,
								 c.Status
							 }).ToList();

				foreach (var item in query)
				{
					i++;
					CandidateAttendanceViewModel attd = new CandidateAttendanceViewModel
					{
						Index = i,
						Seat = item.SeatID,
						TimeRegistered = item.TimeClaimed.GetValueOrDefault().ToString("dd/MM/yyyy hh:mm tt"),
						CandidateID = item.CandidateID,
						CandidateName = item.Name,
						Status = item.Status
					};
					TestAttendance.Add(attd);
				}
			}

			return View(TestAttendance);
		}


	}
}
