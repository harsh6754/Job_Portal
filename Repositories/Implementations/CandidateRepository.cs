using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Repositories.Interfaces;
using Npgsql;
using Repositories.Models;


namespace Repositories.Implementations
{
    public class CandidateRepository :  ICandidateInterface
    {
        private readonly NpgsqlConnection _conn;

        public CandidateRepository(NpgsqlConnection connection)
        {
            _conn = connection;
        }
        
        public async Task<List<Job_Post1>> GetJobs()
        {
            List<Job_Post1> jobs = new List<Job_Post1>();

            try
            {
                await _conn.OpenAsync();
                var query = "SELECT jp.c_job_id, jp.c_job_title, jp.c_job_desc, jp.c_job_location, jp.c_job_type, jp.c_job_experience, jp.c_salary_range,jp.c_vacancy, jp.c_dept_id, jp.c_qualification_title, jp.c_skills, jp.c_company_id,cp.c_company_name,dp.c_dept_name,cp.c_company_logo FROM t_job_post jp join t_companies cp on jp.c_company_id = cp.c_company_id join t_department dp on jp.c_dept_id = dp.c_dept_id";

                using (var cmd = new NpgsqlCommand(query, _conn))
                {
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            jobs.Add(new Job_Post1
                            {
                                c_job_id = reader.IsDBNull(0) ? (int?)null : reader.GetInt32(0),
                                c_job_title = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                                c_job_desc = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                                c_job_location = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                                c_job_type = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                                c_job_experience = reader.IsDBNull(5) ? 0 : reader.GetInt32(5),
                                c_salary_range = reader.IsDBNull(6) ? string.Empty : reader.GetString(6),
                                c_vacancy = reader.IsDBNull(7) ? 0 : reader.GetInt32(7),
                                c_dept_id = reader.IsDBNull(8) ? 0 : reader.GetInt32(8),
                                c_qualification_title = reader.IsDBNull(9) ? string.Empty : reader.GetString(9),
                                c_skills = reader.IsDBNull(10) ? string.Empty : reader.GetString(10),
                                c_company_id = reader.IsDBNull(11) ? 0 : reader.GetInt32(11),
                                c_company_name  = reader.IsDBNull(12) ? string.Empty : reader.GetString(12),
                                c_dept_name = reader.IsDBNull(13) ? string.Empty : reader.GetString(13),
                                c_company_logo = reader.IsDBNull(14) ? string.Empty : reader.GetString(14)
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                await _conn.CloseAsync();
            }

            return jobs;
        }



        
       public async Task<List<t_CompanyRecruiterInfo>> GetCompaniesWithRecruiters()
{
    List<t_CompanyRecruiterInfo> companiesWithRecruiters = new List<t_CompanyRecruiterInfo>();
    try
    {
       await _conn.OpenAsync();
var query = @"
    SELECT 
        c.c_company_id,
        c.c_company_name,
        c.c_company_phone,
        c.c_industry,
        i.industry_name,              -- <-- industry name from t_industry
        c.c_owner_id,
        c.c_verified_status,
        c.c_company_logo,
        c.c_reg_date,
        u.c_fullname,
        c.c_company_email
    FROM 
        t_companies c
    JOIN 
        t_user u ON c.c_owner_id = u.c_uid
    JOIN 
        industries i ON c.c_industry = i.id::varchar -- <-- join with t_industry
    WHERE 
        c.c_verified_status = true 
        AND c.c_company_logo IS NOT NULL 
        AND c.c_owner_id IS NOT NULL";


        using (var cmd = new NpgsqlCommand(query, _conn))
        {
            using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    companiesWithRecruiters.Add(new t_CompanyRecruiterInfo
                    {
                        c_id = reader.GetInt32(0),
                        c_company_name = reader.GetString(1),
                        c_contact_number = reader.GetString(2),
                        c_industry_type = reader.GetString(3),
                        c_dept_name = reader.GetString(4),
                        c_owner_id = reader.GetInt32(5),
                        c_is_approved = reader.IsDBNull(6) ? false : reader.GetBoolean(6),
                        c_logo_url = reader.IsDBNull(7) ? null : reader.GetString(7),
                        c_created_at = reader.GetDateTime(8),
                        c_recruiter_name = reader.GetString(9),
                        c_recruiter_email = reader.IsDBNull(10) ? null : reader.GetString(10)
                    });
                }
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
    finally
    {
        await _conn.CloseAsync();
    }
    return companiesWithRecruiters;
}

    }
}