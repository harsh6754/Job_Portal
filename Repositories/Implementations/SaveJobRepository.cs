using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Repositories.Interfaces;
using Npgsql;
using Repositories.Models;

namespace Repositories.Implementations
{
    public class SaveJobRepository : ISaveJobInterface
    {

        private readonly NpgsqlConnection conn;

        public SaveJobRepository(NpgsqlConnection connection)
        {
            conn = connection;
        }


        public bool Add(int userId, int jobPostId)
        {

            conn.Open();

            var checkCmd = new NpgsqlCommand("SELECT COUNT(*) FROM t_Save_Job WHERE c_user_Id = @uid AND c_jobPost_Id = @jid", conn);
            checkCmd.Parameters.AddWithValue("uid", userId);
            checkCmd.Parameters.AddWithValue("jid", jobPostId);

            var exists = (long)checkCmd.ExecuteScalar();
            if (exists > 0) return false;

            var insertCmd = new NpgsqlCommand("INSERT INTO t_Save_Job (c_user_Id, c_jobPost_Id) VALUES (@uid, @jid)", conn);
            insertCmd.Parameters.AddWithValue("uid", userId);
            insertCmd.Parameters.AddWithValue("jid", jobPostId);
            insertCmd.ExecuteNonQuery();

            return true;
        }

        public void Remove(int userId, int jobPostId)
        {
            conn.Open();

            var deleteCmd = new NpgsqlCommand("DELETE FROM t_Save_Job WHERE c_user_Id = @uid AND c_jobPost_Id = @jid ", conn);
            deleteCmd.Parameters.AddWithValue("uid", userId);
            deleteCmd.Parameters.AddWithValue("jid", jobPostId);
            deleteCmd.ExecuteNonQuery();
        }

        public List<t_save_job> GetSavedJobs(int userId)
        {
            var list = new List<t_save_job>();
            conn.Open();

            var cmd = new NpgsqlCommand(@"
        SELECT jp.c_job_id, jp.c_job_title, jp.c_job_desc, jp.c_post_date,
               jp.c_job_location, jp.c_job_type, jp.c_job_experience,
               jp.c_salary_range, jp.c_vacancy,
               jp.c_qualification_title, jp.c_skills,
               comp.c_company_name, comp.c_company_logo,
               dept.c_dept_name
        FROM t_Save_Job sj
        JOIN t_job_post jp ON sj.c_jobPost_Id = jp.c_job_id
        JOIN t_companies comp ON jp.c_company_id = comp.c_company_id
        JOIN t_department dept ON jp.c_dept_id = dept.c_dept_id
        WHERE sj.c_user_Id = @uid AND jp.c_expire_date > Now() order by jp.c_post_date desc", conn);

            cmd.Parameters.AddWithValue("uid", userId);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(new t_save_job
                {
                    UserId = userId,
                    JobPostId = reader.GetInt32(reader.GetOrdinal("c_job_id")),
                    c_job_title = reader.GetString(reader.GetOrdinal("c_job_title")),
                    c_job_desc = reader.GetString(reader.GetOrdinal("c_job_desc")),
                    c_post_date = reader.IsDBNull(reader.GetOrdinal("c_post_date"))
                        ? null
                        : reader.GetDateTime(reader.GetOrdinal("c_post_date")).ToString(),
                    c_job_location = reader.GetString(reader.GetOrdinal("c_job_location")),
                    c_job_type = reader.GetString(reader.GetOrdinal("c_job_type")),
                    c_job_experience = int.TryParse(reader.GetString(reader.GetOrdinal("c_job_experience")), out var exp) ? exp : 0,
                    c_salary_range = reader.GetString(reader.GetOrdinal("c_salary_range")),
                    c_vacancy = reader.GetInt32(reader.GetOrdinal("c_vacancy")),
                    c_qualification_title = reader.GetString(reader.GetOrdinal("c_qualification_title")),
                    c_skills = reader.GetString(reader.GetOrdinal("c_skills")),
                    c_company_name = reader.GetString(reader.GetOrdinal("c_company_name")),
                    c_company_logo = reader.IsDBNull(reader.GetOrdinal("c_company_logo"))
                        ? null
                        : reader.GetString(reader.GetOrdinal("c_company_logo")),
                    c_dept_name = reader.GetString(reader.GetOrdinal("c_dept_name"))
                });
            }

            conn.Close();
            return list;
        }


    }
}