using System;

namespace Shared.Models;

public class UserModel
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Pin { get; set; } = string.Empty;
    public bool IsAdmin { get; set; } = false;
}
