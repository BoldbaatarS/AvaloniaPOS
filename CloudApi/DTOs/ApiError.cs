namespace CloudApi.DTOs;

public class ApiError
{
    public string Message { get; set; } = string.Empty;
    public string? Details { get; set; } // зөвхөн development-д харуулах
}