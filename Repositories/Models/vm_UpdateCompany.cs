using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;

namespace Repositories.Models
{
    public class vm_UpdateCompany
    {
        public int c_company_id { get; set; }
        public int c_owner_id { get; set; }
        public string? c_company_email { get; set; }
        public string? c_company_phone { get; set; }
        public string? c_company_address { get; set; }
        public string? c_website { get; set; }
        public string[]? c_legal_documents { get; set; }  // Array of Legal Document URLs
        public string? c_company_logo { get; set; }  // Company Logo URL (Stored in DB)

        [NotMapped]
        [JsonIgnore]
        public List<IFormFile>? LegalDocuments { get; set; }  // For Uploading Legal Docs

        [NotMapped]
        [JsonIgnore]
        public IFormFile? CompanyLogo { get; set; }  // For Uploading Company Logo
    }
}