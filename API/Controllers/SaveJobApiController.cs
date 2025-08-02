using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Repositories.Interfaces;
using Repositories.Models;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SaveJobApiController : ControllerBase
    {
        private readonly ISaveJobInterface _repo;
        public SaveJobApiController(ISaveJobInterface repo)
        {
            _repo = repo;
        }

        [HttpPost("add")]
        public IActionResult AddJob(int userId, int jobPostId)
        {
            var result = _repo.Add(userId, jobPostId);
            if (result)
                return Ok(new { message = "Job saved successfully." });
            else
                return Conflict(new { message = "Job already saved." });
        }

        // ❌ Remove a saved job
        [HttpDelete("remove")]
        public IActionResult RemoveJob(int userId, int jobPostId)
        {
            _repo.Remove(userId, jobPostId);
            return Ok(new { message = "Job removed successfully." });
        }
        [HttpGet("saved-jobs/{userId}")]
        public ActionResult<List<t_save_job>> GetSavedJobs(int userId)
        {
            var jobs = _repo.GetSavedJobs(userId);
            return Ok(jobs);
        }
    }
}