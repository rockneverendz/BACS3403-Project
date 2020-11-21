using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace BACS3403_Project.Controllers
{
	public class TestAdminController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}

		public IActionResult GenerateSeatNumber()
		{
			return View();
		}

		public IActionResult ViewAttendance()
		{
			return View();
		}


	}
}
