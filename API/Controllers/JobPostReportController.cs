using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repositories.Model;

namespace MyApp.Namespace
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobPostReportController : ControllerBase
    {
        private readonly IJobReportInterface _jobReportInterface;
        public JobPostReportController(IJobReportInterface jobReportInterface)
        {
            _jobReportInterface = jobReportInterface;
        }

        [HttpPost("SaveJobPostReport")]
        public async Task<IActionResult> SaveJobPostReport([FromForm]t_Job_Report job_Report){
            var res = await _jobReportInterface.SaveJobPostReport(job_Report);
            if(res != null){
                return Ok(new {
                    success = true,
                    msg = "Successfully submitted job post report",
                    data = res
                });
            }
            else{
                return BadRequest(new {
                    success = false,
                    msg = "Failed to submit job post report",
                    data = res
                });
            }
        }
    
        [HttpGet("GetJobReport")]
        public async Task<List<t_Job_Report>> GetJobReport(){
            List<t_Job_Report> tt = await _jobReportInterface.GetJobPostReport();
            return tt;
        }
    }
}
