using System.Collections.Generic;

namespace CareerLink.API.Models
{
    public class ATSResumeOptimizationResult
    {
        public string OptimizedContent { get; set; }
        public Dictionary<string, string> SectionOptimizations { get; set; }
        public List<string> AddedKeywords { get; set; }
        public List<string> RemovedKeywords { get; set; }
        public Dictionary<string, double> ImprovementScores { get; set; }
        public List<string> OptimizationSuggestions { get; set; }
    }
} 