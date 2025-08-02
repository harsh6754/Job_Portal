using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.MachineLearning;
using Npgsql;
using Repositories.Model;


public class ApplyJobRepository : IApplyjobInterface
{
    private readonly NpgsqlConnection _conn;
    public ApplyJobRepository(NpgsqlConnection conn)
    {
        _conn = conn;
    }

    public async Task<int> ApplyJob(t_apply_job apply_Job)
    {
        try
        {
            await _conn.OpenAsync();

            // First: Check if already applied
            string checkqry = "SELECT 1 FROM t_apply_jobs WHERE c_uid = @c_uid AND c_job_id = @c_job_id";
            var checkcmd = new NpgsqlCommand(checkqry, _conn);
            checkcmd.Parameters.AddWithValue("@c_uid", apply_Job.c_uid);
            checkcmd.Parameters.AddWithValue("@c_job_id", apply_Job.c_job_id);

            var dr = await checkcmd.ExecuteReaderAsync();
            if (await dr.ReadAsync())
            {
                await dr.CloseAsync();  // ✅ close reader before returning
                return 2; // Already applied
            }
            await dr.CloseAsync(); // ✅ close reader to avoid conflict

            apply_Job.c_status = "Applied";
            

            // Insert if not applied
            string qry = "INSERT INTO t_apply_jobs (c_job_id, c_uid, c_status,c_ats_score) VALUES (@c_job_id, @c_uid, @c_status,@c_ats_score)";
            var cmd = new NpgsqlCommand(qry, _conn);
            cmd.Parameters.AddWithValue("@c_job_id", apply_Job.c_job_id);
            cmd.Parameters.AddWithValue("@c_uid", apply_Job.c_uid);
            
            cmd.Parameters.AddWithValue("@c_status", apply_Job.c_status);
            cmd.Parameters.AddWithValue("@c_ats_score", apply_Job.c_ats_score);

            await cmd.ExecuteNonQueryAsync();
            return 1; // Successfully applied
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error while applying job: " + ex.Message);
            return 0; // Error
        }
        finally
        {
            await _conn.CloseAsync();
        }
    }


    public async Task<FieldCheckResult?> CheckFields(int id)
    {
        try
        {
            await _conn.OpenAsync();
            string qry = @"
            SELECT 
                CASE WHEN EXISTS (SELECT 1 FROM t_education e WHERE e.c_user_id = u.c_uid) THEN 1 ELSE 0 END AS HasEducation,
                CASE WHEN EXISTS (SELECT 1 FROM t_userprojects ex WHERE ex.c_user_id = u.c_uid) THEN 1 ELSE 0 END AS HasProjects,
                CASE WHEN EXISTS (SELECT 1 FROM t_userresumes r WHERE r.c_user_id = u.c_uid) THEN 1 ELSE 0 END AS HasResume
            FROM t_user u
            WHERE u.c_uid = @c_uid";

            var cmd = new NpgsqlCommand(qry, _conn);
            cmd.Parameters.AddWithValue("@c_uid", id);

            using var dr = await cmd.ExecuteReaderAsync();
            if (await dr.ReadAsync())
            {
                return new FieldCheckResult
                {
                    HasEducation = dr.GetInt32(0) == 1,
                    HasProject = dr.GetInt32(1) == 1,
                    HasResume = dr.GetInt32(2) == 1
                };
            }

            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error while checking fields: " + ex.Message);
            return null;
        }
        finally
        {
            await _conn.CloseAsync();
        }
    }

    // public async Task<int> DeleteJob(int id)
    // {
    //     try
    //     {
    //         await _conn.OpenAsync();
    //         string qry = "delete from t_apply_jobs where c_application_id=@c_application_id";
    //         var cmd = new NpgsqlCommand(qry, _conn);
    //         cmd.Parameters.AddWithValue("@c_application_id", id);
    //         await cmd.ExecuteNonQueryAsync();
    //         return 1;
    //     }
    //     catch (System.Exception ex)
    //     {

