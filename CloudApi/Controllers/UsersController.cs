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

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<UserDto>>> GetById(Guid id)
    {
        var user = await _db.Users
            .Include(u => u.Branch)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user == null)
            return Ok(ApiResponse<UserDto>.Fail(HttpContext, StatusCodes.Status404NotFound, "Хэрэглэгч олдсонгүй"));

        var dto = _mapper.Map<UserDto>(user);
        return Ok(ApiResponse<UserDto>.Success(HttpContext, dto));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<UserDto>>> Create(UserCreateDto dto)
    {
        // Жишээ: зөвхөн SuperAdmin шинэ хэрэглэгч үүсгэж чадна гэж үзье
        var currentUserRole = User?.FindFirst("role")?.Value; // JWT-д role хадгална
        if (currentUserRole != UserRole.SuperAdmin.ToString())
        {
            return Ok(ApiResponse<UserDto>.Fail(HttpContext, StatusCodes.Status403Forbidden, "Зөвхөн SuperAdmin хэрэглэгч үүсгэж чадна"));
        }

        var user = _mapper.Map<UserModel>(dto);
        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        var result = _mapper.Map<UserDto>(user);
        return Ok(ApiResponse<UserDto>.Success(HttpContext, result));
    }
}

