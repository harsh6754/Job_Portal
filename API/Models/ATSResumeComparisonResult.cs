using System.Collections.Generic;

namespace CareerLink.API.Models
{
    public class ATSResumeComparisonResult
    {
        public double MatchPercentage { get; set; }
        public List<string> MatchingKeywords { get; set; }
        public List<string> MissingKeywords { get; set; }
        public Dictionary<string, double> SectionMatchScores { get; set; }
        public List<string> Recommendations { get; set; }
    }
} 