using ApiError = CloudApi.DTOs.ApiError;

namespace CloudApi.DTOs;

public class ApiResponse<T>
{
    public int StatusCode { get; set; }
    public T? Data { get; set; }
    public ApiError? ErrorMessage { get; set; }

   public ApiResponse(HttpContext context, T? data, ApiError? error = null)
    {
        StatusCode = context.Response.StatusCode;
        Data = data;
        ErrorMessage = error;
    }

    public static ApiResponse<T> Success(HttpContext context, T data)
    {
        context.Response.StatusCode = StatusCodes.Status200OK;
        return new ApiResponse<T>(context, data);
    }

   public static ApiResponse<T> Fail(HttpContext context, int statusCode, string message, string? details = null)
    {
        context.Response.StatusCode = statusCode;
        return new ApiResponse<T>(context, default, new ApiError
        {
            Message = message,
            Details = details
        });
    }
}