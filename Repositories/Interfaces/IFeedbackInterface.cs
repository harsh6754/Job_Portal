using Repositories.Model;
public interface IFeedbackInterface
{
    Task<int> SubmitFeedback(t_Feedback feedback);
    Task<List<t_Feedback>> GetFeedBack();
}