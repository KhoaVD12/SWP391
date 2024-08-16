using BusinessObject.IService;
using BusinessObject.Models.GiftDTO;
using BusinessObject.Models.VenueDTO;
using BusinessObject.Service;
using Microsoft.AspNetCore.Mvc;

namespace TicketAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GiftController:ControllerBase
    {
        private readonly IGiftService _giftService;
        public GiftController(IGiftService service)
        {
            _giftService = service;
        }
        [HttpPost]
        public async Task<IActionResult> CreateGift(CreateGiftDTO giftDTO)
        {
            var result = await _giftService.CreateGift(giftDTO);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllGifts([FromQuery] int page = 1, [FromQuery] int pageSize = 5,
            [FromQuery] string search = "", [FromQuery] string sort = "")
        {
            var result = await _giftService.GetAllGifts(page, pageSize, search, sort);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetGiftById(int id)
        {
            var result = await _giftService.GetGiftById(id);
            return Ok(result);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteGift(int id)
        {
            var result = await _giftService.DeleteGift(id);
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateGift(int id, CreateGiftDTO giftDTO)
        {
            var result = await _giftService.UpdateGift(id, giftDTO);
            return Ok(result);
        }
    }
}
