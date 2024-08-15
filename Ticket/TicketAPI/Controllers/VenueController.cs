using BusinessObject.IService;
using BusinessObject.Models.VenueDTO;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace TicketAPI.Controllers
{
    [EnableCors("Allow")]
    [ApiController]
    [Route("api/[controller]")]
    public class VenueController : ControllerBase
    {
        private readonly IVenueService _venueService;

        public VenueController(IVenueService venueService)
        {
            _venueService = venueService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateVenue(CreateVenueDTO venueDTO)
        {
            var result = await _venueService.CreateVenue(venueDTO);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllVenue(int page, int pageSize, string search, string sort)
        {
            var result = await _venueService.GetAllVenues(page, pageSize, search, sort);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetVenueById(int id)
        {
            var result = await _venueService.GetVenueById(id);
            return Ok(result);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteVenue(int id)
        {
            var result = await _venueService.DeleteVenue(id);
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateVenue(int id, ViewVenueDTO venueDTO)
        {
            var result = await _venueService.UpdateVenue(id, venueDTO);
            return Ok(result);
        }
    }
}