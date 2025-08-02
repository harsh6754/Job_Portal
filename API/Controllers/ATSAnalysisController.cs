using Microsoft.AspNetCore.Mvc;
using CareerLink.API.Models;
using CareerLink.API.Services;
using System.Threading.Tasks;
using Repositories.Interfaces;
using Repositories.Interface;
using Repositories.Models;
using Repositories.Model;
using System.IO;
using Microsoft.AspNetCore.Http;
using System;

namespace CareerLink.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ATSAnalysisController : ControllerBase
    {
        private readonly IATSAnalysisService _atsAnalysisService;
        private readonly IUserResume _userResumeRepository;
        private readonly IPostedJobInterface _jobRepository;

        public ATSAnalysisController(
            IATSAnalysisService atsAnalysisService,
            IUserResume userResumeRepository,
            IPostedJobInterface jobRepository)
        {
            _atsAnalysisService = atsAnalysisService;
            _userResumeRepository = userResumeRepository;
            _jobRepository = jobRepository;
        }

        [HttpGet]
        public IActionResult GetInfo()
        {
            return Ok(new { 
                message = "ATS Analysis API is available",
                endpoints = new[] {
                    new { method = "POST", path = "/api/ATSAnalysis/analyze", description = "Full ATS analysis with scoring" },
                    new { method = "POST", path = "/api/ATSAnalysis/analyze-candidate-job", description = "Analyze a candidate's resume against a specific job" }
                }
            });
        }

        [HttpPost("analyze")]
        public async Task<IActionResult> AnalyzeResume([FromForm] ATSAnalysisViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _atsAnalysisService.AnalyzeResume(model.Resume, model.JobDescription);
            return Ok(result);
        }

        [HttpPost("analyze-candidate-job")]
        public async Task<IActionResult> AnalyzeCandidateJob([FromBody] CandidateJobAnalysisRequest request)
        {
            try
            {
                // Validate input
                if (request.CandidateId <= 0)
                {
                    return BadRequest("Invalid candidate ID");
                }

                if (request.JobId <= 0)
                {
                    return BadRequest("Invalid job ID");
                }

                // Get candidate's resume
                var candidateResume = await _userResumeRepository.GetUserResume(request.CandidateId);
                if (candidateResume == null)
                {
                    return NotFound("Candidate resume not found");
                }

                // Get job details
                var jobDetails = await _jobRepository.GetJobDescription(request.JobId);
                if (jobDetails == null)
                {
                    return NotFound("Job not found");
                }

                // Create a file from the resume path
                var resumeFilePath = Path.Combine("../MVC/wwwroot/user_resume", candidateResume.c_ResumeFilePath);
                if (!System.IO.File.Exists(resumeFilePath))
                {
                    return NotFound("Resume file not found");
                }

                // Create an IFormFile from the file path
                var fileStream = new FileStream(resumeFilePath, FileMode.Open, FileAccess.Read);
                var formFile = new FormFile(
                    fileStream, 
                    0, 
                    fileStream.Length, 
                    "Resume", 
                    Path.GetFileName(resumeFilePath)
                );

                // Analyze the resume against the job description
                var result = await _atsAnalysisService.MatchJob(formFile, jobDetails.c_job_desc);

                return Ok(new
                {
                    CandidateId = request.CandidateId,
                    JobId = request.JobId,
                    JobTitle = jobDetails.c_job_title,
                    CompanyName = jobDetails.c_company_name,
                    AnalysisResult = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }

    public class CandidateJobAnalysisRequest
    {
        public int CandidateId { get; set; }
        public int JobId { get; set; }
    }
} 