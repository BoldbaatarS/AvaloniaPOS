
using Shared.Models;

namespace CloudApi.DTOs;

public class UserDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public Guid BranchId { get; set; }
    public string BranchName { get; set; } = string.Empty;
}

public class UserCreateDto
{
    public string Name { get; set; } = string.Empty;
    public string Pin { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.User;
    public Guid BranchId { get; set; }
}

public class UserUpdateDto
{
    public string Name { get; set; } = string.Empty;
    public string Pin { get; set; } = string.Empty;
    public UserRole Role { get; set; }
}