namespace CloudApi.DTOs;

public class UserDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Pin { get; set; } = string.Empty;
    public bool IsAdmin { get; set; } = false;
    public Guid BranchId { get; set; }
    public BranchDto Branch { get; set; } = default!;
}