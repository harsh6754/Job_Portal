using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace MVC.Controllers
{
    [Authorize]
    public class ATSAnalysisController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;
        private readonly ILogger<ATSAnalysisController> _logger;

        public ATSAnalysisController(IConfiguration configuration, ILogger<ATSAnalysisController> logger)
        {
            _configuration = configuration;
            _httpClient = new HttpClient();
            _apiBaseUrl = _configuration["ApiSettings:BaseUrl"] ?? "http://localhost:5086";
            _logger = logger;
        }

        [HttpPost("analyze-resume")]
        public async Task<IActionResult> AnalyzeResume([FromBody] ResumeAnalysisRequest request)
        {
            try
            {
                // Get resume content from the API
                var resumeResponse = await _httpClient.GetAsync($"{_apiBaseUrl}/api/UserDetails/GetUserResume?id={request.ResumeId}");
                if (!resumeResponse.IsSuccessStatusCode)
                {
                    return StatusCode((int)resumeResponse.StatusCode, "Failed to get resume");
                }

                var resumeData = await resumeResponse.Content.ReadFromJsonAsync<dynamic>();
                var resumeContent = resumeData.GetProperty("data").GetProperty("c_ResumeContent").GetString();

                // Call the existing analyze endpoint
                var analysisRequest = new
                {
                    Resume = resumeContent,
                    JobDescription = "General ATS Analysis" // Using a generic job description for ATS friendliness check
                };

                var response = await _httpClient.PostAsync(
                    $"{_apiBaseUrl}/api/ATSAnalysis/analyze",
                    new StringContent(JsonSerializer.Serialize(analysisRequest), Encoding.UTF8, "application/json")
                );

                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode((int)response.StatusCode, "Failed to analyze resume");
                }

                var result = await response.Content.ReadFromJsonAsync<dynamic>();
                var score = result.GetProperty("score").GetInt32();

                return Json(new { 
                    success = true, 
                    score = score
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error analyzing resume: {ex.Message}");
                return StatusCode(500, "An error occurred while analyzing the resume");
            }
        }

        public class ResumeAnalysisRequest
        {
            public string ResumeId { get; set; }
        }
    }
} 