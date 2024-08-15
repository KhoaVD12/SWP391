using BusinessObject.IService;
using BusinessObject.Models.TransactionDTO;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace TicketAPI.Controllers;

[EnableCors("Allow")]
[ApiController]
[Route("api/[controller]")]
public class TransactionController : ControllerBase
{
    private readonly ITransactionService _transactionService;

    public TransactionController(ITransactionService transactionService)
    {
        _transactionService = transactionService;
    }

    [HttpGet("attendees/{attendeeId}/transactions")]
    public async Task<IActionResult> GetTransactionsByAttendee(int attendeeId)
    {
        var transactions = await _transactionService.GetTransactionsByAttendeeAsync(attendeeId);
        return Ok(transactions);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTransaction(int id)
    {
        var transaction = await _transactionService.GetTransactionByIdAsync(id);
        if (transaction == null) return NotFound("Transaction not found.");
        return Ok(transaction);
    }

    [HttpPost]
    public async Task<IActionResult> CreateTransaction([FromBody] CreateTransactionDto dto)
    {
        var result = await _transactionService.CreateTransactionAsync(dto);
        if (!result.Success) return BadRequest(result.Message);
        return Ok(result.Data);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTransaction(int id, [FromBody] TransactionDto dto)
    {
        var result = await _transactionService.UpdateTransactionAsync(id, dto);
        if (!result.Success) return BadRequest(result.Message);
        return Ok(result.Data);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTransaction(int id)
    {
        var result = await _transactionService.DeleteTransactionAsync(id);
        if (!result.Success) return BadRequest(result.Message);
        return Ok(result.Message);
    }
}