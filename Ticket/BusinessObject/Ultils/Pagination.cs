using BusinessObject.Responses;

namespace BusinessObject.Ultils;

public class Pagination
{
    public static async Task<PaginationModel<T>> GetPagination<T>(List<T> list, int page, int pageSize)
    {
        var startIndex = (page - 1) * pageSize;
        var currentPageData = list.Skip(startIndex).Take(pageSize).ToList(); 
        await Task.Delay(1);
        var paginationModel = new PaginationModel<T>
        {
            Page = page,
            TotalRecords = list.Count,
            TotalPage = (int)Math.Ceiling(list.Count / (double)pageSize),
            ListData = currentPageData
        };
        return paginationModel;
    }

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