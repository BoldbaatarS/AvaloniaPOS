
using System;

namespace Shared.Models;

public class Product
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public decimal Price { get; set; }

    public Guid BranchId { get; set; }
    public Branch Branch { get; set; } = default!;

    public Guid CategoryId { get; set; }
    public Category Category { get; set; } = default!;
}