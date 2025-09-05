namespace CloudApi.DTOs;

public class BranchDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;

    public Guid CompanyId { get; set; }

    // Сонголтоор Company-г товчхон харуулах
    public string? CompanyName { get; set; }

    // Categories ба Products жагсаалт
    public List<CategoryDto> Categories { get; set; } = new();
    public List<ProductDto> Products { get; set; } = new();
}
public class BranchCreateDto
{
    public string Name { get; set; } = default!;
    public Guid CompanyId { get; set; }
}
public class BranchUpdateDto
{
    public string Name { get; set; } = default!;
}