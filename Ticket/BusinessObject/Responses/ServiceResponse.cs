namespace BusinessObject.Responses;

public class ServiceResponse<T>
{
    public T Data { get; set; }
    public string? Role { get; set; }
    public bool Success { get; set; } = true;
    public string Message { get; set; } = null!;
    public string Error { get; set; } = null!;
    public string Hint { get; set; } = null!;
    public decimal PriceTotal { get; set; }
    public List<string> ErrorMessages { get; set; } = new List<string>();
}