using Microsoft.AspNetCore.Mvc;
using Shared.Models;
using CloudApi;

[ApiController]
[Route("api/[controller]")]
public class CompaniesController : ControllerBase
{
    private readonly CloudDbContext _db;

    public CompaniesController(CloudDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public IActionResult GetAll() => Ok(_db.Companies.ToList());

    [HttpGet("{id}")]
    public IActionResult Get(Guid id)
    {
        var company = _db.Companies.Find(id);
        return company == null ? NotFound() : Ok(company);
    }

    [HttpPost]
    public IActionResult Create([FromBody] Company company)
    {
        company.Id = Guid.NewGuid();
        _db.Companies.Add(company);
        _db.SaveChanges();
        return CreatedAtAction(nameof(Get), new { id = company.Id }, company);
    }

    [HttpPut("{id}")]
    public IActionResult Update(Guid id, [FromBody] Company company)
    {
        var existing = _db.Companies.Find(id);
        if (existing == null) return NotFound();

        existing.Name = company.Name;
        _db.SaveChanges();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(Guid id)
    {
        var company = _db.Companies.Find(id);
        if (company == null) return NotFound();

        _db.Companies.Remove(company);
        _db.SaveChanges();
        return NoContent();
    }
}
