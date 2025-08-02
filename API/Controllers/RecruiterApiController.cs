using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using Repositories.Interfaces;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecruiterApiController : ControllerBase
    {
        private readonly IDashboardInterface _dashboardRepo;

        public RecruiterApiController(IConfiguration configuration, IDashboardInterface dashboardInterface)
        {
            _dashboardRepo = dashboardInterface;
        }

        [HttpGet("counts")]
        public async Task<IActionResult> GetCounts([FromQuery] int companyid)
        {
            try
            {
                var counts = await _dashboardRepo.GetCounts(companyid);
                return Ok(counts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Failed to fetch counts", details = ex.Message });
            }
        }

        [HttpGet("interview-trends")]
        public async Task<IActionResult> GetInterviewTrends(DateTime startDate, DateTime endDate, [FromQuery] int companyid)
        {
            try
            {
                var trends =  await _dashboardRepo.GetInterviewTrends(startDate, endDate, companyid);

                return Ok(trends);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Failed to fetch interview trends", details = ex.Message });
            }
        }

        [HttpGet("application-trends")]
        public async Task<IActionResult> GetApplicationTrends(DateTime startDate, DateTime endDate, [FromQuery] int companyid)
        {
            try
            {
                var trends = await _dashboardRepo.GetApplicationTrends(startDate, endDate, companyid);

                return Ok(trends);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Failed to fetch application trends", details = ex.Message });
            }
        }
    }
}