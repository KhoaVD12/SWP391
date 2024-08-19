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
        if (!transactions.Success) return BadRequest(transactions);
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
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTransaction(int id, [FromBody] TransactionDto dto)
    {
        var result = await _transactionService.UpdateTransactionAsync(id, dto);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTransaction(int id)
    {
        var result = await _transactionService.DeleteTransactionAsync(id);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPut("{transactionId}/status")]
    public async Task<IActionResult> UpdateTransactionStatus(int transactionId, [FromBody] string status)
    {
        var result = await _transactionService.UpdateTransactionStatusAsync(transactionId, status);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Get the total amount spent by an attendee.
    /// </summary>
    /// <param name="attendeeId">The ID of the attendee.</param>
    /// <returns>A ServiceResponse containing the total amount.</returns>
    [HttpGet("total-amount/{attendeeId}")]
    public async Task<IActionResult> GetTotalAmountByAttendee(int attendeeId)
    {
        var response = await _transactionService.GetTotalAmountByAttendeeAsync(attendeeId);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }
}