using BusinessObject.IService;
using BusinessObject.Models.TicketDTO;
using Microsoft.AspNetCore.Mvc;

namespace TicketAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TicketController:ControllerBase
    {
        private readonly ITicketService _ticketService;
        public TicketController(ITicketService service)
        {
            _ticketService = service;
        }
        [HttpPost]
        public async Task<IActionResult> CreateTicket(CreateTicketDTO ticketDTO)
        {
            var result=await _ticketService.CreateTicket(ticketDTO);
            return Ok(result);
        } 
    }
}
