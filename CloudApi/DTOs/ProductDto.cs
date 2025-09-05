namespace CloudApi.DTOs;

public class ProductDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public decimal Price { get; set; }

    public Guid BranchId { get; set; }
    public Guid CategoryId { get; set; }
}

public class ProductCreateDto
{
    public string Name { get; set; } = default!;
    public decimal Price { get; set; }

    public Guid BranchId { get; set; }
    public Guid CategoryId { get; set; }
}

public class ProductUpdateDto
{
    public string Name { get; set; } = default!;
    public decimal Price { get; set; }
    public Guid CategoryId { get; set; }
}