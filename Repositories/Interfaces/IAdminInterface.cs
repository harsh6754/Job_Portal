using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Repositories.Model;
using Repositories.Models;

namespace Repositories.Interfaces
{
    public interface IAdminInterface
    {

        public Task<List<t_user>> GetUsers();


        public Task<bool> DeleteUser(int id);

        public Task<int> GetUsersCount();

        public Task<int> GetJobPostCount();

        public Task<int> GetApplicationsCount();

        public Task<int> GetRecruitersCount();

        public Task<List<t_recruiter>> GetRecruiters();

        Task<t_recruiter> GetRecruiterByCompanyId(int companyId);

        Task<bool> UpdateRecruiterStatus(int companyId, bool approved = true);

        Task<bool> UpdateRecruiterstatusReject(int companyId, string reason, bool approved = false);

        Task<bool> BulkUpdateRecruiterStatus(List<int> companyIds, bool approved = true);

        Task<bool> BulkUpdateRecruiterStatusReject(List<int> companyIds, string reason, bool approved = false);
        Task<bool> DeleteRecruiter(int companyId);
        Task<List<RegistrationStats>> GetRegistrationStats(DateTime startDate, DateTime endDate);
        Task<List<dynamic>> GetUserDistributionData();


        Task<List<t_recruiter>> PendingApproval();
        public Task<List<t_job>> GetJobs();
        


        Task<List<t_CompanyRecruiterInfo>> GetCompaniesWithRecruiters();

        Task<List<t_JobWithCompanyInfo>> GetJobsWithCompanyInfo(int companyId);

        Task<List<CandidateJobApplied>> GetAppliedJobs(int userId);



        void BlockOrUnblockUser(int userId);

        Task<List<ApplicationCountByDate>> GetApplicationStats(DateTime startDate, DateTime endDate);


        Task<IEnumerable<JobApplicationStats>> GetJobApplicationStatsAsync(DateTime startDate, DateTime endDate, string status = null);

        public Task<int> GetNotificationCount();

        public Task<int> MarkAllNotificationsAsRead();

        public Task<List<Notification>> GetUnreadNotifications();

        public Task<List<Notification>> GetAllNotifications();

        public Task<bool> DeleteMultipleNotifications(List<int> notificationIds);
    }
}