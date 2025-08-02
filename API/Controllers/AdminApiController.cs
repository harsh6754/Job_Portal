using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MailKit;
using Microsoft.AspNetCore.Mvc;
using Repositories.Interfaces;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminApiController : ControllerBase
    {

        private readonly IAdminInterface _adminRepository;

        public AdminApiController(IAdminInterface adminRepository)
        {
            _adminRepository = adminRepository;
            
        }

        

        [HttpGet("get-notification-count")]
        public async Task<IActionResult> GetNotificationCount()
        {
            var count = await _adminRepository.GetNotificationCount();
            return Ok(new { totalNotifications = count });
        }

        [HttpGet("all-notifications")]
        public async Task<IActionResult> GetAllNotifications()
        {
            var notifications = await _adminRepository.GetAllNotifications();
            return Ok(notifications);
        }

        [HttpDelete("delete-notifications")]
        public async Task<IActionResult> DeleteMultipleNotifications([FromBody] List<int> notificationIds)
        {
            if (notificationIds == null || notificationIds.Count == 0)
            {
                return BadRequest(new { message = "No notification IDs provided" });
            }

            var result = await _adminRepository.DeleteMultipleNotifications(notificationIds);
            if (result)
            {
                return Ok(new { message = "Notifications deleted successfully" });
            }
            return StatusCode(500, new { message = "Failed to delete notifications" });
        }

        [HttpPut("mark-all-notifications-as-read")]
        public async Task<IActionResult> MarkAllNotificationsAsRead()
        {
            var result = await _adminRepository.MarkAllNotificationsAsRead();
            if (result > 0)
                return Ok(new { message = "All notifications marked as read." });
            return NotFound(new { message = "No notifications found." });
        }

        [HttpGet("unread-notifications")]
        public async Task<IActionResult> GetUnreadNotifications()
        {
            var notifications = await _adminRepository.GetUnreadNotifications();

            if (notifications == null || notifications.Count == 0)
                return NotFound(new { message = "No unread notifications found." });

            return Ok(notifications);
        }

        // [HttpPost("send-email")]
        // public IActionResult SendEmail([FromBody] EmailRequest request)
        // {
        //     _emailService.SendEmail(request.ToEmail, request.UserName, request.Link);
        //     return Ok("Email sent successfully.");
        // }

        [HttpGet("get-user")]
        public async Task<IActionResult> GetUser()
        {
            var user = await _adminRepository.GetUsers();
            return Ok(user);
        }

        [HttpDelete("delete-user/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var result = await _adminRepository.DeleteUser(id);
            return Ok(result);
        }
        [HttpGet("get-user-count")]
        public async Task<IActionResult> GetUserCount()
        {
            var count = await _adminRepository.GetUsersCount();
            return Ok(new { totalUsers = count });
        }


        [HttpGet("get-recruiter-count")]
        public async Task<IActionResult> GetRecruiterCount()
        {
            var count = await _adminRepository.GetRecruitersCount();
            return Ok(new { totalUsers = count });
        }

        [HttpGet("getrecruiter")]
        public async Task<IActionResult> GetRecruiter()
        {
            var recruiter = await _adminRepository.GetRecruiters();
            return Ok(recruiter);
        }

        [HttpGet("getrecruiter/{companyId}")]
        public async Task<IActionResult> GetRecruiterById(int companyId)
        {
            try
            {
                var recruiter = await _adminRepository.GetRecruiterByCompanyId(companyId);
                
                if (recruiter == null)
                {
                    return NotFound(new { message = "Recruiter not found" });
                }
                
                return Ok(recruiter);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching recruiter details", error = ex.Message });
            }
        }

        [HttpPut("updaterecruiterstatus/{companyId}")]
        public async Task<IActionResult> UpdateRecruiterStatus(int companyId, bool approved)
        {
            var result = await _adminRepository.UpdateRecruiterStatus(companyId, approved);
            if (result)
                return Ok(new { message = "Recruiter status updated successfully" });
            return NotFound(new { message = "Recruiter not found" });
        }
        [HttpPut("updaterecruiterstatusreject/{companyId}")]
        public async Task<IActionResult> UpdateRecruiterStatusReject(int companyId,string reason, bool approved)
        {
            var result = await _adminRepository.UpdateRecruiterstatusReject(companyId,reason, approved);
            if (result)
                return Ok(new { message = "Recruiter status updated successfully" });
            return NotFound(new { message = "Recruiter not found" });
        }
        [HttpPut("bulkupdaterecruiterstatus")]
        public async Task<IActionResult> BulkUpdateRecruiterStatus(List<int> companyIds, bool approved)
        {
            var result = await _adminRepository.BulkUpdateRecruiterStatus(companyIds, approved);
            if (result)
                return Ok(new { message = "Recruiter status updated successfully" });
            return NotFound(new { message = "Recruiter not found" });
        }

       

        [HttpPut("bulkupdaterecruiterstatusreject")]
        public async Task<IActionResult> BulkUpdateRecruiterStatusReject(List<int> companyIds, string reason,bool approved)
        {
            var result = await _adminRepository.BulkUpdateRecruiterStatusReject(companyIds,reason, approved);
            if (result)
                return Ok(new { message = "Recruiter status updated successfully" });
            return NotFound(new { message = "Recruiter not found" });
        }

        [HttpDelete("deleterecruiter/{companyId}")]
        public async Task<IActionResult> DeleteRecruiter(int companyId)
        {
            var result = await _adminRepository.DeleteRecruiter(companyId);
            return Ok(result);
        }

        [HttpGet("pendingapproval")]
        public async Task<IActionResult> PendingApproval()
        {
            var pending = await _adminRepository.PendingApproval();
            return Ok(pending);
        }

        // [HttpGet("get-jobpost-count")]
        // public async Task<IActionResult> GetJobPostCount()
        // {
        //     var count = await _adminRepository.GetJobPostCount();
        //     return Ok(new { totalJobs = count });
        // }

        [HttpGet("get-registration-stats")]
        public async Task<IActionResult> GetRegistrationStats([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var stats = await _adminRepository.GetRegistrationStats(startDate, endDate);
            return Ok(stats);
        }

        [HttpGet("get-user-distribution")]
        public async Task<IActionResult> GetUserDistribution()
        {
            var distribution = await _adminRepository.GetUserDistributionData();
            return Ok(distribution);
        }


        

        

        [HttpGet("getjob")]
        public async Task<IActionResult> GetJobs()
        {
            var job = await _adminRepository.GetJobs();
            return Ok(job);
        }
        public class EmailRequest
        {
            public string ToEmail { get; set; }
            public string UserName { get; set; }
            public string Link { get; set; }
        }
            
    [HttpGet("companies-with-recruiters")]
    public async Task<IActionResult> GetCompaniesWithRecruiters()
    {
        try
        {
            var data = await _adminRepository.GetCompaniesWithRecruiters();
            return Ok(data);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Something went wrong", error = ex.Message });
        }
    }
    
     [HttpGet("jobs-with-company-info/{companyId}")]
    public async Task<IActionResult> GetJobsWithCompanyInfo(int companyId)
    {
        try
        {
            var data = await _adminRepository.GetJobsWithCompanyInfo(companyId);
            return Ok(data);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Something went wrong.", error = ex.Message });
        }
    }
    

    [HttpGet("applied-jobs/{userId}")]
public async Task<IActionResult> GetAppliedJobs(int userId)
{
    try
    {
        var appliedJobs = await _adminRepository.GetAppliedJobs(userId);
        if (appliedJobs == null || !appliedJobs.Any())
        {
            return NotFound(new { message = "No applied jobs found for this user." });
        }
        return Ok(appliedJobs);
    }
    catch (Exception ex)
    {
        return StatusCode(500, new { message = "An error occurred while fetching applied jobs.", error = ex.Message });
    }
}


 [HttpPost("block-unblock/{userId}")]
        public IActionResult BlockOrUnblockUser(int userId)
        {
            try
            {
                _adminRepository.BlockOrUnblockUser(userId);
                return Ok(new { message = "User block status toggled successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("application-stats")]
    public async Task<IActionResult> GetApplicationStats([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        try
        {
            var stats = await _adminRepository.GetApplicationStats(startDate, endDate);
            return Ok(stats);
        }
        catch (Exception ex)
        {
            // Optional: log the error
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

     [HttpGet("application-stats-status")]
    public async Task<IActionResult> GetJobApplicationStatsAsync([FromQuery] DateTime startDate, [FromQuery] DateTime endDate,string status)
    {
        try
        {
            var stats = await _adminRepository.GetJobApplicationStatsAsync(startDate, endDate, status);
            return Ok(stats);
        }
        catch (Exception ex)
        {
            // Optional: log the error
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpGet("get-jobpost-count")]
        public async Task<IActionResult> GetJobPostCount()
        {
            var count = await _adminRepository.GetJobPostCount();
            return Ok(new { totalJobs = count });
        }

        [HttpGet("get-application-count")]
        public async Task<IActionResult> GetApplicationtCount()
        {
            var count = await _adminRepository.GetApplicationsCount();
            return Ok(new { totalJobs = count });
        }
}
}

