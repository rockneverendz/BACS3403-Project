using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using Microsoft.Azure.CognitiveServices.Vision.Face;
using BACS3403_Project.Data;
using BACS3403_Project.Models;
using Microsoft.Extensions.Configuration;

namespace BACS3403_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CandidatesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IFaceClient _faceClient;

        public CandidatesController(ApplicationDbContext context, IConfiguration configuration)
        {
            string key = configuration["Face:SubscriptionKey"];
            string endpoint = configuration["Face:Endpoint"];

            _context = context;
            _faceClient = new FaceClient(
                new ApiKeyServiceClientCredentials(key),
                new System.Net.Http.DelegatingHandler[] { }
            );
            _faceClient.Endpoint = endpoint;
        }

        // GET: api/Candidates/RedeemToken
        [HttpGet("RedeemToken")]
        public async Task<ActionResult<Candidate>> RedeemToken(string token)
        {
            Candidate candidate = await GetCandidateByToken(token);

            if (candidate == null)
            {
                return NotFound();
            }

            return candidate;
        }

        // GET: api/Candidates/VerifyCandidate
        [HttpPost("VerifyCandidate")]
        public async Task<ActionResult<Candidate>> VerifyCandidate([FromForm] CandidateFaceModel candidateFaceModel)
        {
            var token = candidateFaceModel.Token;
            var recentPicture = candidateFaceModel.Face;

            Candidate candidate = await GetCandidateByToken(token);

            if (candidate == null)
                return NotFound();

            string recentPicturePath = SaveRecentPicture(token, recentPicture);

            var face1 = await UploadAndDetectFace(candidate.IdentificationCard);
            var face2 = await UploadAndDetectFace(recentPicturePath);

            var result = await _faceClient.Face.VerifyFaceToFaceAsync((Guid)face1.FaceId, (Guid)face2.FaceId);

            if (result.IsIdentical)
            {
                // Delete old picture if exsits
                if (candidate.RecentPicture != null) System.IO.File.Delete(candidate.RecentPicture);
                candidate.RecentPicture = recentPicturePath;
                candidate.Status = "Verified";
                _context.SaveChanges();
                return Ok();
            }
            else
            {
                System.IO.File.Delete(recentPicturePath);
                return Unauthorized(); 
            }
        }

        private async Task<Candidate> GetCandidateByToken(string Token)
        {
            return await _context.Candidates
                            .Include(candidate => candidate.Test)
                            .FirstOrDefaultAsync(candidate => candidate.Token == Token);
        }

        private static string SaveRecentPicture(string Token, IFormFile recentPicture)
        {
            // Create a path
            string filePart = "Storage\\Candidate\\" + Token + "\\";
            string uniqueFileName = Guid.NewGuid().ToString() + "_" + recentPicture.FileName;
            string path = Path.Combine(filePart, uniqueFileName);

            // Create a directory if it doesn't exsits
            Directory.CreateDirectory(filePart);

            // Save to storage
            using (var stream = new FileStream(path, FileMode.Create))
            {
                recentPicture.CopyTo(stream);
            }

            return path;
        }

        // PUT: api/Candidates/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCandidate(int id, Candidate candidate)
        {
            if (id != candidate.CandidateID)
            {
                return BadRequest();
            }

            _context.Entry(candidate).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CandidateExists(id))
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

        // POST: api/Candidates
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Candidate>> PostCandidate(Candidate candidate)
        {
            _context.Candidates.Add(candidate);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCandidate", new { id = candidate.CandidateID }, candidate);
        }

        // DELETE: api/Candidates/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Candidate>> DeleteCandidate(int id)
        {
            var candidate = await _context.Candidates.FindAsync(id);
            if (candidate == null)
            {
                return NotFound();
            }

            _context.Candidates.Remove(candidate);
            await _context.SaveChangesAsync();

            return candidate;
        }

        private bool CandidateExists(int id)
        {
            return _context.Candidates.Any(e => e.CandidateID == id);
        }

        // Uploads the image file and calls DetectWithStreamAsync.
        private async Task<DetectedFace> UploadAndDetectFace(string imageFilePath)
        {
            // Call the Face API.
            try
            {
                using (Stream imageFileStream = System.IO.File.OpenRead(imageFilePath))
                {
                    // The second argument specifies to return the faceId, while
                    // the third argument specifies not to return face landmarks.
                    IList<DetectedFace> face =
                        await _faceClient.Face.DetectWithStreamAsync(
                            imageFileStream, true, false);

                    if (face.Count == 1)
                    {
                        return face[0];
                    }
                    else
                    {
                        throw new Exception("Multiple face detected.");
                    }
                }
            }
            // Catch and display Face API errors.
            catch (APIErrorException f)
            {
                throw f;
                //MessageBox.Show(f.Message);
                //return new List<DetectedFace>();
            }
            // Catch and display all other errors.
            catch (Exception e)
            {
                throw e;
                //MessageBox.Show(e.Message, "Error");
                //return new List<DetectedFace>();
            }
        }
    }

    public class CandidateAPIModel
    {
        public int CandidateID { get; set; }
        public string Name { get; set; }
        public string Token { get; set; }
        public int TestID { get; set; }
        public string Status { get; set; }
        public string Grade { get; set; }

        public ICollection<RecordingList> RecordingLists { get; set; }
        public Test Test { get; set; }
    }

    public class CandidateFaceModel
    {
        public string Token { get; set; }
        public IFormFile Face { get; set; }
    }
}
