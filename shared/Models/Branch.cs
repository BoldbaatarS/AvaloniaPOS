
using System;


namespace Shared.Models;


public class Branch
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public Guid CompanyId { get; set; }

    public Company Company { get; set; } = default!;
    public ICollection<Category> Categories { get; set; } = new List<Category>();
    public ICollection<Product> Products { get; set; } = new List<Product>();
}