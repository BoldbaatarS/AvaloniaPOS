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
}
