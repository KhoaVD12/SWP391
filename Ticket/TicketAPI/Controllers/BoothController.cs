using BusinessObject.IService;
using BusinessObject.Models.BoothDTO;
using Microsoft.AspNetCore.Mvc;

namespace TicketAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BoothController:ControllerBase
    {
        private readonly IBoothService _boothService;
        public BoothController(IBoothService service)
        {
            _boothService = service;
        }
        [HttpPost]
        public async Task<IActionResult>CreateBooth(CreateBoothDTO boothDTO)
        {
            var result=await _boothService.CreateBooth(boothDTO);
            return Ok(result);
        }
    }
}
