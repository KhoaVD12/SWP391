using BusinessObject.IService;
using BusinessObject.Models.BoothRequestDTO;
using Microsoft.AspNetCore.Mvc;

namespace TicketAPI.Controllers
{
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
        public async Task<IEnumerable<ViewBoothRequestDTO>> GetAllBoothRequest()
        {
            var result=await _boothRequestService.GetAllBoothRequest();
            return result;
        }
        [HttpPost]
        public async Task<CreateBoothRequestDTO> CreateBoothRequest(CreateBoothRequestDTO requestDTO)
        {
            var result=await _boothRequestService.CreateBoothRequest(requestDTO);   
            return result;
        }
    }
}
