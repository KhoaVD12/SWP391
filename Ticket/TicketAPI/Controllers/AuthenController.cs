using BusinessObject.IService;
using BusinessObject.Models;
using BusinessObject.Models.UserDTO;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace TicketAPI.Controllers
{
    [EnableCors("Allow")]
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthenController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login(LoginResquestDto login)
        {
            var result = await _authenticationService.LoginAsync(login);
            if (!result.Success)
            {
                return StatusCode(401, result);
            }

            return Ok(new
            {
                success = result.Success,
                message = result.Message,
                token = result.DataToken,
                role = result.Role,
                hintId = result.HintId
            });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassWord(ResetPassDTO dto)
        {
            var response = await _authenticationService.ResetPass(dto);
            if (!response.Success)
            {
                return StatusCode(401, response);
            }

            return Ok(new
            {
                success = response.Success,
                message = response.Message
            });
        }
    }
}