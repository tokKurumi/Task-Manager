namespace Task_Manager.Common;

public interface IPagination
{
    int Page { get; }
    int PageSize { get; }
}

public record Pagination(int Page, int PageSize) : IPagination;

public enum SortDirection
{
    Ascending,
    Descending
}

public sealed record SortablePagination<TSortField>(
    int Page,
    int PageSize,
    TSortField? SortBy,
    SortDirection? SortDirection
) : Pagination(Page, PageSize)
    where TSortField : Enum;
