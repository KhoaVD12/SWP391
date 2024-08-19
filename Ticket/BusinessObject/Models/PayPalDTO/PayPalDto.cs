using System.Text.Json.Serialization;

namespace BusinessObject.Models.PayPalDTO;

public class PayPalDto;

public class PayPalTokenResponse
{
    [JsonPropertyName("access_token")] public string AccessToken { get; set; }
    [JsonPropertyName("token_type")] public string TokenType { get; set; }

    [JsonPropertyName("expires_in")] public int ExpiresIn { get; set; }
}

public class PayPalPaymentResponse
{
    public string id { get; set; }
    public List<PayPalLink> links { get; set; }
}

public class PayPalLink
{
    public string href { get; set; }
    public string rel { get; set; }
}

public class PayPalExecuteResponse
{
    public string id { get; set; }
    public string state { get; set; }
}