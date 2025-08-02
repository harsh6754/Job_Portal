using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Npgsql;
using Microsoft.Extensions.Configuration;
using Repositories.Interfaces;

namespace Repositories.Implementations
{
    public class DashboardRepository : IDashboardInterface
    {
        private readonly string _connectionString;
        public DashboardRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("pgcon");
        }
        public async Task<object> GetCounts(int companyid)
        {
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                await connection.OpenAsync();

                var counts = new
                {
                    totalJobs = await GetCountAsync(connection, "SELECT COUNT(*) FROM t_job_post WHERE c_company_id = @companyid", new NpgsqlParameter("@companyid", companyid)),
                    totalApplicants = await GetCountAsync(connection, "SELECT COUNT(*) FROM t_apply_jobs aj INNER JOIN t_job_post t ON aj.c_job_id = t.c_job_id WHERE t.c_company_id = @companyid", new NpgsqlParameter("@companyid", companyid)),
                    totalRejected = await GetCountAsync(connection, "SELECT count(*) FROM t_apply_jobs aj INNER JOIN t_job_post t ON aj.c_job_id = t.c_job_id WHERE t.c_company_id =  @companyid AND aj.c_status='Rejected'", new NpgsqlParameter("@companyid", companyid)),
                    totalInterviews = await GetCountAsync(connection, "SELECT COUNT(*) FROM t_interview_schedule WHERE c_company_id = @companyid", new NpgsqlParameter("@companyid", companyid)),
                    totalOffers = await GetCountAsync(connection, "SELECT count(*) FROM t_apply_jobs aj INNER JOIN t_job_post t ON aj.c_job_id = t.c_job_id WHERE t.c_company_id =  @companyid AND aj.c_status='Shortlisted'", new NpgsqlParameter("@companyid", companyid)),
                    totalHired = await GetCountAsync(connection, "SELECT COUNT(*) FROM t_hired_candidates WHERE c_company_id = @companyid", new NpgsqlParameter("@companyid", companyid))
                };

                return counts;
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.Message);
                return null;
            }
        }

        public async Task<List<object>> GetInterviewTrends(DateTime startDate, DateTime endDate, int companyid)
        {
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                await connection.OpenAsync();

                var trends = new List<object>();
                using var command = new NpgsqlCommand(
                    "SELECT c_interview_date AS date, COUNT(*) AS interviews " +
                    "FROM t_interview_schedule " +
                    "WHERE c_interview_date BETWEEN @startDate AND @endDate AND c_company_id=@Id " + // <-- space fixed here
                    "GROUP BY c_interview_date " +
                    "ORDER BY c_interview_date", connection);

                command.Parameters.AddWithValue("startDate", startDate);
                command.Parameters.AddWithValue("endDate", endDate);
                command.Parameters.AddWithValue("@Id", companyid);
                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    trends.Add(new
                    {
                        date = reader.GetDateTime(0).ToString("yyyy-MM-dd"),
                        interviews = reader.GetInt64(1)
                    });
                }

                return trends;
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.Message);
                return null;
            }
        }

        public async Task<List<object>> GetApplicationTrends(DateTime startDate, DateTime endDate, int companyid)
        {
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                await connection.OpenAsync();

                var trends = new List<object>();
                using var command = new NpgsqlCommand(
                @"SELECT 
                    aj.c_apply_date::date AS date, 
                    COUNT(*) AS applications
                FROM t_apply_jobs aj
                INNER JOIN t_job_post t ON aj.c_job_id = t.c_job_id
                WHERE aj.c_apply_date::date BETWEEN @startDate AND @endDate 
                    AND t.c_company_id = @CompanyId
                GROUP BY aj.c_apply_date::date
                ORDER BY aj.c_apply_date::date", connection);

                command.Parameters.AddWithValue("startDate", startDate);
                command.Parameters.AddWithValue("endDate", endDate);
                command.Parameters.AddWithValue("CompanyId", companyid);

                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    trends.Add(new
                    {
                        date = reader.GetDateTime(0).ToString("yyyy-MM-dd"), // safer with GetDateTime instead of GetString
                        applications = reader.GetInt64(1)
                    });
                }

                return trends;
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.Message);
                return null;
            }
        }

        private async Task<long> GetCountAsync(NpgsqlConnection connection, string query, params NpgsqlParameter[] parameters)
        {
            using var command = new NpgsqlCommand(query, connection);
            command.Parameters.AddRange(parameters);
            return (long)await command.ExecuteScalarAsync();
        }

    }
}