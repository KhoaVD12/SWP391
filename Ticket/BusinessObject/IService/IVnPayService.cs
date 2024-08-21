using BusinessObject.Models.VnPayDTO;
using BusinessObject.Responses;
using DataAccessObject.Entities;
using Microsoft.AspNetCore.Http;

namespace BusinessObject.IService;

public interface IVnPayService
{
    Task<ServiceResponse<VnPaymentResponseModel>> CreatePaymentRequest(int attendeeId, decimal amount, HttpContext httpContext);
    Task<ServiceResponse<Payment>> ProcessPaymentResponse(IQueryCollection queryParams);
}