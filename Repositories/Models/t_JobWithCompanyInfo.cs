using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repositories.Models
{
    public class t_JobWithCompanyInfo
    {
    public int c_company_id { get; set; }
    public string c_company_name { get; set; }
    public int c_job_id { get; set; }
    public string c_job_title { get; set; }
    public string c_job_description { get; set; }
    public string c_job_location { get; set; }
    public string c_salary { get; set; }
    public int c_vacant_seats { get; set; }
    public DateTime c_created_at { get; set; }

    public DateTime? c_expire_date { get; set; }
    }
}