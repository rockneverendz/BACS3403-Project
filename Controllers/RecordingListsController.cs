using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BACS3403_Project.Data;
using BACS3403_Project.Models;
using System.ComponentModel.DataAnnotations;

namespace BACS3403_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecordingListsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RecordingListsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // POST: api/RecordingLists/CreateRecordingList
        [HttpPost("CreateRecordingList")]
        public async Task<ActionResult<RecordingDTO[]>> CreateRecordingList(string token)
        {
            if (token == null) return BadRequest();

            // Get CandidateBy Token
            Candidate candidate = await _context.Candidates
                                        .Include(candidate => candidate.RecordingLists)
                                        .FirstOrDefaultAsync(candidate => candidate.Token == token);

            // Generate list if not exists (first time)
            if (candidate.RecordingLists.Count == 0)
            {
                GenerateRecordingList(candidate);
            }

            var recordingDTO = _context.RecordingLists
                                .Include(recordingLists => recordingLists.Recording.QuestionGroups)
                                .OrderBy(item => item.Recording.Part)
                                .ToArray();

            return RecordingToDTO(recordingDTO);
        }

        private RecordingList[] GenerateRecordingList(Candidate candidate)
        {
            // Get list of available recordings
            var recordingList = _context.Recordings
                                .Where(r => r.Available == true).AsQueryable();

            // Randomize question
            RecordingList[] recordpart = new RecordingList[4];
            for (int i = 0; i < 4; i++)
            {
                Recording[] recordingArray = recordingList.Where(r => r.Part == i + 1).ToArray();

                Random random = new Random();
                int randomIndex = random.Next(recordingArray.Length);

                RecordingList part = new RecordingList
                {
                    RecordingID = recordingArray[randomIndex].RecordingId,
                    CandidateID = candidate.CandidateID
                };

                recordpart[i] = part;
            }

            _context.RecordingLists.AddRange(recordpart);
            _context.SaveChanges();

            return recordpart;
        }

        // PUT: api/RecordingLists/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRecordingList(int id, RecordingList recordingList)
        {
            if (id != recordingList.CandidateID)
            {
                return BadRequest();
            }

            _context.Entry(recordingList).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RecordingListExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/RecordingLists
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<RecordingList>> PostRecordingList(RecordingList recordingList)
        {
            _context.RecordingLists.Add(recordingList);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (RecordingListExists(recordingList.CandidateID))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetRecordingList", new { id = recordingList.CandidateID }, recordingList);
        }

        // DELETE: api/RecordingLists/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<RecordingList>> DeleteRecordingList(int id)
        {
            var recordingList = await _context.RecordingLists.FindAsync(id);
            if (recordingList == null)
            {
                return NotFound();
            }

            _context.RecordingLists.Remove(recordingList);
            await _context.SaveChangesAsync();

            return recordingList;
        }

        private bool RecordingListExists(int id)
        {
            return _context.RecordingLists.Any(e => e.CandidateID == id);
        }

        private RecordingDTO[] RecordingToDTO(RecordingList[] recordingList)
        {
            // Get the current domain name
            var domainName = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}";
            RecordingDTO[] recordingDTO = new RecordingDTO[recordingList.Length];

            for (int i = 0; i < recordingList.Length; i++)
            {
                var item = recordingList[i].Recording;

                QuestionGroupDTO[] questiongroupDTO = new QuestionGroupDTO[item.QuestionGroups.Count];
                for (int j = 0; j < item.QuestionGroups.Count; j++)
                {
                    var source = item.QuestionGroups.ElementAt(j);
                    questiongroupDTO[j] = new QuestionGroupDTO
                    {
                        QuestionNoStart = source.QuestionNoStart,
                        QuestionNoEnd = source.QuestionNoEnd,
                        QuestionGroupId = source.QuestionGroupId,
                        QuestionGroupURL = domainName + '/' + source.QuestionGroupURL,
                        TaskType = source.TaskType,
                    };
                }

                recordingDTO[i] = new RecordingDTO
                {
                    RecordingId = item.RecordingId,
                    Title = item.Title,
                    Part = item.Part,
                    AudioURL = domainName + "/Storage/AudioRecordings/Part" + item.Part + "/" + item.AudioURL,
                    QuestionGroups = questiongroupDTO,
                };
            }

            return recordingDTO;
        }
    }
}

public class RecordingDTO
{
    public int RecordingId { get; set; }
    public string Title { get; set; }
    public int Part { get; set; }
    public string AudioURL { get; set; }

    public ICollection<QuestionGroupDTO> QuestionGroups { get; set; }
}

public class QuestionGroupDTO
{
    /*
        Task type 1 – Multiple choice
        Task type 2 – Matching
        Task type 3 – Plan, map, diagram labelling
        Task type 4 – Form, note, table, flow-chart, summary completion
        Task type 5 – Sentence completion
        Task type 6 – Short-answer questions
    */

    public int QuestionGroupId { get; set; }
    public int QuestionNoStart { get; set; }
    public int QuestionNoEnd { get; set; }
    public int TaskType { get; set; }
    public string QuestionGroupURL { get; set; }
}
