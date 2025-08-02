using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Repositories.Interfaces;
using Repositories.Model;
using StackExchange.Redis;
using RabbitMQ.Client;
using System.Security.Claims;
using BCrypt.Net;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using MimeKit;
using MailKit.Net.Smtp;
using Repositories.Models;


namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthApiController : ControllerBase
    {
        private readonly IAuthInterface _authInterface;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthApiController> _logger;
        private readonly IDatabase _redis;
        private readonly IModel _channel;
        private readonly CloudinaryService _cloudinaryService;

        public AuthApiController(IAuthInterface authInterface, ILogger<AuthApiController> logger, IConnectionMultiplexer redis, IModel channel, IConfiguration configuration, CloudinaryService cloudinaryService)
        {
            _authInterface = authInterface ?? throw new ArgumentNullException(nameof(authInterface));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _redis = redis.GetDatabase();
            _channel = channel;
            _cloudinaryService = cloudinaryService;
        }

        //Forgot password changes made here...
        [HttpPost("SendOtp")]
        public async Task<IActionResult> SendOtp([FromBody] OtpRequest request)
        {
            if (string.IsNullOrEmpty(request.Email))
            {
                return BadRequest(new { success = false, message = "Email is required." });
            }
            int result = await _authInterface.GetUserEmail(request.Email);
            if (result == 0)
            {
                return BadRequest(new { success = false, message = "Email not registered." });
            }
            if (result == -1)
            {
                return BadRequest(new { success = false, message = "Error in fetching the registered email" });
            }
            string otp = new Random().Next(100000, 999999).ToString();
            bool isSet = await _redis.StringSetAsync(request.Email, otp, TimeSpan.FromMinutes(5)); // Store OTP for 5 minutes

            if (!isSet)
            {
                return BadRequest(new { success = false, message = "Failed to store OTP." });
            }

            bool emailSent = await SendOtpEmailAsync(request.Email, otp);
            if (!emailSent)
            {
                return StatusCode(500, new { success = false, message = "Failed to send OTP email." });
            }

            return Ok(new { success = true, message = "OTP sent to your email." });
        }

        private async Task<bool> SendOtpEmailAsync(string email, string otp)
        {
            try
            {
                var smtpServer = _configuration["EmailSettings:SmtpServer"];
                var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"]);
                var senderEmail = _configuration["EmailSettings:SenderEmail"];
                var senderPassword = _configuration["EmailSettings:SenderPassword"];

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("CareerLink", senderEmail));
                message.To.Add(new MailboxAddress("", email));
                message.Subject = "Your OTP Code";

                string emailBody = $@"
                <html>
                    <head>
                        <meta charset='UTF-8'>
                        <title>OTP Verification</title>
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
                            .otp-box {{
                                font-size: 26px;
                                font-weight: bold;
                                background-color: #e7f3ff;
                                color: #0b65c3;
                                display: inline-block;
                                padding: 14px 28px;
                                border-radius: 8px;
                                margin: 20px 0;
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
                                    <h2>OTP Verification</h2>
                                    <p>Please use the following One-Time Password (OTP) to complete your verification.</p>
                                    <div class='otp-box'>{otp}</div>
                                    <p>This OTP is valid for <strong style='color:black'>5 minutes</strong>. Do not share it with anyone.</p>
                                    <p>If you did not request this, please ignore this email.</p>
                                </div>
                                <div class='email-footer'>
                                    &copy; CareerLink. All rights reserved.
                                </div>
                            </div>
                        </div>
                    </body>
                    </html>";
                message.Body = new TextPart("html")
                {
                    Text = emailBody
                };

                using (var client = new MailKit.Net.Smtp.SmtpClient())
                {
                    client.Connect(smtpServer, smtpPort, false);
                    await client.AuthenticateAsync(senderEmail, senderPassword);
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email: {ex.Message}");
                return false;
            }
        }


        [HttpPost("VerifyOtp")]
        public async Task<IActionResult> VerifyOtp([FromForm] OtpVerificationRequest request)
        {
            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Otp))
            {
                return BadRequest(new { success = false, message = "Email and OTP are required." });
            }

            string storedOtp = await _redis.StringGetAsync(request.Email);
            if (storedOtp == request.Otp)
            {
                return Ok(new { success = true, message = "OTP verified." });
            }
            return BadRequest(new { success = false, message = "Invalid or expired OTP." });
        }

        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromForm] ResetPasswordRequest request)
        {
            Console.WriteLine($"Received Email: {request.Email}, NewPassword: {request.NewPassword}");

            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.NewPassword))
            {
                return BadRequest(new { success = false, message = "Email and new password are required." });
            }

            bool updated = await _authInterface.ChangePassword(request.Email, request.NewPassword);
            if (updated)
            {
                return Ok(new { success = true, message = "Password reset successful." });
            }
            return StatusCode(500, new { success = false, message = "Failed to reset password." });
        }
        [HttpGet("GetUserProfile")]
        public IActionResult GetUserProfile()
        {
            var claims = User.Claims.ToList();
            foreach (var claim in claims)
            {
                Console.WriteLine($"Claim Type: {claim.Type}, Value: {claim.Value}");
            }

            var userId = User.FindFirst("uid")?.Value; // "sub" claim holds user ID in JWT

            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest(new { success = false, message = "User ID not found in token." });
            }
            return Ok(new { success = true, userId });
        }

        [HttpGet("GetUsersEmail")]
        public async Task<IActionResult> GetUsersEmail()
        {
            List<t_user> users = await _authInterface.GetEmailRecords();
            return Ok(users);
        }
        
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromForm] vm_Login login)
        {
            try
            {
                if (login == null || string.IsNullOrEmpty(login.c_email) || string.IsNullOrEmpty(login.c_password) || string.IsNullOrEmpty(login.c_userRole))
                {
                    return BadRequest(new { success = false, message = "Invalid login data." });
                }

                _logger.LogInformation("Login Attempt: {Email}", login.c_email);
                // if(login.c_email=="ritadehrawala3@gmail.com" && login.c_password=="Krishna!1" && login.c_userRole=="Admin")

                t_user user = await _authInterface.Login(login);

                if (user == null)
                {
                    _logger.LogError("Login failed: No user found for email {Email}", login.c_email);
                    return Ok(new { success = false, message = "Invalid email or password, or an incorrect role selection." });
                }
                if(user.c_IsBlock == true)
                {
                    return Ok(new { success = false, message = "Your Access is Blocked By Admin" });
                }
                // if (user.c_userRole != login.c_userRole)
                // {
                //     return Ok(new { success = false, message = $"You are not registered as {login.c_userRole}." });
                // }

                // if (string.IsNullOrEmpty(user.c_password) || !BCrypt.Net.BCrypt.Verify(login.c_password, user.c_password))
                // {
                //     return Ok(new { success = false, message = "Wrong Email or Password." });
                // }

                var token = GenerateJwtToken(user);
                 Response.Cookies.Append("UserRole", user.c_userRole, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddHours(1)
                });

                // var userIdClaim = User.FindFirst("UserId")?.Value;// this is just for the person who see the code dont uncomment this
                // Console.WriteLine("UserId in claim: " + userIdClaim);
                return Ok(new { success = true, message = "User logged in successfully.", userId = user.c_userId, userrole = user.c_userRole, token });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in Login: {ex.Message}");
                return StatusCode(500, new { success = false, message = "An error occurred while processing your request." });
            }
        }

        private string GenerateJwtToken(t_user user)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);

            var claims = new[]
            {

                new Claim("uid", user.c_userId.ToString()),
                new Claim("role", user.c_userRole.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.c_email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(2),
                Issuer = jwtSettings["Issuer"],
                Audience = jwtSettings["Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        
        private async Task<string> UploadProfilePic(IFormFile ProfilePic)
        {
            string logoExtension = Path.GetExtension(ProfilePic.FileName).ToLower();
            if (logoExtension != ".png" && logoExtension != ".jpg" && logoExtension != ".jpeg")
                return null;

            using var stream = ProfilePic.OpenReadStream();
            return await _cloudinaryService.UploadImageAsync(stream, ProfilePic.FileName, "ProfileImage");
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromForm] t_user1 user)
        {
            try
            {
                if (user == null) return BadRequest("Invalid user data.");
                _logger.LogInformation("User Registration Attempt: {Email}", user.c_email);

                if (user.ProfilePic != null && user.ProfilePic.Length > 0)
                {
                    string logoUrl = await UploadProfilePic(user.ProfilePic);
                    if (logoUrl == null) return BadRequest("Only PNG and JPG formats are allowed for the logo.");
                    user.c_profileImage = logoUrl;
                }
                var status = await _authInterface.Register(user);
                if (status == 1)
                {
                    var userData = new { Email = user.c_email, Name = user.c_username, Role = user.c_userRole };
                    _redis.StringSet($"User:{user.c_email}", JsonSerializer.Serialize(userData));

                    _channel.QueueDeclare(queue: "UserRegistrationQueue", durable: false, exclusive: false, autoDelete: false, arguments: null);
                    var message = JsonSerializer.Serialize(userData);
                    var body = Encoding.UTF8.GetBytes(message);
                    _channel.BasicPublish(exchange: "", routingKey: "UserRegistrationQueue", basicProperties: null, body: body);

                    _logger.LogInformation("User registration event published to RabbitMQ: {Email}", user.c_email);
                    return Ok(new { success = true, message = "User Registered successfully. Admin will be notified." });
                }
                else if (status == 0)
                {
                    return Ok(new { success = false, message = "User Already Exists" });
                }
                else
                {
                    return BadRequest(new { success = false, message = "An error occurred during registration." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in Register: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpPost("CheckOldPassword")]
        public async Task<IActionResult> CheckOldPassword([FromBody] PasswordCheck model)
        {
            var userId = model.userId;
            Console.WriteLine($"User ID : {userId}");

            var hashedPassword = await _authInterface.GetOldPassword(userId); // from DB

            if (string.IsNullOrEmpty(hashedPassword))
            {
                return NotFound(new { success = false, message = "User or password not found." });
            }

            var isValid = BCrypt.Net.BCrypt.Verify(model.OldPassword, hashedPassword);

            return Ok(new { success = true, isValid });
        }

        
    }

    
    public class OtpRequest
    {
        public string Email { get; set; }
    }

    public class OtpVerificationRequest
    {
        public string Email { get; set; }
        public string Otp { get; set; }
    }

    public class ResetPasswordRequest
    {
        public string Email { get; set; }
        public string NewPassword { get; set; }
    }
}