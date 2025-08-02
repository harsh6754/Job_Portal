using Microsoft.Extensions.Configuration;
using Npgsql;
using Repositories.Interfaces;

public class LandingRepository : ILandingPageInterface
{

    private readonly NpgsqlConnection _connectionString;
    public LandingRepository(NpgsqlConnection connectionString)
    {
        _connectionString = connectionString;
    }
    public async Task<List<int>> GetLandingDetails()
    {
        var qry = @"SELECT 
    (SELECT COUNT(*) FROM t_job_post) AS JobPostedCount,
    (SELECT COUNT(*) FROM t_job_post where c_expire_date > NOW()) AS ActiveJobCount,
    (SELECT COUNT(*) FROM t_companies) AS CompaniesCount,
    (SELECT COUNT(*) FROM t_user) AS RegisterCandidateCount,
    (SELECT COUNT(*) FROM t_interview_schedule) AS InterviewHeldCount,
    (SELECT COUNT(*) FROM t_hired_candidates) AS HiredCandidatesCount
    ;
";

     var LandingDetails = new List<int>();
            try
            {
                await _connectionString.OpenAsync();
                using (var cmd = new NpgsqlCommand(qry, _connectionString))
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var jobPostedCount = reader.GetInt32(0);
                        var activeJobCount = reader.GetInt32(1);
                        var companiesCount = reader.GetInt32(2);
                        var registerCandidateCount = reader.GetInt32(3);
                        var interviewHeldCount = reader.GetInt32(4);
                        var HiredCandidatesCount = reader.GetInt32(5);

                        LandingDetails.Add(jobPostedCount);
                        LandingDetails.Add(activeJobCount);
                        LandingDetails.Add(companiesCount);
                        LandingDetails.Add(registerCandidateCount);
                        LandingDetails.Add(interviewHeldCount);
                        LandingDetails.Add(HiredCandidatesCount);
                    }
                
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions as needed
                Console.WriteLine(ex.Message);
            }
            finally
            {
                await _connectionString.CloseAsync();
            }
            return LandingDetails;
    }
}