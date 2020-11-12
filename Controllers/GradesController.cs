using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace BACS3403_Project.Controllers
{
	public class GradesController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}

		public IActionResult ReviewTestAnswers()
		{
			return View();
		}

		public IActionResult GenerateGradeReport()
		{
			return View();
		}



	}
}
