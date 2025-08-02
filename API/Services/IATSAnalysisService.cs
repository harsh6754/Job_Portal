using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using CareerLink.API.Models;

namespace CareerLink.API.Services
{
    public interface IATSAnalysisService
    {
        Task<ATSAnalysisResult> AnalyzeResume(IFormFile resume, string jobDescription);
        Task<ATSJobMatchResult> MatchJob(IFormFile resume, string jobDescription);
    }
} 