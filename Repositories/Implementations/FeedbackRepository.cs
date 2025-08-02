using Npgsql;
using Repositories.Model;

public class FeedbackRepository : IFeedbackInterface
{
    private readonly NpgsqlConnection _conn;

    public FeedbackRepository(NpgsqlConnection conn){
        _conn = conn;
    }

    public async Task<int> SubmitFeedback(t_Feedback feedback)
    {
        try
        {
            await _conn.OpenAsync();
            string qry = "INSERT INTO t_feedback (c_role, c_user_email, c_rating, c_feedback_msg, c_date) " +
                        "VALUES (@c_role, @c_user_email, @c_rating, @c_feedback_msg, @c_date)";
            var cmd = new NpgsqlCommand(qry, _conn);

            // Set parameters
            cmd.Parameters.AddWithValue("@c_role", feedback.c_role);
            cmd.Parameters.AddWithValue("@c_user_email", feedback.c_user_email);
            cmd.Parameters.AddWithValue("@c_rating", feedback.c_rating);
            cmd.Parameters.AddWithValue("@c_feedback_msg", feedback.c_feedback_msg);

            // 👇 Set current date/time as string if your column is varchar
            cmd.Parameters.AddWithValue("@c_date", DateTime.Now.Date); // ✅ Sends a proper DATE type

            await cmd.ExecuteNonQueryAsync();
            return 1;

        }
        catch (System.Exception ex)
        {
            System.Console.WriteLine("Error while submit feedback:"+ex.Message);
            return 0;
        }
        finally{
            await _conn.CloseAsync();
        }
    }

    public async Task<List<t_Feedback>> GetFeedBack()
    {
        List<t_Feedback> tt = new List<t_Feedback>();
        try
        {
            await _conn.OpenAsync();
            string qry = "select c_role,c_user_email,c_rating,c_feedback_msg,c_date from t_feedback";
            var cmd = new NpgsqlCommand(qry,_conn);
            var dr = await cmd.ExecuteReaderAsync();
            if(dr.HasRows){
                while (await dr.ReadAsync())
                {
                    tt.Add(new t_Feedback{
                        c_role = dr["c_role"].ToString(),
                        c_user_email = dr["c_user_email"].ToString(),
                        c_rating = Convert.ToInt32(dr["c_rating"]),
                        c_feedback_msg = dr["c_feedback_msg"].ToString(),
                        c_date = dr["c_date"].ToString(),
                    });
                }
            }
        }
        catch (System.Exception ex)
        {
            System.Console.WriteLine("Error while getting feedbacked:"+ex.Message);
        }
        finally{
            await _conn.CloseAsync();
        }
        return tt;
    }
}