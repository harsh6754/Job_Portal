using System.Collections.Generic;

namespace CareerLink.API.Models
{
    public class ATSAnalysisResult
    {
        public int Score { get; set; }
        public List<string> Keywords { get; set; }
        public List<string> MissingKeywords { get; set; }
        public List<string> Suggestions { get; set; }
        public List<string> ResumeSkills { get; set; }
        public List<string> JobSkills { get; set; }
        public List<string> MatchingSkills { get; set; }
        public List<string> MissingSkills { get; set; }
    }
} 