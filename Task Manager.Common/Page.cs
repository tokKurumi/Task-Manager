namespace Task_Manager.Common;

public class Page<T>
{
    public IReadOnlyCollection<T> Items { get; }
    public int PageNumber { get; }
    public int PageSize { get; }
    public long TotalItems { get; }
    public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;

    public Page(IEnumerable<T> items, int pageNumber, int pageSize, long totalItems)
    {
        if (pageNumber < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(pageNumber), "Page number must be greater than or equal to 1.");
        }

        if (pageSize < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(pageSize), "Page size must be greater than or equal to 1.");
        }

        if (totalItems < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(totalItems), "Total items cannot be negative.");
        }

        Items = items?.ToList().AsReadOnly() ?? throw new ArgumentNullException(nameof(items));
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalItems = totalItems;
    }
}
