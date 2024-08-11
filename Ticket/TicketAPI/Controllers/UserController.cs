using BusinessObject.IService;
using BusinessObject.Models;
using BusinessObject.Models.UserDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace TicketAPI.Controllers
{
    [EnableCors("Allow")]
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }
        [HttpPost("login")]
        public async Task<IActionResult>Login(LoginResquestDto login)
        {
            var result =await _userService.LoginAsync(login);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(
                new
                {
                    success = result.Success,
                    message = result.Message,
                    token = result.Data,
                    role = result.Role,
                }
            );
        }

        [HttpPost("staff")] //Admin
        public async Task<IActionResult> NewAccountStaff(CreateUserDto registerObject)
        {
            var result = await _userService.CreateStaff(registerObject);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}
