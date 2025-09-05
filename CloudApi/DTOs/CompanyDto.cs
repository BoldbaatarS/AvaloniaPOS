namespace CloudApi.DTOs;

public class CompanyDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
     public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Register { get; set; }
    public List<BranchDto> Branches { get; set; } = new();
}

public class CompanyCreateDto
{
    public string Name { get; set; } = default!;
    public string? Address { get; set; }
    public string? Phone { get; set; } // "99112233,88113344"
    public string? Email { get; set; }
    public string? Register { get; set; }
}

public class CompanyUpdateDto
{
    public string Name { get; set; } = default!;
     public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Register { get; set; }
}
