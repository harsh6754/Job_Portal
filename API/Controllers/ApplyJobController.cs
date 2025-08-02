using MailKit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repositories.Model;

namespace MyApp.Namespace
{
    [Route("api/[controller]")]
    [ApiController]


    public class ApplyJobController : ControllerBase
    {
        private readonly IApplyjobInterface _applyjobInterface;
        private readonly EmailService _emailService;
        public ApplyJobController(IApplyjobInterface applyjobInterface, EmailService emailService)
        {
            _applyjobInterface = applyjobInterface;
            _emailService = emailService;
        }

        [HttpGet("CheckFields/{id}")]
        public async Task<IActionResult> CheckFields(int id)
        {
            var res = await _applyjobInterface.CheckFields(id);
            if (res != null)
            {
                return Ok(new
                {
                    success = true,
                    msg = "Field check successful",
                    data = res
                });
            }
            else
            {
                return BadRequest(new
                {
                    success = false,
                    msg = "User not found or required fields missing",
                    data = res
                });
            }
        }



        [HttpPost("ApplyJob")]
        public async Task<IActionResult> ApplyJob([FromForm] t_apply_job apply_Job)
        {
            var res = await _applyjobInterface.ApplyJob(apply_Job);

            if (res == 1)
            {
                return Ok(new
                {
                    success = true,
                    msg = "Job applied successfully.",
                    data = res
                });
            }
            else if (res == 2)
            {
                return Ok(new
                {
                    success = false,
                    msg = "You have already applied for this job.",
                    data = res
                });
            }
            else
            {
                return BadRequest(new
                {
                    success = false,
                    msg = "Failed to apply for the job due to an error.",
                    data = res
                });
            }
        }


        // [HttpDelete("DeleteAppliedJobs/{id}")]
        // public async Task<IActionResult> DeleteAppliedJobs(int id){
        //     var res = await _applyjobInterface.DeleteJob(id);
        //     if(res != null){
        //         return Ok(new {
        //             success = true,
        //             msg = "Deleted applied job",
        //             data = res
        //         });
        //     }
        //     else{
        //         return BadRequest(new {
        //             success = true,
        //             msg = "Deletion failed for applied job",
        //             data = res
        //         });
        //     }
        // }

        [HttpGet("GetApplyJobApplication/{id}")]
        public async Task<List<t_apply_job>> GetApplyJobApplication(int id, string? jobTitle = null, string? status = null)
        {
            List<t_apply_job> tt = await _applyjobInterface.GetApplyJobApplication(id, jobTitle, status);
            return tt;
        }

        [HttpPut("UpdateStatusOfJobApplication")]
        public async Task<IActionResult> UpdateStatusOfJobApplication([FromForm] t_apply_job apply_Job)
        {
            var res = await _applyjobInterface.UpdateStatusOfJobApplication(apply_Job);
            if (res != null)
            {
                return Ok(new
                {
                    success = true,
                    msg = "Success update status of job application",
                    data = res
                });
            }
            else
            {
                return BadRequest(new
                {
                    success = false,
                    msg = "Failed to update status of job application",
                    data = res
                });
            }
        }

        [HttpGet("GetUserAppliedJobApplication/{id}")]
        public async Task<List<t_apply_job>> GetUserAppliedJobApplication(int id)
        {
            List<t_apply_job> tt = await _applyjobInterface.GetUserAppliedJobApplication(id);
            return tt;
        }

        [HttpGet("GetJobTitle/{id}")]
        public async Task<List<Job_Post>> GetJobTitle(int id)
        {
            List<Job_Post> tt = await _applyjobInterface.GetJobTitles(id);
            return tt;
        }

