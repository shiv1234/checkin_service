#nullable enable
namespace OneGuru.CFR.Domain.ResponseModels;

/// <summary>
/// Standardized API response wrapper for all endpoints.
/// </summary>
/// <typeparam name="T">The type of the response entity.</typeparam>
public class Payload<T>
{
    public bool IsSuccess { get; set; } = true;
    public int Status { get; set; }
    public string MessageType { get; set; } = string.Empty;
    public Dictionary<string, string> MessageList { get; set; } = new();
    public PageInfo? PagingInfo { get; set; }
    public T? Entity { get; set; }
    public List<T>? EntityList { get; set; }
}

/// <summary>
/// Pagination information for list responses.
/// </summary>
public class PageInfo
{
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
    public int TotalRecords { get; set; }
    public int TotalPages { get; set; }
}

/// <summary>
/// API result helper for creating standardized responses.
/// </summary>
public static class ApiResult
{
    public static Payload<T> Success<T>(T entity, string message = "Success")
    {
        return new Payload<T>
        {
            IsSuccess = true,
            Status = 200,
            MessageType = "Success",
            Entity = entity,
            MessageList = new Dictionary<string, string> { { "message", message } }
        };
    }

    public static Payload<T> SuccessList<T>(List<T> entities, int pageIndex = 1, int pageSize = 10, int totalRecords = 0)
    {
        var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
        return new Payload<T>
        {
            IsSuccess = true,
            Status = 200,
            MessageType = "Success",
            EntityList = entities,
            PagingInfo = new PageInfo
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalRecords = totalRecords,
                TotalPages = totalPages
            }
        };
    }

    public static Payload<T> Error<T>(string message, int status = 400)
    {
        return new Payload<T>
        {
            IsSuccess = false,
            Status = status,
            MessageType = "Error",
            MessageList = new Dictionary<string, string> { { "error", message } }
        };
    }

    public static Payload<T> NotFound<T>(string message = "Resource not found")
    {
        return new Payload<T>
        {
            IsSuccess = false,
            Status = 404,
            MessageType = "Error",
            MessageList = new Dictionary<string, string> { { "error", message } }
        };
    }

    public static Payload<T> Unauthorized<T>(string message = "Unauthorized")
    {
        return new Payload<T>
        {
            IsSuccess = false,
            Status = 401,
            MessageType = "Error",
            MessageList = new Dictionary<string, string> { { "error", message } }
        };
    }

    public static Payload<T> Forbidden<T>(string message = "Forbidden")
    {
        return new Payload<T>
        {
            IsSuccess = false,
            Status = 403,
            MessageType = "Error",
            MessageList = new Dictionary<string, string> { { "error", message } }
        };
    }
}
