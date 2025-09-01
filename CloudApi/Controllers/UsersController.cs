using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CloudApi.DTOs;
using Shared.Models;

namespace CloudApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly CloudDbContext _db;
    private readonly IMapper _mapper;


    public UsersController(CloudDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserModel>>> GetUsers()
    {
        var users = await _db.Users
            .ProjectTo<UserDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
        return Ok(_db.Users);
    }

     // GET api/user/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> GetById(Guid id)
    {
        var user = await _db.Users
            .ProjectTo<UserDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(h => h.Id == id);
        if (user == null) return NotFound();

        return Ok(user);
    }

    // POST api/users
    [HttpPost]
    public async Task<ActionResult<UserDto>> Create(UserDto dto)
    {
        var user = _mapper.Map<Shared.Models.UserModel>(dto);
        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        var result = _mapper.Map<UserDto>(user);
        return CreatedAtAction(nameof(GetUsers), new { id = result.Id }, result);
    }

    // PUT api/users/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, UserDto dto)
    {
        var user = await _db.Users.FindAsync(id);
        if (user == null) return NotFound();

        _mapper.Map(dto, user); // DTO-г Entity рүү хуулна
        await _db.SaveChangesAsync();

        return NoContent();
    }

    

    // DELETE api/Users/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var user = await _db.Users.FindAsync(id);
        if (user == null) return NotFound();

        _db.Users.Remove(user);
        await _db.SaveChangesAsync();

        return NoContent();
    }

    
}
