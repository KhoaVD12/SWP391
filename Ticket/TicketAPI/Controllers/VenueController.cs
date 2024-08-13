using BusinessObject.IService;
using BusinessObject.Models.VenueDTO;
using Microsoft.AspNetCore.Mvc;

namespace TicketAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VenueController : ControllerBase
    {
        private readonly IVenueService _venueService;
        public VenueController(IVenueService venueService)
        {
            _venueService = venueService;
        }
        [HttpPost("Create")]
        public async Task<IActionResult> CreateVenue(CreateVenueDTO venueDTO)
        {
            var result = await _venueService.CreateVenue(venueDTO);
            return Ok(result);
        }
    }
}
