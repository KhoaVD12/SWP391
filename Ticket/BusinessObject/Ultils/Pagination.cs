using BusinessObject.Responses;

namespace BusinessObject.Ultils;

public class Pagination
{
    public static async Task<PaginationModel<T>> GetPaginationEnum<T>(IEnumerable<T> enumerable, int page, int pageSize)
    {
        var startIndex = (page - 1) * pageSize;
        var enumerable1 = enumerable as T[] ?? enumerable.ToArray();
        var currentPageData = enumerable1.Skip(startIndex).Take(pageSize).ToList();
        await Task.Delay(1); // Simulating async operation
        var totalRecords = enumerable1.Length; // Counting total records in enumerable

        var paginationModel = new PaginationModel<T>
        {
            Page = page,
            TotalRecords = totalRecords,
            TotalPage = (int)Math.Ceiling(totalRecords / (double)pageSize),
            ListData = currentPageData
        };

        return paginationModel;
    }
}