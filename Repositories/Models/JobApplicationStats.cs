using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repositories.Models
{
    public class JobApplicationStats
    {
        public DateTime Date { get; set; }
    public int Applications { get; set; }
    public string Status { get; set; }  
    }
}