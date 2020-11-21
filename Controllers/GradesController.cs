using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BACS3403_Project.Data;
using BACS3403_Project.Models;
using BACS3403_Project.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BACS3403_Project.Controllers
{
	public class GradesController : Controller
	{
		private readonly ApplicationDbContext _context;
		private readonly UserManager<Examiner> _userManager;
		public GradesController(ApplicationDbContext context, UserManager<Examiner> userManager)
		{
			_context = context;
			_userManager = userManager;
		}

		//NO INDEX VIEW FOR NOW
		public IActionResult Index()
		{
			return View();
		}

		public IActionResult ReviewTestAnswers()
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

		public JsonResult GetTestDate(string Venue)
		{
			int i = 0;
			var result = (from tv in _context.Tests
						  where tv.Venue == Venue
						  select tv.Date.Date).Distinct();

			/* select distinct FORMAT(t.Date, 'dd/mm/yyyy')
			 * from Test t 
			 * where t.Venue = 'British Council Kuala Lumpur'; 
			*/

			List<SelectVenueDateViewModel> venueDateVM = new List<SelectVenueDateViewModel>();
			venueDateVM.Insert(i, new SelectVenueDateViewModel { VenueDateValue = "0", VenueDateName = "Select" });
			i++;

			foreach (var item in result)
			{
				venueDateVM.Insert(i, new SelectVenueDateViewModel 
				{ 
					VenueDateValue = item.ToString("dd/MM/yyyy"),															
					VenueDateName = item.ToString("dd/MM/yyyy")
				});
				i++;
			}

			return Json(new SelectList(venueDateVM, "VenueDateValue", "VenueDateName"));
		}

		public JsonResult GetTestSessions(string Venue, string Date)
		{
			int i = 0;
			var result = _context.Tests
							.Where(t => t.Venue == Venue)
							.Where(t => t.Date.Date == Convert.ToDateTime(Date));

			List<SelectVenueSessionViewModel> venueSessionVM = new List<SelectVenueSessionViewModel>();
			venueSessionVM.Insert(i, new SelectVenueSessionViewModel { VenueSessionValue = "0", VenueSessionName = "Select" });
			i++;


			if(Date != "0")
			{
				foreach (var item in result)
				{
					venueSessionVM.Insert(i, new SelectVenueSessionViewModel
					{
						VenueSessionValue = item.Time.ToString("hh:mm tt"),
						VenueSessionName = item.Time.ToString("hh:mm tt")
					});
					i++;
				}
			}


			return Json(new SelectList(venueSessionVM, "VenueSessionValue", "VenueSessionName"));
		}

		public IActionResult GetGradeViewComponent(string TestVenue, string TestDate, string TestSession)
		{
			return ViewComponent("Grade", new { venue = TestVenue, date = TestDate, time = TestSession });
		}

		[HttpPost]
		public async Task<IActionResult> ReviewTestAnswers(int? id)
		{
			//RECEIVED DATA AND REDIRECT TO CANDIDATES ANSWERS

			return View();
		}

		public IActionResult CandidateAnswers(int? id)
		{
			ViewData["CandidateID"] = id;
			return View();
		}

		public IActionResult GenerateGradeReport()
		{
			return View();
		}



	}
}