    //         System.Console.WriteLine("Error while deleting applied jobs:" + ex.Message);
    //         return 0;
    //     }
    //     finally
    //     {
    //         await _conn.CloseAsync();
    //     }
    // }


    //To get all job application, Also For filter on job title and also filter by status
    public async Task<List<t_apply_job>> GetApplyJobApplication(int id, string job_title, string status)
    {
        Dictionary<int, t_apply_job> applications = new Dictionary<int, t_apply_job>();

        try
        {
            await _conn.OpenAsync();
            string qry = @"
        SELECT 
            aj.c_application_id,
            aj.c_apply_date,
            aj.c_status,
            aj.c_ats_score,

            u.c_uid,
            u.c_fullname,
            u.c_email,
            u.c_phone_number,
            u.c_image,

            ed.c_highestqualification,
            ed.c_degree,
            ed.c_universityname,
            ed.c_specialization,
            ed.c_yearofpassing,
            ed.c_board,
            ed.c_percentage,

            we.c_companyname,
            we.c_jobtitle,
            we.c_years_work,

            r.c_resumefilepath,
            t.c_job_title,
            t.c_job_id

        FROM t_apply_jobs aj
        INNER JOIN t_job_post t ON aj.c_job_id = t.c_job_id
        INNER JOIN t_user u ON aj.c_uid = u.c_uid
        LEFT JOIN t_education ed ON u.c_uid = ed.c_user_id
        LEFT JOIN t_workexperience we ON u.c_uid = we.c_user_id
        LEFT JOIN t_userresumes r ON u.c_uid = r.c_user_id
        WHERE t.c_company_id = @c_company_id
        AND (@job_title::varchar IS NULL OR t.c_job_title ILIKE @job_title)
        AND (@c_status::varchar IS NULL OR aj.c_status ILIKE @c_status)
        ORDER BY aj.c_apply_date DESC;";

            var cmd = new NpgsqlCommand(qry, _conn);
            cmd.Parameters.AddWithValue("@c_company_id", id);
            cmd.Parameters.AddWithValue("@job_title", string.IsNullOrEmpty(job_title) ? (object)DBNull.Value : job_title);
            cmd.Parameters.AddWithValue("@c_status", string.IsNullOrEmpty(status) ? (object)DBNull.Value : status);

            var dr = await cmd.ExecuteReaderAsync();

            while (await dr.ReadAsync())
            {
                int applicationId = Convert.ToInt32(dr["c_application_id"]);

                if (!applications.ContainsKey(applicationId))
                {
                    var app = new t_apply_job
                    {
                        c_application_id = applicationId,
                        c_apply_date = dr["c_apply_date"].ToString(),
                        c_status = dr["c_status"].ToString(),
                        c_ats_score = dr["c_ats_score"] != DBNull.Value ? Convert.ToInt32(dr["c_ats_score"]) : 0,
                        c_job_id = dr["c_job_id"] != DBNull.Value ? Convert.ToInt32(dr["c_job_id"]) : 0,
                        c_uid = dr["c_uid"] != DBNull.Value ? Convert.ToInt32(dr["c_uid"]) : 0,
                        user = new t_user
                        {
                            c_userId = Convert.ToInt32(dr["c_uid"]),
                            c_fullName = dr["c_fullname"]?.ToString(),
                            c_email = dr["c_email"]?.ToString(),
                            c_phoneNumber = dr["c_phone_number"]?.ToString(),
                            c_profileImage = dr["c_image"]?.ToString(),
                        },
                        userResume = new t_UserResume
                        {
                            c_ResumeFilePath = dr["c_resumefilepath"]?.ToString(),
                        },
                        job_Post = new Job_Post
                        {
                            c_job_title = dr["c_job_title"]?.ToString(),
                            c_job_id = dr["c_job_id"] != DBNull.Value ? Convert.ToInt32(dr["c_job_id"]) : 0,
                        },
                        education_Details = new List<t_Education_Details>(),
                        work_Experience = new List<t_Work_Experience>()
                    };

                    applications[applicationId] = app;
                }

                var currentApp = applications[applicationId];

                // Add Education if not already exists
                if (!string.IsNullOrEmpty(dr["c_degree"]?.ToString()))
                {
                    var edu = new t_Education_Details
                    {
                        c_HighestQualification = dr["c_HighestQualification"]?.ToString(),
                        c_Degree = dr["c_Degree"]?.ToString(),
                        c_UniversityName = dr["c_UniversityName"]?.ToString(),
                        c_Specialization = dr["c_Specialization"]?.ToString(),
                        c_YearOfPassing = dr["c_YearOfPassing"] != DBNull.Value ? Convert.ToInt32(dr["c_YearOfPassing"]) : 0,
                        c_percentage = Convert.ToInt32(dr["c_percentage"]),
                    };

                    if (!currentApp.education_Details.Any(e =>
                        e.c_Degree == edu.c_Degree &&
                        e.c_UniversityName == edu.c_UniversityName &&
                        e.c_YearOfPassing == edu.c_YearOfPassing))
                    {
                        currentApp.education_Details.Add(edu);
                    }
                }

                // Add Work Experience if not already exists
                if (!string.IsNullOrEmpty(dr["c_companyname"]?.ToString()))
                {
                    var work = new t_Work_Experience
                    {
                        c_CompanyName = dr["c_CompanyName"]?.ToString(),
                        c_JobTitle = dr["c_JobTitle"]?.ToString(),
                        c_years_work = dr["c_years_work"] != DBNull.Value ? Convert.ToInt32(dr["c_years_work"]) : 0
                    };

                    if (!currentApp.work_Experience.Any(w =>
                        w.c_CompanyName == work.c_CompanyName &&
                        w.c_JobTitle == work.c_JobTitle &&
                        w.c_years_work == work.c_years_work))
                    {
                        currentApp.work_Experience.Add(work);
                    }
                }
            }

            return applications.Values.ToList();
        }
        catch (Exception ex)
        {
            throw;
        }
        finally
        {
            await _conn.CloseAsync();
        }
    }

