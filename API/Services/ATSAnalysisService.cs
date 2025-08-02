using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;
using CareerLink.API.Models;
using CareerLink.API.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Net.Http;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using System.Text.RegularExpressions;

namespace CareerLink.API.Services
{
    public class ATSAnalysisService : IATSAnalysisService
    {
        private readonly ILogger<ATSAnalysisService> _logger;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private const string GEMINI_API_URL = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent";
        private static readonly Dictionary<string, (int Score, DateTime Timestamp)> _scoreCache = new();
        private const int CACHE_DURATION_MINUTES = 30;
        private const int MAX_SCORE_VARIATION = 5; // Maximum allowed score variation in percentage

        // Add skill normalization dictionary with more comprehensive variations
        private static readonly Dictionary<string, List<string>> _skillVariations = new(StringComparer.OrdinalIgnoreCase)
        {
            // Programming Languages
            { "C#", new List<string> { "CSharp", "C Sharp", "C#.NET", "C# Programming" } },
            { "JavaScript", new List<string> { "JS", "ECMAScript", "ES6", "ES2015", "ES2016", "ES2017", "ES2018", "ES2019", "ES2020" } },
            { "Python", new List<string> { "Python3", "Python 3", "Py", "Python Programming" } },
            { "Java", new List<string> { "Java Programming", "Java Development", "Java SE", "Java EE" } },
            { "TypeScript", new List<string> { "TS", "TypeScript Programming" } },
            
            // Databases
            { "SQL", new List<string> { "MySQL", "PostgreSQL", "SQL Server", "T-SQL", "PL/SQL", "SQLite", "SQL Database", "Database SQL" } },
            { "MongoDB", new List<string> { "Mongo", "Mongo DB", "Mongo Database" } },
            { "Redis", new List<string> { "Redis Cache", "Redis Database" } },
            
            // Frontend
            { "React", new List<string> { "React.js", "ReactJS", "React JS", "React Development", "React Frontend" } },
            { "Angular", new List<string> { "AngularJS", "Angular 2+", "Angular Development" } },
            { "Vue.js", new List<string> { "Vue", "VueJS", "Vue JS", "Vue Development" } },
            
            // Backend
            { "Node.js", new List<string> { "Node", "NodeJS", "Node JS", "Node Development", "Express.js", "Express" } },
            { "ASP.NET", new List<string> { "ASP.NET Core", "ASP.NET MVC", "ASP.NET Web API" } },
            { "Spring Boot", new List<string> { "Spring", "Spring Framework", "Spring MVC" } },
            
            // Cloud & DevOps
            { "AWS", new List<string> { "Amazon Web Services", "Amazon AWS", "AWS Cloud", "AWS Services" } },
            { "Azure", new List<string> { "Microsoft Azure", "Azure Cloud", "Azure Services" } },
            { "Google Cloud", new List<string> { "GCP", "Google Cloud Platform", "GCP Services" } },
            
            // Version Control & CI/CD
            { "Git", new List<string> { 
                "GitHub", "GitLab", "Git Version Control", "Git Commands", "Git Operations",
                "Git Workflow", "Git Branching", "Git Merge", "Git Pull", "Git Push",
                "Git Repository", "Git Remote", "Git Local", "Git Stash", "Git Rebase"
            } },
            { "CI/CD", new List<string> { 
                "Continuous Integration", "Continuous Deployment", "Continuous Delivery",
                "Jenkins", "GitHub Actions", "GitLab CI", "Azure DevOps", "CircleCI",
                "Travis CI", "TeamCity", "Bamboo"
            } },
            
            // Containerization & Orchestration
            { "Docker", new List<string> { 
                "Docker Container", "Docker Engine", "Docker Compose", "Dockerfile",
                "Docker Images", "Docker Hub", "Docker Registry"
            } },
            { "Kubernetes", new List<string> { 
                "K8s", "Kubernetes Cluster", "Kubernetes Deployment",
                "Kubernetes Service", "Kubernetes Pod", "Kubernetes Node"
            } },
            
            // Testing
            { "Unit Testing", new List<string> { 
                "Test-Driven Development", "TDD", "JUnit", "NUnit", "xUnit",
                "Jest", "Mocha", "Chai", "Pytest", "Unit Tests"
            } },
            { "Integration Testing", new List<string> { 
                "End-to-End Testing", "E2E Testing", "System Testing",
                "API Testing", "Service Testing"
            } }
        };