        [HttpPost("InterviewSchedule")]
        public async Task<IActionResult> InterviewSchedule([FromForm] t_interview_schedule interview_Schedule)
        {
            var res = await _applyjobInterface.InterviewSchedule(interview_Schedule);
            // var res = 0;
            if (res == 1)
            {
                string body = $@"
                   <!DOCTYPE html>
                    <html lang='en'>
                    <head>
                        <meta charset='UTF-8'>
                        <title>Interview Scheduled</title>
                        <meta name='viewport' content='width=device-width, initial-scale=1.0'/>
                        <style>
                            * {{
                                box-sizing: border-box;
                            }}

                            body {{
                                margin: 0;
                                padding: 0;
                                font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
                                background-color: #e9edf2;
                                width: 100%;
                            }}

                            .page {{
                                width: 100%;
                                min-height: 80vh;
                                padding: 50px 20px;
                                display: flex;
                                align-items: center;
                                justify-content: center;
                            }}
                            .email-wrapper {{
                                width: 100%;
                                max-width: 800px;
                                background-color: #ffffff;
                                border: 1px solid #dcdcdc;
                                border-radius: 12px;
                                box-shadow: 0 6px 20px rgba(0, 0, 0, 0.08);
                                overflow: hidden;
                            }}

                            .email-header {{
                                background-color: #f1f5f9;
                                padding: 30px 20px;
                                text-align: center;
                            }}

                            .email-header img {{
                                width: 200px;
                                height: auto;
                            }}

                            .email-body {{
                                padding: 30px 40px;
                                text-align: center;
                            }}

                            .email-body h2 {{
                                color: black;
                                font-size: 30px;
                                margin-bottom: 25px;
                            }}

                            .email-body p {{
                                font-size: 17px;
                                color: #444;
                                line-height: 1.8;
                                margin-bottom: 18px;
                            }}

                            .highlight {{
                                font-weight: bold;
                                color: black;
                            }}

                            .email-body a.button {{
                                display: inline-block;
                                background-color: #0b65c3;
                                color: #fff;
                                padding: 14px 30px;
                                margin-top: 20px;
                                text-decoration: none;
                                border-radius: 6px;
                                font-size: 16px;
                                font-weight: bold;
                                transition: background-color 0.3s ease;
                            }}

                            .email-body a.button:hover {{
                                background-color: #094cb1;
                            }}

                            .email-footer {{
                                padding: 25px 20px;
                                font-size: 14px;
                                color: #888;
                                background-color: #f8f9fa;
                                text-align: center;
                                border-top: 1px solid #e0e0e0;
                            }}
                        </style>
                    </head>
                    <body>
                        <div class='page'>
                            <div class='email-wrapper'>
                                <div class='email-header'>
                                    <img src='https://res.cloudinary.com/dhruvil20/image/upload/v1743946773/2removebg-preview_x58xrm.png' alt='CareerLink Logo'>
                                </div>
                                <div class='email-body'>
                                    <h2>Interview Scheduled for {interview_Schedule.c_fullName}</h2>
                                    <p>Dear <span class='highlight'>{interview_Schedule.c_fullName}</span>,</p>
                                    <p>Your interview has been successfully scheduled.</p>
                                    <p><strong>Date:</strong> {interview_Schedule.c_interview_date}<br />
                                    <strong>Time:</strong> {interview_Schedule.c_interview_time}<br />
                                    <strong>Meeting Link:</strong> <a href='{interview_Schedule.c_meeting_url}' target='_blank'>{interview_Schedule.c_meeting_url}</a>
                                    </p>
                                    <p>Good luck! We’re excited to speak with you.</p>
                                    <a href='{interview_Schedule.c_meeting_url}' class='button'>Join Interview</a>
                                </div>
                                <div class='email-footer'>
                                    &copy; CareerLink. All rights reserved.
                                </div>
                            </div>
                        </div>
                    </body>
                    </html>";
                await _emailService.SendEmailAsync(
                    interview_Schedule.c_email,
                    "Interview Scheduled",
                    body
                );
                System.Console.WriteLine("Email send suucessfully to " + interview_Schedule.c_email);

                return Ok(new
                {
                    success = true,
                    msg = "Successfully interview scheduled and email sent",
                    data = res
                });
            }
            else if (res == 2)
            {
                return Ok(new
                {
                    success = false,
                    msg = "Already applied",
                    data = res
                });
            }
            else
            {
                return BadRequest(new
                {
                    success = false,
                    msg = "Failed to schedule interview schedule",
                    data = res
                });
            }
        }

        [HttpGet("GetInterviews_ScheduleByCompany/{id}")]
        public async Task<List<t_interview_schedule>> GetInterviews_ScheduleByCompany(int id)
        {
            List<t_interview_schedule> tt = await _applyjobInterface.GetInterviews_ScheduleByCompany(id);
            return tt;
        }

