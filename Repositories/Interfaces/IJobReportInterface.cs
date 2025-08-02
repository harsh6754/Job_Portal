using Repositories.Model;
public interface IJobReportInterface
{
    Task<int> SaveJobPostReport(t_Job_Report job_Report);

    Task<List<t_Job_Report>> GetJobPostReport();
}