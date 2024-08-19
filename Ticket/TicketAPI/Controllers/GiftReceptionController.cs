using BusinessObject.IService;
using BusinessObject.Models.GiftReceptionDTO;
using Microsoft.AspNetCore.Mvc;

namespace TicketAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GiftReceptionController:ControllerBase
    {
        private readonly IGiftReceptionService _giftReceptionService;
        public GiftReceptionController(IGiftReceptionService giftReceptionService)
        {
            _giftReceptionService = giftReceptionService;
        }
        /// <param name="page">Page number.</param>
        /// <param name="pageSize">Number of Receptions per page.</param>
        /// <param name="sort">Sort by Reception Date.</param>
        [HttpGet]
        public async Task<IActionResult> GetAllReceptions([FromQuery] int page = 1, [FromQuery] int pageSize = 5, [FromQuery] string sort = "")
        {
            var result = await _giftReceptionService.GetReceptions(page,pageSize,sort);
            if (!result.Success)
            {
                return NotFound(result);
            }
            return Ok(result);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetReceptionById(int id)
        {
            var result=await _giftReceptionService.GetReceptionById(id);
            if(!result.Success)
            {
                return NotFound(result);
            }
            return Ok(result);
        }
        [HttpGet("attendee/{attendeeId}")]
        public async Task<IActionResult> GetReceptionByAttendeeId(int attendeeId)
        {
            var result = await _giftReceptionService.GetReceptionByAttendeeId(attendeeId);
            if (!result.Success)
            {
                return NotFound(result);
            }
            return Ok(result);
        }
        [HttpGet("gift/{giftId}")]
        public async Task<IActionResult> GetReceptionByGiftId(int giftId)
        {
            var result = await _giftReceptionService.GetReceptionByGiftId(giftId);
            if (!result.Success)
            {
                return NotFound(result);
            }
            return Ok(result);
        }
        [HttpPost]
        public async Task<IActionResult> CreateReception(CreateGiftReceptionDTO receptionDTO)
        {
            var result = await _giftReceptionService.CreateReception(receptionDTO);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGiftReception(int id)
        {
            var result = await _giftReceptionService.DeleteGiftReception(id);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
}
