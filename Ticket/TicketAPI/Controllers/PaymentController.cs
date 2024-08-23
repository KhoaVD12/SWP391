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
    private readonly IVnPayService _vnPayService;

    public PaymentController(IPaymentService paymentService, IVnPayService vnPayService)
    {
        _paymentService = paymentService;
        _vnPayService = vnPayService;
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
    /// Creates a new order for an attendee using Paypal.
    /// </summary>
    /// <param name="request">The request containing the attendee ID, amount, and currency in VND.</param>
    /// <returns>Returns the order creation response.</returns>
    /// <remarks>
    /// The amount will be converted from VND to USD for PayPal processing, as PayPal does not support VND.
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

    /*/// <summary>
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
    }*/

    /// <summary>
    /// Creates a payment request using VNPay for a specified attendee and amount.
    /// This action generates a payment URL that can use to redirect the user to VNPay's payment page.
    /// </summary>
    /// <param name="request">The payment request details including attendee ID and amount.</param>
    /// <returns>Returns the payment request response which includes a URL for the VNPay payment page.</returns>
    /// /// <response code="200">Returns the payment URL and other details if the request was successful.</response>
    /// <response code="400">Returns an error response if the payment request failed.</response>
    [HttpPost("create-payment-request")]
    public async Task<IActionResult> CreatePaymentRequest([FromBody] PaymentRequestDto request)
    {
        var response = await _vnPayService.CreatePaymentRequest(request.AttendeeId, request.Amount, HttpContext);
        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    /// <summary>
    /// Handles the callback from VNPay after a payment transaction is completed.
    /// VNPay sends various parameters as query parameters to this endpoint to notify about the payment result.
    /// No input is required from the front-end or user for this callback endpoint.
    /// This endpoint processes the response and updates the payment status accordingly.
    /// </summary>
    /// <returns>An IActionResult indicating the success or failure of processing the VNPay callback.</returns>
    [HttpGet("vnpay/callback")]
    public async Task<IActionResult> VnPayCallback()
    {
        var queryParams = HttpContext.Request.Query;

        // Process the VNPay payment response
        var paymentResult = await _vnPayService.ProcessPaymentResponse(queryParams);

        // Handle failure, maybe redirect to an error page
        return Redirect("http://localhost:3000");
    }
}