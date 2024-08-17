using BusinessObject.IService;
using BusinessObject.Models.BoothRequestDTO;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace TicketAPI.Controllers
{
    [EnableCors("Allow")]

    [ApiController]
    [Route("api/[controller]")]
    public class BoothRequestController:ControllerBase
    {
        private readonly IBoothRequestService _boothRequestService;
        public BoothRequestController(IBoothRequestService service)
        {
            _boothRequestService = service;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllBoothRequests([FromQuery] int page = 1, [FromQuery] int pageSize = 5,
            [FromQuery] string search = "", [FromQuery] string sort = "")
        {
            var result=await _boothRequestService.GetAllBoothRequests(page,pageSize,search,sort);
            if (result == null)
            {
                return NotFound(result);
            }
            return Ok(result);
        }
        [HttpPost]
        public async Task<IActionResult> CreateBoothRequest(CreateBoothRequestDTO requestDTO)
        {
            var result=await _boothRequestService.CreateBoothRequest(requestDTO);
            if (result == null)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        [HttpDelete]
        public async Task<IActionResult>DeleteBoothRequest(int id)
        {
            var result = await _boothRequestService.DeleteBoothRequest(id);
            if (result == null)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        [HttpPut]
        public async Task<IActionResult>UpdateBoothRequest(int id, CreateBoothRequestDTO boothRequestDTO)
        {
            var result = await _boothRequestService.UpdateBoothRequest(id, boothRequestDTO);
            if (result == null)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult>GetBoothRequestById(int id)
        {
            var result=await _boothRequestService.GetBoothRequestById(id);
            if (result == null)
            {
                return NotFound(result);
            }
            return Ok(result);
        }
    }
}
