
namespace Oocw.Backend.Models;

public class PaginationParams
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;

    public void Sanitize()
    {
        PageSize = PageSize > 5 ? PageSize : 5;
        PageSize = PageSize < 100 ? PageSize : 100;

        Page = Page > 1 ? Page : 1;
    }
}