    public async Task<int> UpdateStatusOfJobApplication(t_apply_job apply_Job)
    {
        try
        {
            await _conn.OpenAsync();
            string qry = "update t_apply_jobs set c_status=@c_status where c_application_id=@c_application_id";
            var cmd = new NpgsqlCommand(qry, _conn);
            cmd.Parameters.AddWithValue("@c_status", apply_Job.c_status);
            cmd.Parameters.AddWithValue("@c_application_id", apply_Job.c_application_id);
            await cmd.ExecuteNonQueryAsync();
            return 1;
        }
        catch (System.Exception ex)
        {
            System.Console.WriteLine("Error while update job application status:" + ex.Message);
            return 0;
        }
        finally
        {
            await _conn.CloseAsync();
        }
    }

    public async Task<List<t_apply_job>> GetUserAppliedJobApplication(int id)
    {
        List<t_apply_job> tt = new List<t_apply_job>();
        try
        {
            await _conn.OpenAsync();
            string qry = @"SELECT 
                        aj.c_application_id,
                        aj.c_job_id,
                        aj.c_status,
                        jp.c_job_title AS JobTitle,
                        c.c_company_name AS CompanyName,
                        c.c_company_logo AS CompanyLogo,
                        jp.c_job_location AS Location,
                        aj.c_apply_date
                    FROM t_apply_jobs aj
                    INNER JOIN t_user u ON aj.c_uid = u.c_uid
                    INNER JOIN t_job_post jp ON jp.c_job_id = aj.c_job_id
                    INNER JOIN t_companies c ON jp.c_company_id = c.c_company_id
                    WHERE u.c_uid = @c_uid;";
            var cmd = new NpgsqlCommand(qry, _conn);
            cmd.Parameters.AddWithValue("@c_uid", id);
            var dr = await cmd.ExecuteReaderAsync();
            if (dr.HasRows)
            {
                while (await dr.ReadAsync())
                {
                    tt.Add(new t_apply_job
                    {
                        c_application_id = Convert.ToInt32(dr["c_application_id"]),
                        c_status = dr["c_status"]?.ToString(),
                        c_job_id = Convert.ToInt32(dr["c_job_id"]),
                        c_apply_date = dr["c_apply_date"].ToString(),
                        job_Post = new Job_Post
                        {
                            c_job_title = dr["JobTitle"]?.ToString(),
                            c_job_location = dr["Location"]?.ToString(),
                        },
                        company = new t_Company
                        {
                            c_company_name = dr["CompanyName"]?.ToString(),
                            c_company_logo = dr["CompanyLogo"]?.ToString()
                        }
                    });
                }
            }
        }
        catch (System.Exception ex)
        {
            System.Console.WriteLine("Error while getting user applied job application:" + ex.Message);
        }
        finally
        {
            await _conn.CloseAsync();
        }
        return tt;
    }