        [HttpPut("UpdateInterviewSchedule")]
        public async Task<IActionResult> UpdateInterviewSchedule([FromForm] t_interview_schedule interview_Schedule)
        {
            var res = await _applyjobInterface.UpdateInterviewSchedule(interview_Schedule);
            if (res == 1)
            {
            string body = $@"
                <!DOCTYPE html>
                    <html lang='en'>
                    <head>
                        <meta charset='UTF-8'>
                        <title>Interview Schedule Updated</title>
                        <meta name='viewport' content='width=device-width, initial-scale=1.0'/>
                        <style>
                        * {{
                            box-sizing: border-box;
                        }}

                        body {{
                            margin: 0;
                            padding: 0;
                            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
                            background-color: #e9edf2;
                            width: 100%;
                        }}

                        .page {{
                            width: 100%;
                            min-height: 80vh;
                            padding: 50px 20px;
                            display: flex;
                            align-items: center;
                            justify-content: center;
                        }}
                        .email-wrapper {{
                            width: 100%;
                            max-width: 800px;
                            background-color: #ffffff;
                            border: 1px solid #dcdcdc;
                            border-radius: 12px;
                            box-shadow: 0 6px 20px rgba(0, 0, 0, 0.08);
                            overflow: hidden;
                        }}

                        .email-header {{
                            background-color: #f1f5f9;
                            padding: 30px 20px;
                            text-align: center;
                        }}

                        .email-header img {{
                            width: 200px;
                            height: auto;
                        }}

                        .email-body {{
                            padding: 30px 40px;
                            text-align: center;
                        }}

                        .email-body h2 {{
                            color: black;
                            font-size: 30px;
                            margin-bottom: 25px;
                        }}

                        .email-body p {{
                            font-size: 17px;
                            color: #444;
                            line-height: 1.8;
                            margin-bottom: 18px;
                        }}

                        .highlight {{
                            font-weight: bold;
                            color: black;
                        }}

                        .email-body a.button {{
                            display: inline-block;
                            background-color: #0b65c3;
                            color: #fff;
                            padding: 14px 30px;
                            margin-top: 20px;
                            text-decoration: none;
                            border-radius: 6px;
                            font-size: 16px;
                            font-weight: bold;
                            transition: background-color 0.3s ease;
                        }}

                        .email-body a.button:hover {{
                            background-color: #094cb1;
                        }}

                        .email-footer {{
                            padding: 25px 20px;
                            font-size: 14px;
                            color: #888;
                            background-color: #f8f9fa;
                            text-align: center;
                            border-top: 1px solid #e0e0e0;
                        }}
                        </style>
                    </head>
                    <body>
                        <div class='page'>
                        <div class='email-wrapper'>
                            <div class='email-header'>
                            <img src='https://res.cloudinary.com/dhruvil20/image/upload/v1743946773/2removebg-preview_x58xrm.png' alt='CareerLink Logo'>
                            </div>
                            <div class='email-body'>
                            <h2>Interview Schedule Updated</h2>
                            <p>Dear <span class='highlight'>{interview_Schedule.c_fullName}</span>,</p>
                            <p>Your interview schedule has been <strong style='color:black;'>updated</strong>. Please find the new details below:</p>
                            <p>
                                <strong>Date:</strong> {interview_Schedule.c_interview_date}<br />
                                <strong>Time:</strong> {interview_Schedule.c_interview_time}<br />
                                <strong>Meeting Link:</strong> <a href='{interview_Schedule.c_meeting_url}' target='_blank'>{interview_Schedule.c_meeting_url}</a>
                            </p>
                            <p>Please make note of the new schedule and ensure your availability.</p>
                            <a href='{interview_Schedule.c_meeting_url}' class='button'>Join Updated Interview</a>
                            </div>
                            <div class='email-footer'>
                            &copy; CareerLink. All rights reserved.
                            </div>
                        </div>
                        </div>
                    </body>
                    </html>";

            await _emailService.SendEmailAsync(
                interview_Schedule.c_email,
                "Interview Schedule Updated",
                body
            );
            return Ok(new
            {
                success = true,
                msg = "Successfully updated interview schedule",
                data = res
            });
            }
            else if (res == 2)
            {
            return Ok(new
            {
                success = false,
                msg = "Interview already done, cannot reschedule",
                data = res
            });
            }
            else
            {
            return BadRequest(new
            {
                success = false,
                msg = "Failed to update interview schedule",
                data = res
            });
            }
        }


