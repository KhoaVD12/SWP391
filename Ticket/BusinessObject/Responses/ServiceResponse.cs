using DataAccessObject.Enums;

namespace BusinessObject.Responses;

public class ServiceResponse<T>
{
    public T Data { get; set; }
    public bool Success { get; set; } = true;
    public string Message { get; set; } = null!;
    public List<string> ErrorMessages { get; set; } = [];
    public int? Id { get; set; }
}

public class PaginationModel<T>
{
    public int Page { get; set; }
    public int TotalPage { get; set; }
    public int TotalRecords { get; set; }
    public List<T> ListData { get; set; }
}