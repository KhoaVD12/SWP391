﻿using BusinessObject.IService;
using BusinessObject.Models.BoothDTO;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace TicketAPI.Controllers
{
    [EnableCors("Allow")]
    [ApiController]
    [Route("api/[controller]")]
    public class BoothController : ControllerBase
    {
        private readonly IBoothService _boothService;
        public BoothController(IBoothService service)
        {
            _boothService = service;
        }
        /// <summary>
        /// Get a list of events with pagination, search, and sort options.
        /// </summary>                                                                                                  
        /// <param name="page">Page number.</param>
        /// <param name="pageSize">Number of Booths per page.</param>
        /// <param name="search">Search by Location, Name, Status.</param>
        /// <param name="sort">Sort by Location or Name.</param>
        /// <returns>A paginated list of events.</returns>
        [HttpGet]
        public async Task<IActionResult> GetBooths([FromQuery] int page = 1, [FromQuery] int pageSize = 5,
            [FromQuery] string search = "", [FromQuery] string sort = "")
        {
            var result = await _boothService.GetAllBooths(page, pageSize, search, sort);
            if (!result.Success)
            {
                return NotFound(result);
            }
            return Ok(result);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBoothById(int id)
        {
            var result = await _boothService.GetBoothById(id);
            if (!result.Success)
            {
                return NotFound(result);
            }
            return Ok(result);
        }
        [HttpGet("Request/{requestId}")]
        public async Task<IActionResult> GetBoothByRequest(int requestId)
        {
            var result = await _boothService.GetBoothByRequest(requestId);
            if (!result.Success)
            {
                return NotFound(result);
            }
            return Ok(result);
        }
        [HttpPost]
        public async Task<IActionResult> CreateBooth(CreateBoothDTO boothDTO)
        {
            var result = await _boothService.CreateBooth(boothDTO);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBooth(int id)
        {
            var result = await _boothService.DeleteBooth(id);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBooth(int id, CreateBoothDTO boothDTO)
        {
            var result = await _boothService.UpdateBooth(id, boothDTO);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        [HttpPut("ChangeBoothStatus/{id}")]
        public async Task<IActionResult> ChangeBoothStatus(int id, BoothStatusDTO boothStatusDTO)
        {
            var result = await _boothService.ChangeStatusBooth(id, boothStatusDTO);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
}
