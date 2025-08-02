using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Repositories.Implementations;
using Repositories.Models;
namespace Repositories.Interfaces
{
    public interface IJobRecommendationInterface
    {
        Task<List<Job_Post1>> RecommendJobs(t_JobPreference jobPreference);
    }
}