        [HttpPut("UpdateInterviewStatus")]
        public async Task<IActionResult> UpdateInterviewStatus([FromForm] t_interview_schedule interview_Schedule)
        {
            var res = await _applyjobInterface.UpdateInterviewScheduleStatus(interview_Schedule);
            if (res != null)
            {
                return Ok(new
                {
                    success = true,
                    msg = "Successfully updated interview status",
                    data = res
                });
            }
            else
            {
                return BadRequest(new
                {
                    success = false,
                    msg = "Failed to update interview status",
                    data = res
                });
            }
        }

        [HttpGet("UserAppliedStatus")]
        public async Task<IActionResult> UserAppliedStatus(int userid, int jobid)
        {
            var res = await _applyjobInterface.UserAppliedStatus(userid, jobid);
            if (res != null)
            {
                return Ok(new
                {
                    success = true,
                    msg = "Successfully getting user applied for job",
                    data = res
                });
            }
            else
            {
                return BadRequest(new
                {
                    success = false,
                    msg = "Failed to getting user applied for job",
                    data = res
                });
            }
        }

        [HttpGet("GetInterviewDoneCandidates/{id}")]
        public async Task<List<t_interview_schedule>> GetInterviewDoneCandidates(int id)
        {
            List<t_interview_schedule> tt = await _applyjobInterface.GetInterviewDoneCandidates(id);
            return tt;
        }