        private List<string> NormalizeSkills(List<string> skills)
        {
            var normalizedSkills = new List<string>();
            foreach (var skill in skills)
            {
                var normalizedSkill = skill.Trim().ToLower();
                
                // First check if this skill is a variation of a standard skill
                bool isVariation = false;
                foreach (var standardSkill in _skillVariations)
                {
                    // Check if the skill is a variation of a standard skill
                    if (standardSkill.Value.Any(v => v.Equals(normalizedSkill, StringComparison.OrdinalIgnoreCase)))
                    {
                        normalizedSkill = standardSkill.Key.ToLower();
                        isVariation = true;
                        break;
                    }
                    
                    // Check if the skill contains or is contained within a standard skill
                    if (standardSkill.Key.ToLower().Contains(normalizedSkill) || 
                        normalizedSkill.Contains(standardSkill.Key.ToLower()))
                    {
                        normalizedSkill = standardSkill.Key.ToLower();
                        isVariation = true;
                        break;
                    }
                }
                
                // If not a variation, check if it's a compound skill (e.g., "React Developer" -> "React")
                if (!isVariation)
                {
                    foreach (var standardSkill in _skillVariations)
                    {
                        if (normalizedSkill.Contains(standardSkill.Key.ToLower()))
                        {
                            normalizedSkill = standardSkill.Key.ToLower();
                            break;
                        }
                    }
                }
                
                if (!normalizedSkills.Contains(normalizedSkill))
                {
                    normalizedSkills.Add(normalizedSkill);
                }
            }
            return normalizedSkills;
        }

        private (List<string> MatchingSkills, List<string> MissingSkills) MatchSkills(List<string> resumeSkills, List<string> jobSkills)
        {
            var normalizedResumeSkills = NormalizeSkills(resumeSkills);
            var normalizedJobSkills = NormalizeSkills(jobSkills);
            
            var matchingSkills = new List<string>();
            var missingSkills = new List<string>();

            // First pass: Find exact matches and variations
            foreach (var jobSkill in normalizedJobSkills)
            {
                bool isMatched = false;
                
                // Check for exact match
                if (normalizedResumeSkills.Contains(jobSkill))
                {
                    matchingSkills.Add(jobSkill);
                    isMatched = true;
                }
                {
                    // Check for variations in the skill variations dictionary
                    foreach (var standardSkill in _skillVariations)
                    {
                        if (standardSkill.Key.ToLower() == jobSkill)
                        {
                            // Check if any variation of this skill exists in resume
                            if (standardSkill.Value.Any(v => normalizedResumeSkills.Contains(v.ToLower())))
                            {
                                matchingSkills.Add(jobSkill);
                                isMatched = true;
                                break;
                            }
                        }
                    }
                }

                if (!isMatched)
                {
                    missingSkills.Add(jobSkill);
                }
            }

            // Second pass: Find partial matches and related skills
            foreach (var resumeSkill in normalizedResumeSkills)
            {
                // Skip if already matched
                if (matchingSkills.Contains(resumeSkill))
                    continue;

                // Check for partial matches in job skills
                foreach (var jobSkill in normalizedJobSkills)
                {
                    if (resumeSkill.Contains(jobSkill) || jobSkill.Contains(resumeSkill))
                    {
                        if (!matchingSkills.Contains(jobSkill))
                        {
                            matchingSkills.Add(jobSkill);
                            missingSkills.Remove(jobSkill);
                        }
                        break;
                    }
                }
            }

            // Third pass: Check for compound skills
            foreach (var jobSkill in normalizedJobSkills)
            {
                if (matchingSkills.Contains(jobSkill))
                    continue;

                foreach (var resumeSkill in normalizedResumeSkills)
                {
                    // Check if resume skill contains job skill as part of a compound skill
                    if (resumeSkill.Contains(jobSkill) && !matchingSkills.Contains(jobSkill))
                    {
                        matchingSkills.Add(jobSkill);
                        missingSkills.Remove(jobSkill);
                        break;
                    }
                }
            }

            // Remove duplicates and sort
            matchingSkills = matchingSkills.Distinct().ToList();
            missingSkills = missingSkills.Distinct().ToList();

            return (matchingSkills, missingSkills);
        }

