using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.Models;

namespace CloudApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly CloudDbContext _db;

    public UsersController(CloudDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserModel>>> GetUsers()
        => await _db.Users.ToListAsync();

    [HttpPost]
    public async Task<ActionResult<UserModel>> CreateUser(UserModel user)
    {
        user.Id = Guid.NewGuid();
        _db.Users.Add(user);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetUsers), new { id = user.Id }, user);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(Guid id, UserModel user)
    {
        if (id != user.Id) return BadRequest();
        _db.Entry(user).State = EntityState.Modified;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        var user = await _db.Users.FindAsync(id);
        if (user == null) return NotFound();
        _db.Users.Remove(user);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
