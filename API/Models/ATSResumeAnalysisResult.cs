using System.Collections.Generic;

namespace CareerLink.API.Models
{
    public class ATSResumeAnalysisResult
    {
        public double OverallScore { get; set; }
        public Dictionary<string, double> SectionScores { get; set; }
        public List<string> Strengths { get; set; }
        public List<string> Weaknesses { get; set; }
        public List<string> Suggestions { get; set; }
        public Dictionary<string, List<string>> KeywordAnalysis { get; set; }
        public Dictionary<string, string> FormatAnalysis { get; set; }
    }
} 