    public async Task<List<Job_Post>> GetJobTitles(int id)
    {
        List<Job_Post> tt = new List<Job_Post>();
        try
        {
            await _conn.OpenAsync();
            string qry = "select c_job_title from t_job_post where c_company_id=@c_company_id";
            var cmd = new NpgsqlCommand(qry, _conn);
            cmd.Parameters.AddWithValue("@c_company_id", id);
            var dr = await cmd.ExecuteReaderAsync();
            if (dr.HasRows)
            {
                while (await dr.ReadAsync())
                {
                    tt.Add(new Job_Post
                    {
                        c_job_title = dr["c_job_title"].ToString(),
                    });
                }
            }
        }
        catch (System.Exception ex)
        {
            System.Console.WriteLine("Error while fetching job title:" + ex.Message);
        }
        finally
        {
            await _conn.CloseAsync();
        }
        return tt;
    }

    public async Task<int> InterviewSchedule(t_interview_schedule interview_Schedule)
    {
        try
        {
            Random random = new Random();
            int num = random.Next(1000, 10000); // generates a number from 1000 to 9999
            string url = "http://localhost:5000/UserDashboard/Meeting?roomID=" + num;
            interview_Schedule.c_meeting_url = url;

            await _conn.OpenAsync();

            // Check if the interview is already scheduled
            string checkQry = "SELECT 1 FROM t_interview_schedule WHERE c_job_id=@c_job_id and c_user_id=@c_user_id;";
            var checkCmd = new NpgsqlCommand(checkQry, _conn);
            checkCmd.Parameters.AddWithValue("@c_job_id", interview_Schedule.c_job_id);
            checkCmd.Parameters.AddWithValue("@c_user_id", interview_Schedule.c_user_id);

            var dr = await checkCmd.ExecuteReaderAsync();
            if (await dr.ReadAsync())
            {
                await dr.CloseAsync(); // Close reader before returning
                return 2; // Interview already scheduled
            }
            await dr.CloseAsync(); // Close reader to avoid conflict

            // 1. Insert into interview schedule table
            string qry = "INSERT INTO t_interview_schedule (c_user_id, c_interview_date, c_interview_time, c_company_id, c_meeting_url,c_interview_status,c_job_id) " +
                         "VALUES (@c_user_id, @c_interview_date, @c_interview_time, @c_company_id, @c_meeting_url,@c_interview_status,@c_job_id)";
            var cmd = new NpgsqlCommand(qry, _conn);
            cmd.Parameters.AddWithValue("@c_user_id", interview_Schedule.c_user_id);
            cmd.Parameters.AddWithValue("@c_interview_date", DateTime.Parse(interview_Schedule.c_interview_date));
            cmd.Parameters.AddWithValue("@c_interview_time", TimeSpan.Parse(interview_Schedule.c_interview_time));
            cmd.Parameters.AddWithValue("@c_company_id", interview_Schedule.c_company_id);
            cmd.Parameters.AddWithValue("@c_meeting_url", url);
            cmd.Parameters.AddWithValue("@c_interview_status", interview_Schedule.c_interview_status);
            cmd.Parameters.AddWithValue("@c_job_id", interview_Schedule.c_job_id);
            await cmd.ExecuteNonQueryAsync();
            return 1;
        }
        catch (System.Exception ex)
        {
            System.Console.WriteLine("Error while scheduling interview: " + ex.Message);
            return 0;
        }
        finally
        {
            await _conn.CloseAsync();
        }
    }