        [HttpPost("HireCandidate")]
        public async Task<IActionResult> HireCandidate([FromForm] t_hired_candidate hired_Candidate)
        {
            var res = await _applyjobInterface.HireCandidate(hired_Candidate);

            if (res == 1)
            {
                // Build HTML body
                           string htmlBody = $@"
                            <!DOCTYPE html>
                            <html lang='en'>
                            <head>
                                <meta charset='UTF-8'>
                                <title>You're Hired!</title>
                                <meta name='viewport' content='width=device-width, initial-scale=1.0'/>
                                <style>
                                    * {{
                                        box-sizing: border-box;
                                    }}

                                    body {{
                                        margin: 0;
                                        padding: 0;
                                        font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
                                        background-color: #e9edf2;
                                        width: 100%;
                                    }}
                                    .page {{
                                        width: 100%;
                                        min-height: 80vh;
                                        padding: 50px 20px;
                                        display: flex;
                                        align-items: center;
                                        justify-content: center;
                                    }}

                                    .email-wrapper {{
                                        width: 100%;
                                        max-width: 800px;
                                        background-color: #ffffff;
                                        border: 1px solid #dcdcdc;
                                        border-radius: 12px;
                                        box-shadow: 0 6px 20px rgba(0, 0, 0, 0.08);
                                        overflow: hidden;
                                    }}

                                    .email-header {{
                                        background-color: #f1f5f9;
                                        padding: 30px 20px;
                                        text-align: center;
                                    }}

                                    .email-header img {{
                                        max-height: 70px;
                                        object-fit: contain;
                                        background: white;
                                        padding: 6px;
                                        border-radius: 6px;
                                    }}

                                    .email-body {{
                                        padding: 30px 40px;
                                        text-align: center;
                                    }}

                                    .email-body h2 {{
                                        color: #2563eb;
                                        font-size: 30px;
                                        margin-bottom: 25px;
                                    }}

                                    .email-body p {{
                                        font-size: 17px;
                                        color: #444;
                                        line-height: 1.8;
                                        margin-bottom: 18px;
                                    }}

                                    .email-body a {{
                                        color: #2563eb;
                                        text-decoration: none;
                                        font-weight: bold;
                                    }}

                                    .email-footer {{
                                        padding: 25px 20px;
                                        font-size: 14px;
                                        color: #888;
                                        background-color: #f8f9fa;
                                        text-align: center;
                                        border-top: 1px solid #e0e0e0;
                                    }}
                                </style>
                            </head>
                            <body>
                                <div class='page'>
                                    <div class='email-wrapper'>
                                        <div class='email-header'>
                                            <img src='{hired_Candidate.c_company_logo}' alt='Company Logo'>
                                        </div>
                                        <div class='email-body'>
                                            <h2>🎉 Congratulations, {hired_Candidate.c_fullName}!</h2>

                            <p>We’re absolutely delighted to inform you that you’ve been <strong>officially hired</strong> by <strong>{hired_Candidate.c_company_name}</strong>! Your dedication, talent, and potential truly impressed us throughout the hiring process.</p>

                            <p>This is the beginning of an exciting chapter. At <strong>{hired_Candidate.c_company_name}</strong>, we’re not just building teams—we’re building a community of innovators, problem-solvers, and leaders. And now, you're part of it!</p>

                            <p><strong>What’s next?</strong><br/>
                            Our HR team will soon get in touch with you to provide all the necessary onboarding information including:<br/>
                            <ul style='text-align:left; margin: 0 auto; display:inline-block;'>
                            <li style='font-size: 16px;'>Start date and office hours</li>
                            <li style='font-size: 16px;'>Documentation requirements</li>
                            <li style='font-size: 16px;'>Initial training schedule</li>
                            <li style='font-size: 16px;'>Team introductions and communication channels</li>
                            </ul>
                            </p>

                            <p>In the meantime, feel free to:
                            <ul style='text-align:left; margin: 0 auto; display:inline-block;'>
                            <li style='font-size: 16px;'>Explore our website and social channels to know more about us</li>
                            <li style='font-size: 16px;'>Prepare any queries you may have</li>
                            <li style='font-size: 16px;'>Start thinking about how you'd like to grow with us</li>
                            </ul>
                            </p>

                            <p>If you need any assistance or have immediate questions, please don’t hesitate to reach out at <a href='mailto:{hired_Candidate.c_company_email}'>{hired_Candidate.c_company_email}</a>. We're here to support you at every step.</p>

                            <p style='margin-top: 30px;'>Once again, congratulations! We’re honored to have you join us and can’t wait to see the incredible things you’ll accomplish.</p>

                            <p style='margin-top: 30px;'>Warmest welcome,<br/>
                            <strong>{hired_Candidate.c_company_name} Team</strong></p>

                                        </div>
                                        <div class='email-footer'>
                                            <p>&copy; CareerLink. All rights reserved.</p>
                                        </div>
                                    </div>
                                </div>
                            </body>
                            </html>";
                // Send email
                await _emailService.SendEmailAsync(
                    hired_Candidate.c_user_email,
                    $"You're Hired at {hired_Candidate.c_company_name}!",
                    htmlBody
                );

                return Ok(new
                {
                    success = true,
                    msg = "Successfully hired candidate",
                    data = res
                });
            }
            else if (res == 2)
            {
                return Ok(new
                {
                    success = false,
                    msg = "Candidate is already hired",
                    data = res
                });
            }
            else
            {
                return BadRequest(new
                {
                    success = false,
                    msg = "Failed to hire candidate",
                    data = res
                });
            }
        }

        [HttpGet("GetHiredCandidates/{id}")]
        public async Task<List<t_hired_candidate>> GetHiredCandidates(int id)
        {
            List<t_hired_candidate> tt = await _applyjobInterface.GetHiredCandidates(id);
            return tt;
        }

        [HttpGet("GetUnreadCount/{companyId}")]
    public async Task<int> GetUnreadNotificationCount(int companyId)
    {
        return await _applyjobInterface.GetNotificationCount(companyId);
    }

    [HttpPut("MarkAllAsRead/{companyId}")]
    public async Task<int> MarkAllNotificationsAsRead(int companyId)
    {
        return await _applyjobInterface.MarkAllNotificationsAsRead(companyId);
    }

    [HttpGet("Unread/{companyId}")]
    public async Task<List<Notiy>> GetUnreadNotifications(int companyId)
    {
        return await _applyjobInterface.GetUnreadNotifications(companyId);
    }

    [HttpGet("All/{companyId}")]
    public async Task<List<Notiy>> GetAllNotifications(int companyId)
    {
        return await _applyjobInterface.GetAllNotifications(companyId);
    }

    [HttpDelete("DeleteMultiple")]
    public async Task<bool> DeleteMultipleNotifications([FromBody] List<int> notificationIds)
    {
        return await _applyjobInterface.DeleteMultipleNotifications(notificationIds);
    }

    }
}
