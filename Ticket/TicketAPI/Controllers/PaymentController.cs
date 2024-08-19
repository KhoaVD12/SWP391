using BusinessObject.IService;
using BusinessObject.Models.PaymentDTO;
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

    [HttpPost("create")]
    public async Task<IActionResult> CreatePayment(decimal amount)
    {
        var response = await _paymentService.CreatePayment(amount, "VND", "https://localhost:3000", "https://localhost:3000");

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpPost("execute")]
    public async Task<IActionResult> ExecutePayment(string paymentId, string payerId)
    {
        var response = await _paymentService.ExecutePayment(paymentId, payerId);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }


}