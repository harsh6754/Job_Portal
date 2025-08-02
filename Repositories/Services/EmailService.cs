using System;
using System.IO;
using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Text;
using Repositories.Interfaces;
using System.Net;
using System.Net.Mail;
using MailKit;

public class EmailService
{
    private readonly string _smtpServer = "smtp.gmail.com";
    private readonly int _smtpPort = 587;
    private readonly string _smtpUser;
    private readonly string _smtpPass;

    public EmailService()
    {
        _smtpUser = Environment.GetEnvironmentVariable("SMTP_USER") ?? "careerlink2025@gmail.com";
        _smtpPass = Environment.GetEnvironmentVariable("SMTP_PASS") ?? "nayv ftgb lewn ylzf";
    }

    public void SendApprovalEmail(string recruiterEmail, string companyName)
    {
        try
        {
            string emailBody = LoadEmailTemplate("ApprovalTemplate.cshtml", companyName, "", "");

            using (var smtpClient = new System.Net.Mail.SmtpClient(_smtpServer, _smtpPort))
            using (var mailMessage = new MailMessage())
            {
                smtpClient.Credentials = new NetworkCredential(_smtpUser, _smtpPass);
                smtpClient.EnableSsl = true;

                mailMessage.From = new MailAddress(_smtpUser, "CareerLink");
                mailMessage.To.Add(recruiterEmail);
                mailMessage.Subject = "Recruiter Approved - You Can Now Post Jobs";
                mailMessage.Body = emailBody;
                mailMessage.IsBodyHtml = true;

                smtpClient.Send(mailMessage);
                Console.WriteLine($"✅ Approval email sent to {recruiterEmail}");
            }
        }
        catch (SmtpException smtpEx)
        {
            Console.WriteLine($"❌ SMTP Error: {smtpEx.StatusCode} - {smtpEx.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Unexpected Error: {ex.Message}");
        }
    }

    public void SendRejectionEmail(string recruiterEmail, string companyName, string recruiterName, string reason)
    {
        try
        {
            string emailBody = LoadEmailTemplate("RejectionTemplate.cshtml", companyName, recruiterName, reason);

            using (var smtpClient = new System.Net.Mail.SmtpClient(_smtpServer, _smtpPort))
            using (var mailMessage = new MailMessage())
            {
                smtpClient.Credentials = new NetworkCredential(_smtpUser, _smtpPass);
                smtpClient.EnableSsl = true;

                mailMessage.From = new MailAddress(_smtpUser, "CareerLink");
                mailMessage.To.Add(recruiterEmail);
                mailMessage.Subject = "Recruiter Application Rejected";
                mailMessage.Body = emailBody;
                mailMessage.IsBodyHtml = true;

                smtpClient.Send(mailMessage);
                Console.WriteLine($"✅ Rejection email sent to {recruiterEmail}");
            }
        }
        catch (SmtpException smtpEx)
        {
            Console.WriteLine($"❌ SMTP Error: {smtpEx.StatusCode} - {smtpEx.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Unexpected Error: {ex.Message}");
        }
    }

    private string LoadEmailTemplate(string templateFile, string companyName, string recruiterName, string reason)
    {
        if (!File.Exists(templateFile))
        {
            Console.WriteLine("❌ Email template file not found.");
            return "Email template missing.";
        }

        string templateContent = File.ReadAllText(templateFile);
        templateContent = templateContent.Replace("{{CompanyName}}", companyName)
                                         .Replace("{{RecruiterName}}", recruiterName)
                                         .Replace("{{Reason}}", reason);

        return templateContent;
    }


