using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BACS3403_Project.Data;
using BACS3403_Project.Models;
using BACS3403_Project.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BACS3403_Project.Controllers
{
	public class TestAdminController : Controller
	{
		private readonly ApplicationDbContext _context;
		private readonly UserManager<Examiner> _userManager;

		public TestAdminController(ApplicationDbContext context, UserManager<Examiner> userManager)
		{
			_context = context;
			_userManager = userManager;
		}

		public IActionResult GenerateSeatNumber()
		{
			//GET DATABASE DATA AND RETURN TO VIEW
			int i = 0;
			var result = (from tv in _context.Tests
						  select tv.Venue).Distinct();

			var venueList = result.ToList();

			List<SelectVenueViewModel> venueVM = new List<SelectVenueViewModel>();
			venueVM.Insert(i, new SelectVenueViewModel { VenueValue = "0", VenueName = "Select" });
			i++;

			foreach (var item in venueList)
			{
				venueVM.Insert(i, new SelectVenueViewModel { VenueValue = item, VenueName = item });
				i++;
			}

			ViewBag.ListOfVenue = venueVM;

			return View();
		}

		public async Task<IActionResult> PrintSeatQR(TestViewModel testDetails)
		{
			if (ModelState.IsValid)
			{
				//Find the Test ID with Details
				var test = await (from t in _context.Tests
								  where t.Venue == testDetails.Venue
								  where t.Date.Date == DateTime.Parse(testDetails.Date).Date
								  where t.Time.Hour == DateTime.Parse(testDetails.Session).Hour
								  select new
								  {
									  t.TestID,
									  t.Venue,
									  t.Date,
									  t.Time
								  }).FirstOrDefaultAsync();

				//Find all candidates with test == testDetails
				var candidatesList = await _context.Candidates
										.Where(x => x.TestID == test.TestID).ToListAsync();

				//Check whether seats no. already generated
				var seats = await _context.Seats
							.Where(x => x.TestID == test.TestID).ToListAsync();

				Console.WriteLine(seats.Count);

				if (seats.Count == 0)
				{
					foreach (var candidate in candidatesList)
					{
						var seatPerTest = new Seat()
						{
							TestID = test.TestID
						};
						_context.Add(seatPerTest);
						_context.SaveChanges();
					}
					seats = await _context.Seats
							.Where(x => x.TestID == test.TestID).ToListAsync();
					Console.WriteLine(seats.Count);
				}

				ViewBag.TestVenue = test.Venue;
				ViewBag.TestDate = test.Date.ToString("dd/MM/yyyy");
				ViewBag.TestSession = test.Time.ToString("hh:mm tt");

				return View(seats);
			}

			return RedirectToAction("GenerateSeatNumber");
		}

		public IActionResult ViewAttendance()
		{
			//GET DATABASE DATA AND RETURN TO VIEW
			int i = 0;
			var result = (from tv in _context.Tests
						  select tv.Venue).Distinct();

			var venueList = result.ToList();

			List<SelectVenueViewModel> venueVM = new List<SelectVenueViewModel>();
			venueVM.Insert(i, new SelectVenueViewModel { VenueValue = "0", VenueName = "Select" });
			i++;

			foreach (var item in venueList)
			{
				venueVM.Insert(i, new SelectVenueViewModel { VenueValue = item, VenueName = item });
				i++;
			}

			ViewBag.ListOfVenue = venueVM;
			return View();
		}

		public IActionResult GetAttendanceViewComponent(string TestVenue, string TestDate, string TestSession)
		{
			return ViewComponent("Attendance", new { venue = TestVenue, date = TestDate, time = TestSession });
		}

		public IActionResult PrintAttendanceReport(string venue, string date, string time)
		{
			int i = 0;
			var AttdReportVM = new PrintAttendanceReportViewModel();
			List<CandidateAttendanceViewModel> Attd = new List<CandidateAttendanceViewModel>();

			if (venue != null && date != null && time != null)
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
					Attd.Add(attd);
				}
				AttdReportVM.Venue = venue;
				AttdReportVM.Date = date;
				AttdReportVM.Time = time;
				AttdReportVM.CandidateAttendanceViewModels = Attd;
			}
			return View(AttdReportVM);
		}
	}
}
