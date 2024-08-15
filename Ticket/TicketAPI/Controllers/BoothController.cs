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
        public async Task<IActionResult> GetBooths(int page, int pageSize, string search, string sort)
        {
            var result=await _boothService.GetAllBooths(page, pageSize, search, sort);
            return Ok(result);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBoothById(int id)
        {
            var result= await _boothService.GetBoothById(id);
            return Ok(result);
        }
        [HttpPost]
        public async Task<IActionResult>CreateBooth(CreateBoothDTO boothDTO)
        {
            var result=await _boothService.CreateBooth(boothDTO);
            return Ok(result);
        }
        [HttpDelete]
        public async Task<IActionResult>DeleteBooth(int id)
        {
            var result=await _boothService.DeleteBooth(id);
            return Ok(result);
        }
        [HttpPut]
        public async Task<IActionResult>UpdateBooth(int id, CreateBoothDTO boothDTO)
        {
            var result = await _boothService.UpdateBooth(id, boothDTO);
            return Ok(result);
        }
    }
}
