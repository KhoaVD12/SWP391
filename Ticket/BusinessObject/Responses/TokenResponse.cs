namespace BusinessObject.Responses;

public class TokenResponse<T>
{
    public T DataToken { get; set; }
    public string? Role { get; set; }
    public bool Success { get; set; } = true;
    public string? Message { get; set; } = null;
    public string? Error { get; set; } = null;
    public string? Hint { get; set; } = null;
    public int? HintId { get; set; } = null;
    public string? Code { get; set; } = null;
    public List<string>? ErrorMessages { get; set; } = null;
}