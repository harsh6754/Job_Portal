using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repositories.Models
{
    public class CandidateJobApplied
    {
        public int c_jobId { get; set; }

        public int c_applicationId { get; set; }

        public string c_userName {get; set;}

        public int c_userId { get; set; }

       
        public string c_apply_at { get; set; } 
        public string c_status { get; set; }

        public string c_email {get; set;}

        public string c_mobile {get; set;}
    }
}