using BusinessObject.IService;
using BusinessObject.Models.PaymentDTO;
using DataAccessObject.Entities;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace TicketAPI.Controllers;

[EnableCors("Allow")]
[ApiController]
[Route("api/[controller]")]
public class PaymentController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    public PaymentController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllPaymentMethods([FromQuery] int page = 1, [FromQuery] int pageSize = 5,
        [FromQuery] string search = "")
    {
        var result = await _paymentService.GetAllPaymentMethodsAsync(page, pageSize, search);
        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PaymentMethodDto>> GetPaymentMethod(int id)
    {
        var result = await _paymentService.GetPaymentMethodByIdAsync(id);
        if (!result.Success)
        {
            return NotFound(result);
        }

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreatePaymentMethod([FromBody] CreatePaymentMethodDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new
                { success = false, message = "Invalid data", errors = ModelState.Values.SelectMany(v => v.Errors) });
        }

        var result = await _paymentService.CreatePaymentMethodAsync(dto);
        if (!result.Success)
        {
            return BadRequest(result);
        }

        return CreatedAtAction(nameof(GetPaymentMethod), new { id = result.Data.Id },
            new { success = result.Success, data = result.Data });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePaymentMethod(int id, [FromBody] PaymentMethodDto dto)
    {
        if (id != dto.Id)
        {
            return BadRequest(new { success = false, message = "Payment method ID mismatch" });
        }

        var result = await _paymentService.UpdatePaymentMethodAsync(id, dto);
        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePaymentMethod(int id)
    {
        var result = await _paymentService.DeletePaymentMethodAsync(id);
        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Creates a new order for an attendee.
    /// </summary>
    /// <param name="request">The request containing the attendee ID, amount, and currency.</param>
    /// <returns>Returns the order creation response.</returns>
    /// <remarks>
    /// The currency is defaulted to VND (Vietnamese Dong).
    /// </remarks>
    [HttpPost("create-order")]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequestDto request)
    {
        try
        {
            var response =
                await _paymentService.CreateOrderAsync(request.AttendeeId, request.Amount, request.Currency = "VND");

            return Ok(response);
        }
        catch (Exception e)
        {
            var error = new { e.GetBaseException().Message };
            return BadRequest(error);
        }
    }

    /// <summary>
    /// Captures an existing order using the order ID and transaction ID.
    /// </summary>
    /// <param name="request">The request containing the order ID and transaction ID.</param>
    /// <returns>Returns the order capture response.</returns>
    [HttpPost("capture-order")]
    public async Task<IActionResult> CaptureOrder([FromBody] CaptureOrderRequestDto request)
    {
        try
        {
            var response = await _paymentService.CaptureOrderAsync(request.OrderId, request.TransactionId);

            return Ok(response);
        }
        catch (Exception e)
        {
            var error = new { e.GetBaseException().Message };
            return BadRequest(error);
        }
    }
}