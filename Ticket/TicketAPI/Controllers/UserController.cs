using BusinessObject.IService;
using BusinessObject.Models.UserDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace TicketAPI.Controllers;

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


    [HttpGet]
    public async Task<IActionResult> GetAllUsers([FromQuery] int page = 1, [FromQuery] int pageSize = 5,
        [FromQuery] string search = "", [FromQuery] string sort = "")
    {
        var result = await _userService.GetAllUsers(page, pageSize, search, sort);
        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Creates a new user with the specified details.
    /// </summary>
    /// <param name="registerObject">The details of the user to be created. The role should be one of the following: 'Organizer', 'Staff', 'Sponsor'.</param>
    /// <returns>A result indicating whether the user was created successfully or if there were any errors.</returns>
    [HttpPost]
    public async Task<ActionResult> CreateUser([FromBody] CreateUserDto registerObject)
    {
        var result = await _userService.CreateUserAsync(registerObject);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpGet("staff")]
    public async Task<IActionResult> GetAllUsersCustomer([FromQuery] int page = 1, [FromQuery] int pageSize = 5,
        [FromQuery] string search = "", [FromQuery] string sort = "")
    {
        var result = await _userService.GetAllUsersByStaff(page, pageSize, search, sort);
        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpGet("sponsor")]
    public async Task<IActionResult> GetAllUsersAdmin([FromQuery] int page = 1, [FromQuery] int pageSize = 5,
        [FromQuery] string search = "", [FromQuery] string sort = "")
    {
        var result = await _userService.GetAllUsersBySponsor(page, pageSize, search, sort);
        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpGet("organizer")]
    public async Task<IActionResult> GetAllUsersStaff([FromQuery] int page = 1, [FromQuery] int pageSize = 5,
        [FromQuery] string search = "", [FromQuery] string sort = "")
    {
        var result = await _userService.GetAllUsersByOrganizer(page, pageSize, search, sort);
        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetUserById(int id)
    {
        var result = await _userService.GetUserById(id);
        if (!result.Success)
        {
            return NotFound(result);
        }

        return Ok(result);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateUser([FromBody] UserUpdateDTO userUpdateDto)
    {
        var result = await _userService.UpdateUser(userUpdateDto);
        if (!result.Success)
        {
            return NotFound(result);
        }

        return Ok(result);
    }

    [HttpPatch("{id}/status")]
    public async Task<IActionResult> ChangeStatus(int id, [FromBody] UserStatusDTO statusDto)
    {
        var result = await _userService.ChangeStatus(id, statusDto);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}