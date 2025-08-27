namespace CloudApi.DTOs;

public class HallDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public string? ImageUrl { get; set; } // ImagePath биш, frontend-д илүү ойлгомжтой нэр
}
