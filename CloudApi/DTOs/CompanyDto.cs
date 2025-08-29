namespace CloudApi.DTOs;

public class CompanyDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public List<BranchDto> Branches { get; set; } = new();
}