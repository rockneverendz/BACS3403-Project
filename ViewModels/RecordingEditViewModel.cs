using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BACS3403_Project.ViewModels
{
	public class RecordingEditViewModel
	{
		/*
			Part 1 – a conversation between two people set in an everyday 
			Part 2 - a monologue set in an everyday social context, 
					 e.g. a speech about local facilities.
			Part 3 – a conversation between up to four people set in an ed
					 e.g. a university tutor and a student discussing an a
			Part 4 - a monologue on an academic subject, 
					 e.g. a university lecture.
		*/
		public int RecordingId { get; set; }

		public string Title { get; set; }
		public int Part { get; set; }

		//Current AudioRec URL
		public string AudioURL { get; set; }

		public IFormFile AudioRecording { get; set; }
	}
}
