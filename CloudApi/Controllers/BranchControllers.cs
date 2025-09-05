using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.Models;
using CloudApi.DTOs;
using CloudApi;
using CloudApi.Utils;


namespace CloudApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BranchesController : ControllerBase
{
    private readonly CloudDbContext _db;
    private readonly IMapper _mapper;

    public BranchesController(CloudDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    // GET: api/branches
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<BranchDto>>>> GetAll()
    {
        var branches = await _db.Branches
            .ProjectTo<BranchDto>(_mapper.ConfigurationProvider)
            .ToListAsync();

        return Ok(ApiResponse<IEnumerable<BranchDto>>.Success(HttpContext, branches));
    }

    // GET: api/branches/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<BranchDto>>> GetById(Guid id)
    {
        var branch = await _db.Branches
            .Include(b => b.Company)
            .Include(b => b.Categories)
            .Include(b => b.Products)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (branch == null)
        {
            return Ok(ApiResponse<BranchDto>.Fail(HttpContext, StatusCodes.Status404NotFound, "Салбар олдсонгүй"));
        }

        var dto = _mapper.Map<BranchDto>(branch);
        return Ok(ApiResponse<BranchDto>.Success(HttpContext, dto));
    }

    // POST: api/branches
    [HttpPost]
    public async Task<ActionResult<ApiResponse<BranchDto>>> Create(BranchCreateDto dto)
    {
        // Эх компанийн ID зөв эсэхийг шалгана
        if (!await _db.Companies.AnyAsync(c => c.Id == dto.CompanyId))
        {
            return Ok(ApiResponse<BranchDto>.Fail(HttpContext, StatusCodes.Status400BadRequest, "Компанийн ID буруу байна"));
        }

        var branch = _mapper.Map<Branch>(dto);
        _db.Branches.Add(branch);
        await _db.SaveChangesAsync();

        var result = _mapper.Map<BranchDto>(branch);
        return Ok(ApiResponse<BranchDto>.Success(HttpContext, result));
    }

    // PUT: api/branches/{id}
    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<BranchDto>>> Update(Guid id, BranchUpdateDto dto)
    {
        var branch = await _db.Branches.FindAsync(id);
        if (branch == null)
        {
            return Ok(ApiResponse<BranchDto>.Fail(HttpContext, StatusCodes.Status404NotFound, "Салбар олдсонгүй"));
        }

        _mapper.Map(dto, branch);
        await _db.SaveChangesAsync();

        var result = _mapper.Map<BranchDto>(branch);
        return Ok(ApiResponse<BranchDto>.Success(HttpContext, result));
    }

    // DELETE: api/branches/{id}
    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<string>>> Delete(Guid id)
    {
        var branch = await _db.Branches.FindAsync(id);
        if (branch == null)
        {
            return Ok(ApiResponse<string>.Fail(HttpContext, StatusCodes.Status404NotFound, "Салбар олдсонгүй"));
        }

        _db.Branches.Remove(branch);
        await _db.SaveChangesAsync();

        return Ok(ApiResponse<string>.Success(HttpContext, "Салбар амжилттай устгагдлаа"));
    }
}
