using System;

namespace Shared.Models;

public enum UserRole
{
    User = 0,       // энгийн хэрэглэгч
    BranchAdmin = 1,// зөвхөн өөрийн Branch дотор
    CompanyAdmin = 2,// өөрийн Company бүх Branch дээр
    SuperAdmin = 3  // бүх систем дээр
}


public class UserModel
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Pin { get; set; } = string.Empty;
    public bool IsAdmin { get; set; } = false;
    public UserRole Role { get; set; } = UserRole.User;
    public Guid? BranchId { get; set; }
    public Branch? Branch { get; set; } = default!;
}
