using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Repositories.Interfaces;
using Repositories.Models;
using Npgsql;

namespace Repositories.Implementations
{
    public class JobRecommendationRepository : IJobRecommendationInterface
    {
        private readonly NpgsqlConnection _conn;
        public JobRecommendationRepository(NpgsqlConnection conn)
        {
            _conn = conn;
        }
        public async Task<List<Job_Post1>> RecommendJobs(t_JobPreference jobPreference)
        {
            var jobTitlesList = jobPreference.c_PreferredRoles?.Split(',')
                                    .Select(title => title.Trim())
                                    .Where(title => !string.IsNullOrEmpty(title))
                                    .ToList() ?? new List<string>();

            var jobLocationsList = jobPreference.c_PreferredLocations?.Split(',')
                                    .Select(loc => loc.Trim())
                                    .Where(loc => !string.IsNullOrEmpty(loc))
                                    .ToList() ?? new List<string>();

            List<Job_Post1> jobs = new List<Job_Post1>();

            try
            {
                await _conn.OpenAsync();

                var sql = @"
            SELECT jp.c_job_id, jp.c_job_title, jp.c_post_date, jp.c_job_desc, jp.c_job_location, jp.c_job_type, 
                   jp.c_job_experience, jp.c_salary_range, jp.c_vacancy, jp.c_dept_id, 
                   jp.c_qualification_title, jp.c_skills, jp.c_company_id,
                   cp.c_company_name, dp.c_dept_name, cp.c_company_logo 
            FROM t_job_post jp 
            JOIN t_companies cp ON jp.c_company_id = cp.c_company_id 
            JOIN t_department dp ON jp.c_dept_id = dp.c_dept_id";

                using (var cmd = new NpgsqlCommand(sql, _conn))
                {
                    var conditions = new List<string>();

                    // Generate all role-location combinations if both exist
                    if (jobTitlesList.Count > 0 && jobLocationsList.Count > 0)
                    {
                        var combinationConditions = new List<string>();
                        int paramIndex = 0;

                        foreach (var title in jobTitlesList)
                        {
                            foreach (var location in jobLocationsList)
                            {
                                combinationConditions.Add($"(jp.c_job_title ILIKE @p{paramIndex} OR jp.c_job_location ILIKE @p{paramIndex + 1})");
                                cmd.Parameters.AddWithValue($"p{paramIndex}", $"%{title}%");
                                cmd.Parameters.AddWithValue($"p{paramIndex + 1}", $"%{location}%");
                                paramIndex += 2;
                            }
                        }
                        conditions.Add($"({string.Join(" OR ", combinationConditions)})");
                    }
                    else if (jobTitlesList.Count > 0) // Only titles
                    {
                        var titleConditions = jobTitlesList.Select((title, i) =>
                        {
                            cmd.Parameters.AddWithValue($"p{i}", $"%{title}%");
                            return $"jp.c_job_title ILIKE @p{i}";
                        });
                        conditions.Add($"({string.Join(" OR ", titleConditions)})");
                    }
                    else if (jobLocationsList.Count > 0) // Only locations
                    {
                        var locationConditions = jobLocationsList.Select((loc, i) =>
                        {
                            cmd.Parameters.AddWithValue($"p{i}", $"%{loc}%");
                            return $"jp.c_job_location ILIKE @p{i}";
                        });
                        conditions.Add($"({string.Join(" OR ", locationConditions)})");
                    }

                    if (conditions.Count > 0)
                    {
                        cmd.CommandText += " WHERE " + string.Join(" OR ", conditions);
                    }

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            jobs.Add(new Job_Post1
                            {
                                c_job_id = reader.IsDBNull(0) ? (int?)null : reader.GetInt32(0),
                                c_job_title = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                                c_post_date = reader.IsDBNull(2) ? string.Empty : reader.GetDateTime(2).ToString(), // ✅ moved here
                                c_job_desc = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                                c_job_location = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                                c_job_type = reader.IsDBNull(5) ? string.Empty : reader.GetString(5),
                                c_job_experience = reader.IsDBNull(6) ? 0 : reader.GetInt32(6),
                                c_salary_range = reader.IsDBNull(7) ? string.Empty : reader.GetString(7),
                                c_vacancy = reader.IsDBNull(8) ? 0 : reader.GetInt32(8),
                                c_dept_id = reader.IsDBNull(9) ? 0 : reader.GetInt32(9),
                                c_qualification_title = reader.IsDBNull(10) ? string.Empty : reader.GetString(10),
                                c_skills = reader.IsDBNull(11) ? string.Empty : reader.GetString(11),
                                c_company_id = reader.IsDBNull(12) ? 0 : reader.GetInt32(12),
                                c_company_name = reader.IsDBNull(13) ? string.Empty : reader.GetString(13),
                                c_dept_name = reader.IsDBNull(14) ? string.Empty : reader.GetString(14),
                                c_company_logo = reader.IsDBNull(15) ? string.Empty : reader.GetString(15)
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
    }
}