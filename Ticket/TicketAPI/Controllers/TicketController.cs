using BusinessObject.IService;
using BusinessObject.Models.TicketDTO;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace TicketAPI.Controllers
{
    [EnableCors("Allow")]
    [ApiController]
    [Route("api/[controller]")]
    public class TicketController:ControllerBase
    {
        private readonly ITicketService _ticketService;
        public TicketController(ITicketService service)
        {
            _ticketService = service;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllTickets(int page, int pageSize, string sort)
        {
            var result=await _ticketService.GetAllTickets(page, pageSize, sort);
            return Ok(result);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult>GetTicketById(int ticketId)
        {
            var result=await _ticketService.GetTicketById(ticketId);
            return Ok(result);
        }
        [HttpPost]
        public async Task<IActionResult> CreateTicket(CreateTicketDTO ticketDTO)
        {
            var result=await _ticketService.CreateTicket(ticketDTO);
            return Ok(result);
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteTicket(int ticketId)
        {
            var result=await _ticketService.DeleteTicket(ticketId);
            return Ok(result);
        }
        [HttpPut]
        public async Task<IActionResult>UpdateTicket(int id, ViewTicketDTO ticketDTO)
        {
            var result=await _ticketService.UpdateTicket(id, ticketDTO);
            return Ok(result);
        }
    }
}
