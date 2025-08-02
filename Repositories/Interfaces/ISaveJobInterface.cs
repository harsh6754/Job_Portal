using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Repositories.Models;

namespace Repositories.Interfaces
{
    public interface ISaveJobInterface
    {
        bool Add(int userId, int jobPostId);
        void Remove(int userId, int jobPostId);
        List<t_save_job> GetSavedJobs(int userId);
    }
}