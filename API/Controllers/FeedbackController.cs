using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repositories.Model;

namespace MyApp.Namespace
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeedbackController : ControllerBase
    {
        private readonly IFeedbackInterface _feedbackInterface;
        public FeedbackController(IFeedbackInterface feedbackInterface){
            _feedbackInterface = feedbackInterface;
        }

        [HttpPost("SubmitFeedback")]
        public async Task<IActionResult> SubmitFeedback([FromBody]t_Feedback feedback){
            var res = await _feedbackInterface.SubmitFeedback(feedback);
            if(res != null){
                return Ok(new {
                    success = true,
                    msg = "Feedback Submit Successfully",
                    data = res
                });
            }
            else{
                return BadRequest(new {
                    success = true,
                    msg = "Failed to submit Feedback",
                    data = res
                });
            }
        } 

        [HttpGet("GetFeedbacks")]
        public async Task<List<t_Feedback>> GetFeedbacks(){
            List<t_Feedback> tt = await _feedbackInterface.GetFeedBack();
            return tt;
        }
    }
}
