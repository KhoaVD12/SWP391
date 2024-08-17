using BusinessObject.IService;
using BusinessObject.Models.BoothDTO;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace TicketAPI.Controllers
{
    [EnableCors("Allow")]
    [ApiController]
    [Route("api/[controller]")]
    public class BoothController:ControllerBase
    {
        private readonly IBoothService _boothService;
        public BoothController(IBoothService service)
        {
            _boothService = service;
        }
        [HttpGet]
        public async Task<IActionResult> GetBooths([FromQuery] int page = 1, [FromQuery] int pageSize = 5,
            [FromQuery] string search = "", [FromQuery] string sort = "")
        {
            var result=await _boothService.GetAllBooths(page, pageSize, search, sort);
            if (result == null)
            {
                return NotFound(result);
            }
            return Ok(result);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBoothById(int id)
        {
            var result= await _boothService.GetBoothById(id);
            if (result == null)
            {
                return NotFound(result);
            }
            return Ok(result);
        }
        [HttpPost]
        public async Task<IActionResult>CreateBooth(CreateBoothDTO boothDTO)
        {
            var result=await _boothService.CreateBooth(boothDTO);
            if (result == null)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        [HttpDelete]
        public async Task<IActionResult>DeleteBooth(int id)
        {
            var result=await _boothService.DeleteBooth(id);
            if (result == null)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        [HttpPut]
        public async Task<IActionResult>UpdateBooth(int id, CreateBoothDTO boothDTO)
        {
            var result = await _boothService.UpdateBooth(id, boothDTO);
            if (result == null)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
}
