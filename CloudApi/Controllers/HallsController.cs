using AutoMapper;
using AutoMapper.QueryableExtensions;
using CloudApi.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CloudApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HallsController : ControllerBase
{
    private readonly CloudDbContext _db;
    private readonly IMapper _mapper;

    public HallsController(CloudDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    // GET api/halls
    [HttpGet]
    public async Task<ActionResult<IEnumerable<HallDto>>> GetAll()
    {
        var halls = await _db.Halls
            .ProjectTo<HallDto>(_mapper.ConfigurationProvider)
            .ToListAsync();

        return Ok(halls);
    }
    // GET api/halls/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<HallDto>> GetById(Guid id)
    {
        var hall = await _db.Halls
            .ProjectTo<HallDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(h => h.Id == id);

        if (hall == null) return NotFound();

        return Ok(hall);
    }

    // POST api/halls
    [HttpPost]
    public async Task<ActionResult<HallDto>> Create(HallDto dto)
    {
        var hall = _mapper.Map<Shared.Models.HallModel>(dto);

        _db.Halls.Add(hall);
        await _db.SaveChangesAsync();

        var result = _mapper.Map<HallDto>(hall);
        return CreatedAtAction(nameof(GetAll), new { id = result.Id }, result);
        
    }
    // PUT api/halls/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, HallDto dto)
    {
        var hall = await _db.Halls.FindAsync(id);
        if (hall == null) return NotFound();

        _mapper.Map(dto, hall); // DTO-г Entity рүү хуулна
        await _db.SaveChangesAsync();

        return NoContent();
    }

    // DELETE api/halls/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var hall = await _db.Halls.FindAsync(id);
        if (hall == null) return NotFound();

        _db.Halls.Remove(hall);
        await _db.SaveChangesAsync();

        return NoContent();
    }
}
