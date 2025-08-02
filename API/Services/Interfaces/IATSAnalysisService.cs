using Microsoft.AspNetCore.Http;
using CareerLink.API.Models;
using System.Threading.Tasks;

namespace CareerLink.API.Services.Interfaces
{
    public interface IATSAnalysisService
    {
        Task<ATSAnalysisResult> AnalyzeResume(IFormFile resume, string jobDescription);
        Task<ATSJobMatchResult> MatchJob(IFormFile resume, string jobDescription);
    }
} 