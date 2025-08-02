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
    [Route("api/jobrecommendation")]
    public class JobRecommendationApiController : ControllerBase
    {
        private readonly IJobRecommendationInterface _jobRecommendation;
        public JobRecommendationApiController(IJobRecommendationInterface jobRecommedation)
        {
             _jobRecommendation = jobRecommedation;
        }
        [HttpPost("GetJobRecommendation")]
        public async Task<IActionResult> GetJobRecommendation([FromBody]t_JobPreference jobPreference){
            // var res = await _jobRecommendation.RecommendJobs(jobPreference);

            // return Ok(res);

            var jobs = await _jobRecommendation.RecommendJobs(jobPreference);
            return Ok(jobs);
        } 
    }
}