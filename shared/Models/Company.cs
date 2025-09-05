using System;


namespace Shared.Models;

public class Company
{
    public Guid Id { get; set; }=Guid.NewGuid();
    public string Name { get; set; } = default!;
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Register { get; set; }
    public ICollection<Branch> Branches { get; set; } = new List<Branch>();
}