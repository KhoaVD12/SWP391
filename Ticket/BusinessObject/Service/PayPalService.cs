using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using BusinessObject.IService;
using BusinessObject.Models.PaymentDTO;
using BusinessObject.Models.PayPalDTO;
using Microsoft.Extensions.Configuration;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace BusinessObject.Service;

public class PayPalService : IPayPalService
{
    private readonly HttpClient _httpClient;
    private readonly string _payPalApiUrl;
    private readonly string _clientId;
    private readonly string _clientSecret;

    public PayPalService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        var payPalConfig = configuration.GetSection("PayPal");
        _clientId = payPalConfig["ClientId"];
        _clientSecret = payPalConfig["ClientSecret"];
        _payPalApiUrl = payPalConfig["Mode"] == "live" ? "https://api.paypal.com" : "https://api-m.sandbox.paypal.com";
    }

    private async Task<string> GetAccessTokenAsync()
    {
        var basicAuth = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_clientId}:{_clientSecret}"));
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_payPalApiUrl}/v1/oauth2/token")
        {
            Headers =
            {
                Authorization = new AuthenticationHeaderValue("Basic", basicAuth)
            },
            Content = new StringContent("grant_type=client_credentials", Encoding.UTF8,
                "application/x-www-form-urlencoded")
        };

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var tokenResponse = JsonSerializer.Deserialize<PayPalTokenResponse>(json);

        return tokenResponse?.AccessToken ?? throw new InvalidOperationException("Access token is null");
    }

    public async Task<string> CreatePayment(decimal amount, string currency, string returnUrl, string cancelUrl)
    {
        var accessToken = await GetAccessTokenAsync();

        var paymentPayload = new
        {
            intent = "sale",
            payer = new { payment_method = "paypal" },
            transactions = new[]
            {
                new
                {
                    amount = new { total = amount.ToString("F2"), currency },
                    description = "Your transaction description here"
                }
            },
            redirect_urls = new { return_url = returnUrl, cancel_url = cancelUrl }
        };

        var request = new HttpRequestMessage(HttpMethod.Post, $"{_payPalApiUrl}/v1/payments/payment")
        {
            Headers =
            {
                Authorization = new AuthenticationHeaderValue("Bearer", accessToken)
            },
            Content = new StringContent(JsonSerializer.Serialize(paymentPayload), Encoding.UTF8, "application/json")
        };

        // Log the request content for debugging
        var requestContent = await request.Content.ReadAsStringAsync();
        Console.WriteLine($"Request Payload: {requestContent}");

        var response = await _httpClient.SendAsync(request);

        // Log the response for debugging
        var responseContent = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"Response Content: {responseContent}");

        response.EnsureSuccessStatusCode();

        var paymentResponse = JsonSerializer.Deserialize<PayPalPaymentResponse>(responseContent,
            new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

        var approvalUrl = paymentResponse?.links?.FirstOrDefault(link => link.rel == "approval_url")?.href;
        if (approvalUrl == null)
        {
            throw new InvalidOperationException("Approval URL is not available");
        }

        return approvalUrl;
    }

    public async Task<bool> ExecutePayment(string paymentId, string payerId)
    {
        var accessToken = await GetAccessTokenAsync();

        var request =
            new HttpRequestMessage(HttpMethod.Post, $"{_payPalApiUrl}/v1/payments/payment/{paymentId}/execute")
            {
                Headers =
                {
                    Authorization = new AuthenticationHeaderValue("Bearer", accessToken)
                },
                Content = new StringContent(JsonSerializer.Serialize(new { payer_id = payerId }), Encoding.UTF8,
                    "application/json")
            };

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var executeResponse = JsonSerializer.Deserialize<PayPalExecuteResponse>(json);

        return executeResponse?.state?.ToLower() == "approved";
    }

    public async Task<PayPalPaymentDetailsDto> GetPaymentDetails(string paymentId)
    {
        var accessToken = await GetAccessTokenAsync();

        var request = new HttpRequestMessage(HttpMethod.Get, $"{_payPalApiUrl}/v1/payments/payment/{paymentId}")
        {
            Headers =
            {
                Authorization = new AuthenticationHeaderValue("Bearer", accessToken)
            }
        };

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var paymentDetails = JsonSerializer.Deserialize<PayPalPaymentDetailsDto>(json, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        return paymentDetails;
    }
}