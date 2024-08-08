using DataAccessObject.IService;
using Microsoft.AspNetCore.Mvc;

namespace TicketAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }
        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult>Login(string email, string password)
        {
            var result =await _userService.Login(email, password);
            return Ok(result);
        }
    }
}
