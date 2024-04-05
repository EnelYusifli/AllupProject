namespace AllupProject.Helpers;

public class PaginatedList<T>:List<T>
{
    public PaginatedList(List<T> values,int totalCount,int pageSize,int currentPageIndex)
    {
        TotalPageCount=(int)Math.Ceiling(totalCount/(double)pageSize);
        CurrentPage=currentPageIndex;
        AddRange(values);
    }
    public int TotalPageCount { get; set; }
    public int CurrentPage { get; set; }
    public bool HasNext => CurrentPage < TotalPageCount;
    public bool HasPrev => CurrentPage > 1;  
    
    public static PaginatedList<T> Create(IQueryable<T> values,int pageSize,int currentPageIndex)
    {
        return new PaginatedList<T>(values.Skip((currentPageIndex-1) * pageSize).Take(pageSize).ToList(),values.Count(),pageSize,currentPageIndex);
    }
}
