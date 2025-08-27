using System;


namespace Shared.Models;

public class Company
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;

    public ICollection<Branch> Branches { get; set; } = new List<Branch>();
}