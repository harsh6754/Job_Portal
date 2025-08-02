using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Repositories.Interfaces;
namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LandingApiController : ControllerBase
    {
        private readonly ILandingPageInterface _landingPageRepository;
        public LandingApiController(ILandingPageInterface landingPageInterface)
        {
            _landingPageRepository = landingPageInterface;
        }

        [HttpGet("getLandingDetails")]
        public async Task<IActionResult> GetLandingDetails()
        {
            try
            {
                var result = await _landingPageRepository.GetLandingDetails();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}