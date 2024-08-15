using DataAccessObject.Enums;

namespace BusinessObject.Responses;

public class ServiceResponse<T>
{
    public T Data { get; set; }
    public Role Role { get; set; }
    public bool Success { get; set; } = true;
    public string Message { get; set; } = null!;
    public string Error { get; set; } = null!;
    public string Hint { get; set; } = null!;
    public decimal PriceTotal { get; set; }
    public List<string> ErrorMessages { get; set; } = [];
}

public class PaginationModel<T>
{
    public int Page { get; set; }
    public int TotalPage { get; set; }
    public int TotalRecords { get; set; }
    public List<T> ListData { get; set; }
}