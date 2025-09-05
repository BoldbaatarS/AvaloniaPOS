using System;


namespace Shared.Models;

public class Category
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = default!;
    // 🔁 Self-reference for hierarchy
    public Guid? ParentId { get; set; }
    public Category? Parent { get; set; }
    public Guid BranchId { get; set; }
    public Branch Branch { get; set; } = default!;
    public ICollection<Category> Children { get; set; } = new List<Category>();
    public ICollection<Product> Products { get; set; } = new List<Product>();
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; } = false;    
    
}