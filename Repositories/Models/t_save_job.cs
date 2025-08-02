using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Repositories.Models
{
    public class t_save_job
    {
        public int UserId { get; set; }
        public int JobPostId { get; set; }
        public string c_job_title { get; set; }

        public string c_job_desc { get; set; }

        public string? c_post_date { get; set; }

        public string c_job_location { get; set; }

        public string c_job_type { get; set; }

        public int c_job_experience { get; set; }

        public string c_salary_range { get; set; }

        public int c_vacancy { get; set; }

        public string c_dept_name { get; set; }

        public string c_qualification_title { get; set; }

        public string c_skills { get; set; }
        public string c_company_name { get; set; }
        public string? c_company_logo { get; set; }  
        
        // public IFormFile? CompanyLogo { get; set; } 

    }
}