        public ATSAnalysisService(ILogger<ATSAnalysisService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _httpClient = new HttpClient();
        }

        private int StabilizeScore(string cacheKey, int newScore)
        {
            if (_scoreCache.TryGetValue(cacheKey, out var cached))
            {
                // If cache is still valid
                if ((DateTime.UtcNow - cached.Timestamp).TotalMinutes < CACHE_DURATION_MINUTES)
                {
                    // Only allow variation within MAX_SCORE_VARIATION percentage
                    int minAllowedScore = cached.Score - (cached.Score * MAX_SCORE_VARIATION / 100);
                    int maxAllowedScore = cached.Score + (cached.Score * MAX_SCORE_VARIATION / 100);
                    
                    if (newScore >= minAllowedScore && newScore <= maxAllowedScore)
                    {
                        return newScore;
                    }
                    else
                    {
                        _logger.LogInformation($"Score variation too high: {newScore} vs cached {cached.Score}. Using cached score.");
                        return cached.Score;
                    }
                }
            }

            // Update cache with new score
            _scoreCache[cacheKey] = (newScore, DateTime.UtcNow);
            return newScore;
        }

        public async Task<ATSAnalysisResult> AnalyzeResume(IFormFile resume, string jobDescription)
        {
            try
            {
                _logger.LogInformation("Starting resume analysis");

                if (resume == null)
                {
                    _logger.LogError("Resume file is null");
                    throw new ArgumentNullException(nameof(resume), "Resume file cannot be null");
                }

                // Get the resume file path
                var resumePath = Path.Combine("../MVC/wwwroot/user_resume", resume.FileName);
                if (!System.IO.File.Exists(resumePath))
                {
                    _logger.LogError("Resume file not found at path: {Path}", resumePath);
                    throw new FileNotFoundException("Resume file not found", resumePath);
                }

                // Create a new IFormFile from the file path
                using var fileStream = new FileStream(resumePath, FileMode.Open, FileAccess.Read);
                var formFile = new FormFile(
                    fileStream,
                    0,
                    fileStream.Length,
                    "Resume",
                    Path.GetFileName(resumePath)
                );

                if (formFile.Length == 0)
                {
                    _logger.LogError("Resume file is empty");
                    throw new ArgumentException("Resume file cannot be empty", nameof(resume));
                }

                if (!formFile.ContentType.Equals("application/pdf", StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogError("Invalid file type: {ContentType}", formFile.ContentType);
                    throw new ArgumentException("Only PDF files are supported", nameof(resume));
                }

                if (string.IsNullOrEmpty(_configuration["GoogleAI:ApiKey"]))
                {
                    _logger.LogError("Google AI API key is not configured");
                    throw new InvalidOperationException("Google AI API key is not configured");
                }

                using var stream = formFile.OpenReadStream();
                var resumeText = await ExtractTextFromPdf(stream);

                if (string.IsNullOrWhiteSpace(resumeText))
                {
                    _logger.LogError("No text could be extracted from the resume");
                    throw new Exception("No text could be extracted from the resume. Please ensure the PDF contains text and is not scanned.");
                }

                var requestBody = new
                {
                    contents = new[]
                    {
                        new
                        {
                            parts = new[]
                            {
                                new { text = @"You are an advanced ATS (Applicant Tracking System) analyzer. Your task is to analyze the candidate's resume against the job description and provide a detailed matching analysis.

RESUME ANALYSIS INSTRUCTIONS:
1. Extract all skills, qualifications, and experience from the resume
2. Identify the required skills and qualifications from the job description
3. Compare the candidate's profile with the job requirements
4. Calculate a match score based on the following criteria:
   - Keyword matching (50% of total score)
   - Education, certificates, and courses (30% of total score)
   - Job description alignment (10% of total score)
   - Resume formatting (10% of total score)

SCORING RULES:
- Each matching keyword = +10 points (up to 50 points)
- Each matching education/certificate/course = +5 points (up to 30 points)
- Job description alignment = 0-10 points
- Resume formatting quality = 0-10 points
- Missing required keywords = -10 points each
- Missing required education/certificates = -5 points each
- Final score should be between 0-100

SKILL MATCHING RULES:
- Extract skills from both resume and job description
- Return skills in their original form (do not normalize)
- Include both technical and soft skills
- Skills must be explicitly mentioned in the resume
- Do not infer skills from job titles or descriptions
- Consider variations and synonyms of skills
-Return all matching skills in matching skills , and Missing skills in missing skills .Follow this strictly.
- Return skills in the following format:
  {
    ""resumeSkills"": [""skill1"", ""skill2"", ...],
    ""jobSkills"": [""skill1"", ""skill2"", ...]
  }

Resume:
" + resumeText + @"

Job Description:
" + jobDescription + @"

Please provide your analysis in the following JSON format:
{
    ""matchScore"": <number between 0-100>,
    ""resumeSkills"": [""skill1"", ""skill2"", ...],
    ""jobSkills"": [""skill1"", ""skill2"", ...],
    ""recommendations"": [
        ""Add missing skill: [skill]"",
        ""Highlight matching skill: [skill]"",
        ""Improve skill presentation"",
        ""Add relevant certifications""
    ]
}

IMPORTANT: Return ONLY the JSON object, no additional text or formatting." }
                            }
                        }
                    }
                };

                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("x-goog-api-key", _configuration["GoogleAI:ApiKey"]);

                var response = await httpClient.PostAsJsonAsync(GEMINI_API_URL, requestBody);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Gemini API returned error: {StatusCode} - {ErrorContent}", response.StatusCode, errorContent);
                    throw new Exception($"Gemini API returned error: {response.StatusCode} - {errorContent}");
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("Received response from Gemini API: {ResponseContent}", responseContent);

                var jsonContent = ExtractJsonFromResponse(responseContent);
                var result = JsonSerializer.Deserialize<ATSAnalysisResult>(jsonContent);

                if (result == null)
                {
                    _logger.LogError("Failed to deserialize ATS analysis result");
                    throw new Exception("Failed to parse the analysis result from Gemini API");
                }

                _logger.LogInformation("Successfully analyzed resume with score: {Score}", result.Score);

                // Perform skill matching using our enhanced logic
                var (matchingSkills, missingSkills) = MatchSkills(
                    result.ResumeSkills ?? new List<string>(),
                    result.JobSkills ?? new List<string>()
                );
                
                result.MatchingSkills = matchingSkills;
                result.MissingSkills = missingSkills;
                
                // Stabilize the score
                result.Score = StabilizeScore($"{resume.FileName}_{jobDescription.GetHashCode()}", result.Score);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error analyzing resume");
                throw;
            }
        }

        public async Task<ATSJobMatchResult> MatchJob(IFormFile resume, string jobDescription)
        {
            try
            {
                if (resume == null || resume.Length == 0)
                {
                    _logger.LogError("Resume file is null or empty");
                    throw new ArgumentException("Resume file is required");
                }

                if (string.IsNullOrEmpty(jobDescription))
                {
                    _logger.LogError("Job description is null or empty");
                    throw new ArgumentException("Job description is required");
                }

                _logger.LogInformation("Starting job matching analysis for file: {FileName}", resume.FileName);

                // Extract text from resume
                string resumeText;
                try
                {
                    using (var stream = resume.OpenReadStream())
                    {
                        resumeText = await ExtractTextFromPdf(stream);
                        _logger.LogInformation("Successfully extracted text from PDF. Length: {Length}", resumeText.Length);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error extracting text from PDF");
                    throw new Exception("Failed to extract text from PDF: " + ex.Message);
                }

                var apiKey = _configuration["GoogleAI:ApiKey"];
                if (string.IsNullOrEmpty(apiKey))
                {
                    _logger.LogError("Google AI API key is not configured");
                    throw new Exception("Google AI API key is not configured");
                }

                _logger.LogInformation("Preparing request to Gemini API");

                var requestBody = new
                {
                    contents = new[]
                    {
                        new
                        {
                            parts = new[]
                            {
                                new
                                {
                                    text = @"You are an advanced ATS (Applicant Tracking System) analyzer. Your task is to analyze the candidate's resume against the job description and provide a detailed matching analysis.

RESUME ANALYSIS INSTRUCTIONS:
1. Extract all skills, qualifications, and experience from the resume
2. Identify the required skills and qualifications from the job description
3. Compare the candidate's profile with the job requirements
4. Calculate a match score based on the following criteria:
   - Keyword matching (50% of total score)
   - Education, certificates, and courses (30% of total score)
   - Job description alignment (10% of total score)
   - Resume formatting (10% of total score)

SCORING RULES:
- Each matching keyword = +10 points (up to 50 points)
- Each matching education/certificate/course = +5 points (up to 30 points)
- Job description alignment = 0-10 points
- Resume formatting quality = 0-10 points
- Missing required keywords = -10 points each
- Missing required education/certificates = -5 points each
- Final score should be between 0-100

SKILL MATCHING RULES:
- Exact matches are preferred (case-insensitive)
- Consider variations and synonyms of skills
- Include both technical and soft skills
- Skills must be explicitly mentioned in the resume
- Do not infer skills from job titles or descriptions

Resume:
" + resumeText + @"

Job Description:
" + jobDescription + @"

Please provide your analysis in the following JSON format:
{
    ""matchScore"": <number between 0-100>,
    ""matchingSkills"": [""skill1"", ""skill2"", ...],
    ""missingSkills"": [""skill1"", ""skill2"", ...],
    ""recommendations"": [
        ""Add missing skill: [skill]"",
        ""Highlight matching skill: [skill]"",
        ""Improve skill presentation"",
        ""Add relevant certifications""
    ]
}

IMPORTANT: Return ONLY the JSON object, no additional text or formatting."
                                }
                            }
                        }
                    }
                };

                var jsonRequest = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                _logger.LogInformation("Sending request to Gemini API");
                var response = await _httpClient.PostAsync($"{GEMINI_API_URL}?key={apiKey}", content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Gemini API error: {StatusCode} - {ErrorContent}", response.StatusCode, errorContent);
                    throw new Exception($"Gemini API error: {response.StatusCode} - {errorContent}");
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("Received response from Gemini API. Length: {Length}", responseContent.Length);

                try
                {
                    var responseObject = JsonSerializer.Deserialize<JsonElement>(responseContent);
                    _logger.LogInformation("Successfully deserialized response from Gemini API");

                    // Extract the generated text from the response
                    var candidates = responseObject.GetProperty("candidates");
                    var firstCandidate = candidates[0];
                    var content2 = firstCandidate.GetProperty("content");
                    var parts = content2.GetProperty("parts");
                    var firstPart = parts[0];
                    string rawAnalysis = firstPart.GetProperty("text").GetString() ?? string.Empty;

                    _logger.LogInformation("Extracted raw analysis from response. Length: {Length}", rawAnalysis.Length);

                    // Extract JSON from the response (remove any markdown formatting)
                    string jsonContent = ExtractJsonFromResponse(rawAnalysis);
                    _logger.LogInformation("Extracted JSON content. Length: {Length}", jsonContent.Length);

                    // Parse the JSON response from Gemini
                    var analysisJson = JsonSerializer.Deserialize<JsonElement>(jsonContent);
                    _logger.LogInformation("Successfully parsed JSON content");

                    var result = new ATSJobMatchResult
                    {
                        MatchScore = analysisJson.GetProperty("matchScore").GetInt32(),
                        MatchingSkills = ParseStringArray(analysisJson.GetProperty("matchingSkills")),
                        MissingSkills = ParseStringArray(analysisJson.GetProperty("missingSkills")),
                        Recommendations = ParseStringArray(analysisJson.GetProperty("recommendations"))
                    };

                    _logger.LogInformation("Successfully created ATSJobMatchResult with score: {Score}", result.MatchScore);

                    // Stabilize the score
                    result.MatchScore = StabilizeScore($"{resume.FileName}_{jobDescription.GetHashCode()}", result.MatchScore);

                    return result;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error parsing Gemini API response: {ResponseContent}", responseContent);
                    throw new Exception("Error parsing Gemini API response: " + ex.Message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error matching job");
                throw;
            }
        }

        private List<string> ParseStringArray(JsonElement element)
        {
            var result = new List<string>();
            foreach (var item in element.EnumerateArray())
            {
                result.Add(item.GetString() ?? string.Empty);
            }
            return result;
        }

        private async Task<string> ExtractTextFromPdf(Stream pdfStream)
        {
            try
            {
                _logger.LogInformation("Starting PDF text extraction");

                if (pdfStream == null)
                {
                    _logger.LogError("PDF stream is null");
                    throw new ArgumentNullException(nameof(pdfStream), "PDF stream cannot be null");
                }

                if (!pdfStream.CanRead)
                {
                    _logger.LogError("PDF stream is not readable");
                    throw new InvalidOperationException("PDF stream is not readable");
                }

                using var pdfReader = new PdfReader(pdfStream);
                using var pdfDocument = new PdfDocument(pdfReader);

                _logger.LogInformation("PDF document opened successfully. Number of pages: {PageCount}", pdfDocument.GetNumberOfPages());

                var text = new StringBuilder();
                var pageCount = pdfDocument.GetNumberOfPages();

                for (int i = 1; i <= pageCount; i++)
                {
                    try
                    {
                        var page = pdfDocument.GetPage(i);
                        var strategy = new SimpleTextExtractionStrategy();
                        var pageText = PdfTextExtractor.GetTextFromPage(page, strategy);

                        if (string.IsNullOrWhiteSpace(pageText))
                        {
                            _logger.LogWarning("Page {PageNumber} extracted text is empty", i);
                        }
                        else
                        {
                            _logger.LogInformation("Successfully extracted text from page {PageNumber}. Text length: {TextLength}", i, pageText.Length);
                            text.Append(pageText);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error extracting text from page {PageNumber}", i);
                        throw new Exception($"Error extracting text from page {i}: {ex.Message}");
                    }
                }

                var extractedText = text.ToString();
                if (string.IsNullOrWhiteSpace(extractedText))
                {
                    _logger.LogError("No text was extracted from the PDF");
                    throw new Exception("No text could be extracted from the PDF. The file might be scanned or image-based.");
                }

                _logger.LogInformation("Successfully extracted text from PDF. Total text length: {TextLength}", extractedText.Length);
                return extractedText;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error extracting text from PDF");
                throw new Exception($"Failed to extract text from PDF: {ex.Message}");
            }
        }

        private string ExtractJsonFromResponse(string response)
        {
            try
            {
                _logger.LogInformation("Extracting JSON from response: {Response}", response);

                // First try to find JSON between triple backticks
                var backtickMatch = Regex.Match(response, @"```(?:json)?\s*(\{[\s\S]*?\})\s*```");
                if (backtickMatch.Success)
                {
                    var jsonContent = backtickMatch.Groups[1].Value;
                    _logger.LogInformation("Found JSON between backticks: {JsonContent}", jsonContent);
                    return jsonContent;
                }

                // If no backticks, try to find JSON between curly braces
                var braceMatch = Regex.Match(response, @"(\{[\s\S]*?\})");
                if (braceMatch.Success)
                {
                    var jsonContent = braceMatch.Groups[1].Value;
                    _logger.LogInformation("Found JSON between braces: {JsonContent}", jsonContent);
                    return jsonContent;
                }

                // If still no match, try to find any content that looks like JSON
                var anyMatch = Regex.Match(response, @"(\{.*\})", RegexOptions.Singleline);
                if (anyMatch.Success)
                {
                    var jsonContent = anyMatch.Groups[1].Value;
                    _logger.LogInformation("Found potential JSON content: {JsonContent}", jsonContent);
                    return jsonContent;
                }

                _logger.LogError("Could not extract JSON from response");
                throw new Exception("Could not extract JSON from Gemini API response");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error extracting JSON from response");
                throw;
            }
        }
    }
}