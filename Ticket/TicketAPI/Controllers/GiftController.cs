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
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        /// <param name="page">Page number.</param>
        /// <param name="pageSize">Number of Booths per page.</param>
        /// <param name="search">Search by Name.</param>
        /// <param name="sort">Sort by Name, Quantity.</param>
        [HttpGet]
        public async Task<IActionResult> GetAllGifts([FromQuery] int page = 1, [FromQuery] int pageSize = 5,
            [FromQuery] string search = "", [FromQuery] string sort = "")
        {
            var result = await _giftService.GetAllGifts(page, pageSize, search, sort);
            if (!result.Success )
            {
                return NotFound(result);
            }
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetGiftById(int id)
        {
            var result = await _giftService.GetGiftById(id);
            if (!result.Success)
            {
                return NotFound(result);
            }
            return Ok(result);
        }
        [HttpGet("Booth/{boothId}")]
        public async Task<IActionResult> GetGiftByBoothId(int boothId)
        {
            var result = await _giftService.GetGiftByBoothId(boothId);
            if (!result.Success)
            {
                return NotFound(result);
            }
            return Ok(result);
        }
        [HttpGet("Sponsor/{sponsorId}")]
        public async Task<IActionResult> GetGiftBySponsorId(int sponsorId)
        {
            var result = await _giftService.GetGiftsBySponsorId(sponsorId);
            if (!result.Success)
            {
                return NotFound(result);
            }
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGift(int id)
        {
            var result = await _giftService.DeleteGift(id);
            if (!result.Success)
            {
                return NotFound(result);
            }
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGift(int id, CreateGiftDTO giftDTO)
        {
            var result = await _giftService.UpdateGift(id, giftDTO);
            if (!result.Success)
            {
                return NotFound(result);
            }
            return Ok(result);
        }
    }
}
