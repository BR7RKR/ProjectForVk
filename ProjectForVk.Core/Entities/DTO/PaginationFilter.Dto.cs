namespace ProjectForVk.Core.Entities.DTO;

public sealed class PaginationFilterDto
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }

    private const int MIN_PAGE_NUMBER = 1;
    
    private const int MAX_PAGE_SIZE = 50;

    public PaginationFilterDto(int pageNumber = MIN_PAGE_NUMBER, int pageSize = MAX_PAGE_SIZE)
    {
        PageNumber = pageNumber < MIN_PAGE_NUMBER ? MIN_PAGE_NUMBER : pageNumber;
        PageSize = pageSize > MAX_PAGE_SIZE ? MAX_PAGE_SIZE : pageSize;
    }
}