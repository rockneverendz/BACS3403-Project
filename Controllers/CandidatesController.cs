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
using System.ComponentModel.DataAnnotations;

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
            if (!ModelState.IsValid) return BadRequest();

            string recentPicturePath = null;

            try
            {
                var token = candidateFaceModel.Token;
                var recentPicture = candidateFaceModel.Face;

                Candidate candidate = await GetCandidateByToken(token);

                if (candidate == null)
                    return NotFound();

                recentPicturePath = SaveRecentPicture(token, recentPicture);

                var face1 = await UploadAndDetectFace(candidate.IdentificationCard);
                var face2 = await UploadAndDetectFace(recentPicturePath);

                var result = await _faceClient.Face.VerifyFaceToFaceAsync((Guid)face1.FaceId, (Guid)face2.FaceId);

                // Check if face is identical
                if (!result.IsIdentical) throw new Exception("Face is not identical!");

                // Delete old picture if exsits
                if (candidate.RecentPicture != null) System.IO.File.Delete(candidate.RecentPicture);
                candidate.RecentPicture = recentPicturePath;
                candidate.Status = "Verified";
                _context.SaveChanges();
                return Ok();
            }
            catch (Exception e)
            {
                if (recentPicturePath != null)
                {
                    System.IO.File.Delete(recentPicturePath);
                }
                return BadRequest(e);
            }
        }

        public async Task<Candidate> GetCandidateByToken(string token)
        {
            return await _context.Candidates
                            .Include(candidate => candidate.Test)
                            .FirstOrDefaultAsync(candidate => candidate.Token == token);
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

        private bool CandidateExists(int id)
        {
            return _context.Candidates.Any(e => e.CandidateID == id);
        }

        // Uploads the image file and calls DetectWithStreamAsync.
        private async Task<DetectedFace> UploadAndDetectFace(string imageFilePath)
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
                else if (face.Count == 0)
                {
                    throw new Exception("No face detected.");
                }
                else
                {
                    throw new Exception("Multiple face detected.");
                }
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
        [Required]
        public string Token { get; set; }
        [Required]
        public IFormFile Face { get; set; }
    }
}