    public void SendBlockStatusEmail(string email, string userName, bool isBlocked, string role)
    {
        // In your database, true = BLOCKED, false = UNBLOCKED
        // isBlocked == true => BLOCKED
        // isBlocked == false => UNBLOCKED

        string subject = isBlocked ? "Your CareerLink Account Has Been Blocked" : "Welcome Back to CareerLink!";
        string messageContent = "";
        string buttonLabel = isBlocked ? "Contact Support" : "Go to Dashboard";
        string buttonLink = isBlocked ? "mailto:careerlink2025@gmail.com" : "http://localhost:5000/"; // Replace with actual dashboard URL

        if (isBlocked)
        {
            messageContent = $@"
            <p>
                Dear <strong>{userName}</strong>,<br/><br/>
                Your CareerLink account has been <strong style='color: #d0342c;'>blocked</strong> by the admin. 
                You will no longer be able to log in or use our services.
            </p>";
        }
        else if (role == "Candidate")
        {
            messageContent = $@"
            <p>
                Great news, <strong>{userName}</strong>!<br/><br/>
                Your CareerLink account has been <strong style='color: #28a745;'>unblocked</strong> and is now fully active. 
                You can now log in and start applying for jobs, managing your profile, and accessing all features again.
            </p>";
        }
        else if (role == "Recruiter")
        {
            messageContent = $@"
            <p>
                Welcome back, <strong>{userName}</strong>!<br/><br/>
                Your CareerLink recruiter account has been <strong style='color: #28a745;'>unblocked</strong> and reactivated. 
                You can now log in, manage your company profile, and continue posting jobs and tracking applicants.
            </p>";
        }

        string emailBody = $@"
            <!DOCTYPE html>
            <html lang='en'>
            <head>
                <meta charset='UTF-8'>
                <title>Account Status - CareerLink</title>
                <meta name='viewport' content='width=device-width, initial-scale=1.0'/>
                <style>
                    body {{
                    margin: 0;
                padding: 0;
                height: 100vh;
                background-color: #f4f6f8;
                display: flex;
                align-items: center;
                justify-content: center;
                font-family: Arial, sans-serif;
                    }}
                    .page {{
                        padding: 50px 20px;
                        display: flex;
                        align-items: center;
                        justify-content: center;
                    }}
                    .email-wrapper {{
                        max-width: 800px;
                        background-color: #fff;
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
                        font-size: 28px;
                        margin-bottom: 25px;
                        color: #333;
                    }}
                    .email-body p {{
                        font-size: 17px;
                        color: #444;
                        line-height: 1.8;
                        margin-bottom: 18px;
                    }}
                    .email-footer {{
                        padding: 25px 20px;
                        font-size: 14px;
                        color: #888;
                        background-color: #f8f9fa;
                        text-align: center;
                        border-top: 1px solid #e0e0e0;
                    }}
                    .button {{
                        display: inline-block;
                        background-color:  #4361ee;
                        color: #ffffff !important;
                        padding: 14px 30px;
                        margin-top: 20px;
                        text-decoration: none;
                        border-radius: 6px;
                        font-size: 16px;
                        font-weight: bold;
                    }}
                    .button:hover {{
                        background-color: #094cb1;
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
                            <h2>{subject}</h2>
                            {messageContent}
                            <p>If you have any questions or face issues, feel free to reach out to our support team.</p>
                            <a href='{buttonLink}' class='button'>{buttonLabel}</a>
                        </div>
                        <div class='email-footer'>
                            &copy; CareerLink. All rights reserved.
                        </div>
                    </div>
                </div>
            </body>
            </html>";
        try
        {
            // using (var smtpClient = new SmtpClient(_smtpServer, _smtpPort))
            using (var smtpClient = new System.Net.Mail.SmtpClient(_smtpServer, _smtpPort))

            using (var mailMessage = new MailMessage())
            {
                smtpClient.Credentials = new NetworkCredential(_smtpUser, _smtpPass);
                smtpClient.EnableSsl = true;

                mailMessage.From = new MailAddress(_smtpUser, "CareerLink");
                mailMessage.To.Add(email);
                mailMessage.Subject = subject;
                mailMessage.Body = emailBody;
                mailMessage.IsBodyHtml = true;

                smtpClient.Send(mailMessage);
                Console.WriteLine($"✅ Block status email sent to {email}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Email send failed: {ex.Message}");
        }
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        using (var smtpClient = new System.Net.Mail.SmtpClient(_smtpServer)
        {
            Port = _smtpPort,
            Credentials = new NetworkCredential(_smtpUser, _smtpPass),
            EnableSsl = true,
        })
        {
            var mailMessage = new MailMessage
            {
                From = new MailAddress(_smtpUser),
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
            };

            mailMessage.To.Add(to);

            await smtpClient.SendMailAsync(mailMessage);
        }
    }

    public static implicit operator EmailService(MailService v)
    {
        throw new NotImplementedException();
    }
}
