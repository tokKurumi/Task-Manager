using System.Collections;

namespace Task_Manager.Common;

public sealed record Page<T> : IReadOnlyCollection<T>
{
    public IReadOnlyCollection<T> Items { get; init; }
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
    public long TotalItems { get; init; }

    public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
    public int Count => Items.Count;
    public IEnumerator<T> GetEnumerator() => Items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public Page(IEnumerable<T> items, int pageNumber, int pageSize, long totalItems)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(pageNumber, 1);
        ArgumentOutOfRangeException.ThrowIfLessThan(pageSize, 1);
        ArgumentOutOfRangeException.ThrowIfNegative(totalItems);

        Items = items.ToList().AsReadOnly();
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalItems = totalItems;
    }

    public Page(IEnumerable<T> items, IPagination pagination, long totalItems)
        : this(items, pagination.Page, pagination.PageSize, totalItems) { }
}