    public async Task<List<t_interview_schedule>> GetInterviews_ScheduleByCompany(int id)
    {
        List<t_interview_schedule> tt = new List<t_interview_schedule>();
        try
        {
            await _conn.OpenAsync();
            string qry = @"select ts.c_interview_status,ts.c_meeting_url,ts.c_interview_id,ts.c_interview_date,ts.c_interview_time,u.c_uid,u.c_fullname,u.c_email,u.c_phone_number, c_image ,jp.c_job_title,jp.c_job_id
                        from t_interview_schedule ts
                        inner join t_user u
                        on ts.c_user_id = u.c_uid
                        inner join t_job_post jp
                        on ts.c_job_id = jp.c_job_id
                        where ts.c_company_id = @c_company_id
                        order by c_interview_date asc;";
            var cmd = new NpgsqlCommand(qry, _conn);
            cmd.Parameters.AddWithValue("@c_company_id", id);
            var dr = await cmd.ExecuteReaderAsync();
            if (dr.HasRows)
            {
                while (await dr.ReadAsync())
                {
                    tt.Add(new t_interview_schedule
                    {
                        user = new t_user
                        {
                            c_userId = Convert.ToInt32(dr["c_uid"]),
                            c_fullName = dr["c_fullName"].ToString(),
                            c_email = dr["c_email"].ToString(),
                            c_phoneNumber = dr["c_phone_number"].ToString(),
                            c_profileImage = dr["c_image"].ToString(),
                        },
                        c_interview_id = Convert.ToInt32(dr["c_interview_id"]),
                        c_interview_date = dr["c_interview_date"].ToString(),
                        c_interview_time = dr["c_interview_time"].ToString(),
                        c_meeting_url = dr["c_meeting_url"].ToString(),
                        c_interview_status = dr["c_interview_status"].ToString(),
                        job_post = new Job_Post
                        {
                            c_job_id = Convert.ToInt32(dr["c_job_id"]),
                            c_job_title = dr["c_job_title"].ToString()
                        }
                    });
                }
            }
        }
        catch (System.Exception ex)
        {
            System.Console.WriteLine("Error while fetching schedule interviews by company:" + ex.Message);
        }
        finally
        {
            await _conn.CloseAsync();
        }
        return tt;
    }

