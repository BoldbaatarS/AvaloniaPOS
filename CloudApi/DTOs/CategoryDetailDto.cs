namespace CloudApi.DTOs;

public class CategoryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;

    public Guid BranchId { get; set; }

    // Parent hierarchy
    public Guid? ParentId { get; set; }

    // Optional: хүүхдийн категори жагсаалт
    public List<CategoryDto> Children { get; set; } = new();
}

public class CategoryCreateDto
{
    public string Name { get; set; } = default!;
    public Guid BranchId { get; set; }
    public Guid? ParentId { get; set; }
}

public class CategoryUpdateDto
{
    public string Name { get; set; } = default!;
    public Guid? ParentId { get; set; }
}
public class CategoryDetailDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";

    public List<ProductDto> Products { get; set; } = new();
    public List<CategoryDetailDto> Children { get; set; } = new();
}