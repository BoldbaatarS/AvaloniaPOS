namespace CloudApi.DTOs;
public class CategoryDetailDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";

    public List<ProductDto> Products { get; set; } = new();
    public List<CategoryDetailDto> Children { get; set; } = new();
}