    public async Task<int> UpdateInterviewSchedule(t_interview_schedule interview_Schedule)
    {
        try
        {
            // Check if the interview status is already 'Done'
            string checkStatusQry = "SELECT c_interview_status FROM t_interview_schedule WHERE c_interview_id=@c_interview_id";
            var checkStatusCmd = new NpgsqlCommand(checkStatusQry, _conn);
            checkStatusCmd.Parameters.AddWithValue("@c_interview_id", interview_Schedule.c_interview_id);
            await _conn.OpenAsync();
            var statusReader = await checkStatusCmd.ExecuteReaderAsync();
            if (await statusReader.ReadAsync())
            {
                string currentStatus = statusReader["c_interview_status"].ToString();
                await statusReader.CloseAsync();
                if (currentStatus == "Done")
                {
                    return 2; // Interview already done, cannot reschedule
                }
            }
            await statusReader.CloseAsync();

            Random random = new Random();
            int num = random.Next(1000, 10000);
            string url = "http://localhost:5000/UserDashboard/Meeting?roomID=" + num;
            interview_Schedule.c_meeting_url = url;

            string qry = "update t_interview_schedule set c_interview_date=@c_interview_date,c_interview_time=@c_interview_time where c_interview_id=@c_interview_id";
            var cmd = new NpgsqlCommand(qry, _conn);
            cmd.Parameters.AddWithValue("@c_interview_date", DateTime.Parse(interview_Schedule.c_interview_date));
            cmd.Parameters.AddWithValue("@c_interview_time", TimeSpan.Parse(interview_Schedule.c_interview_time));
            cmd.Parameters.AddWithValue("@c_interview_id", interview_Schedule.c_interview_id);
            cmd.Parameters.AddWithValue("@c_meeting_url", url);
            await cmd.ExecuteNonQueryAsync();
            return 1;
        }
        catch (System.Exception ex)
        {
            System.Console.WriteLine("Error while updating interview schedule:" + ex.Message);
            return 0;
        }
        finally
        {
            await _conn.CloseAsync();
        }
    }

    public async Task<int> UserAppliedStatus(int userid, int jobid)
    {
        try
        {
            await _conn.OpenAsync();
            string qry = "select * from t_apply_jobs where c_job_id=@c_job_id and c_uid = @c_uid";
            var cmd = new NpgsqlCommand(qry, _conn);
            cmd.Parameters.AddWithValue("@c_uid", userid);
            cmd.Parameters.AddWithValue("@c_job_id", jobid);
            var dr = await cmd.ExecuteReaderAsync();
            if (dr.HasRows)
            {
                return 1;
            }
            return -1;
        }
        catch (System.Exception ex)
        {
            System.Console.WriteLine("Error while getting user applied job status:" + ex.Message);
            return 0;
        }
        finally
        {
            await _conn.CloseAsync();
        }
    }

    public async Task<int> UpdateInterviewScheduleStatus(t_interview_schedule interview_Schedule)
    {
        try
        {
            await _conn.OpenAsync();
            string qry = "update t_interview_schedule set c_interview_status=@c_interview_status where c_interview_id=@c_interview_id";
            var cmd = new NpgsqlCommand(qry, _conn);
            cmd.Parameters.AddWithValue("@c_interview_status", interview_Schedule.c_interview_status);
            cmd.Parameters.AddWithValue("@c_interview_id", interview_Schedule.c_interview_id);
            await cmd.ExecuteNonQueryAsync();
            return 1;
        }
        catch (System.Exception ex)
        {
            System.Console.WriteLine("Error while updating interview status:" + ex.Message);
            return 0;
        }
        finally
        {
            await _conn.CloseAsync();
        }
    }

