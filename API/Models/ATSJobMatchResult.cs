using System.Collections.Generic;

namespace CareerLink.API.Models
{
    public class ATSJobMatchResult
    {
        public int MatchScore { get; set; }
        public List<string> MatchingSkills { get; set; }
        public List<string> MissingSkills { get; set; }
        public List<string> Recommendations { get; set; }
    }
} 