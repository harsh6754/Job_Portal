using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using System.Data.SqlClient;
using CareerLink.WebApi.Models;

namespace CareerLink.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JobPostApiController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public JobPostApiController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("getAllJobs")]
        public async Task<IActionResult> GetAllJobs()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    await connection.OpenAsync();
                    var query = @"SELECT jp.*, c.c_company_name 
                                FROM tbl_job_post jp
                                INNER JOIN tbl_company c ON jp.c_company_id = c.c_company_id
                                WHERE jp.c_is_deleted = 0";

                    var jobs = await connection.QueryAsync<JobPostModel>(query);
                    
                    // Convert/Parse numeric fields explicitly
                    var formattedJobs = jobs.Select(job => new
                    {
                        c_job_id = Convert.ToInt32(job.c_job_id),
                        c_job_title = job.c_job_title,
                        c_job_desc = job.c_job_desc,
                        c_job_type = job.c_job_type,
                        c_work_mode = job.c_work_mode,
                        c_job_location = job.c_job_location,
                        c_job_experience = Convert.ToInt32(job.c_job_experience),
                        c_salary_range = job.c_salary_range,
                        c_vacancy = Convert.ToInt32(job.c_vacancy),
                        c_qualification_title = job.c_qualification_title,
                        c_skills = job.c_skills,
                        c_company_id = Convert.ToInt32(job.c_company_id),
                        c_company_name = job.c_company_name,
                        c_post_date = job.c_post_date,
                        c_expire_date = job.c_expire_date,
                        c_dept_id = Convert.ToInt32(job.c_dept_id),
                        c_is_deleted = Convert.ToBoolean(job.c_is_deleted)
                    });

                    return Ok(formattedJobs);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving jobs: " + ex.Message });
            }
        }

        // ...existing code...
    }
}