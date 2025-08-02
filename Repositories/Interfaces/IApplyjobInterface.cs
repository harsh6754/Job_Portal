using Repositories.Model;

public interface IApplyjobInterface
{
     Task<FieldCheckResult?> CheckFields(int id);

    Task<int> ApplyJob(t_apply_job apply_Job);

    // Task<int> DeleteJob(int id);

    Task<List<t_apply_job>> GetApplyJobApplication(int id,string job_title,string status);
    Task<List<t_interview_schedule>> GetInterviewDoneCandidates(int id);

    Task<int> UpdateStatusOfJobApplication(t_apply_job apply_Job);

    Task<List<t_apply_job>> GetUserAppliedJobApplication(int id);

    Task<List<Job_Post>> GetJobTitles(int id);

    //Interview Schedule starts from here
    Task<int> InterviewSchedule(t_interview_schedule interview_Schedule);

    Task<List<t_interview_schedule>> GetInterviews_ScheduleByCompany(int id);

    Task<int> UpdateInterviewSchedule(t_interview_schedule interview_Schedule);

    Task<int> UpdateInterviewScheduleStatus(t_interview_schedule interview_Schedule);


    //to check if the user has applied for the job or not
    Task<int> UserAppliedStatus(int userid,int jobid);

    //Hire candidate
    Task<int> HireCandidate(t_hired_candidate hired_Candidate);

    Task<List<t_hired_candidate>> GetHiredCandidates(int id);

    /// <summary>
    /// Gets the count of unread notifications for a specific company.
    /// </summary>
    Task<int> GetNotificationCount(int companyId);

    /// <summary>
    /// Marks all unread notifications as read for a specific company.
    /// </summary>
    Task<int> MarkAllNotificationsAsRead(int companyId);

    /// <summary>
    /// Gets a list of unread notifications for a specific company.
    /// </summary>
    Task<List<Notiy>> GetUnreadNotifications(int companyId);

    /// <summary>
    /// Gets a list of all notifications for a specific company.
    /// </summary>
    Task<List<Notiy>> GetAllNotifications(int companyId);

    /// <summary>
    /// Deletes multiple notifications by their IDs.
    /// </summary>
    Task<bool> DeleteMultipleNotifications(List<int> notificationIds);

}