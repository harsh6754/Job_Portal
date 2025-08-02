using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repositories.Models
{
    public class PasswordCheck
    {
        public int userId { get; set; }
        public string OldPassword { get; set; }

    }
}