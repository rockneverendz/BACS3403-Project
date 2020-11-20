using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BACS3403_Project.Data;
using BACS3403_Project.Models;

namespace BACS3403_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnswersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AnswersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // POST: api/Answers/PostAnswer
        [HttpPost("PostAnswer")]
        public async Task<ActionResult<Answer>> PostAnswer([FromBody] CandidateDTO request)
        {
            if (ModelState.IsValid)
            {
                Candidate candidate = await _context.Candidates
                            .Include(candidate => candidate.Test)
                            .FirstOrDefaultAsync(c => c.Token == request.Token);

                List<Answer> answers = new List<Answer>();

                foreach (AnswerDTO item in request.answers)
                {
                    answers.Add(new Answer()
                    {
                        Index = item.Index,
                        WrittenAnswer = item.WrittenAnswer,
                        CandidateID = candidate.CandidateID,
                        RecordingID = item.RecordingID,
                    });
                }

                _context.Answers.AddRange(answers);
                await _context.SaveChangesAsync();

                return Accepted();
            }

            return BadRequest();
        }
    }

    public class CandidateDTO
    {
        public string Token { get; set; }

        public AnswerDTO[] answers { get; set; }
    }

    public class AnswerDTO
    {
        public int Index { get; set; }
        public string WrittenAnswer { get; set; }
        public int RecordingID { get; set; }
    }

}