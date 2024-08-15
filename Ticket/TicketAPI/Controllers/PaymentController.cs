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
        var paymentMethods = await _paymentService.GetAllPaymentMethodsAsync(page, pageSize, search);
        return Ok(paymentMethods);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPaymentMethod(int id)
    {
        var result = await _paymentService.GetPaymentMethodByIdAsync(id);
        if (!result.Success) return BadRequest(result.Message);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreatePaymentMethod([FromBody] CreatePaymentMethodDto dto)
    {
        var result = await _paymentService.CreatePaymentMethodAsync(dto);
        if (!result.Success) return BadRequest(result.Message);
        return Ok(result.Data);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePaymentMethod(int id, [FromBody] PaymentMethodDto dto)
    {
        var result = await _paymentService.UpdatePaymentMethodAsync(id, dto);
        if (!result.Success) return BadRequest(result.Message);
        return Ok(result.Data);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePaymentMethod(int id)
    {
        var result = await _paymentService.DeletePaymentMethodAsync(id);
        if (!result.Success) return BadRequest(result.Message);
        return Ok(result.Message);
    }
}