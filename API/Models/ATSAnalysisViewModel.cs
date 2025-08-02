using System.ComponentModel.DataAnnotations;

namespace CareerLink.API.Models
{
    public class ATSAnalysisViewModel
    {
        [Required(ErrorMessage = "Please upload your resume")]
        public IFormFile Resume { get; set; }

        // Job description is optional for general ATS analysis
        public string JobDescription { get; set; }

        public string AnalysisResult { get; set; }
        public int Score { get; set; }
        public List<string> MatchingKeywords { get; set; }
        public List<string> MissingKeywords { get; set; }
        public bool IsEligible { get; set; }
        public List<string> Recommendations { get; set; }
    }
} 