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
				var testID = await (from t in _context.Tests
						   where t.Venue == testDetails.Venue
						   where t.Date.Date == DateTime.Parse(testDetails.Date).Date
						   where t.Time.Hour == DateTime.Parse(testDetails.Session).Hour
						   select t.TestID).FirstOrDefaultAsync();

				//Find all candidates with test == testDetails
				var candidatesList = await _context.Candidates
										.Where(x => x.TestID == testID).ToListAsync();

				//Check whether seats no. already generated
				var seats = await _context.Seats
							.Where(x => x.TestID == testID).ToListAsync();

				Console.WriteLine(seats.Count);

				if (seats.Count == 0)
				{
					foreach (var candidate in candidatesList)
					{
						var seatPerTest = new Seat()
						{
							TestID = testID
						};
						_context.Add(seatPerTest);
						_context.SaveChanges();
					}
					seats = await _context.Seats
							.Where(x => x.TestID == testID).ToListAsync();
					Console.WriteLine(seats.Count);
				}

				return View(seats);
			}

			return RedirectToAction("GenerateSeatNumber");
		}




		public IActionResult ViewAttendance()
		{
			return View();
		}


	}
}
