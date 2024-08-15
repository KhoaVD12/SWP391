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
        public async Task<IActionResult> GetAllBoothRequests(int page, int pageSize, string search, string sort)
        {
            var result=await _boothRequestService.GetAllBoothRequests(page,pageSize,search,sort);
            return Ok(result);
        }
        [HttpPost]
        public async Task<IActionResult> CreateBoothRequest(CreateBoothRequestDTO requestDTO)
        {
            var result=await _boothRequestService.CreateBoothRequest(requestDTO);   
            return Ok(result);
        }
        [HttpDelete]
        public async Task<IActionResult>DeleteBoothRequest(int id)
        {
            var result = await _boothRequestService.DeleteBoothRequest(id);
            return Ok(result);
        }
        [HttpPut]
        public async Task<IActionResult>UpdateBoothRequest(int id, CreateBoothRequestDTO boothRequestDTO)
        {
            var result = await _boothRequestService.UpdateBoothRequest(id, boothRequestDTO);
            return Ok(result);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult>GetBoothRequestById(int id)
        {
            var result=await _boothRequestService.GetBoothRequestById(id);
            return Ok(result);
        }
    }
}
