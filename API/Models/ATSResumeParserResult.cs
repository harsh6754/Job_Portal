using System.Collections.Generic;

namespace CareerLink.API.Models
{
    public class ATSResumeParserResult
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Location { get; set; }
        public string Summary { get; set; }
        public List<string> Skills { get; set; }
        public List<WorkExperience> WorkExperience { get; set; }
        public List<Education> Education { get; set; }
        public List<string> Certifications { get; set; }
        public List<string> Languages { get; set; }
    }

    public class WorkExperience
    {
        public string Company { get; set; }
        public string Position { get; set; }
        public string Duration { get; set; }
        public string Description { get; set; }
        public List<string> Achievements { get; set; }
    }

    public class Education
    {
        public string Institution { get; set; }
        public string Degree { get; set; }
        public string Field { get; set; }
        public string Duration { get; set; }
        public double? GPA { get; set; }
    }
} 