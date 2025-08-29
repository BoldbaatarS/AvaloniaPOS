namespace CloudApi.DTOs;

public class BranchDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public Guid CompanyId { get; set; }
    public CompanyDto Company { get; set; } = default!;
    
}