using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Repositories.Interfaces;
using Repositories.Models;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CompanyApiController : ControllerBase
    {
        private readonly ICompanyInterface _companyRepository;
        private readonly ILogger<CompanyApiController> _logger;
        private readonly CloudinaryService _cloudinaryService;
        private readonly IConfiguration _configuration;

        public CompanyApiController(ICompanyInterface companyRepository,
                                    ILogger<CompanyApiController> logger,
                                    CloudinaryService cloudinaryService,
                                    IConfiguration configuration)
        {
            _companyRepository = companyRepository ?? throw new ArgumentNullException(nameof(companyRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _cloudinaryService = cloudinaryService ?? throw new ArgumentNullException(nameof(cloudinaryService));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        [HttpGet("getCompanyName/{id}")]
        public async Task<IActionResult> GetCompanyName(int id)
        {
            var company = await _companyRepository.GetCompanyName(id);
            return Ok(company);
        }

        [HttpGet("getCompany")]
        public async Task<IActionResult> GetCompany(int id)
        {
            try
            {
                var company = await _companyRepository.GetCompany(id);
                if (company == null) return NotFound("Company Not Found");
                return Ok(company);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching Company Details: {ex.Message}");
                return StatusCode(500, "Error retrieving Company details.");
            }
        }

        [HttpGet("getCompanyId/{id}")]
        public Task<int> GetCompanyId(int id)
        {
            var res = _companyRepository.GetCompanyId(id);
            return res;
        }

        /// <summary>
        /// Registers a new company.
        /// </summary>
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromForm] t_companies company, IFormFile companyLogo)
        {
            try
            {
                if (company == null) return BadRequest("Invalid company data.");
                if (company.LegalDocuments == null || company.LegalDocuments.Count == 0)
                    return BadRequest("Legal documents are required.");

                var uploadedFiles = await UploadLegalDocuments(company.LegalDocuments);
                if (uploadedFiles == null) return StatusCode(500, "Failed to upload legal documents.");

                company.c_legal_documents = uploadedFiles.ToArray();
                company.LegalDocuments = null; // Prevent sending in response

                if (companyLogo != null)
                {
                    string logoUrl = await UploadCompanyLogo(companyLogo);
                    if (logoUrl == null) return BadRequest("Only PNG and JPG formats are allowed for the logo.");
                    company.c_company_logo = logoUrl;
                }

                int result = await _companyRepository.RegisterCompany(company);

                return result switch
                {
                    0 => Ok(new { success = false, message = "Company is already registered" }),
                    1 => Ok(new { success = true, message = "Company registered successfully" }),
                    _ => BadRequest(new { success = false, message = "Something went wrong" })
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error registering company: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        //Registration ke liye
        private async Task<List<string>> UploadLegalDocuments(List<IFormFile> legalDocuments)
        {
            var uploadedFiles = new List<string>();

            foreach (var file in legalDocuments)
            {
                using var stream = file.OpenReadStream();
                string fileUrl = await _cloudinaryService.UploadImageAsync(stream, file.FileName, "Documents");
                if (string.IsNullOrEmpty(fileUrl)) return null;
                uploadedFiles.Add(fileUrl);
            }

            return uploadedFiles;
        }
        //Registration ke liye
        private async Task<string> UploadCompanyLogo(IFormFile companyLogo)
        {
            string logoExtension = Path.GetExtension(companyLogo.FileName).ToLower();
            if (logoExtension != ".png" && logoExtension != ".jpg" && logoExtension != ".jpeg")
                return null;

            using var stream = companyLogo.OpenReadStream();
            return await _cloudinaryService.UploadImageAsync(stream, companyLogo.FileName, "Logos");
        }
        [HttpGet("getRecruiter")]
        public async Task<IActionResult> GetRecruiter(int id)
        {
            try
            {
                var result = await _companyRepository.GetRecruiterById(id);
                if (result == null) return NotFound("Recruiter not found.");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching recruiter: {ex.Message}");
                return StatusCode(500, "Error retrieving recruiter details.");
            }
        }

        
        [HttpPut("UpdateCompanyDetails")]
        public async Task<IActionResult> Update([FromForm] vm_UpdateCompany company)
        {
            try
            {
                // Validate input model
                if (company == null)
                {
                    return BadRequest(new { success = false, message = "Invalid company data." });
                }

                // Validate required fields
                if (string.IsNullOrEmpty(company.c_company_email) ||
                    string.IsNullOrEmpty(company.c_company_phone) ||
                    string.IsNullOrEmpty(company.c_company_address) ||
                    string.IsNullOrEmpty(company.c_website))
                {
                    return BadRequest(new { success = false, message = "All company fields are required." });
                }

                // Handle legal documents (PDFs)
                var allDocuments = company.c_legal_documents?.ToList() ?? new List<string>();

                if (company.LegalDocuments != null && company.LegalDocuments.Any())
                {
                    // Validate total documents (existing + new)
                    if (allDocuments.Count + company.LegalDocuments.Count > 5)
                    {
                        return BadRequest(new { success = false, message = "Maximum 5 legal documents allowed." });
                    }

                    // Validate file types (PDFs only)
                    foreach (var file in company.LegalDocuments)
                    {
                        if (file.ContentType != "application/pdf")
                        {
                            return BadRequest(new { success = false, message = "Only PDF files are allowed for legal documents." });
                        }
                    }

                    // Upload new PDFs and append to existing documents
                    var uploadedFiles = await UploadLegalDocuments(company.LegalDocuments, company.c_owner_id);
                    if (uploadedFiles == null || !uploadedFiles.Any())
                    {
                        return StatusCode(500, new { success = false, message = "Failed to upload legal documents." });
                    }
                    allDocuments.AddRange(uploadedFiles);
                }

                // Deduplicate and set c_legal_documents
                company.c_legal_documents = allDocuments
                    .Where(doc => !string.IsNullOrEmpty(doc))
                    .Distinct()
                    .ToArray();

                // Handle company logo (PNG/JPG)
                if (company.CompanyLogo != null)
                {
                    // Validate logo file type
                    if (!new[] { "image/png", "image/jpeg" }.Contains(company.CompanyLogo.ContentType))
                    {
                        return BadRequest(new { success = false, message = "Only PNG and JPG formats are allowed for the logo." });
                    }


                    // Upload logo
                    string logoUrl = await UploadCompanyLogo(company.CompanyLogo, company.c_owner_id);
                    if (string.IsNullOrEmpty(logoUrl))
                    {
                        return StatusCode(500, new { success = false, message = "Failed to upload company logo." });
                    }
                    company.c_company_logo = logoUrl;
                }

                // Update company in repository
                int result = await _companyRepository.UpdateCompany(company);

                return result switch
                {
                    1 => Ok(new { success = true, message = "Company updated successfully" }),
                    _ => Ok(new { success = false, message = "Company not updated" })
                };
            }
            catch (ArgumentException ex)
            {
                _logger.LogError($"Validation error: {ex.Message}");
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError($"Upload error: {ex.Message}");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating company: {ex.Message}");
                return StatusCode(500, new { success = false, message = "An error occurred while processing your request." });
            }
        }

        //Update ke liye 

        private async Task<List<string>> UploadLegalDocuments(List<IFormFile> legalDocuments, int id)
        {
            var uploadedFiles = new List<string>();
            Console.WriteLine($"Id :{id}");
            t_companies company = await _companyRepository.GetCompany(id);
            if (company == null)
            {
                throw new ArgumentException("Company not found for the provided ID.");
            }

            if (company.c_legal_documents != null && company.c_legal_documents.Length > 0)
            {
                foreach (var oldUrl in company.c_legal_documents)
                {
                    await _cloudinaryService.DeleteImageAsync(oldUrl);
                }
            }

            foreach (var file in legalDocuments)
            {
                using var stream = file.OpenReadStream();
                string fileUrl = await _cloudinaryService.UploadImageAsync(stream, file.FileName, "Documents");
                if (string.IsNullOrEmpty(fileUrl))
                {
                    return null;
                }
                uploadedFiles.Add(fileUrl);
            }

            return uploadedFiles;
        }

        //Update ke liye 
        private async Task<string> UploadCompanyLogo(IFormFile companyLogo, int id)
        {
            t_companies company = await _companyRepository.GetCompany(id);
            if (company == null)
            {
                throw new ArgumentException("Company not found for the provided ID.");
            }


            //data lega logo ka
            var existingFileUrl = company.c_company_logo;

            //for delete and update
            if (!string.IsNullOrEmpty(existingFileUrl))
            {
                await _cloudinaryService.DeleteImageAsync(existingFileUrl);
            }
            using var stream = companyLogo.OpenReadStream();

            return await _cloudinaryService.UploadImageAsync(stream, companyLogo.FileName, "Logos");
        }

        [HttpGet("getMandatoryFields")]
        public async Task<List<t_companies>> GetMandatoryFields()
        {
            var mandatoryFields = await _companyRepository.GetMandatoryFields();
            return mandatoryFields;
        }

        [HttpGet("getCompanyStatus/{id}")]
        public async Task<IActionResult> GetCompanyStatus(int id)
        {
            var result = await _companyRepository.GetCompanyStatus(id);
            return Ok(result);
        }
    }
}