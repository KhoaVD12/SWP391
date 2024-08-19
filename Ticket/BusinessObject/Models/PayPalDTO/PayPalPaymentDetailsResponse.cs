namespace BusinessObject.Models.PayPalDTO;

public class PayPalPaymentDetailsResponse
{
    public string id { get; set; }
    public string state { get; set; }
    public Payer payer { get; set; }
    public List<TransactionDetails> transactions { get; set; }
    public List<PayPalLink> links { get; set; }
}

public class Payer
{
    public string payment_method { get; set; }
    public PayerInfo payer_info { get; set; }
}

public class PayerInfo
{
    public string email { get; set; }
    public string first_name { get; set; }
    public string last_name { get; set; }
}

public class TransactionDetails
{
    public Amount amount { get; set; }
    public string description { get; set; }
}

public class Amount
{
    public string total { get; set; }
    public string currency { get; set; }
}