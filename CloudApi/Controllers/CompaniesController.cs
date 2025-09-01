using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.Models;
using CloudApi.DTOs;
using CloudApi;


namespace CloudApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CompaniesController : ControllerBase
{
    private readonly CloudDbContext _db;
    private readonly IMapper _mapper;


    public CompaniesController(CloudDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    [HttpGet]
     public async Task<ActionResult<IEnumerable<CompanyDto>>> GetAll()
     {
        var companies = await _db.Companies
            .ProjectTo<CompanyDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
        return Ok(companies);
     }

    [HttpGet("{id}")]
    public async Task<ActionResult<CompanyDto>> GetById(Guid id)
    {
        var company = await _db.Companies
            .ProjectTo<CompanyDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (company == null) return NotFound();

        return Ok(company);
    }
   

    // POST api/companies
    [HttpPost]
    public async Task<ActionResult<HallDto>> Create(HallDto dto)
    {
        var company = _mapper.Map<Shared.Models.Company>(dto);

        _db.Companies.Add(company);
        await _db.SaveChangesAsync();

        var result = _mapper.Map<HallDto>(company);
        return CreatedAtAction(nameof(GetAll), new { id = result.Id }, result);
    }


     // PUT api/companies/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] CompanyDto dto)
    {
        var company = await _db.Companies.FindAsync(id);
        if (company == null) return NotFound();

        _mapper.Map(dto, company); // DTO-г Entity рүү хуулна
        await _db.SaveChangesAsync();

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
