using System.Data.Common;
using Npgsql;
using Repositories.Model;

public class JobPostReportRepository : IJobReportInterface
{
    private readonly NpgsqlConnection _conn;
    public JobPostReportRepository(NpgsqlConnection conn){
        _conn = conn;
    }

    public async Task<List<t_Job_Report>> GetJobPostReport()
    {
        List<t_Job_Report> tt = new List<t_Job_Report>();
        try
        {
            await _conn.OpenAsync();
            string qry = @"select jr.c_report_topic,jr.c_report_desc,jp.c_job_title,u.c_image,u.c_fullname,cp.c_company_name,cp.c_company_logo
            from t_job_report jr
            inner join t_user u
            on jr.c_user_id = u.c_uid
            inner join t_job_post jp
            on jp.c_job_id = jr.c_job_id
            inner join t_companies cp
            on jr.c_company_id=cp.c_company_id;";
            var cmd = new NpgsqlCommand(qry,_conn);
            var dr = await cmd.ExecuteReaderAsync();
            if(dr.HasRows){
                while (await dr.ReadAsync())
                {
                  tt.Add(new t_Job_Report{
                    c_report_topic = dr["c_report_topic"].ToString(),
                    c_report_desc = dr["c_report_desc"].ToString(),
                    job_Post = new Job_Post{
                        c_job_title = dr["c_job_title"].ToString(),
                    },
                    company = new t_Company{
                        c_company_logo = dr["c_company_logo"].ToString(),
                        c_company_name = dr["c_company_name"].ToString(),
                    },
                    user = new t_user{
                        c_fullName = dr["c_fullName"].ToString(),
                        c_profileImage = dr["c_image"].ToString()
                    }
                  });     
                }
            }
        }
        catch (System.Exception ex)
        {
            System.Console.WriteLine("Error while getting job post report:"+ex.Message);
        }
        finally{
            await _conn.CloseAsync();
        }
        return tt;
    }

    public async Task<int> SaveJobPostReport(t_Job_Report job_Report)
    {
        try
        {
            await _conn.OpenAsync();
            string qry = "insert into t_job_report (c_user_id,c_job_id,c_company_id,c_report_topic,c_report_desc) values (@c_user_id,@c_job_id,@c_company_id,@c_report_topic,@c_report_desc)";
            var cmd = new NpgsqlCommand(qry,_conn);
            cmd.Parameters.AddWithValue("@c_user_id",job_Report.c_user_id);
            cmd.Parameters.AddWithValue("@c_job_id",job_Report.c_job_id);
            cmd.Parameters.AddWithValue("@c_company_id",job_Report.c_company_id);
            cmd.Parameters.AddWithValue("@c_report_topic",job_Report.c_report_topic);
            cmd.Parameters.AddWithValue("@c_report_desc",job_Report.c_report_desc);
            await cmd.ExecuteNonQueryAsync();
            return 1;
        }
        catch (System.Exception ex)
        {
            System.Console.WriteLine("Error while save job post report:"+ex.Message);
            return 0;
        }
        finally{
            await _conn.CloseAsync();
        }
    }
}