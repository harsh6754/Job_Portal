using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repositories.Models
{
    public class t_CompanyRecruiterInfo
    {
    public int c_id { get; set; }
    public string c_company_name { get; set; }
   
    public string c_contact_number { get; set; }
    public string c_industry_type { get; set; }


    public string c_dept_name {get; set;}
    public int c_owner_id { get; set; }   
    public bool c_is_approved { get; set; }
    public string c_logo_url { get; set; }
    public DateTime c_created_at { get; set; }

    public string c_recruiter_name { get; set; }

    public string c_recruiter_email { get; set; }

    }
}