using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repositories.Interfaces
{
    public interface IDashboardInterface
    {
        Task<object> GetCounts(int companyid);

        Task<List<object>> GetInterviewTrends(DateTime startDate, DateTime endDate, int companyid);

        Task<List<object>> GetApplicationTrends(DateTime startDate, DateTime endDate, int companyid);
    }
}