    public Task<List<t_interview_schedule>> GetInterviewDoneCandidates(int id)
    {
        List<t_interview_schedule> tt = new List<t_interview_schedule>();
        try
        {
            _conn.Open();
            string qry = @"select ts.c_interview_status,ts.c_meeting_url,ts.c_interview_id,ts.c_interview_date,ts.c_interview_time,u.c_uid,u.c_fullname,u.c_email,u.c_phone_number,c_image,
                        c.c_company_name,c.c_company_email,c.c_company_logo,jp.c_job_title,jp.c_job_id
                        from t_interview_schedule ts
                        inner join t_user u
                        on ts.c_user_id = u.c_uid
                        inner join t_companies c
                        on ts.c_company_id = c.c_company_id
                        inner join t_job_post jp
                        on ts.c_job_id = jp.c_job_id
                        where ts.c_company_id = @c_company_id and ts.c_interview_status='Done'
                        order by c_interview_date asc;";
            var cmd = new NpgsqlCommand(qry, _conn);
            cmd.Parameters.AddWithValue("@c_company_id", id);
            var dr = cmd.ExecuteReader();
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    tt.Add(new t_interview_schedule
                    {
                        user = new t_user
                        {
                            c_userId = Convert.ToInt32(dr["c_uid"]),
                            c_fullName = dr["c_fullName"].ToString(),
                            c_email = dr["c_email"].ToString(),
                            c_phoneNumber = dr["c_phone_number"].ToString(),
                            c_profileImage = dr["c_image"].ToString(),
                        },
                        company = new t_Company
                        {
                            c_company_name = dr["c_company_name"].ToString(),
                            c_company_email = dr["c_company_email"].ToString(),
                            c_company_logo = dr["c_company_logo"].ToString()
                        },
                        c_interview_id = Convert.ToInt32(dr["c_interview_id"]),
                        c_interview_date = dr["c_interview_date"].ToString(),
                        c_interview_time = dr["c_interview_time"].ToString(),
                        c_meeting_url = dr["c_meeting_url"].ToString(),
                        c_interview_status = dr["c_interview_status"].ToString(),
                        job_post = new Job_Post
                        {
                            c_job_title = dr["c_job_title"].ToString(),
                            c_job_id = Convert.ToInt32(dr["c_job_id"])
                        }
                    });
                }
            }
        }
        catch (System.Exception ex)
        {
            System.Console.WriteLine("Error while fetching schedule interviews by company:" + ex.Message);
        }
        finally
        {
            _conn.Close();
        }
        return Task.FromResult(tt);
    }

    public async Task<int> HireCandidate(t_hired_candidate hired_Candidate)
    {
        try
        {
            await _conn.OpenAsync();

            // Check if the candidate is already hired
            string checkQry = "SELECT 1 FROM t_hired_candidates WHERE c_user_id = @c_user_id AND c_company_id = @c_company_id";
            var checkCmd = new NpgsqlCommand(checkQry, _conn);
            checkCmd.Parameters.AddWithValue("@c_user_id", hired_Candidate.c_user_id);
            checkCmd.Parameters.AddWithValue("@c_company_id", hired_Candidate.c_company_id);

            var dr = await checkCmd.ExecuteReaderAsync();
            if (await dr.ReadAsync())
            {
                await dr.CloseAsync(); // Close reader before returning
                return 2; // Already hired
            }
            await dr.CloseAsync(); // Close reader to avoid conflict

            // Insert into hired candidates table
            string qry = "insert into t_hired_candidates (c_user_id, c_company_id, c_status,c_job_id) values (@c_user_id, @c_company_id, @c_status,@c_job_id)";
            var cmd = new NpgsqlCommand(qry, _conn);
            cmd.Parameters.AddWithValue("@c_user_id", hired_Candidate.c_user_id);
            cmd.Parameters.AddWithValue("@c_company_id", hired_Candidate.c_company_id);
            cmd.Parameters.AddWithValue("@c_status", hired_Candidate.c_status);
            cmd.Parameters.AddWithValue("@c_job_id", hired_Candidate.c_job_id);
            await cmd.ExecuteNonQueryAsync();
            return 1;
        }
        catch (System.Exception ex)
        {
            System.Console.WriteLine("Error while hiring candidate:" + ex.Message);
            return 0;
        }
        finally{
            await _conn.CloseAsync();
        }
    }

    public async Task<List<t_hired_candidate>> GetHiredCandidates(int id)
    {
        List<t_hired_candidate> tt = new List<t_hired_candidate>();
        try
        {
            await _conn.OpenAsync();
            string qry = @"select hc.*,u.c_fullname,u.c_email,u.c_image,u.c_phone_number,jp.c_job_title,hc.c_hire_date
                        from t_hired_candidates hc
                        inner join t_user u
                        on u.c_uid = hc.c_user_id
                        inner join t_job_post jp
                        on jp.c_job_id = hc.c_job_id
                        where hc.c_company_id = @c_company_id;";
            var cmd = new NpgsqlCommand(qry, _conn);
            cmd.Parameters.AddWithValue("@c_company_id", id);
            var dr = await cmd.ExecuteReaderAsync();
            if (dr.HasRows)
            {
                while (await dr.ReadAsync())
                {
                    tt.Add(new t_hired_candidate
                    {
                        c_hired_date = dr["c_hire_date"].ToString(),
                        user = new t_user
                        {
                            c_userId = Convert.ToInt32(dr["c_user_id"]),
                            c_fullName = dr["c_fullname"].ToString(),
                            c_email = dr["c_email"].ToString(),
                            c_profileImage = dr["c_image"].ToString(),
                            c_phoneNumber = dr["c_phone_number"].ToString(),
                        },
                        c_status = dr["c_status"].ToString(),
                        job_Post = new Job_Post
                        {
                            c_job_title = dr["c_job_title"].ToString(),
                        },
                    });
                }
            }
        }
        catch (System.Exception ex)
        {
            System.Console.WriteLine("Error while fetching hired candidates:" + ex.Message);
        }
        finally
        {
            await _conn.CloseAsync();
        }
        return tt;
    }

    public async Task<int> GetNotificationCount(int companyId)
{
    int count = 0;
    try
    {
        _conn.Open();
        var query = "SELECT COUNT(*) FROM t_notifications WHERE c_is_read = false AND c_company_id = @companyId";
        using (var cmd = new NpgsqlCommand(query, _conn))
        {
            cmd.Parameters.AddWithValue("@companyId", companyId);
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


    public async Task<int> MarkAllNotificationsAsRead(int companyId)
{
    int rowsAffected = 0;
    try
    {
        await _conn.OpenAsync();
        var query = "UPDATE t_notifications SET c_is_read = true WHERE c_is_read = false AND c_company_id = @companyId";
        using (var cmd = new NpgsqlCommand(query, _conn))
        {
            cmd.Parameters.AddWithValue("@companyId", companyId);
            rowsAffected = await cmd.ExecuteNonQueryAsync();
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

    return rowsAffected;
}

public async Task<bool> DeleteMultipleNotifications(List<int> notificationIds)
{
    try
    {
        await _conn.OpenAsync();
        var query = "DELETE FROM t_notifications WHERE c_notification_id = ANY(@notificationIds)";
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

    public async Task<List<Notiy>> GetUnreadNotifications(int companyId)
{
    var notifications = new List<Notiy>();
    try
    {
        await _conn.OpenAsync();
        var query = @"
            SELECT c_notification_id, c_company_id, c_message, c_is_read, c_created_at
            FROM t_notifications
            WHERE c_is_read = false AND c_company_id = @companyId
            ORDER BY c_created_at DESC";
        
        using (var cmd = new NpgsqlCommand(query, _conn))
        {
            cmd.Parameters.AddWithValue("@companyId", companyId);
            using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    notifications.Add(new Notiy
                    {
                        NotificationId = reader.GetInt32(0),
                        CompanyId = reader.GetInt32(1),
                        Message = reader.GetString(2),
                        IsRead = reader.GetBoolean(3),
                        CreatedAt = reader.GetDateTime(4)
                    });
                }
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
public async Task<List<Notiy>> GetAllNotifications(int companyId)
{
    var notifications = new List<Notiy>();
    try
    {
        await _conn.OpenAsync();
        var query = @"
            SELECT c_notification_id, c_company_id, c_message, c_is_read, c_created_at
            FROM t_notifications
            WHERE c_company_id = @companyId
            ORDER BY c_created_at DESC";
        
        using (var cmd = new NpgsqlCommand(query, _conn))
        {
            cmd.Parameters.AddWithValue("@companyId", companyId);
            using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    notifications.Add(new Notiy
                    {
                        NotificationId = reader.GetInt32(0),
                        CompanyId = reader.GetInt32(1),
                        Message = reader.GetString(2),
                        IsRead = reader.GetBoolean(3),
                        CreatedAt = reader.GetDateTime(4)
                    });
                }
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
}