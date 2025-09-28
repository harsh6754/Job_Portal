using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using Repositories.Interfaces;
using Repositories.Model;
using Repositories.Models;

namespace Repositories.Implimentation
{
    public class AdminRepository : IAdminInterface
    {
        private readonly NpgsqlConnection _conn;

        public AdminRepository(NpgsqlConnection connection)
        {
            _conn = connection;
        }


        public async Task<List<t_user>> GetUsers()
        {
            List<t_user> users = new List<t_user>();
            try
            {
                await _conn.OpenAsync();
                var query = "SELECT * FROM t_user WHERE c_role = 'Candidate' OR c_Role='Recruiter'";
                using (var cmd = new NpgsqlCommand(query, _conn))
                {
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            users.Add(new t_user
                            {
                                c_userId = reader.GetInt32(0),
                                c_username = reader.GetString(1),
                                c_fullName = reader.GetString(2),
                                c_email = reader.GetString(3),
                                c_phoneNumber = reader.GetString(5),
                                c_gender = reader.GetString(6),
                                c_profileImage = !reader.IsDBNull(7) ? reader.GetString(7) : "default-profile.png",
                                c_userRole = reader.GetString(8),
                                c_IsBlock = reader.GetBoolean(10)
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
            return users;
        }

        // ✅ Delete a user safely
        public async Task<bool> DeleteUser(int id)
        {
            try
            {
                await _conn.OpenAsync();
                var query = "DELETE FROM t_user WHERE c_uid = @id";
                using (var cmd = new NpgsqlCommand(query, _conn))
                {
                    cmd.Parameters.AddWithValue("id", id);
                    int affectedRows = await cmd.ExecuteNonQueryAsync();
                    return affectedRows > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }
            finally
            {
                await _conn.CloseAsync();
            }
        }
        public async Task<int> GetUsersCount()
        {
            try
            {
                await _conn.OpenAsync();
                var query = "SELECT COUNT(*) FROM t_user where c_role='Candidate'";
                using (var cmd = new NpgsqlCommand(query, _conn))
                {
                    return Convert.ToInt32(await cmd.ExecuteScalarAsync());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return 0;
            }
            finally
            {
                await _conn.CloseAsync();
            }
        }

        public async Task<int> GetAllUsersCount()
        {
            try
            {
                await _conn.OpenAsync();
                var query = "SELECT COUNT(*) FROM t_user where c_role='Candidate' OR c_role='Recruiter'";
                using (var cmd = new NpgsqlCommand(query, _conn))
                {
                    return Convert.ToInt32(await cmd.ExecuteScalarAsync());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return 0;
            }
            finally
            {
                await _conn.CloseAsync();
            }
        }

        public async Task<int> GetAllBlockedUsersCount()
        {
            try
            {
                await _conn.OpenAsync();
                var query = "SELECT COUNT(*) FROM t_user where c_is_blocked=true";
                using (var cmd = new NpgsqlCommand(query, _conn))
                {
                    return Convert.ToInt32(await cmd.ExecuteScalarAsync());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return 0;
            }
            finally
            {
                await _conn.CloseAsync();
            }
        }

        public async Task<int> GetAllCandidatesCount()
        {
            try
            {
                await _conn.OpenAsync();
                var query = "SELECT COUNT(*) FROM t_user where c_role='Candidate'";
                using (var cmd = new NpgsqlCommand(query, _conn))
                {
                    return Convert.ToInt32(await cmd.ExecuteScalarAsync());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return 0;
            }
            finally
            {
                await _conn.CloseAsync();
            }
        }

        public async Task<int> GetRecruitersCount()
        {
            try
            {
                await _conn.OpenAsync();
                var query = "SELECT COUNT(*) FROM t_user WHERE c_role = 'Recruiter'";
                using (var cmd = new NpgsqlCommand(query, _conn))
                {
                    return Convert.ToInt32(await cmd.ExecuteScalarAsync());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return 0;
            }
            finally
            {
                await _conn.CloseAsync();
            }
        }

        public async Task<List<t_recruiter>> GetRecruiters()
        {
            List<t_recruiter> recruiters = new List<t_recruiter>();
            try
            {
                await _conn.OpenAsync();

                // Select only relevant fields from t_companies
                var query = @"
            SELECT 
                tc.c_company_id,
                tc.c_company_name,
                tc.c_owner_id,
                tc.c_company_email,
                tc.c_company_phone,
                tc.c_company_address,
                tc.c_company_reg_number,
                tc.c_tax_id_number,
                tc.c_industry,
                i.industry_name, -- Assuming this is the correct field for industry name
                tc.c_website,
                tc.c_verified_status,
                tc.c_legal_documents,
                tc.c_company_logo
            FROM t_companies tc
            JOIN industries i ON tc.c_industry = i.id::text;;
        ";

                using (var cmd = new NpgsqlCommand(query, _conn))
                {
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            recruiters.Add(new t_recruiter
                            {
                                c_company_id = reader.GetInt32(0),
                                c_company_name = reader.GetString(1),
                                c_owner_id = reader.GetInt32(2),
                                c_company_email = reader.GetString(3),
                                c_company_phone = reader.GetString(4),
                                c_company_address = reader.GetString(5),
                                c_company_reg_number = reader.GetString(6),
                                c_tax_id_number = reader.GetString(7),
                                c_industry = reader.GetString(8),
                                c_industry_name = reader.GetString(9),
                                c_website = reader.IsDBNull(10) ? null : reader.GetString(10),
                                c_verified_status = reader.IsDBNull(11) ? (bool?)null : reader.GetBoolean(11),
                                c_legal_documents = reader.IsDBNull(12) ? null : reader.GetFieldValue<string[]>(12),
                                c_company_logo = reader.IsDBNull(13) ? null : reader.GetString(13)
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
            return recruiters;
        }

       public async Task<t_recruiter> GetRecruiterByCompanyId(int companyId)
{
    try
    {
        await _conn.OpenAsync();
        var query = @"
            SELECT 
                c_company_id,
                c_company_name,
                c_owner_id,
                c_company_email,
                c_company_phone,
                c_company_address,
                c_company_reg_number,
                c_tax_id_number,
                c_industry,
                c_website,
                c_verified_status,
                c_legal_documents,
                c_company_logo
            FROM t_companies
            WHERE c_company_id = @companyId";
        using (var cmd = new NpgsqlCommand(query, _conn))
        {
            cmd.Parameters.AddWithValue("@companyId", companyId);
            using (var reader = await cmd.ExecuteReaderAsync())
            {
                if (await reader.ReadAsync())
                {
                    return new t_recruiter
                    {
                        c_company_id = reader.GetInt32(0),
                        c_company_name = reader.GetString(1),
                        c_owner_id = reader.GetInt32(2),
                        c_company_email = reader.GetString(3),
                        c_company_phone = reader.GetString(4),
                        c_company_address = reader.GetString(5),
                        c_company_reg_number = reader.GetString(6),
                        c_tax_id_number = reader.GetString(7),
                        c_industry = reader.GetString(8),
                        c_website = reader.IsDBNull(9) ? null : reader.GetString(9),
                        c_verified_status = reader.IsDBNull(10) ? (bool?)null : reader.GetBoolean(10),
                        c_legal_documents = reader.IsDBNull(11) ? null : reader.GetFieldValue<string[]>(11),
                        c_company_logo = reader.IsDBNull(12) ? null : reader.GetString(12)
                    };
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
    return new t_recruiter();
}

        public async Task<bool> UpdateRecruiterStatus(int companyId, bool approved = true)
        {
            try
            {
                await _conn.OpenAsync();

                // Update the recruiter's status
                var query = "UPDATE t_companies SET c_verified_status = @verifiedStatus WHERE c_company_id = @companyId";
                using (var cmd = new NpgsqlCommand(query, _conn))
                {
                    cmd.Parameters.AddWithValue("@verifiedStatus", approved);
                    cmd.Parameters.AddWithValue("@companyId", companyId);

                    int rowsAffected = await cmd.ExecuteNonQueryAsync();

                    if (rowsAffected > 0) // If update was successful
                    {
                        // Fetch recruiter's email and company name
                        string recruiterEmail = "";
                        string companyName = "";

                        var getRecruiterQuery = "SELECT c_company_email, c_company_name FROM t_companies WHERE c_company_id = @companyId";
                        using (var cmd2 = new NpgsqlCommand(getRecruiterQuery, _conn))
                        {
                            cmd2.Parameters.AddWithValue("@companyId", companyId);
                            using (var reader = await cmd2.ExecuteReaderAsync())
                            {
                                if (await reader.ReadAsync())
                                {
                                    recruiterEmail = reader.GetString(0);
                                    companyName = reader.GetString(1);
                                }
                            }
                        }

                        // Send approval email
                        if (approved && !string.IsNullOrEmpty(recruiterEmail))
                        {
                            var emailService = new EmailService();
                            emailService.SendApprovalEmail(recruiterEmail, companyName);
                        }

                        return true;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating status: {ex.Message}");
                return false;
            }
            finally
            {
                await _conn.CloseAsync();
            }
        }

        public async Task<bool> DeleteRecruiter(int companyId)
        {
            try
            {
                await _conn.OpenAsync();
                var query = "DELETE FROM t_companies WHERE c_company_id = @companyId";
                using (var cmd = new NpgsqlCommand(query, _conn))
                {
                    cmd.Parameters.AddWithValue("@companyId", companyId);
                    int affectedRows = await cmd.ExecuteNonQueryAsync();
                    return affectedRows > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting recruiter: {ex.Message}");
                return false;
            }
            finally
            {
                await _conn.CloseAsync();
            }
        }

        public async Task<bool> BulkUpdateRecruiterStatus(List<int> companyIds, bool approved = true)
        {
            try
            {
                await _conn.OpenAsync();

                // Update recruiter status in bulk
                var query = "UPDATE t_companies SET c_verified_status = @verifiedStatus WHERE c_company_id = ANY(@companyIds)";
                using (var cmd = new NpgsqlCommand(query, _conn))
                {
                    cmd.Parameters.AddWithValue("@verifiedStatus", approved);
                    cmd.Parameters.AddWithValue("@companyIds", companyIds.ToArray());

                    int rowsAffected = await cmd.ExecuteNonQueryAsync();
                    if (rowsAffected > 0)
                    {
                        // Fetch all recruiter emails and company names
                        var getRecruitersQuery = "SELECT c_company_email, c_company_name FROM t_companies WHERE c_company_id = ANY(@companyIds)";
                        using (var cmd2 = new NpgsqlCommand(getRecruitersQuery, _conn))
                        {
                            cmd2.Parameters.AddWithValue("@companyIds", companyIds.ToArray());

                            List<(string Email, string CompanyName)> recruiters = new List<(string, string)>();
                            using (var reader = await cmd2.ExecuteReaderAsync())
                            {
                                while (await reader.ReadAsync())
                                {
                                    recruiters.Add((reader.GetString(0), reader.GetString(1)));
                                }
                            }

                            // Send bulk approval emails
                            var emailService = new EmailService();
                            foreach (var recruiter in recruiters)
                            {
                                emailService.SendApprovalEmail(recruiter.Email, recruiter.CompanyName);
                            }
                        }
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error in Bulk Approval: {ex.Message}");
                return false;
            }
            finally
            {
                await _conn.CloseAsync();
            }
        }


        public async Task<bool> BulkUpdateRecruiterStatusReject(List<int> companyIds, string rejectionReason, bool approved = false)
        {
            try
            {
                await _conn.OpenAsync();

                // Update recruiter status in bulk
                var query = "UPDATE t_companies SET c_verified_status = @verifiedStatus WHERE c_company_id = ANY(@companyIds)";
                using (var cmd = new NpgsqlCommand(query, _conn))
                {
                    cmd.Parameters.AddWithValue("@verifiedStatus", approved);
                    cmd.Parameters.AddWithValue("@companyIds", companyIds.ToArray());

                    int rowsAffected = await cmd.ExecuteNonQueryAsync();
                    if (rowsAffected > 0)
                    {
                        // Fetch all recruiter emails, company names, and recruiter names
                        var getRecruitersQuery = "SELECT c_company_email, c_company_name, c_owner_id FROM t_companies WHERE c_company_id = ANY(@companyIds)";
                        using (var cmd2 = new NpgsqlCommand(getRecruitersQuery, _conn))
                        {
                            cmd2.Parameters.AddWithValue("@companyIds", companyIds.ToArray());

                            List<(string Email, string CompanyName, string RecruiterName)> recruiters = new List<(string, string, string)>();
                            using (var reader = await cmd2.ExecuteReaderAsync())
                            {
                                while (await reader.ReadAsync())
                                {
                                    string recruiterName = reader.IsDBNull(2) ? "Unknown" : reader.GetValue(2).ToString();
                                    recruiters.Add((reader.GetString(0), reader.GetString(1), recruiterName));
                                }
                            }

                            // Send bulk rejection emails
                            var emailService = new EmailService();
                            foreach (var recruiter in recruiters)
                            {
                                emailService.SendRejectionEmail(recruiter.Email, recruiter.CompanyName, recruiter.RecruiterName, rejectionReason);
                            }
                        }
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error in Bulk Rejection: {ex.Message}");
                return false;
            }
            finally
            {
                await _conn.CloseAsync();
            }
        }
        public async Task<bool> UpdateRecruiterstatusReject(int companyId, string rejectionReason, bool approved = false)
        {
            try
            {
                await _conn.OpenAsync();

                // Update the recruiter's status
                var query = "UPDATE t_companies SET c_verified_status = @verifiedStatus WHERE c_company_id = @companyId";
                using (var cmd = new NpgsqlCommand(query, _conn))
                {
                    cmd.Parameters.AddWithValue("@verifiedStatus", approved);
                    cmd.Parameters.AddWithValue("@companyId", companyId);

                    int rowsAffected = await cmd.ExecuteNonQueryAsync();

                    if (rowsAffected > 0) // If update was successful
                    {
                        // Fetch recruiter's email, company name, and recruiter name
                        string recruiterEmail = "";
                        string companyName = "";
                        string recruiterName = "";

                        var getRecruiterQuery = "SELECT c_company_email, c_company_name, c_owner_id FROM t_companies WHERE c_company_id = @companyId";
                        using (var cmd2 = new NpgsqlCommand(getRecruiterQuery, _conn))
                        {
                            cmd2.Parameters.AddWithValue("@companyId", companyId);
                            using (var reader = await cmd2.ExecuteReaderAsync())
                            {
                                if (await reader.ReadAsync())
                                {
                                    recruiterEmail = reader.GetString(0);
                                    companyName = reader.GetString(1);
                                    recruiterName = reader.IsDBNull(2) ? "Unknown" : reader.GetValue(2).ToString(); // ✅ Fix here
                                }
                            }
                        }

                        // Send email notification
                        var emailService = new EmailService();
                        if (approved && !string.IsNullOrEmpty(recruiterEmail))
                        {
                            emailService.SendApprovalEmail(recruiterEmail, companyName);
                        }
                        else if (!approved && !string.IsNullOrEmpty(recruiterEmail))
                        {
                            emailService.SendRejectionEmail(recruiterEmail, companyName, recruiterName, rejectionReason);
                        }

                        return true;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error updating status: {ex.Message}");
                return false;
            }
            finally
            {
                await _conn.CloseAsync();
            }
        }

        public async Task<List<t_recruiter>> PendingApproval()
        {
            List<t_recruiter> pendingRecruiters = new List<t_recruiter>();

            try
            {
                await _conn.OpenAsync();
                var query = "SELECT * FROM t_companies WHERE c_verified_status  IS NULL ";

                using (var cmd = new NpgsqlCommand(query, _conn))
                {
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync()) // Loop to fetch all pending recruiters
                        {
                            pendingRecruiters.Add(new t_recruiter
                            {
                                c_company_id = reader.GetInt32(0),
                                c_company_name = reader.GetString(1),
                                c_owner_id = reader.GetInt32(2),
                                c_company_email = reader.GetString(3),
                                c_company_phone = reader.GetString(4),
                                c_company_address = reader.GetString(5),
                                c_company_reg_number = reader.GetString(6),
                                c_tax_id_number = reader.GetString(7),
                                c_industry = reader.GetString(8),
                                c_website = reader.IsDBNull(9) ? null : reader.GetString(9),
                                c_verified_status = reader.IsDBNull(10) ? null : reader.GetBoolean(10),
                                c_legal_documents = reader.IsDBNull(11) ? null : reader.GetValue(11) as string[],
                                c_company_logo = reader.IsDBNull(12) ? null : reader.GetString(12)
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching pending approvals: {ex.Message}");
            }
            finally
            {
                await _conn.CloseAsync();
            }

            return pendingRecruiters; // Returns an empty list if no records are found
        }

        public async Task<List<RegistrationStats>> GetRegistrationStats(DateTime startDate, DateTime endDate)
        {
            List<RegistrationStats> stats = new List<RegistrationStats>();
            try
            {
                await _conn.OpenAsync();

                var query = @"
                    WITH dates AS (
                        SELECT generate_series(@startDate, @endDate, '1 day'::interval)::date as date
                    ),
                    user_counts AS (
                        SELECT DATE(c_reg_date) as date, COUNT(*) as new_users
                        FROM t_user
                        WHERE c_reg_date BETWEEN @startDate AND @endDate
                        GROUP BY DATE(c_reg_date)
                    ),
                    company_counts AS (
                        SELECT DATE(c_reg_date) as date, COUNT(*) as new_companies
                        FROM t_companies
                        WHERE c_reg_date BETWEEN @startDate AND @endDate
                        GROUP BY DATE(c_reg_date)
                    )
                    SELECT 
                        d.date,
                        COALESCE(uc.new_users, 0) as new_users,
                        COALESCE(cc.new_companies, 0) as new_companies
                    FROM dates d
                    LEFT JOIN user_counts uc ON d.date = uc.date
                    LEFT JOIN company_counts cc ON d.date = cc.date
                    ORDER BY d.date";

                using (var cmd = new NpgsqlCommand(query, _conn))
                {
                    cmd.Parameters.AddWithValue("@startDate", startDate);
                    cmd.Parameters.AddWithValue("@endDate", endDate);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            stats.Add(new RegistrationStats
                            {
                                Date = reader.GetDateTime(0),
                                NewUsers = reader.GetInt32(1),
                                NewCompanies = reader.GetInt32(2)
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting registration stats: {ex.Message}");
            }
            finally
            {
                await _conn.CloseAsync();
            }
            return stats;
        }

        public async Task<List<dynamic>> GetUserDistributionData()
        {
            var data = new List<dynamic>();
            await _conn.OpenAsync();
            var query = @"
        SELECT 'Job Seekers' AS category, COUNT(*) 
        FROM t_user WHERE c_role = 'Candidate'
        UNION ALL
        SELECT 'Recruiters' AS category, COUNT(*) 
        FROM t_user WHERE c_role = 'Recruiter';";

            using (var cmd = new NpgsqlCommand(query, _conn))
            using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    data.Add(new
                    {
                        category = reader.GetString(0),
                        value = reader.GetInt32(1)
                    });
                }
            }

            await _conn.CloseAsync();
            return data;
        }

       

        public async Task<List<t_job>> GetJobs()
        {
            List<t_job> job = new List<t_job>();
            try
            {
                await _conn.OpenAsync();
                var query = "SELECT c_job_id,c_job_title,c_job_location FROM t_job_post";
                using (var cmd = new NpgsqlCommand(query, _conn))
                {
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            job.Add(new t_job
                            {
                                c_job_id = reader.GetInt32(0),
                                c_job_title = reader.GetString(1),

                                c_job_location = reader.GetString(2)
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
            return job;
        }

        

        public async Task<int> GetJobPostCount()
        {
            try
            {
                await _conn.OpenAsync();
                var query = "SELECT COUNT(*) FROM t_job_post";
                using (var cmd = new NpgsqlCommand(query, _conn))
                {
                    return Convert.ToInt32(await cmd.ExecuteScalarAsync());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return 0;
            }
            finally
            {
                await _conn.CloseAsync();
            }
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
        public async Task<List<t_JobWithCompanyInfo>> GetJobsWithCompanyInfo(int companyId)
        {
            List<t_JobWithCompanyInfo> jobsWithCompanyInfo = new List<t_JobWithCompanyInfo>();

            try
            {
                await _conn.OpenAsync();

                var query = @"
            SELECT 
                c.c_company_id,
                c.c_company_name,
                j.c_job_id,
                j.c_job_title,
                j.c_job_desc,
                j.c_job_location,
                j.c_salary_range,
                j.c_vacancy,
                j.c_post_date,
                j.c_expire_date
            FROM 
                t_job_post j
            INNER JOIN 
                t_companies c 
            ON 
                j.c_company_id = c.c_company_id
            WHERE 
                c.c_company_id = @companyId
            ORDER BY 
                c.c_company_name, j.c_post_date DESC;";

                using (var cmd = new NpgsqlCommand(query, _conn))
                {
                    cmd.Parameters.AddWithValue("@companyId", companyId);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            jobsWithCompanyInfo.Add(new t_JobWithCompanyInfo
                            {
                                c_company_id = reader.GetInt32(0),
                                c_company_name = reader.GetString(1),
                                c_job_id = reader.GetInt32(2),
                                c_job_title = reader.GetString(3),
                                c_job_description = reader.GetString(4),
                                c_job_location = reader.GetString(5),
                                c_salary = reader.GetString(6),
                                c_vacant_seats = reader.GetInt32(7),
                                c_created_at = reader.GetDateTime(8),
                                c_expire_date = reader.IsDBNull(9) ? null : reader.GetDateTime(9)
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[GetJobsWithCompanyInfo] Error: {ex.Message}");
            }
            finally
            {
                await _conn.CloseAsync();
            }

            return jobsWithCompanyInfo;
        }

        public async Task<List<CandidateJobApplied>> GetAppliedJobs(int jobId)
        {
            List<CandidateJobApplied> appliedJobs = new List<CandidateJobApplied>();
            try
            {
                await _conn.OpenAsync();

                var query = @"
            SELECT 
                a.c_job_id,
                a.c_application_id,
                u.c_username,
                a.c_uid,
                a.c_apply_date,
                a.c_status,
                u.c_email,
                u.c_phone_number
            FROM 
                t_apply_jobs a
            JOIN 
                t_user u ON a.c_uid = u.c_uid
            WHERE 
                a.c_job_id = @jobId";

                using (var cmd = new NpgsqlCommand(query, _conn))
                {
                    cmd.Parameters.AddWithValue("@jobId", jobId); // <-- corrected parameter

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            appliedJobs.Add(new CandidateJobApplied
                            {
                                c_jobId = reader.GetInt32(0),
                                c_applicationId = reader.GetInt32(1),
                                c_userName = reader.GetString(2),
                                c_userId = reader.GetInt32(3),
                                c_apply_at = reader.GetString(4),
                                c_status = reader.GetString(5),
                                c_email = reader.GetString(6),
                                c_mobile = reader.GetString(7)
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

            return appliedJobs;
        }

        public void BlockOrUnblockUser(int userId)
        {
            _conn.Open();

            var userCmd = new NpgsqlCommand("SELECT c_is_blocked, c_role, c_email, c_username FROM t_user WHERE c_uid = @id", _conn);
            userCmd.Parameters.AddWithValue("id", userId);

            bool isBlocked = false;
            string role = "", email = "", name = "";
            using (var reader = userCmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    isBlocked = reader.GetBoolean(reader.GetOrdinal("c_is_blocked"));
                    role = reader.GetString(reader.GetOrdinal("c_role"));
                    email = reader.GetString(reader.GetOrdinal("c_email"));
                    name = reader.GetString(reader.GetOrdinal("c_username"));
                }
            }

            // Toggle the block status
            bool newBlockStatus = !isBlocked;

            var toggleCmd = new NpgsqlCommand("UPDATE t_user SET c_is_blocked = @status WHERE c_uid = @id", _conn);
            toggleCmd.Parameters.AddWithValue("status", newBlockStatus);
            toggleCmd.Parameters.AddWithValue("id", userId);
            toggleCmd.ExecuteNonQuery();

            if (role == "Recruiter")
            {
                // Recruiter is blocked → make company & jobs inactive
                var companyCmd = new NpgsqlCommand("UPDATE t_companies SET c_is_active = @active WHERE c_owner_id = @id", _conn);
                companyCmd.Parameters.AddWithValue("active", !newBlockStatus);
                companyCmd.Parameters.AddWithValue("id", userId);
                companyCmd.ExecuteNonQuery();

                var updateJobsCmd = new NpgsqlCommand(@"
            UPDATE t_job_post 
            SET c_is_active = @jobStatus 
            WHERE c_company_id = (
                SELECT c_company_id FROM t_companies WHERE c_owner_id = @id
            )", _conn);
                updateJobsCmd.Parameters.AddWithValue("jobStatus", !newBlockStatus);
                updateJobsCmd.Parameters.AddWithValue("id", userId);
                updateJobsCmd.ExecuteNonQuery();
            }

            _conn.Close();

            // ✅ Use updated block status for email notification
            var emailService = new EmailService();
            emailService.SendBlockStatusEmail(email, name, newBlockStatus, role);
        }
        public async Task<List<ApplicationCountByDate>> GetApplicationStats(DateTime startDate, DateTime endDate)
        {
            List<ApplicationCountByDate> stats = new List<ApplicationCountByDate>();

            try
            {
                await _conn.OpenAsync();

                var query = @"
            SELECT 
                DATE(c_apply_date) AS apply_date,
                COUNT(*) AS application_count
            FROM 
                t_apply_jobs
            WHERE 
                DATE(c_apply_date) BETWEEN @startDate AND @endDate
            GROUP BY 
                DATE(c_apply_date)
            ORDER BY 
                apply_date;";

                using (var cmd = new NpgsqlCommand(query, _conn))
                {
                    cmd.Parameters.AddWithValue("@startDate", startDate);
                    cmd.Parameters.AddWithValue("@endDate", endDate);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            stats.Add(new ApplicationCountByDate
                            {
                                ApplyDate = reader.GetDateTime(0),
                                ApplicationCount = reader.GetInt32(1)
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching application stats: {ex.Message}");
            }
            finally
            {
                await _conn.CloseAsync();
            }

            return stats;
        }

        public async Task<int> GetApplicationsCount()
        {
            try
            {
                await _conn.OpenAsync();
                var query = "SELECT COUNT(*) FROM t_apply_jobs";
                using (var cmd = new NpgsqlCommand(query, _conn))
                {
                    return Convert.ToInt32(await cmd.ExecuteScalarAsync());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return 0;
            }
            finally
            {
                await _conn.CloseAsync();
            }
        }
        public async Task<IEnumerable<JobApplicationStats>> GetJobApplicationStatsAsync(DateTime startDate, DateTime endDate, string status = null)
        {
            List<JobApplicationStats> stats = new List<JobApplicationStats>();

            try
            {
                await _conn.OpenAsync();

                var queryBuilder = new StringBuilder();

                // 1. From t_apply_jobs
                queryBuilder.Append(@"
            SELECT 
                DATE(c_apply_date) AS date,
                COUNT(*) AS applications,
                c_status
            FROM 
                t_apply_jobs
            WHERE 
                DATE(c_apply_date) BETWEEN @startDate AND @endDate");

                if (!string.IsNullOrEmpty(status))
                {
                    queryBuilder.Append(" AND c_status = @status");
                }

                queryBuilder.Append(@"
            GROUP BY DATE(c_apply_date), c_status");

                // 2. From t_hired_candidates
                if (string.IsNullOrEmpty(status) || status.ToLower() == "hired")
                {
                    queryBuilder.Append(@"
            UNION ALL
            SELECT 
                DATE(c_hire_date) AS date,
                COUNT(*) AS applications,
                'Hired' AS c_status
            FROM 
                t_hired_candidates
            WHERE 
                DATE(c_hire_date) BETWEEN @startDate AND @endDate");

                    if (!string.IsNullOrEmpty(status) && status.ToLower() == "hired")
                    {
                        queryBuilder.Append(" AND 'Hired' = @status");
                    }

                    queryBuilder.Append(@"
            GROUP BY DATE(c_hire_date)");
                }

                queryBuilder.Append(" ORDER BY date;");

                using (var cmd = new NpgsqlCommand(queryBuilder.ToString(), _conn))
                {
                    cmd.Parameters.AddWithValue("@startDate", startDate);
                    cmd.Parameters.AddWithValue("@endDate", endDate);

                    if (!string.IsNullOrEmpty(status))
                    {
                        cmd.Parameters.AddWithValue("@status", status);
                    }

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            stats.Add(new JobApplicationStats
                            {
                                Date = reader.GetDateTime(0),
                                Applications = reader.GetInt32(1),
                                Status = reader.GetString(2)
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching job application stats: {ex.Message}");
            }
            finally
            {
                await _conn.CloseAsync();
            }

            return stats;
        }

        public async Task<List<Notification>> GetAllNotifications()
        {
            var notifications = new List<Notification>();
            try
            {
                await _conn.OpenAsync();
                var query = @"
                    SELECT notification_id, user_id, message, type, is_read, created_at
                    FROM notifications
                    ORDER BY created_at DESC";
                
                using (var cmd = new NpgsqlCommand(query, _conn))
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        notifications.Add(new Notification
                        {
                            NotificationId = reader.GetInt32(0),
                            UserId = reader.IsDBNull(1) ? null : reader.GetInt32(1),
                            Message = reader.GetString(2),
                            Type = reader.GetString(3),
                            IsRead = reader.GetBoolean(4),
                            CreatedAt = reader.GetDateTime(5)
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching all notifications: {ex.Message}");
            }
            finally
            {
                await _conn.CloseAsync();
            }

            return notifications;
        }

        public async Task<bool> DeleteMultipleNotifications(List<int> notificationIds)
        {
            try
            {
                await _conn.OpenAsync();
                var query = "DELETE FROM notifications WHERE notification_id = ANY(@notificationIds)";
                using (var cmd = new NpgsqlCommand(query, _conn))
                {
                    cmd.Parameters.AddWithValue("@notificationIds", notificationIds.ToArray());
                    int rowsAffected = await cmd.ExecuteNonQueryAsync();
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting notifications: {ex.Message}");
                return false;
            }
            finally
            {
                await _conn.CloseAsync();
            }
        }

        public async Task<int> GetNotificationCount()
        {
            int count = 0;
            try
            {
                _conn.Open();
                var query = "SELECT COUNT(*) FROM notifications where is_read = false";
                using (var cmd = new NpgsqlCommand(query, _conn))
                {
                    count = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                _conn.Close();
            }
            return count;
        }

        public async Task<int> MarkAllNotificationsAsRead()
        {
            int rowsAffected = 0;
            try
            {
                await _conn.OpenAsync();
                var query = "UPDATE notifications SET is_read = true WHERE is_read = false";
                using (var cmd = new NpgsqlCommand(query, _conn))
                {
                    rowsAffected = await cmd.ExecuteNonQueryAsync(); // async call
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                // Optionally log or rethrow
            }
            finally
            {
                await _conn.CloseAsync();
            }

            return rowsAffected;
        }

        public async Task<List<Notification>> GetUnreadNotifications()
        {
            var notifications = new List<Notification>();
            try
            {
                await _conn.OpenAsync();
                var query = @"
                    SELECT notification_id, user_id, message, type, is_read, created_at
                    FROM notifications
                    WHERE is_read = false
                    ORDER BY created_at DESC";
                
                using (var cmd = new NpgsqlCommand(query, _conn))
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        notifications.Add(new Notification
                        {
                            NotificationId = reader.GetInt32(0),
                            UserId = reader.IsDBNull(1) ? null : reader.GetInt32(1),
                            Message = reader.GetString(2),
                            Type = reader.GetString(3),
                            IsRead = reader.GetBoolean(4),
                            CreatedAt = reader.GetDateTime(5)
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching notifications: {ex.Message}");
            }
            finally
            {
                await _conn.CloseAsync();
            }

            return notifications;
        }
    }
}
